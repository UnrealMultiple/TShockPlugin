using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using TerrariaApi.Server;
using TShockAPI;


namespace WorldModify
{
    [ApiVersion(2, 1)]
    public class WorldModify : TerrariaPlugin
    {
        public static readonly string SaveDir = Path.Combine(TShock.SavePath, "WorldModify");

        private readonly Main main;

        private readonly Utils utils;


        private static readonly Dictionary<string, int> _worldModes = new Dictionary<string, int>
    {
        { "经典", 1 },
        { "专家", 2 },
        { "大师", 3 },
        { "旅行", 4 }
    };

        public override string Author => "hufang360";

        public override string Description => "简易的世界修改器（测试版本：V20221116）";

        public override string Name => "WorldModify";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public WorldModify(Main game)
            : base(game)
        {
            main = game;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("worldmodify", WMCommand, "worldmodify", "wm")
            {
                HelpText = "简易的世界修改器"
            });
            Commands.ChatCommands.Add(new Command("moonphase", MoonHelper.ChangeMoonPhase, "moonphase", "moon")
            {
                HelpText = "月相管理"
            });
            Commands.ChatCommands.Add(new Command("moonstyle", MoonHelper.ChangeMoonStyle, "moonstyle", "ms")
            {
                HelpText = "月亮样式管理"
            });
            Commands.ChatCommands.Add(new Command("bossmanage", BossHelper.Manage, "bossmanage", "boss")
            {
                HelpText = "boss管理"
            });
            Commands.ChatCommands.Add(new Command("npcmanage", NPCHelper.Manage, "npcmanage", "npc")
            {
                HelpText = "npc管理"
            });
            Commands.ChatCommands.Add(new Command("igen", iGen.Manage, "igen")
            {
                HelpText = "建造世界"
            });
            Commands.ChatCommands.Add(new Command("worldinfo", WorldInfo, "worldinfo", "wi")
            {
                HelpText = "世界信息"
            });
            Commands.ChatCommands.Add(new Command("bossinfo", BossHelper.BossInfo, "bossinfo", "bi")
            {
                HelpText = "boss进度信息"
            });
            Commands.ChatCommands.Add(new Command("relive", NPCHelper.Relive, "relive")
            {
                HelpText = "复活NPC"
            });
            Commands.ChatCommands.Add(new Command("cleartomb", ClearToolWM.ClearTomb, "cleartomb", "ct")
            {
                HelpText = "清理墓碑"
            });
            Utils.SaveDir = SaveDir;
            BackupHelper.BackupPath = Utils.CombinePath("backups");
            RetileTool.SaveFile = Utils.CombinePath("retile.json");
            ResearchHelper.SaveFile = Utils.CombinePath("research.csv");
            BestiaryHelper.SaveFile = Utils.CombinePath("bestiary.csv");
        }

