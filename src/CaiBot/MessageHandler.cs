using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rests;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CaiBot;

public static class MessageHandle
{
    public static bool IsWebsocketConnected =>
        Plugin.WebSocket.State == WebSocketState.Open;

    public static async Task SendDateAsync(string message)
    {
        if (Program.LaunchParameters.ContainsKey("-caidebug"))
        {
            TShock.Log.ConsoleInfo($"[CaiAPI]发送BOT数据包：{message}");
        }

        var messageBytes = Encoding.UTF8.GetBytes(message);
        await Plugin.WebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    private static string FileToBase64String(string path)
    {
        FileStream fsForRead = new(path, FileMode.Open); //文件路径
        var base64Str = "";
        try
        {
            fsForRead.Seek(0, SeekOrigin.Begin);
            var bs = new byte[fsForRead.Length];
            var log = Convert.ToInt32(fsForRead.Length);
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

    public static string CompressBase64(string base64String)
    {
        var base64Bytes = Encoding.UTF8.GetBytes(base64String);
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(base64Bytes, 0, base64Bytes.Length);
            }
            return Convert.ToBase64String(outputStream.ToArray());
        }
    }

    public static async Task HandleMessageAsync(string receivedData)
    {
        var jsonObject = JObject.Parse(receivedData);
        var type = (string) jsonObject["type"]!;
        RestObject result;
        switch (type)
        {
            case "delserver":
                TShock.Log.ConsoleInfo("[CaiAPI]BOT发送解绑命令...");
                Config.config.Token = "";
                Config.config.Write();
                Random rnd = new();
                Plugin.InitCode = rnd.Next(10000000, 99999999);
                TShock.Log.ConsoleError($"[CaiBot]您的服务器绑定码为: {Plugin.InitCode}");
                break;
            case "hello":
                TShock.Log.ConsoleInfo("[CaiAPI]CaiBOT连接成功...");
                //发送服务器信息
                result = new RestObject
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
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "groupid":
                var groupId = (long) jsonObject["groupid"]!;
                TShock.Log.ConsoleInfo($"[CaiAPI]群号获取成功: {groupId}");
                if (Config.config.GroupNumber != 0L)
                {
                    TShock.Log.ConsoleWarn($"[CaiAPI]检测到你在配置文件中已设置群号[{Config.config.GroupNumber}],BOT自动获取的群号将被忽略！");
                }
                else
                {
                    Config.config.GroupNumber = groupId;
                }

                break;
            case "cmd":
                var cmd = (string) jsonObject["cmd"]!;
                CaiBotPlayer tr = new();
                Commands.HandleCommand(tr, cmd);
                result = new RestObject
                {
                    { "type", "cmd" },
                    { "result", string.Join('\n', tr.GetCommandOutput()) },
                    { "at", (string)jsonObject["at"]! },
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "online":
                var onlineResult = "";
                if (TShock.Utils.GetActivePlayerCount() == 0)
                {
                    onlineResult = "当前没有玩家在线捏";
                }
                else
                {
                    onlineResult += $"在线的玩家({TShock.Utils.GetActivePlayerCount()}/{TShock.Config.Settings.MaxSlots})\n";


                    List<string> players = new();

                    foreach (var ply in TShock.Players)
                    {
                        if (ply is { Active: true })
                        {
                            players.Add(ply.Name);
                        }
                    }

                    onlineResult += string.Join(',', players);
                }

                List<string> onlineProcessList = new();

                #region 进度查询

                if (!NPC.downedSlimeKing)
                {
                    onlineProcessList.Add("史王");
                }

                if (!NPC.downedBoss1)
                {
                    onlineProcessList.Add("克眼");
                }

                if (!NPC.downedBoss2)
                {
                    if (Main.drunkWorld)
                    {
                        onlineProcessList.Add("世吞/克脑");
                    }
                    else
                    {
                        if (WorldGen.crimson)
                        {
                            onlineProcessList.Add("克脑");
                        }
                        else
                        {
                            onlineProcessList.Add("世吞");
                        }
                    }
                }

                if (!NPC.downedBoss3)
                {
                    onlineProcessList.Add("骷髅王");
                }

                if (!Main.hardMode)
                {
                    onlineProcessList.Add("血肉墙");
                }

                if (!NPC.downedMechBoss2 || !NPC.downedMechBoss1 || !NPC.downedMechBoss3)
                {
                    if (Main.zenithWorld)
                    {
                        onlineProcessList.Add("美杜莎");
                    }
                    else
                    {
                        onlineProcessList.Add("新三王");
                    }
                }

                if (!NPC.downedPlantBoss)
                {
                    onlineProcessList.Add("世花");
                }

                if (!NPC.downedGolemBoss)
                {
                    onlineProcessList.Add("石巨人");
                }

                if (!NPC.downedAncientCultist)
                {
                    onlineProcessList.Add("拜月教徒");
                }

                if (!NPC.downedTowers)
                {
                    onlineProcessList.Add("四柱");
                }

                if (!NPC.downedMoonlord)
                {
                    onlineProcessList.Add("月总");
                }

                string onlineProcess;
                onlineProcess = !onlineProcessList.Any() ? "已毕业" : onlineProcessList.ElementAt(0) + "前";

                #endregion

                result = new RestObject
                {
                    { "type", "online" },
                    { "result", onlineResult },
                    { "worldname", Main.worldName },
                    { "process", onlineProcess },
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "process":
                List<Dictionary<string, bool>> processList = new(
                    new Dictionary<string, bool>[]
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
                result = new RestObject
                {
                    { "type", "process" },
                    { "result", processList },
                    { "worldname", Main.worldName },
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "whitelist":
                var name = (string) jsonObject["name"]!;
                var code = (int) jsonObject["code"]!;
                var uuids = jsonObject["uuids"]!.ToObject<List<string>>()!;
                if (await Login.CheckWhiteAsync(name, code, uuids))
                {
                    var playerList = TSPlayer.FindByNameOrID("tsn:" + name);
                    if (playerList.Count == 0)
                    {
                        return;
                    }

                    var plr = playerList[0];
                    Login.HandleLogin(plr, Guid.NewGuid().ToString());
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
            case "mappng":
                var bitmap = MapGenerator.Create();
                using (MemoryStream ms = new())
                {
                    bitmap.Save(ms, new PngEncoder());
                    var imageBytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(imageBytes);
                    result = new RestObject
                    {
                        { "type", "mappngV2" },
                        { "result", CompressBase64(base64) },
                        { "group", (long)jsonObject["group"]! }
                    };
                }
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "lookbag":
                name = (string) jsonObject["name"]!;
                var playerList3 = TSPlayer.FindByNameOrID("tsn:" + name);
                List<int> buffs;
                if (playerList3.Count != 0)
                {
                    #region 查背包 在线

                    // 在线
                    var plr = playerList3[0].TPlayer;
                    buffs = plr.buffType.ToList();
                    var netItems = new NetItem[NetItem.MaxInventory];
                    var inventory = plr.inventory;
                    var armor = plr.armor;
                    var dye = plr.dye;
                    var miscEqups = plr.miscEquips;
                    var miscDyes = plr.miscDyes;
                    var piggy = plr.bank.item;
                    var safe = plr.bank2.item;
                    var forge = plr.bank3.item;
                    var voidVault = plr.bank4.item;
                    var trash = plr.trashItem;
                    var loadout1Armor = plr.Loadouts[0].Armor;
                    var loadout1Dye = plr.Loadouts[0].Dye;
                    var loadout2Armor = plr.Loadouts[1].Armor;
                    var loadout2Dye = plr.Loadouts[1].Dye;
                    var loadout3Armor = plr.Loadouts[2].Armor;
                    var loadout3Dye = plr.Loadouts[2].Dye;
                    for (var i = 0; i < NetItem.MaxInventory; i++)
                    {
                        if (i < NetItem.InventoryIndex.Item2)
                        {
                            //0-58
                            netItems[i] = (NetItem) inventory[i];
                        }
                        else if (i < NetItem.ArmorIndex.Item2)
                        {
                            //59-78
                            var index = i - NetItem.ArmorIndex.Item1;
                            netItems[i] = (NetItem) armor[index];
                        }
                        else if (i < NetItem.DyeIndex.Item2)
                        {
                            //79-88
                            var index = i - NetItem.DyeIndex.Item1;
                            netItems[i] = (NetItem) dye[index];
                        }
                        else if (i < NetItem.MiscEquipIndex.Item2)
                        {
                            //89-93
                            var index = i - NetItem.MiscEquipIndex.Item1;
                            netItems[i] = (NetItem) miscEqups[index];
                        }
                        else if (i < NetItem.MiscDyeIndex.Item2)
                        {
                            //93-98
                            var index = i - NetItem.MiscDyeIndex.Item1;
                            netItems[i] = (NetItem) miscDyes[index];
                        }
                        else if (i < NetItem.PiggyIndex.Item2)
                        {
                            //98-138
                            var index = i - NetItem.PiggyIndex.Item1;
                            netItems[i] = (NetItem) piggy[index];
                        }
                        else if (i < NetItem.SafeIndex.Item2)
                        {
                            //138-178
                            var index = i - NetItem.SafeIndex.Item1;
                            netItems[i] = (NetItem) safe[index];
                        }
                        else if (i < NetItem.TrashIndex.Item2)
                        {
                            //179-219
                            netItems[i] = (NetItem) trash;
                        }
                        else if (i < NetItem.ForgeIndex.Item2)
                        {
                            //220
                            var index = i - NetItem.ForgeIndex.Item1;
                            netItems[i] = (NetItem) forge[index];
                        }
                        else if (i < NetItem.VoidIndex.Item2)
                        {
                            //220
                            var index = i - NetItem.VoidIndex.Item1;
                            netItems[i] = (NetItem) voidVault[index];
                        }
                        else if (i < NetItem.Loadout1Armor.Item2)
                        {
                            var index = i - NetItem.Loadout1Armor.Item1;
                            netItems[i] = (NetItem) loadout1Armor[index];
                        }
                        else if (i < NetItem.Loadout1Dye.Item2)
                        {
                            var index = i - NetItem.Loadout1Dye.Item1;
                            netItems[i] = (NetItem) loadout1Dye[index];
                        }
                        else if (i < NetItem.Loadout2Armor.Item2)
                        {
                            var index = i - NetItem.Loadout2Armor.Item1;
                            netItems[i] = (NetItem) loadout2Armor[index];
                        }
                        else if (i < NetItem.Loadout2Dye.Item2)
                        {
                            var index = i - NetItem.Loadout2Dye.Item1;
                            netItems[i] = (NetItem) loadout2Dye[index];
                        }
                        else if (i < NetItem.Loadout3Armor.Item2)
                        {
                            var index = i - NetItem.Loadout3Armor.Item1;
                            netItems[i] = (NetItem) loadout3Armor[index];
                        }
                        else if (i < NetItem.Loadout3Dye.Item2)
                        {
                            var index = i - NetItem.Loadout3Dye.Item1;
                            netItems[i] = (NetItem) loadout3Dye[index];
                        }
                    }

                    List<List<int>> itemList = new();
                    foreach (var i in netItems)
                    {
                        itemList.Add(new List<int> { i.NetId, i.Stack });
                    }

                    #endregion

                    result = new RestObject
                    {
                        { "type", "lookbag" },
                        { "name", name },
                        { "exist", 1 },
                        { "inventory", itemList },
                        { "buffs", buffs },
                        { "group", (long)jsonObject["group"]! }
                    };
                    await SendDateAsync(JsonConvert.SerializeObject(result));
                }
                else
                {
                    #region 查背包 离线

                    var acc = TShock.UserAccounts.GetUserAccountByName(name);
                    if (acc == null)
                    {
                        result = new RestObject
                        {
                            { "type", "lookbag" },
                            { "exist", 0 }
                        };
                        await SendDateAsync(JsonConvert.SerializeObject(result));
                        return;
                    }

                    var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), acc.ID);

                    if (data == null)
                    {
                        result = new RestObject
                        {
                            { "type", "lookbag" },
                            { "exist", 0 }
                        };
                        await SendDateAsync(JsonConvert.SerializeObject(result));
                        return;
                    }

                    List<List<int>> itemList = new();
                    foreach (var i in data.inventory)
                    {
                        itemList.Add(new List<int> { i.NetId, i.Stack });
                    }

                    buffs = Utils.GetActiveBuffs(TShock.DB, acc.ID, acc.Name);

                    #endregion

                    result = new RestObject
                    {
                        { "type", "lookbag" },
                        { "exist", 1 },
                        { "name", name },
                        { "inventory", itemList },
                        { "buffs", buffs },
                        { "group", (long)jsonObject["group"]! }
                    };
                    await SendDateAsync(JsonConvert.SerializeObject(result));
                }

                break;
            case "mapfile":
                CreateMapFile.Instance.Init();
                var info = CreateMapFile.Instance.Start();
                var mapfileBase64 = Convert.ToBase64String(info.Buffer);
                result = new RestObject
                {
                    { "type", "mapfileV2" },
                    { "base64", CompressBase64(mapfileBase64) },
                    { "name", info.Name },
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
            case "worldfile":
                result = new RestObject
                {
                    { "type", "worldfileV2" },
                    { "name", Main.worldPathName },
                    { "base64",CompressBase64(FileToBase64String(Main.worldPathName)) },
                    { "group", (long)jsonObject["group"]! }
                };
                await SendDateAsync(JsonConvert.SerializeObject(result));
                break;
        }
    }
}