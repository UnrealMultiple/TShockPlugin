using Newtonsoft.Json;
using StatusTextManager.Utils;
using System.Text;
using Terraria;
using TShockAPI;

namespace StatusTextManager.SettingsModel;

public class DynamicText : IStatusTextSetting, IStatusTextUpdateHandler
{
    [JsonProperty] public static string TypeName => nameof(DynamicText);

    public string Text { get; set; }

    public ulong UpdateInterval { get; set; }

    private readonly StringBuilder?[] _playerStringBuilders = new StringBuilder[Main.maxPlayers];

    public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx)
    {
        processedHandlers.Add(this);
    }


    public bool Invoke(TSPlayer player, bool forceUpdate = false)
    {
        if (forceUpdate || (Common.TickCount + (ulong) player.Index) % this.UpdateInterval == 0)
        {
            var sb = this._playerStringBuilders.AcquirePlayerStringBuilder(player);
            sb.Append(this.Text);
            sb.Replace("%PlayerName%", player.Name)
                .Replace("%PlayerGroupName%", player.Group.Name)
                .Replace("%PlayerLife%", player.TPlayer.statLife.ToString())
                .Replace("%PlayerMana%", player.TPlayer.statMana.ToString())
                .Replace("%PlayerLifeMax%", player.TPlayer.statLifeMax2.ToString())
                .Replace("%PlayerManaMax%", player.TPlayer.statManaMax2.ToString())
                .Replace("%PlayerLuck%", player.TPlayer.luck.ToString())
                .Replace("%PlayerCoordinateX%", player.TileX.ToString())
                .Replace("%PlayerCoordinateY%", player.TileY.ToString())
                .Replace("%PlayerCurrentRegion%", player.CurrentRegion == null ? GetString("空区域") : player.CurrentRegion.Name)
                .Replace("%IsPlayerAlive%", player.Dead ? GetString("已死亡") : GetString("存活"))
                .Replace("%RespawnTimer%", player.RespawnTimer == 0 ? GetString("未死亡") : player.RespawnTimer.ToString())
                .Replace("%OnlinePlayersCount%", TShock.Utils.GetActivePlayerCount().ToString())
                .Replace("%OnlinePlayersList%", string.Join(',', TShock.Players.Where(x => x is { Active: true }).Select(x => x.Name)))
                .Replace("%AnglerQuestFishName%", Common.GetAnglerQuestFishName())
                .Replace("%AnglerQuestFishID%", Common.GetAnglerQuestFishId().ToString())
                .Replace("%AnglerQuestFishingBiome%", Common.GetAnglerQuestFishingBiome())
                .Replace("%AnglerQuestCompleted%", Main.anglerWhoFinishedToday.Exists((string x) => x == player.Name) ? GetString("已完成") : GetString("未完成"))
                .Replace("%CurrentTime%", Common.GetCurrentTime())
                .Replace("%RealWorldTime%", DateTime.Now.ToString("HH:mm"))
                .Replace("%WorldName%", Main.worldName)
                .Replace("%CurrentBiomes%", player.GetFormattedBiomesList());
            return true;
        }
        return false;
    }

    public string GetPlayerStatusText(TSPlayer player)
    {
        return this._playerStringBuilders[player.Index]?.ToString() ?? "";
    }
}
