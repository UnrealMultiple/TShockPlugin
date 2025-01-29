using TShockAPI;
using static SpawnInfra.Plugin;

namespace SpawnInfra;

internal class Comds
{
    public static async void Comd(CommandArgs args)
    {
        var plr = args.Player;
        if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(
                GetString("【生成基础建设】\n") +
                GetString("/rm 数量 —— 建监狱\n") +
                GetString("/rm 重置 —— 重置自动建房开关"), 250, 247, 105);
        }

        if (args.Parameters.Count == 1
            && plr.HasPermission("room.admin")
            && (args.Parameters[0].ToLower() == "reset"
            || args.Parameters[0].ToLower() == "重置"))
        {
            Config.Enabled = true;
            args.Player.SendMessage(GetString("【生成基础建设】开关已打开，请重启服务器。"), 250, 247, 105);
            Config.Write();
            return;
        }

        var total = 3;
        if (args.Parameters.Count > 1)
        {
            if (NeedInGame() || NeedWaitTask() || !plr.RealPlayer || args == null)
            {
                return;
            }

            if (!int.TryParse(args.Parameters[1], out total))
            {
                plr.SendErrorMessage(GetString("输入的监狱数量不对"));
                return;
            }

            if (total < 1 || total > 1000)
            {
                total = 3;
            }
        }
        await AsyncGenRoom(isRight: plr.TPlayer.direction != -1, plr: plr, posX: plr.TileX, posY: plr.TileY + 3, total: total, needCenter: true);

        bool NeedInGame() => Utils.NeedInGame(plr);

        bool NeedWaitTask() => TileHelper.NeedWaitTask(plr);
    }

}