        private void WMCommand(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                op.SendErrorMessage("语法错误，输入 /wm help 查询用法");
                return;
            }
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help":
                    ShowHelpText();
                    break;
                default:
                    op.SendErrorMessage("语法不正确！输入 /wm help 查询用法");
                    break;
                case "info":
                    ShowWorldInfo(args, isSuperAdmin: true);
                    break;
                case "name":
                    if (args.Parameters.Count == 1)
                    {
                        op.SendInfoMessage("世界名称：" + Main.worldName + "\n输入 /wm seed <名称> 可更改世界名称");
                        break;
                    }
                    Main.worldName = args.Parameters[1];
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage("世界名称已改成 {0}", args.Parameters[1]);
                    break;
                case "mode":
                    {
                        if (args.Parameters.Count == 1)
                        {
                            op.SendInfoMessage("世界难度：" + _worldModes.Keys.ElementAt(Main.GameMode) + "\n用法：/wm mode <难度>\n可用的难度：" + string.Join(", ", _worldModes.Keys));
                            break;
                        }
                        if (int.TryParse(args.Parameters[1], out var result4))
                        {
                            if (result4 < 1 || result4 > 4)
                            {
                                op.SendErrorMessage("语法错误！用法：/wm mode <难度>\n可用的难度：" + string.Join(", ", _worldModes.Keys));
                                break;
                            }
                        }
                        else
                        {
                            if (!_worldModes.ContainsKey(args.Parameters[1]))
                            {
                                op.SendErrorMessage("语法错误！用法：/wm mode <难度>\n可用的难度：" + string.Join(", ", _worldModes.Keys));
                                break;
                            }
                            result4 = _worldModes[args.Parameters[1]];
                        }
                        Main.GameMode = result4 - 1;
                        TSPlayer.All.SendData((PacketTypes)7);
                        op.SendSuccessMessage("世界模式已改成 {0}", _worldModes.Keys.ElementAt(result4 - 1));
                        break;
                    }
                case "seed":
                    if (args.Parameters.Count == 1)
                    {
                        op.SendInfoMessage($"世界种子：{WorldGen.currentWorldSeed}（{Main.ActiveWorldFileData.GetFullSeedText(false)}）\n输入 /wm seed <种子> 可更改世界种子");
                    }
                    else
                    {
                        Main.ActiveWorldFileData.SetSeed(args.Parameters[1]);
                        TSPlayer.All.SendData((PacketTypes)7);
                        op.SendSuccessMessage("世界种子已改成 {0}", args.Parameters[1]);
                    }
                    break;
                case "id":
                    {
                        int result;
                        if (args.Parameters.Count == 1)
                        {
                            op.SendInfoMessage($"世界ID：{Main.worldID}\n输入 /wm id <id> 可更改世界ID");
                        }
                        else if (int.TryParse(args.Parameters[1], out result))
                        {
                            Main.worldID = result;
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("世界的ID已改成 {0}", args.Parameters[1]);
                        }
                        else
                        {
                            op.SendErrorMessage("世界ID只能由数字组成");
                        }
                        break;
                    }
                case "uuid":
                    {
                        if (args.Parameters.Count == 1)
                        {
                            op.SendInfoMessage($"uuid：{Main.ActiveWorldFileData.UniqueId}\n输入 /wm uuid <uuid> 可更改世界的uuid");
                            break;
                        }
                        string text7 = args.Parameters[1].ToLower();
                        if (text7 == "new")
                        {
                            Main.ActiveWorldFileData.UniqueId = Guid.NewGuid();
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("世界的UUID已改成 {0}", Main.ActiveWorldFileData.UniqueId);
                        }
                        else if (Utils.StringToGuid(text7))
                        {
                            Main.ActiveWorldFileData.UniqueId = new Guid(text7);
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("世界的UUID已改成 {0}", text7);
                        }
                        else
                        {
                            op.SendErrorMessage("uuid格式不正确！");
                        }
                        break;
                    }
                case "sd":
                case "sundial":
                    {
                        if (args.Parameters.Count == 1)
                        {
                            string text2 = GetSundial();
                            if (string.IsNullOrEmpty(text2))
                            {
                                text2 = "附魔日晷：无";
                            }
                            op.SendInfoMessage(text2 + "\n输入 /wm sundial <天数> 可修改修改附魔日晷冷却天数\n输入 /wm sundial <on/off> 可开关附魔日晷");
                            break;
                        }
                        string text5 = args.Parameters[1].ToLowerInvariant();
                        string text6 = text5;
                        if (!(text6 == "on"))
                        {
                            int result5;
                            if (text6 == "off")
                            {
                                if (Main.fastForwardTimeToDawn)
                                {
                                    Main.fastForwardTimeToDawn = false;
                                    TSPlayer.All.SendData((PacketTypes)7);
                                    op.SendSuccessMessage("附魔日晷已关闭");
                                }
                                else
                                {
                                    op.SendSuccessMessage("附魔日晷 已是关闭状态");
                                }
                            }
                            else if (int.TryParse(args.Parameters[1], out result5))
                            {
                                Main.sundialCooldown = result5;
                                TSPlayer.All.SendData((PacketTypes)7);
                                op.SendSuccessMessage("附魔日晷冷却天数 已改成 {0}", result5);
                            }
                            else
                            {
                                op.SendErrorMessage("天数输入错误！");
                            }
                        }
                        else if (!Main.fastForwardTimeToDawn)
                        {
                            Main.fastForwardTimeToDawn = true;
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("附魔日晷 已开启");
                        }
                        else
                        {
                            op.SendSuccessMessage("附魔日晷 已是开启状态");
                        }
                        break;
                    }
                case "md":
                case "moondial":
                    {
                        if (args.Parameters.Count == 1)
                        {
                            string text2 = GetMoondial();
                            if (string.IsNullOrEmpty(text2))
                            {
                                text2 = "附魔月晷：无";
                            }
                            op.SendInfoMessage(text2 + "\n输入 /wm moondial <天数> 可修改修改附魔月晷冷却天数\n输入 /wm moondial <on/off> 可开关附魔月晷");
                            break;
                        }
                        string text3 = args.Parameters[1].ToLowerInvariant();
                        string text4 = text3;
                        if (!(text4 == "on"))
                        {
                            int result2;
                            if (text4 == "off")
                            {
                                if (Main.fastForwardTimeToDusk)
                                {
                                    Main.fastForwardTimeToDusk = false;
                                    TSPlayer.All.SendData((PacketTypes)7);
                                    op.SendSuccessMessage("附魔月晷已关闭");
                                }
                                else
                                {
                                    op.SendSuccessMessage("附魔月晷 已是关闭状态");
                                }
                            }
                            else if (int.TryParse(args.Parameters[1], out result2))
                            {
                                Main.moondialCooldown = result2;
                                TSPlayer.All.SendData((PacketTypes)7);
                                op.SendSuccessMessage("附魔日晷冷却天数 已改成 {0}", result2);
                            }
                            else
                            {
                                op.SendErrorMessage("天数输入错误！");
                            }
                        }
                        else if (!Main.fastForwardTimeToDusk)
                        {
                            Main.fastForwardTimeToDusk = true;
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("附魔月晷 已开启");
                        }
                        else
                        {
                            op.SendSuccessMessage("附魔月晷 已是开启状态");
                        }
                        break;
                    }
                case "surface":
                    {
                        int result6;
                        if (args.Parameters.Count == 1)
                        {
                            op.SendInfoMessage($"表层深度：{Main.worldSurface}\n输入 /wm surface <深度> 可修改地表深度");
                        }
                        else if (int.TryParse(args.Parameters[1], out result6))
                        {
                            Main.worldSurface = result6;
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("表层深度 已改成 {0}", result6);
                        }
                        else
                        {
                            op.SendErrorMessage("深度输入错误！");
                        }
                        break;
                    }
                case "cave":
                    {
                        int result3;
                        if (args.Parameters.Count == 1)
                        {
                            op.SendInfoMessage($"洞穴深度：{Main.rockLayer}\n输入 /wm cave <深度> 可修改洞穴深度");
                        }
                        else if (int.TryParse(args.Parameters[1], out result3))
                        {
                            Main.rockLayer = result3;
                            TSPlayer.All.SendData((PacketTypes)7);
                            op.SendSuccessMessage("洞穴深度 已改成 {0}", result3);
                        }
                        else
                        {
                            op.SendErrorMessage("深度输入错误！");
                        }
                        break;
                    }
                case "spawn":
                    op.SendInfoMessage($"出生点：{Main.spawnTileX}, {Main.spawnTileY} \n进入游戏后，输入 /setspawn 设置出生点 \n进入游戏后，输入 /spawn 传送至出生点");
                    break;
                case "dungeon":
                case "dun":
                    op.SendInfoMessage($"地牢点：{Main.dungeonX}, {Main.dungeonY} \n进入游戏后，输入 /setdungeon 设置地牢点 \n进入游戏后，输入 /tpnpc \"Old Man\" 传送至地牢点");
                    break;
                case "wind":
                    op.SendInfoMessage($"风速：{Main.windSpeedCurrent}\n输入 /wind <速度> 可调节风速");
                    break;
                case "516":
                case "0516":
                case "5162020":
                case "05162020":
                case "2020":
                case "drunk":
                    Main.drunkWorld = !Main.drunkWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.drunkWorld) + " 05162020 秘密世界（醉酒世界 / DrunkWorld）");
                    break;
                case "2011":
                case "2021":
                case "5162011":
                case "5162021":
                case "05162011":
                case "05162021":
                case "celebrationmk10":
                    Main.tenthAnniversaryWorld = !Main.tenthAnniversaryWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.tenthAnniversaryWorld) + " 10周年庆典 秘密世界（05162021）");
                    break;
                case "ftw":
                case "for the worthy":
                    Main.getGoodWorld = !Main.getGoodWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.getGoodWorld) + " for the worthy 秘密世界");
                    break;
                case "ntb":
                    Main.notTheBeesWorld = !Main.notTheBeesWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.notTheBeesWorld) + " not the bees 秘密世界");
                    break;
                case "eye":
                case "dst":
                case "constant":
                    Main.dontStarveWorld = !Main.dontStarveWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.dontStarveWorld) + " 永恒领域 秘密世界（饥荒联动）");
                    break;
                case "remix":
                    Main.remixWorld = !Main.remixWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.remixWorld) + " Remix 秘密世界（don't dig up）");
                    break;
                case "nt":
                case "no traps":
                    Main.noTrapsWorld = !Main.noTrapsWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.noTrapsWorld) + " No Traps 秘密世界");
                    break;
                case "zenith":
                case "gfb":
                case "everything":
                    Main.zenithWorld = !Main.zenithWorld;
                    TSPlayer.All.SendData((PacketTypes)7);
                    op.SendSuccessMessage(Utils.BFlag(Main.zenithWorld) + " 天顶 秘密世界（getfixedboi）");
                    break;
                case "research":
                case "re":
                    ResearchHelper.Manage(args);
                    break;
                case "bestiary":
                case "be":
                    BestiaryHelper.Manage(args);
                    break;
                case "backup":
                    {
                        string notes = "";
                        if (args.Parameters.Count > 1)
                        {
                            args.Parameters.RemoveAt(0);
                            notes = string.Join(" ", args.Parameters);
                        }
                        BackupHelper.Backup(op, notes);
                        break;
                    }
                case "find":
                    FindTool.Manage(args);
                    break;
                case "clear":
                    ClearToolWM.Manage(args);
                    break;
                case "docs":
                case "refer":
                case "dump":
                    DocsHelper.GenDocs(op);
                    break;
                case "xml":
                    ResHelper.DumpXML();
                    break;
                case "debug":
                    TShock.Config.Settings.DebugLogs = !TShock.Config.Settings.DebugLogs;
                    op.SendInfoMessage($"debug模式:{TShock.Config.Settings.DebugLogs}");
                    break;
                case "reloadplugin":
                case "rp":
                    ReloadPlugin(op);
                    break;
                case "version":
                case "v":
                case "ver":
                    {
                        string filePath = "ServerPlugins/WorldModify.dll";
                        string text = Utils.GetMD5HashFromFile(filePath);
                        if (text.Length > 4)
                        {
                            text = text.Substring(0, 4);
                        }
                        op.SendInfoMessage($"{this.Name} v{this.Version}（{text}）");
                        break;
                    }
            }
            void ShowHelpText()
            {
                if (PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out var pageNumber))
                {
                    List<string> dataToPaginate = new List<string>
                {
                    "/wm info，查看 世界信息", "/wm name [世界名]，查看/修改 世界名字", "/wm mode [难度]，查看/修改 世界难度", "/wm 2020，开启/关闭 05162020 秘密世界", "/wm 2021，开启/关闭 05162021 秘密世界", "/wm ftw，开启/关闭 for the worthy 秘密世界", "/wm ntb，开启/关闭 not the bees 秘密世界", "/wm dst，开启/关闭 饥荒联动 秘密世界", "/wm remix，开启/关闭 Remix 秘密世界", "/wm nt，开启/关闭 No Traps 秘密世界",
                    "/wm zenith，开启/关闭 Zenith 秘密世界", "/wm seed [种子]，查看/修改 世界种子", "/wm id [id]，查看/修改 世界ID", "/wm uuid [uuid字符 / new]，查看/修改 世界uuid", "/wm sundial [on / off / 天数]，查看/开关 附魔日晷 / 修改 冷却天数", "/wm moondial [on / off / 天数]，查看/开关 附魔月晷 / 修改 冷却天数", "/wm spawn，查看 出生点", "/wm dungeon，查看 地牢点", "/wm surface [深度]，查看/修改 地表深度", "/wm cave [深度]，查看/修改 洞穴深度",
                    "/wm wind，查看 风速", "/wm backup [备注]，备份地图", "/wm research help，物品研究", "/wm bestiary help，怪物图鉴", "/wm clear help，全图清理指定图格", "/moon help，月相管理", "/moonstyle help，月亮样式管理", "/boss help，boss管理", "/npc help，npc管理", "/igen help，建造世界",
                    "/wm docs，生成参考文档"
                };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "帮助 ({0}/{1})：",
                        FooterFormat = "输入 /wm help {{0}} 查看更多".SFormat(Commands.Specifier)
                    });
                }
            }
        }

        private void ReloadPlugin(TSPlayer op)
        {
            string text = "ServerPlugins/WorldModify.dll";
            string text2 = "E:\\Dev-tr\\TShockWorldModify\\WorldModify\\bin\\Debug\\net6.0\\WorldModify.dll";
            if (!File.Exists(text2))
            {
                Console.WriteLine("指定位置未能找到插件文件：" + text2 + "，重载操作已取消！");
                return;
            }
            try
            {
                PluginContainer val = ServerApi.Plugins.ToList().Find((PluginContainer p) => p.Plugin.Name == this.Name);
                if (val.Initialized)
                {
                    val.DeInitialize();
                    val.Dispose();
                }
                string[] array = new string[18]
                {
                "worldmodify", "wm", "moonphase", "moon", "moonstyle", "ms", "bossmanage", "boss", "npcmanage", "npc",
                "igen", "worldinfo", "wi", "bossinfo", "bi", "relive", "cleartomb", "ct"
                };
                foreach (Command chatCommand in Commands.ChatCommands)
                {
                    string[] array2 = array;
                    foreach (string item in array2)
                    {
                        if (chatCommand.Names.Contains(item))
                        {
                            chatCommand.Names.Remove(item);
                        }
                    }
                }
            }
            catch (Exception value)
            {
                op.SendErrorMessage($"插件:{this.Name}在卸载时出错:{value}");
            }
            File.Copy(text2, text, overwrite: true);
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(text));
                Type[] exportedTypes = assembly.GetExportedTypes();
                foreach (Type type in exportedTypes)
                {
                    if (!type.IsSubclassOf(typeof(TerrariaPlugin)) || !type.IsPublic || type.IsAbstract)
                    {
                        continue;
                    }
                    object[] customAttributes = type.GetCustomAttributes(typeof(ApiVersionAttribute), inherit: false);
                    if (customAttributes.Length != 0)
                    {
                        TerrariaPlugin val2 = (TerrariaPlugin)Activator.CreateInstance(type, main);
                        PluginContainer val3 = new PluginContainer(val2);
                        val3.Initialize();
                        ServerApi.Plugins.Append(val3);
                        string text3 = Utils.GetMD5HashFromFile(text);
                        if (text3.Length > 4)
                        {
                            text3 = text3.Substring(0, 4);
                        }
                        op.SendInfoMessage($"{val3.Plugin.Name} v{val3.Plugin.Version}（{text3}）已重载！");
                    }
                }
            }
            catch (Exception value2)
            {
                Console.WriteLine(value2);
            }
        }

        private void WorldInfo(CommandArgs args)
        {
            ShowWorldInfo(args);
        }

        private void ShowWorldInfo(CommandArgs args, bool isSuperAdmin = false)
        {
            TSPlayer player = args.Player;
            List<string> list = new List<string>
        {
            "名称：" + Main.worldName,
            "大小：" + GetWorldSize(),
            "难度：" + _worldModes.Keys.ElementAt(Main.GameMode),
            "种子：" + WorldGen.currentWorldSeed
        };
            if (isSuperAdmin)
            {
                list.Add($"ID：{Main.worldID}");
                list.Add($"UUID：{Main.ActiveWorldFileData.UniqueId}");
                list.Add($"版本：{278}  {Main.versionNumber}");
            }
            string secretWorldDescription = GetSecretWorldDescription();
            if (!string.IsNullOrEmpty(secretWorldDescription))
            {
                list.Add(secretWorldDescription);
            }
            list.Add(GetCorruptionDescription(isSuperAdmin));
            HashSet<string> hashSet = new HashSet<string>();
            if (NPC.combatBookWasUsed)
            {
                hashSet.Add("[i:4382]");
            }
            if (NPC.combatBookVolumeTwoWasUsed)
            {
                hashSet.Add("[i:5336]");
            }
            if (NPC.peddlersSatchelWasUsed)
            {
                hashSet.Add("[i:5343]");
            }
            if (hashSet.Any())
            {
                list.Add("增强：" + string.Join(",", hashSet));
            }
            hashSet.Clear();
            if (Main.fastForwardTimeToDawn)
            {
                hashSet.Add("日晷生效中");
            }
            if (Main.sundialCooldown > 0)
            {
                hashSet.Add($"日晷冷却：{Main.sundialCooldown}天");
            }
            if (Main.fastForwardTimeToDusk)
            {
                hashSet.Add("月晷生效中");
            }
            if (Main.moondialCooldown > 0)
            {
                hashSet.Add($"月晷冷却：{Main.moondialCooldown}天");
            }
            if (hashSet.Any())
            {
                list.Add("时间：" + string.Join(", ", hashSet));
            }
            if (isSuperAdmin)
            {
                hashSet.Clear();
                hashSet.Add(MoonHelper.MoonPhaseDesc);
                hashSet.Add(MoonHelper.MoonTypeDesc);
                if (Main.bloodMoon)
                {
                    hashSet.Add("血月");
                }
                if (Main.eclipse)
                {
                    hashSet.Add("日食");
                }
                list.Add("月相：" + string.Join(", ", hashSet));
                hashSet.Clear();
                if (Main.raining)
                {
                    hashSet.Add("雨天");
                }
                if (Main.IsItStorming)
                {
                    hashSet.Add("雷雨天");
                }
                if (Main.IsItAHappyWindyDay)
                {
                    hashSet.Add("大风天");
                }
                if (Sandstorm.Happening)
                {
                    hashSet.Add("沙尘暴");
                }
                list.Add($"天气：云量：{Main.numClouds}  风力：{Main.windSpeedCurrent}  {string.Join("  ", hashSet)}");
                string value;
                if (TShock.ServerSideCharacterConfig.Settings.Enabled && Main.GameMode == 3)
                {
                    int sacrificeCompleted = ResearchHelper.GetSacrificeCompleted();
                    int sacrificeTotal = ResearchHelper.GetSacrificeTotal();
                    value = Terraria.Utils.PrettifyPercentDisplay((float)(sacrificeCompleted / sacrificeTotal), "P2");
                    list.Add($"物品研究：{value}（{sacrificeCompleted}/{sacrificeTotal}）");
                }
                BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
                value = Terraria.Utils.PrettifyPercentDisplay((bestiaryProgressReport).CompletionPercent, "P2");
                list.Add($"图鉴：{value}（{bestiaryProgressReport.CompletionAmountTotal}/{bestiaryProgressReport.EntriesTotal}）");
                secretWorldDescription = (DD2Event.DownedInvasionT1 ? "已通过 T1难度" : (DD2Event.DownedInvasionT2 ? "已通过 T2难度" : ((!DD2Event.DownedInvasionT3) ? "" : "已通过 T3难度")));
                if (!string.IsNullOrEmpty(secretWorldDescription))
                {
                    list.Add("撒旦军队：" + secretWorldDescription);
                }
                string text = $"（已清理{Main.invasionProgress}%波）（规模：{Main.invasionSize} ）";
                string text2 = $"（第{Main.invasionProgressWave}波：{Main.invasionProgress}%）";
                secretWorldDescription = ((Main.invasionType == 1) ? ("哥布林入侵（" + text + "）") : ((Main.invasionType == 2) ? ("霜月（" + text2 + "）") : ((Main.invasionType == 3) ? ("海盗入侵（" + text + "）") : ((Main.invasionType == 4) ? ("火星暴乱（" + text + "）") : (Main.pumpkinMoon ? ("南瓜月（" + text2 + "）") : (Main.snowMoon ? ("雪人军团（" + text + "）") : ((!DD2Event.Ongoing) ? "" : ("撒旦军队（" + text2 + "）"))))))));
                if (!string.IsNullOrEmpty(secretWorldDescription))
                {
                    list.Add("入侵：" + secretWorldDescription);
                }
                hashSet.Clear();
                if (BirthdayParty._wasCelebrating)
                {
                    hashSet.Add("派对");
                }
                if (LanternNight.LanternsUp)
                {
                    hashSet.Add("灯笼夜");
                }
                if (Star.starfallBoost > 3f)
                {
                    hashSet.Add("流星雨");
                }
                if (Main.slimeRain)
                {
                    hashSet.Add("史莱姆雨");
                }
                if (WorldGen.spawnMeteor)
                {
                    hashSet.Add("陨石");
                }
                if (hashSet.Any())
                {
                    list.Add("事件：" + string.Join(", ", hashSet));
                }
                hashSet.Clear();
                if (Main.xMas)
                {
                    hashSet.Add("圣诞节");
                }
                if (Main.halloween)
                {
                    hashSet.Add("万圣节");
                }
                if (hashSet.Any())
                {
                    list.Add("节日：" + string.Join(", ", hashSet));
                }
                hashSet.Clear();
                hashSet.Add($"表层深度：{Main.worldSurface}, 洞穴深度：{Main.rockLayer}, 出生点：{Main.spawnTileX},{Main.spawnTileY}, 地牢点：{Main.dungeonX},{Main.dungeonY}");
                if (TShock.Config.Settings.RequireLogin)
                {
                    hashSet.Add("已开启需要登录");
                }
                if (TShock.ServerSideCharacterConfig.Settings.Enabled)
                {
                    hashSet.Add("已开启SSC");
                }
                list.Add("杂项：" + string.Join(", ", hashSet));
            }
            player.SendInfoMessage(string.Join("\n", list));
        }

        private static string GetWorldSize()
        {
            if (Main.maxTilesX == 8400 && Main.maxTilesY == 2400)
            {
                return "大（8400x2400）";
            }
            if (Main.maxTilesX == 6400 && Main.maxTilesY == 1800)
            {
                return "中（6400x1800）";
            }
            if (Main.maxTilesX == 4200 && Main.maxTilesY == 1200)
            {
                return "小（4200x1200）";
            }
            return "未知";
        }

        private string GetSundial()
        {
            string text = (Main.IsFastForwardingTime() ? "生效中" : "");
            string text2 = ((Main.sundialCooldown > 0) ? $"{Main.sundialCooldown}天后可再次使用" : "");
            if (string.IsNullOrEmpty(text))
            {
                text = text2;
            }
            else if (!string.IsNullOrEmpty(text2))
            {
                text = text + " " + text2;
            }
            if (!string.IsNullOrEmpty(text))
            {
                return "附魔日晷：" + text;
            }
            return "";
        }

        private string GetMoondial()
        {
            string text = (Main.IsFastForwardingTime() ? "生效中" : "");
            string text2 = ((Main.moondialCooldown > 0) ? $"{Main.moondialCooldown}天后可再次使用" : "");
            if (string.IsNullOrEmpty(text))
            {
                text = text2;
            }
            else if (!string.IsNullOrEmpty(text2))
            {
                text = text + " " + text2;
            }
            if (!string.IsNullOrEmpty(text))
            {
                return "附魔月晷：" + text;
            }
            return "";
        }

        private string GetSecretWorldDescription()
        {
            List<string> list = new List<string>();
            if (Main.getGoodWorld)
            {
                list.Add("for the worthy");
            }
            if (Main.drunkWorld)
            {
                list.Add("05162020");
            }
            if (Main.tenthAnniversaryWorld)
            {
                list.Add("05162021");
            }
            if (Main.dontStarveWorld)
            {
                list.Add("the constant");
            }
            if (Main.notTheBeesWorld)
            {
                list.Add("not the bees");
            }
            if (Main.remixWorld)
            {
                list.Add("Remix");
            }
            if (Main.noTrapsWorld)
            {
                list.Add("No Traps");
            }
            if (Main.zenithWorld)
            {
                list.Add("Zenith");
            }
            if (list.Count > 0)
            {
                return "彩蛋：" + string.Join(", ", list);
            }
            return "";
        }

        private string GetCorruptionDescription(bool isSuperAdmin = false)
        {
            if (Main.drunkWorld)
            {
                if (WorldGen.crimson)
                {
                    return "腐化：今天是猩红[i:3016], " + more(3);
                }
                return "腐化：今天是腐化[i:3015], " + more(3);
            }
            if (WorldGen.crimson)
            {
                return "腐化：猩红[i:880], " + more(2);
            }
            return "腐化：腐化[i:56], " + more(1);
            string more(int type)
            {
                if (isSuperAdmin)
                {
                    type = 3;
                }
                string text = type switch
                {
                    1 => $"已摧毁 暗影珠x{WorldGen.shadowOrbCount} ",
                    2 => $"已摧毁 猩红之心x{WorldGen.heartCount} ",
                    _ => $"已摧毁 猩红之心x{WorldGen.heartCount} 暗影珠x{WorldGen.shadowOrbCount} ",
                };
                if (Main.hardMode)
                {
                    text += $"祭坛x{WorldGen.altarCount}";
                }
                string worldStatusDialog = GetWorldStatusDialog();
                if (!string.IsNullOrEmpty(worldStatusDialog))
                {
                    text = text + " （" + worldStatusDialog + "）";
                }
                return text;
            }
        }

        public string GetWorldStatusDialog()
        {
            int tGood = WorldGen.tGood;
            int tEvil = WorldGen.tEvil;
            int tBlood = WorldGen.tBlood;
            if (tGood > 0 && tEvil > 0 && tBlood > 0)
            {
                return $"{tGood}%神圣 {tEvil}%腐化 {tBlood}%猩红";
            }
            if (tGood > 0 && tEvil > 0)
            {
                return $"{tGood}%神圣 {tEvil}%腐化";
            }
            if (tGood > 0 && tBlood > 0)
            {
                return $"{tGood}%神圣 {tBlood}%猩红";
            }
            if (tEvil > 0 && tBlood > 0)
            {
                return $"{tEvil}%腐化 {tBlood}%猩红";
            }
            if (tEvil > 0)
            {
                return $"{tEvil}%腐化";
            }
            if (tBlood > 0)
            {
                return $"{tBlood}%猩红";
            }
            return "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _worldModes.Clear();
                SelectionTool.dispose();
            }
           base.Dispose(disposing);
        }
    }
}
