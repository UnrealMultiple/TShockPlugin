using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using Rests;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using Terraria;
using Terraria.Map;
using TShockAPI;
using TShockAPI.DB;

namespace CaiBot;

public class MessageHandle
{
    public static async Task SendDateAsync(string message)
    {
        if (Program.LaunchParameters.ContainsKey("-caidebug"))
            TShock.Log.ConsoleInfo($"[CaiAPI]发送BOT数据包：{message}");
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await Plugin.WebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    public static bool IsWebsocketConnected =>
        Plugin.WebSocket.State == WebSocketState.Open;

    public static string FileToBase64String(string path)
    {
        FileStream fsForRead = new(path, FileMode.Open); //文件路径
        string base64Str = "";
        try
        {
            fsForRead.Seek(0, SeekOrigin.Begin);
            byte[] bs = new byte[fsForRead.Length];
            int log = Convert.ToInt32(fsForRead.Length);
            fsForRead.Read(bs, 0, log);
            base64Str = Convert.ToBase64String(bs);
            return base64Str;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            Console.ReadLine();
            return base64Str;
        }
        finally
        {
            fsForRead.Close();
        }
    }

    public static async Task HandleMessageAsync(string receivedData)
    {
        JObject jsonObject = JObject.Parse(receivedData);

        // 获取 JSON 属性的值
        string type = (string)jsonObject["type"]!;
        switch (type)
        {
            case "delserver":
                TShock.Log.ConsoleInfo("[CaiAPI]BOT发送解绑命令...");
                Config.config!.Token = "";
                Config.config.Write();
                Random rnd = new();
                Plugin.InitCode = rnd.Next(10000000, 99999999);
                TShock.Log.ConsoleError($"[CaiBot]您的服务器绑定码为: {Plugin.InitCode}");
                break;
            case "hello":
                TShock.Log.ConsoleInfo("[CaiAPI]CaiBOT连接成功...");
                //发送服务器信息
                RestObject? serverInfo = new()
                {
                    { "type", "hello" },
                    { "tshock_version", TShock.VersionNum.ToString() },
                    { "plugin_version", Plugin.VersionNum },
                    { "terraria_version", Main.versionNumber },
                    { "cai_whitelist", Config.config.WhiteList },
                    { "os", RuntimeInformation.RuntimeIdentifier },
                    {
                        "world",
                        TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName
                    },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(serverInfo.ToJson());
                break;
            case "groupid":
                long groupId = (long)jsonObject["groupid"];
                TShock.Log.ConsoleInfo($"[CaiAPI]群号获取成功: {groupId}");
                if (Config.config.GroupNumber != 0L)
                    TShock.Log.ConsoleWarn($"[CaiAPI]检测到你在配置文件中已设置群号[{Config.config.GroupNumber}],BOT自动获取的群号将被忽略！");
                else
                    Config.config.GroupNumber = groupId;

                break;
            case "cmd":
                string cmd = (string)jsonObject["cmd"];

                CaiBotPlayer tr = new();
                Commands.HandleCommand(tr, cmd);
                RestObject? re = new()
                {
                    { "type", "cmd" },
                    { "result", string.Join('\n', tr.GetCommandOutput()) },
                    { "at", (string)jsonObject["at"] },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(re.ToJson());
                break;
            case "online":
                string result = "";
                if (TShock.Utils.GetActivePlayerCount() == 0)
                {
                    result = "当前没有玩家在线捏";
                }
                else
                {
                    result += $"在线的玩家({TShock.Utils.GetActivePlayerCount()}/{TShock.Config.Settings.MaxSlots})\n";


                    List<string>? players = new();

                    foreach (TSPlayer ply in TShock.Players)
                        if (ply != null && ply.Active)
                            players.Add(ply.Name);

                    result += string.Join(',', players);
                }

                List<string> list = new();
                List<string> list2 = new();

                #region 进度查询

                if (NPC.downedSlimeKing)
                    list.Add("史王");
                else
                    list2.Add("史王");

                if (NPC.downedBoss1)
                    list.Add("克眼");
                else
                    list2.Add("克眼");

                if (NPC.downedBoss2)
                    list.Add("世吞/克脑");
                else
                    list2.Add("世吞/克脑");

                if (NPC.downedBoss3)
                    list.Add("骷髅王");
                else
                    list2.Add("骷髅王");

                if (Main.hardMode)
                    list.Add("血肉墙");
                else
                    list2.Add("血肉墙");


                if (NPC.downedMechBoss2 && NPC.downedMechBoss1 && NPC.downedMechBoss3)
                    list.Add("新三王");

                else
                    list2.Add("新三王");

                if (NPC.downedPlantBoss)
                    list.Add("世花");
                else
                    list2.Add("世花");

                if (NPC.downedGolemBoss)
                    list.Add("石巨人");
                else
                    list2.Add("石巨人");

                if (NPC.downedAncientCultist)
                    list.Add("拜月教徒");
                else
                    list2.Add("拜月教徒");


                if (NPC.downedMoonlord)
                    list.Add("月总");
                else
                    list2.Add("月总");

                string process = "";
                if (!list2.Any() || list2 == null)
                    process = "已毕业";
                else
                    process = list2.ElementAt(0) + "前";

                #endregion

                re = new RestObject
                {
                    { "type", "online" },
                    { "result", result },
                    { "worldname", Main.worldName },
                    { "process", process },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(re.ToJson());
                break;
            case "process":
                List<Dictionary<string, bool>> processList = new(
                    new Dictionary<string, bool>[21]
                    {
                        new()
                        {
                            {
                                "King Slime",
                                NPC.downedSlimeKing
                            }
                        },
                        new()
                        {
                            {
                                "Eye of Cthulhu",
                                NPC.downedBoss1
                            }
                        },
                        new()
                        {
                            {
                                "Eater of Worlds / Brain of Cthulhu",
                                NPC.downedBoss2
                            }
                        },
                        new()
                        {
                            {
                                "Queen Bee",
                                NPC.downedQueenBee
                            }
                        },
                        new()
                        {
                            {
                                "Skeletron",
                                NPC.downedBoss3
                            }
                        },
                        new()
                        {
                            {
                                "Deerclops",
                                NPC.downedDeerclops
                            }
                        },
                        new()
                        {
                            {
                                "Wall of Flesh",
                                Main.hardMode
                            }
                        },
                        new()
                        {
                            {
                                "Queen Slime",
                                NPC.downedQueenSlime
                            }
                        },
                        new()
                        {
                            {
                                "The Twins",
                                NPC.downedMechBoss2
                            }
                        },
                        new()
                        {
                            {
                                "The Destroyer",
                                NPC.downedMechBoss1
                            }
                        },
                        new()
                        {
                            {
                                "Skeletron Prime",
                                NPC.downedMechBoss3
                            }
                        },
                        new()
                        {
                            {
                                "Plantera",
                                NPC.downedPlantBoss
                            }
                        },
                        new()
                        {
                            {
                                "Golem",
                                NPC.downedGolemBoss
                            }
                        },
                        new()
                        {
                            {
                                "Duke Fishron",
                                NPC.downedFishron
                            }
                        },
                        new()
                        {
                            {
                                "Empress of Light",
                                NPC.downedEmpressOfLight
                            }
                        },
                        new()
                        {
                            {
                                "Lunatic Cultist",
                                NPC.downedAncientCultist
                            }
                        },
                        new()
                        {
                            {
                                "Moon Lord",
                                NPC.downedMoonlord
                            }
                        },
                        new()
                        {
                            {
                                "Solar Pillar",
                                NPC.downedTowerSolar
                            }
                        },
                        new()
                        {
                            {
                                "Nebula Pillar",
                                NPC.downedTowerNebula
                            }
                        },
                        new()
                        {
                            {
                                "Vortex Pillar",
                                NPC.downedTowerVortex
                            }
                        },
                        new()
                        {
                            {
                                "Stardust Pillar",
                                NPC.downedTowerStardust
                            }
                        }
                    });
                re = new RestObject
                {
                    { "type", "process" },
                    { "result", processList },
                    { "worldname", Main.worldName },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(re.ToJson());
                break;
            case "whitelist":
                string name = (string)jsonObject["name"];
                int code = (int)jsonObject["code"];
                List<string> uuids = jsonObject["uuids"].ToObject<List<string>>();
                if (await Login.CheckWhiteAsync(name, code, uuids))
                {
                    List<TSPlayer> playerList = TSPlayer.FindByNameOrID("tsn:" + name);
                    if (playerList.Count == 0) return;

                    TSPlayer plr = playerList[0];
                    Login.HandleLogin(plr, Guid.NewGuid().ToString());
                }

                break;
            case "selfkick":
                name = (string)jsonObject["name"];
                List<TSPlayer> playerList2 = TSPlayer.FindByNameOrID("tsn:" + name);
                if (playerList2.Count == 0) return;
                playerList2[0].Kick("在群中使用自踢命令.", true, saveSSI: true);
                break;
            case "mappng":
                Image bitmap = MapGenerator.Create();
                using (MemoryStream ms = new())
                {
                    bitmap.Save(ms, new PngEncoder());
                    byte[] imageBytes = ms.ToArray();
                    string base64 = Convert.ToBase64String(imageBytes);
                    re = new RestObject
                    {
                        { "type", "mappng" },
                        { "result", base64 },
                        { "group", (long)jsonObject["group"] }
                    };
                }

                await SendDateAsync(re.ToJson());
                break;
            case "lookbag":
                name = (string)jsonObject["name"];
                List<TSPlayer> playerList3 = TSPlayer.FindByNameOrID("tsn:" + name);
                List<int> buffs = new();
                if (playerList3.Count != 0)
                {
                    #region 查背包 在线

                    // 在线
                    Player plr = playerList3[0].TPlayer;
                    buffs = plr.buffType.ToList();
                    NetItem[] invs = new NetItem[NetItem.MaxInventory];
                    Item[] inventory = plr.inventory;
                    Item[] armor = plr.armor;
                    Item[] dye = plr.dye;
                    Item[] miscEqups = plr.miscEquips;
                    Item[] miscDyes = plr.miscDyes;
                    Item[] piggy = plr.bank.item;
                    Item[] safe = plr.bank2.item;
                    Item[] forge = plr.bank3.item;
                    Item[] voidVault = plr.bank4.item;
                    Item trash = plr.trashItem;
                    Item[] loadout1Armor = plr.Loadouts[0].Armor;
                    Item[] loadout1Dye = plr.Loadouts[0].Dye;
                    Item[] loadout2Armor = plr.Loadouts[1].Armor;
                    Item[] loadout2Dye = plr.Loadouts[1].Dye;
                    Item[] loadout3Armor = plr.Loadouts[2].Armor;
                    Item[] loadout3Dye = plr.Loadouts[2].Dye;
                    for (int i = 0; i < NetItem.MaxInventory; i++)
                        if (i < NetItem.InventoryIndex.Item2)
                        {
                            //0-58
                            invs[i] = (NetItem)inventory[i];
                        }
                        else if (i < NetItem.ArmorIndex.Item2)
                        {
                            //59-78
                            int index = i - NetItem.ArmorIndex.Item1;
                            invs[i] = (NetItem)armor[index];
                        }
                        else if (i < NetItem.DyeIndex.Item2)
                        {
                            //79-88
                            int index = i - NetItem.DyeIndex.Item1;
                            invs[i] = (NetItem)dye[index];
                        }
                        else if (i < NetItem.MiscEquipIndex.Item2)
                        {
                            //89-93
                            int index = i - NetItem.MiscEquipIndex.Item1;
                            invs[i] = (NetItem)miscEqups[index];
                        }
                        else if (i < NetItem.MiscDyeIndex.Item2)
                        {
                            //93-98
                            int index = i - NetItem.MiscDyeIndex.Item1;
                            invs[i] = (NetItem)miscDyes[index];
                        }
                        else if (i < NetItem.PiggyIndex.Item2)
                        {
                            //98-138
                            int index = i - NetItem.PiggyIndex.Item1;
                            invs[i] = (NetItem)piggy[index];
                        }
                        else if (i < NetItem.SafeIndex.Item2)
                        {
                            //138-178
                            int index = i - NetItem.SafeIndex.Item1;
                            invs[i] = (NetItem)safe[index];
                        }
                        else if (i < NetItem.TrashIndex.Item2)
                        {
                            //179-219
                            invs[i] = (NetItem)trash;
                        }
                        else if (i < NetItem.ForgeIndex.Item2)
                        {
                            //220
                            int index = i - NetItem.ForgeIndex.Item1;
                            invs[i] = (NetItem)forge[index];
                        }
                        else if (i < NetItem.VoidIndex.Item2)
                        {
                            //220
                            int index = i - NetItem.VoidIndex.Item1;
                            invs[i] = (NetItem)voidVault[index];
                        }
                        else if (i < NetItem.Loadout1Armor.Item2)
                        {
                            int index = i - NetItem.Loadout1Armor.Item1;
                            invs[i] = (NetItem)loadout1Armor[index];
                        }
                        else if (i < NetItem.Loadout1Dye.Item2)
                        {
                            int index = i - NetItem.Loadout1Dye.Item1;
                            invs[i] = (NetItem)loadout1Dye[index];
                        }
                        else if (i < NetItem.Loadout2Armor.Item2)
                        {
                            int index = i - NetItem.Loadout2Armor.Item1;
                            invs[i] = (NetItem)loadout2Armor[index];
                        }
                        else if (i < NetItem.Loadout2Dye.Item2)
                        {
                            int index = i - NetItem.Loadout2Dye.Item1;
                            invs[i] = (NetItem)loadout2Dye[index];
                        }
                        else if (i < NetItem.Loadout3Armor.Item2)
                        {
                            int index = i - NetItem.Loadout3Armor.Item1;
                            invs[i] = (NetItem)loadout3Armor[index];
                        }
                        else if (i < NetItem.Loadout3Dye.Item2)
                        {
                            int index = i - NetItem.Loadout3Dye.Item1;
                            invs[i] = (NetItem)loadout3Dye[index];
                        }

                    List<List<int>>? itemList = new();
                    foreach (NetItem i in invs) itemList.Add(new List<int>() { i.NetId, i.Stack });

                    #endregion

                    re = new RestObject
                    {
                        { "type", "lookbag" },
                        { "name", name },
                        { "exist", 1 },
                        { "inventory", itemList },
                        { "buffs", buffs },
                        { "group", (long)jsonObject["group"] }
                    };
                    await SendDateAsync(re.ToJson());
                    return;
                }
                else
                {
                    #region 查背包 离线

                    UserAccount? acc = TShock.UserAccounts.GetUserAccountByName(name);
                    if (acc == null)
                    {
                        re = new RestObject
                        {
                            { "type", "lookbag" },
                            { "exist", 0 }
                        };
                        await SendDateAsync(re.ToJson());
                        return;
                    }

                    PlayerData? data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), acc.ID);

                    if (data == null)
                    {
                        re = new RestObject
                        {
                            { "type", "lookbag" },
                            { "exist", 0 }
                        };
                        await SendDateAsync(re.ToJson());
                        return;
                    }

                    List<List<int>>? itemList = new();
                    foreach (NetItem i in data.inventory) itemList.Add(new List<int>() { i.NetId, i.Stack });

                    buffs = Utils.GetActiveBuffs(TShock.DB, acc.ID, acc.Name);

                    #endregion

                    re = new RestObject
                    {
                        { "type", "lookbag" },
                        { "exist", 1 },
                        { "name", name },
                        { "inventory", itemList },
                        { "buffs", buffs },
                        { "group", (long)jsonObject["group"] }
                    };
                    await SendDateAsync(re.ToJson());
                }

                break;
            case "mapfile":
                CreateMapFile.Instance.Init();
                MapInfo info = CreateMapFile.Instance.Start();
                string mapfileBase64 = Convert.ToBase64String(info.Buffer);
                re = new RestObject
                {
                    { "type", "mapfile" },
                    { "base64", mapfileBase64 },
                    { "name", info.Name },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(re.ToJson());
                break;
            case "worldfile":
                re = new RestObject
                {
                    { "type", "worldfile" },
                    { "name", Main.worldPathName },
                    { "base64", FileToBase64String(Main.worldPathName) },
                    { "group", (long)jsonObject["group"] }
                };
                await SendDateAsync(re.ToJson());
                break;
        }
    }
}