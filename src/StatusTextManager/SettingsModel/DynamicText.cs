using Newtonsoft.Json;
using StatusTextManager.Utils;
using System.Text.RegularExpressions;
using Terraria;
using TShockAPI;

namespace StatusTextManager.SettingsModel;

public class DynamicText : IStatusTextSetting, IStatusTextUpdateHandler
{
    [JsonProperty] public static string TypeName => nameof(DynamicText);

    public string Text { get; set; } = "";

    public ulong UpdateInterval { get; set; }

    private readonly string?[] _playerStatusTexts = new string[Main.maxPlayers];

    private readonly Regex _interpolationRegex = new Regex(@"(?:{{[^{\s]*?}}|{[^{\s]*?})"); // match {abc} and {{abc}}

    public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx)
    {
        processedHandlers.Add(this);
    }


    public bool Invoke(TSPlayer player, bool forceUpdate = false)
    {
        if (forceUpdate || (Common.TickCount + (ulong) player.Index) % this.UpdateInterval == 0)
        {
            string match_evaluator(Match m)
            {
                return m.Value[1] == '{' // if {{abc}}
                    ? m.Value[1..^1]
                    : m.Value switch
                    {
                        "{PlayerName}" => player.Name,
                        "{PlayerGroupName}" => player.Group.Name,
                        "{PlayerLife}" => player.TPlayer.statLife.ToString(),
                        "{PlayerMana}" => player.TPlayer.statMana.ToString(),
                        "{PlayerLifeMax}" => player.TPlayer.statLifeMax2.ToString(),
                        "{PlayerManaMax}" => player.TPlayer.statManaMax2.ToString(),
                        "{PlayerLuck}" => player.TPlayer.luck.ToString(),
                        "{PlayerCoordinateX}" => player.TileX.ToString(),
                        "{PlayerCoordinateY}" => player.TileY.ToString(),
                        "{PlayerCurrentRegion}" => player.CurrentRegion == null ? GetString("空区域") : player.CurrentRegion.Name,
                        "{IsPlayerAlive}" => player.Dead ? GetString("已死亡") : GetString("存活"),
                        "{RespawnTimer}" => player.RespawnTimer == 0 ? GetString("未死亡") : player.RespawnTimer.ToString(),
                        "{OnlinePlayersCount}" => TShock.Utils.GetActivePlayerCount().ToString(),
                        "{OnlinePlayersList}" => string.Join(',', TShock.Players.Where(x => x is { Active: true }).Select(x => x.Name)),
                        "{AnglerQuestFishName}" => Common.GetAnglerQuestFishName(),
                        "{AnglerQuestFishID}" => Common.GetAnglerQuestFishId().ToString(),
                        "{AnglerQuestFishingBiome}" => Common.GetAnglerQuestFishingBiome(),
                        "{AnglerQuestCompleted}" => Main.anglerWhoFinishedToday.Exists((string x) => x == player.Name) ? GetString("已完成") : GetString("未完成"),
                        "{CurrentTime}" => Common.GetCurrentTime(),
                        "{RealWorldTime}" => DateTime.Now.ToString("HH:mm"),
                        "{WorldName}" => Main.worldName,
                        "{CurrentBiomes}" => player.GetFormattedBiomesList(),
                        _ => m.Value,
                    };

            }

            this._playerStatusTexts[player.Index] = this._interpolationRegex.Replace(this.Text, match_evaluator);

            return true;
        }
        return false;
    }

    public string GetPlayerStatusText(TSPlayer player)
    {
        return this._playerStatusTexts[player.Index] ?? "";
    }
}
