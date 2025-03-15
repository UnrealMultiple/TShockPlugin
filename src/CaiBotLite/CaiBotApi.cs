using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Formats.Png;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using CaiBot;
using Utils = CaiBot.Utils;

namespace CaiBotLite;

internal static class CaiBotApi
{
    internal static bool IsWebsocketConnected =>
        Plugin.WebSocket.State == WebSocketState.Open;
    
    internal static readonly Dictionary<string,(DateTime,int)> WhiteListCaches = new();

    internal static async Task HandleMessageAsync(string receivedData)
    {
        try
        {
            var jsonObject = JObject.Parse(receivedData);
            var type = (string) jsonObject["type"]!;
            
            var group = "";
            var msgId = "";
            if (jsonObject.ContainsKey("group"))
            {
                group = jsonObject["group"]!.ToObject<string>()!;
            }
            
            if (jsonObject.ContainsKey("msg_id"))
            {
                msgId = jsonObject["msg_id"]!.ToObject<string>()!;
            }
            
            var packetWriter = new PacketWriter(group, msgId);
            switch (type)
            {
                case "delserver":
                    TShock.Log.ConsoleInfo("[CaiBotLite]BOT发送解绑命令...");
                    Config.Settings.Token = string.Empty;
                    Config.Settings.Write();
                    Plugin.GenBindCode(EventArgs.Empty);
                    Plugin.WebSocket.Dispose();
                    break;
                case "hello":
                    TShock.Log.ConsoleInfo("[CaiBotLite]CaiBOT连接成功...");
                    //发送服务器信息
                    packetWriter.SetType("hello")
                        .Write("tshock_version", TShock.VersionNum.ToString())
                        .Write("plugin_version", Plugin.VersionNum)
                        .Write("terraria_version", Main.versionNumber)
                        .Write("cai_whitelist", Config.Settings.WhiteList)
                        .Write("os", RuntimeInformation.RuntimeIdentifier)
                        .Write("world", TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName)
                        .Send(); 
                    break;
                case "cmd":
                    var cmd = (string) jsonObject["cmd"]!;
                    CaiBotPlayer tr = new ();
                    Commands.HandleCommand(tr, cmd);
                    TShock.Utils.SendLogs($"[CaiBotLite] `{(string) jsonObject["at"]!}`来自群`{(string) jsonObject["group"]!}`执行了: {(string) jsonObject["cmd"]!}", Color.PaleVioletRed);
                   
                    packetWriter.SetType("cmd")
                        .Write("result", string.Join('\n', tr.GetCommandOutput()))
                        .Send();
                    break;
                case "online":
                    var onlineResult = new StringBuilder();
                    if (TShock.Utils.GetActivePlayerCount() == 0)
                    {
                        onlineResult.Append("没有玩家在线捏...");
                    }
                    else
                    {
                        onlineResult.AppendLine($"在线玩家({TShock.Utils.GetActivePlayerCount()}/{TShock.Config.Settings.MaxSlots})");
                        onlineResult.Append(string.Join(',', TShock.Players.Where(x => x is { Active: true }).Select(x => x.Name)));
                    }

                    var onlineProcessList = Utils.GetOnlineProcessList();
                    var onlineProcess = !onlineProcessList.Any() ? "已毕业" : onlineProcessList.ElementAt(0) + "前";
                    
                    packetWriter.SetType("online")
                        .Write("result", onlineResult.ToString()) // “怎么有种我是男的的感觉” -- 张芷睿大人 (24.12.22)
                        .Write("worldname", string.IsNullOrEmpty(Main.worldName) ? "地图还没加载捏~" : Main.worldName)
                        .Write("process", onlineProcess)
                        .Send();
                    break;
                case "process":
                    packetWriter.SetType("process")
                        .Write("process", Utils.GetProcessList())
                        .Write("kill_counts", Utils.GetKillCountList())
                        .Write("worldname", Main.worldName)
                        .Write("drunk_world", Main.drunkWorld)
                        .Write("zenith_world", Main.zenithWorld)
                        .Write("world_icon", Utils.GetWorldIconName())
                        .Send();
                    break;
                case "whitelist":
                    var name = (string) jsonObject["name"]!;
                    var code = (int) jsonObject["code"]!;

                    WhiteListCaches[name] = (DateTime.Now, code);

                    if (Login.CheckWhite(name, code))
                    {
                        var plr = TShock.Players.FirstOrDefault(x => x.Name == name);
                        if (plr != null)
                        {
                            Login.HandleLogin(plr);
                        }
                    }

                    break;
                case "selfkick":
                    name = (string) jsonObject["name"]!;
                    var playerList2 = TSPlayer.FindByNameOrID("tsn:" + name);
                    if (playerList2.Count == 0)
                    {
                        return;
                    }

                    playerList2[0].Kick("在群中使用自踢命令.", true, saveSSI: true);
                    break;
                case "lookbag":
                    name = (string) jsonObject["name"]!;
                    var playerList3 = TSPlayer.FindByNameOrID("tsn:" + name);
                    if (playerList3.Count != 0)
                    {
                        var plr = playerList3[0].TPlayer;
                        var lookOnlineResult = LookBag.LookOnline(plr);
                        packetWriter.SetType("lookbag")
                            .Write("name", lookOnlineResult.Name)
                            .Write("exist", 1)
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
                        var acc = TShock.UserAccounts.GetUserAccountByName(name);
                        if (acc == null)
                        {
                            packetWriter.SetType("lookbag")
                                .Write("exist", 0)
                                .Send();
                            return;
                        }

                        var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), acc.ID);
                        if (data == null)
                        {
                            packetWriter.SetType("lookbag")
                                .Write("exist", 0)
                                .Send();
                            return;
                        }

                        var lookOnlineResult = LookBag.LookOffline(acc, data);
                        packetWriter.SetType("lookbag")
                                .Write("name", lookOnlineResult.Name )
                                .Write("exist", 1 )
                                .Write("life", $"{lookOnlineResult.Health}/{lookOnlineResult.MaxHealth}")
                                .Write("mana", $"{lookOnlineResult.Mana}/{lookOnlineResult.MaxMana}")
                                .Write("quests_completed", lookOnlineResult.QuestsCompleted)
                                .Write("inventory", lookOnlineResult.ItemList)
                                .Write("buffs", lookOnlineResult.Buffs)
                                .Write("enhances", lookOnlineResult.Enhances )
                                .Write("economic", EconomicData.GetEconomicData(lookOnlineResult.Name))
                                .Send();
                    }
                    break;
                case "mappng":
                    var bitmap = MapGenerator.CreateMapImg();
                    using (MemoryStream ms = new ())
                    {
                        await bitmap.SaveAsync(ms, new PngEncoder());
                        var imageBytes = ms.ToArray();
                        var base64 = Convert.ToBase64String(imageBytes);
                        packetWriter.SetType("mappngV2")
                            .Write("result", Utils.CompressBase64(base64))
                            .Send();
                    }
                    break;
                case "mapfile":
                    var mapfile = MapGenerator.CreateMapFile();
                    packetWriter.SetType("mapfileV2")
                        .Write("name", mapfile.Item2)
                        .Write("base64", Utils.CompressBase64(mapfile.Item1) )
                        .Send();
                    
                    break;
                case "worldfile":
                    packetWriter.SetType("worldfileV2")
                        .Write("name", Path.GetFileName(Main.worldPathName))
                        .Write("base64", Utils.CompressBase64(Utils.FileToBase64String(Main.worldPathName)) )
                        .Send();
                    
                    break;
                case "pluginlist":
                    var pluginList = ServerApi.Plugins.Select(p => new PluginInfo(p.Plugin.Name, p.Plugin.Description, p.Plugin.Author, p.Plugin.Version)).ToList();
                    packetWriter.SetType("pluginlist")
                        .Write("plugins", pluginList)
                        .Send();
                    break;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError("[CaiBotLite] 处理BOT数据包时出错:\n"+ex+$"\n源数据包: {receivedData}");
        }
    }
    
        
}