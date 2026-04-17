using DeltaForce.Game.Modules;
using LazyAPI.Attributes;
using LazyAPI.Utility;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace DeltaForce.Game.Commands;

[Command("df")]
[Permissions("deltaforce.game")]
public class GameCommand
{
    [Alias("leave", "l", "exit", "quit")]
    [RealPlayer]
    public static void LeaveGame(CommandArgs args)
    {
        var player = args.Player;

        if (!GameManager._isGame)
        {
            player.SendErrorMessage(GetString("当前没有进行中的游戏！"));
            return;
        }

        bool wasAlive = !player.TPlayer.ghost;

        if (wasAlive)
        {
            player.SendInfoMessage(GetString("你选择了主动撤离，正在丢弃所有装备..."));
            GameUitls.ClearPlayerInventory(player, true);
            player.TPlayer.ghost = true;
            TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", player.Index);
            player.SendInfoMessage(GetString("装备已丢弃，正在同步数据..."));
        }
        else
        {
            player.SendInfoMessage(GetString("你已死亡，正在同步数据并返回特勤处..."));
        }

        try
        {
            var response = InventoryManager.SavePlayerInventory(player);

            if (response?.Success == true)
            {
                player.SendSuccessMessage(GetString("数据已同步！"));
                TShock.Log.ConsoleInfo(GetString($"[GameCommand] 玩家 {player.Name} 主动离开游戏，数据已同步"));
            }
            else
            {
                player.SendWarningMessage(GetString($"数据同步可能失败: {response?.Message}，但仍将返回特勤处"));
                TShock.Log.ConsoleWarn(GetString($"[GameCommand] 玩家 {player.Name} 数据同步失败: {response?.Message}"));
            }

            if (wasAlive)
            {
                TShock.Utils.Broadcast(GetString($"[DeltaForce] {player.Name} 主动撤离并丢弃所有装备返回特勤处"), Color.Orange);
            }
            else
            {
                TShock.Utils.Broadcast(GetString($"[DeltaForce] {player.Name} 返回特勤处"), Color.Gray);
            }

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                DimensionsSender.SendPlayerToCustomServer(
                    player,
                    Config.Instance.CoreServer.Address,
                    (ushort)Config.Instance.CoreServer.Port
                );
            });
        }
        catch (Exception ex)
        {
            player.SendErrorMessage(GetString($"离开游戏时发生错误: {ex.Message}"));
            TShock.Log.ConsoleError(GetString($"[GameCommand] 玩家 {player.Name} 离开游戏时发生错误: {ex}"));
        }
    }

    [Alias("time", "t")]
    [RealPlayer]
    public static void ShowTime(CommandArgs args)
    {
        if (!GameManager._isGame)
        {
            args.Player.SendErrorMessage(GetString("当前没有进行中的游戏！"));
            return;
        }

        var totalSeconds = Config.Instance.MatchMinute * 60;
        var remainingSeconds = totalSeconds - GameManager.GetGameDuration();
        var remainingMinutes = remainingSeconds / 60;

        if (remainingMinutes > 0)
        {
            args.Player.SendInfoMessage(GetString($"[游戏时间] 还剩 {remainingMinutes} 分 {remainingSeconds % 60} 秒"));
        }
        else
        {
            args.Player.SendWarningMessage(GetString($"[游戏时间] 还剩 {remainingSeconds} 秒！请尽快撤离！"));
        }
    }

    [Alias("evac", "e")]
    [RealPlayer]
    public static void ShowEvacuationPoints(CommandArgs args)
    {
        if (!GameManager._isGame)
        {
            args.Player.SendErrorMessage(GetString("当前没有进行中的游戏！"));
            return;
        }

        var points = EvacuationManager.GetActiveEvacuationPoints();

        if (points.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("当前没有可用的撤离点！"));
            return;
        }

        args.Player.SendInfoMessage(GetString("[撤离点列表]"));
        foreach (var point in points)
        {
            args.Player.SendInfoMessage(GetString($"  - {point.Name}: 坐标 ({point.X}, {point.Y}), 范围 {point.Radius} 格"));
        }
    }
}
