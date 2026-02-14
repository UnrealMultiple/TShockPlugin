using CaiBotLite.Enums;
using CaiBotLite.Models;
using Microsoft.Xna.Framework;
using SixLabors.ImageSharp.Formats.Png;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using CaiBotPlayer = CaiBotLite.Models.CaiBotPlayer;

namespace CaiBotLite.Common;

internal static class CaiBotApi
{
    internal static async Task HandleMessageAsync(string receivedData)
    {
        var package = Package.Parse(receivedData);
        var packetWriter = new PackageWriter(package.Type, package.IsRequest, package.RequestId);
        try
        {
            switch (package.Type)
            {
                case PackageType.UnbindServer:
                    TShock.Log.ConsoleInfo("[CaiBotLite]BOT发送解绑命令...");
                    var reason = package.Read<string>("reason");
                    TShock.Log.ConsoleInfo($"[CaiBotLite]原因: {reason}");
                    Config.Settings.Token = string.Empty;
                    Config.Settings.Write();
                    CaiBotLite.GenBindCode(EventArgs.Empty);
                    WebsocketManager.WebSocket?.Dispose();
                    break;
                case PackageType.CallCommand:
                    var command = package.Read<string>("command");
                    var userOpenId = package.Read<string>("user_open_id");
                    var groupOpenId = package.Read<string>("group_open_id");
                    CaiBotPlayer tr = new ();
                    Commands.HandleCommand(tr, command);
                    TShock.Utils.SendLogs($"[CaiBotLite] \"{userOpenId}\"来自群\"{groupOpenId}\"执行了: {command}", Color.PaleVioletRed);
                    packetWriter
                        .Write("output", tr.GetCommandOutput())
                        .Send();
                    break;
                case PackageType.PlayerList:
                    packetWriter
                        .Write("server_name", string.IsNullOrEmpty(Main.worldName) ? "地图还没加载捏~" : Main.worldName)
                        .Write("player_list", TShock.Players.Where(x => x is { Active: true }).Select(x => x.Name))
                        .Write("current_online", TShock.Utils.GetActivePlayerCount())
                        .Write("max_online", TShock.Config.Settings.MaxSlots)
                        .Write("process", Config.Settings.ShowProcessInPlayerList ? Utils.GetWorldProcess() : "")
                        .Send();
                    break;
                case PackageType.Progress:

                    var bossLock = new Dictionary<string, string>();


                    if (BossLockSupport.Support)
                    {
                        bossLock = BossLockSupport.GetLockBosses();
                    }

                    if (ProgressControlSupport.Support)
                    {
                        var progressControlBosses = ProgressControlSupport.GetLockBosses();
                        bossLock = bossLock.Count < progressControlBosses.Count ? progressControlBosses : bossLock;
                    }

                    packetWriter
                        .Write("is_text", false)
                        .Write("process", Utils.GetProcessList())
                        .Write("kill_counts", Utils.GetKillCountList())
                        .Write("boss_lock", bossLock)
                        .Write("world_name", Main.worldName)
                        .Write("drunk_world", Main.drunkWorld)
                        .Write("zenith_world", Main.zenithWorld)
                        .Write("world_icon", Utils.GetWorldIconName())
                        .Send();
                    break;
                case PackageType.Whitelist:
                    var name = package.Read<string>("player_name");
                    var whitelistResult = package.Read<WhiteListResult>("whitelist_result");

                    var player = TShock.Players.FirstOrDefault(x =>
                        x?.Name == name && x is { State: (int) ConnectionState.AssigningPlayerSlot});

                    if (player == null)
                    {
                        return;
                    }

                    if (LoginHelper.CheckWhitelist(player, whitelistResult))
                    {
                        LoginHelper.HandleLogin(player);
                    }

                    break;
                case PackageType.SelfKick:
                    var selfKickName = package.Read<string>("name");
                    var kickPlr = TShock.Players.FirstOrDefault(x => x?.Name == selfKickName);
                    if (kickPlr == null)
                    {
                        return;
                    }

                    kickPlr.Kick("使用BOT自踢命令", true, saveSSI: true);
                    break;
                case PackageType.LookBag:
                    var lookBagName = package.Read<string>("player_name");

                    packetWriter
                        .Write("is_text", false)
                        .Write("name", lookBagName);

                    var lookPlr = TShock.Players.FirstOrDefault(x => x?.Name == lookBagName);
                    if (lookPlr != null)
                    {
                        var plr = lookPlr.TPlayer;
                        var lookOnlineResult = LookBag.LookOnline(plr);
                        packetWriter
                            .Write("exist", true)
                            .Write("life", $"{lookOnlineResult.Health}/{lookOnlineResult.MaxHealth}")
                            .Write("mana", $"{lookOnlineResult.Mana}/{lookOnlineResult.MaxMana}")
                            .Write("quests_completed", lookOnlineResult.QuestsCompleted)
                            .Write("inventory", lookOnlineResult.ItemList)
                            .Write("buffs", lookOnlineResult.Buffs)
                            .Write("enhances", lookOnlineResult.Enhances)
                            .Write("economic", EconomicData.GetEconomicData(lookOnlineResult.Name))
                            .Send();
                    }
                    else
                    {
                        var acc = TShock.UserAccounts.GetUserAccountByName(lookBagName);
                        if (acc == null)
                        {
                            packetWriter
                                .Write("exist", 0)
                                .Send();
                            return;
                        }

                        var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), acc.ID);
                        if (data == null)
                        {
                            packetWriter
                                .Write("exist", false)
                                .Send();
                            return;
                        }

                        var lookOnlineResult = LookBag.LookOffline(acc, data);
                        packetWriter
                            .Write("exist", true)
                            .Write("life", $"{lookOnlineResult.Health}/{lookOnlineResult.MaxHealth}")
                            .Write("mana", $"{lookOnlineResult.Mana}/{lookOnlineResult.MaxMana}")
                            .Write("quests_completed", lookOnlineResult.QuestsCompleted)
                            .Write("inventory", lookOnlineResult.ItemList)
                            .Write("buffs", lookOnlineResult.Buffs)
                            .Write("enhances", lookOnlineResult.Enhances)
                            .Write("economic", EconomicData.GetEconomicData(lookOnlineResult.Name))
                            .Send();
                    }

                    break;
                case PackageType.MapImage:
                    var bitmap = MapGenerator.CreateMapImg();
                    using (MemoryStream ms = new ())
                    {
                        await bitmap.SaveAsync(ms, new PngEncoder());
                        var imageBytes = ms.ToArray();
                        var base64 = Convert.ToBase64String(imageBytes);
                        packetWriter
                            .Write("base64", Utils.CompressBase64(base64))
                            .Send();
                    }

                    break;
                case PackageType.MapFile:
                    var mapFile = MapGenerator.CreateMapFile();
                    packetWriter
                        .Write("name", mapFile.Item2)
                        .Write("base64", Utils.CompressBase64(mapFile.Item1))
                        .Send();

                    break;
                case PackageType.WorldFile:
                    packetWriter
                        .Write("name", Path.GetFileName(Main.worldPathName))
                        .Write("base64", Utils.CompressBase64(Utils.FileToBase64String(Main.worldPathName)))
                        .Send();

                    break;
                case PackageType.PluginList:
                    var pluginList = ServerApi.Plugins.Select(p => new PluginInfo(p.Plugin.Name, p.Plugin.Description, p.Plugin.Author, p.Plugin.Version)).ToList();
                    packetWriter
                        .Write("is_mod", false)
                        .Write("plugins", pluginList)
                        .Send();
                    break;
                case PackageType.RankData:
                    var rankType = package.Read<string>("rank_type");
                    var arg = package.Read<string>("arg");

                    var rankTypeEnum = Rank.GetRankTypeByName(rankType);

                    switch (rankTypeEnum)
                    {
                        case RankTypes.Boss:
                            var bosses = Rank.GetBossByIdOrName(arg);
                            switch (bosses.Count)
                            {
                                case 0:
                                    packetWriter
                                        .Write("rank_type_support", true)
                                        .Write("need_arg", true)
                                        .Write("arg_support", false)
                                        .Write("message", "没有找到任何相关的BOSS呢~")
                                        .Write("support_args", Array.Empty<string>())
                                        .Send();
                                    break;
                                case > 1:
                                    packetWriter
                                        .Write("rank_type_support", true)
                                        .Write("need_arg", true)
                                        .Write("arg_support", false)
                                        .Write("message", "找到多个匹配的BOSS:\n")
                                        .Write("support_args", bosses.Select(x => $"{x.TypeName} ({x.type})"))
                                        .Send();
                                    break;
                                case 1:
                                    var boss = bosses.First();
                                    packetWriter
                                        .Write("rank_type_support", true)
                                        .Write("need_arg", true)
                                        .Write("arg_support", true)
                                        .Write("rank", Rank.GetBossRank(boss))
                                        .Send();
                                    break;
                            }

                            break;
                        case RankTypes.EconomicCoin:
                            if (string.IsNullOrEmpty(arg))
                            {
                                packetWriter
                                    .Write("rank_type_support", true)
                                    .Write("need_arg", true)
                                    .Write("arg_support", false)
                                    .Write("message", $"需要参数[货币], 以下是支持的货币:\n")
                                    .Write("support_args", EconomicSupport.SupportCoins)
                                    .Send();
                                return;
                            }

                            if (!EconomicSupport.SupportCoins.Contains(arg))
                            {
                                packetWriter
                                    .Write("rank_type_support", true)
                                    .Write("need_arg", true)
                                    .Write("arg_support", false)
                                    .Write("message", $"没有找到{arg}, 以下是支持的货币:\n")
                                    .Write("support_args", EconomicSupport.SupportCoins)
                                    .Send();
                                return;
                            }

                            packetWriter
                                .Write("rank_type_support", true)
                                .Write("need_arg", true)
                                .Write("arg_support", true)
                                .Write("rank", EconomicSupport.GetCoinRank(arg))
                                .Send();


                            break;
                        case RankTypes.Death:
                            packetWriter
                                .Write("rank_type_support", true)
                                .Write("need_arg", false)
                                .Write("rank", Rank.GetDeathRank())
                                .Send();
                            break;
                        case RankTypes.Online:
                            packetWriter
                                .Write("rank_type_support", true)
                                .Write("need_arg", false)
                                .Write("rank", Rank.GetOnlineRank())
                                .Send();
                            break;
                        case RankTypes.Fishing:
                            packetWriter
                                .Write("rank_type_support", true)
                                .Write("need_arg", false)
                                .Write("rank", Rank.GetFishingRank())
                                .Send();
                            break;
                        case RankTypes.Unknown:
                            packetWriter
                                .Write("rank_type_support", false)
                                .Write("support_rank_types", Rank.SupportRankTypes)
                                .Send();
                            break;
                    }

                    break;
                case PackageType.ShopCondition:
                    var itemConditions = package.Read<List<ProgressType>>("item_conditions");
                    packetWriter
                        .Write("unmet_conditions", ProgressHelper.CheckProgresses(itemConditions))
                        .Send();
                    break;
                case PackageType.ShopBuy:
                    var shopPlayerName = package.Read<string>("player_name");
                    var isCommand = package.Read<bool>("is_command");
                    var mail = new Mail { AccountName = shopPlayerName };
                    if (isCommand)
                    {
                        mail.IsCommand = true;
                        mail.Commands = package.Read<List<string>>("commands");
                    }
                    else
                    {
                        mail.IsCommand = false;
                        mail.Items = package.Read<List<MailItem>>("commands");
                    }

                    mail.CreatOrUpdate();
                    break;
                case PackageType.Hello:
                case PackageType.Heartbeat:
                case PackageType.Unknown:
                case PackageType.Error:
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[CaiBotLite] 处理BOT数据包时出错:\n" +
                                    $"{ex}\n" +
                                    $"源数据包: {receivedData}");

            packetWriter.Package.Type = PackageType.Error;
            packetWriter.Write("error", ex)
                .Send();
        }
    }
}