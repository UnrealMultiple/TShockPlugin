using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;


namespace WorldModify
{
    internal class NPCHelper
    {
        public static void Manage(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                player.SendInfoMessage("/npc info, NPC信息");
                player.SendInfoMessage("/npc unique, 城镇NPC去重");
                player.SendInfoMessage("/npc clear help, 移除指定NPC");
                player.SendInfoMessage("/npc tphere help, 将NPC传送到你身边");
                player.SendInfoMessage("/npc relive, 复活NPC（根据怪物图鉴记录）");
                player.SendInfoMessage("/npc find <id/名称>, 查询指定NPC的信息");
                player.SendInfoMessage("/npc <id/名称>, 切换NPC解救状态");
                player.SendInfoMessage("/npc list, 查看支持切换解救状态的NPC");
                player.SendInfoMessage("/npc sm, sm召唤指令备注（SpawnMob NPC召唤指令）");
                player.SendInfoMessage("/npc mq, 召唤美杜莎boss");
                return;
            }
            string text = args.Parameters[0].ToLowerInvariant();
            switch (text)
            {
                default:
                    if (!ToggleNPC(player, text))
                    {
                        player.SendErrorMessage("语法不正确！");
                    }
                    break;
                case "sm":
                case "spawn":
                case "spawnmob":
                    {
                        List<string> list = new List<string>();
                        int num = 0;
                        int[] smIDs = NPCIDHelper.smIDs;
                        foreach (int num2 in smIDs)
                        {
                            string value = ((num != 0 && num % 5 == 0) ? "\n" : "");
                            list.Add($"{value}{NPCIDHelper.GetNameByID(num2)}=/sm {num2}");
                            num++;
                        }
                        player.SendInfoMessage($"以下是城镇NPC生成指令参考（共{NPCIDHelper.smIDs.Length}个）：");
                        player.SendInfoMessage(string.Join(", ", list));
                        break;
                    }
                case "info":
                    NPCInfo(args);
                    break;
                case "tphere":
                case "th":
                    TPHereNPC(args);
                    break;
                case "find":
                    FindNPC(args);
                    break;
                case "clear":
                    ClearNPC(args);
                    break;
                case "unique":
                    UniqueNPC(args);
                    break;
                case "relive":
                    Relive(args);
                    break;
                case "mq":
                    NPC.SpawnMechQueen(player.Index);
                    break;
            }
        }

        private static void NPCInfo(CommandArgs args)
        {
            TSPlayer player = args.Player;
            int num = 0;
            int num2 = 0;
            List<int> list = new List<int>();
            List<string> list2 = new List<string>();
            for (int i = 0; i < 200; i++)
            {
                if (!Main.npc[i].active)
                {
                    continue;
                }
                num++;
                if (Main.npc[i].townNPC)
                {
                    list.Add(Main.npc[i].netID);
                    list2.Add(Main.npc[i].FullName);
                    int netID = Main.npc[i].netID;
                    if (netID != 453 && netID != 368 && netID != 37)
                    {
                        num2++;
                    }
                }
            }
            List<string> list3 = new List<string>();
            List<string> list4 = new List<string>();
            List<string> list5 = new List<string>();
            List<string> list6 = new List<string>();
            List<string> list7 = new List<string>();
            int[] array = new int[8] { 22, 17, 19, 18, 369, 227, 124, 107 };
            int[] array2 = array;
            foreach (int num3 in array2)
            {
                list3.Add(Utils.CFlag(list.Contains(num3), NPCIDHelper.GetNameByID(num3)));
            }
            array = new int[10] { 228, 20, 207, 54, 353, 38, 633, 208, 550, 588 };
            int[] array3 = array;
            foreach (int num4 in array3)
            {
                list4.Add(Utils.CFlag(list.Contains(num4), NPCIDHelper.GetNameByID(num4)));
            }
            array = new int[8] { 108, 229, 209, 441, 160, 142, 178, 663 };
            int[] array4 = array;
            foreach (int num5 in array4)
            {
                list5.Add(Utils.CFlag(list.Contains(num5), NPCIDHelper.GetNameByID(num5)));
            }
            array = new int[6] { 637, 638, 656, 37, 368, 453 };
            int[] array5 = array;
            foreach (int num6 in array5)
            {
                list6.Add(Utils.CFlag(list.Contains(num6), NPCIDHelper.GetNameByID(num6)));
            }
            array = new int[8] { 670, 678, 679, 680, 681, 682, 683, 684 };
            int[] array6 = array;
            foreach (int num7 in array6)
            {
                list7.Add(Utils.CFlag(list.Contains(num7), NPCIDHelper.GetNameByID(num7)));
            }
            List<string> list8 = new List<string>
        {
            $"共{num}个NPC（含敌怪）, {num2}个城镇NPC。",
            "肉前：" + string.Join(", ", list3),
            "肉前：" + string.Join(", ", list4),
            "肉后：" + string.Join(", ", list5),
            "其它：" + string.Join(", ", list6),
            "史莱姆：" + string.Join(", ", list7)
        };
            list4.Clear();
            list5.Clear();
            list3 = (NPC.savedAngler ? list4 : list5);
            list3.Add("渔夫");
            list3 = (NPC.savedWizard ? list4 : list5);
            list3.Add("巫师");
            list3 = (NPC.savedMech ? list4 : list5);
            list3.Add("机械师");
            list3 = (NPC.savedStylist ? list4 : list5);
            list3.Add("发型师");
            list3 = (NPC.savedTaxCollector ? list4 : list5);
            list3.Add("税收官");
            list3 = (NPC.savedBartender ? list4 : list5);
            list3.Add("酒馆老板");
            list3 = (NPC.savedGoblin ? list4 : list5);
            list3.Add("哥布林工匠");
            list3 = (NPC.savedGolfer ? list4 : list5);
            list3.Add("高尔夫球手");
            if (list5.Count > 0)
            {
                list8.Add("待解救：" + string.Join(", ", list5));
            }
            list4.Clear();
            list5.Clear();
            list3 = (NPC.boughtCat ? list4 : list5);
            list3.Add("猫咪");
            list3 = (NPC.boughtDog ? list4 : list5);
            list3.Add("狗狗");
            list3 = (NPC.boughtBunny ? list4 : list5);
            list3.Add("兔兔");
            if (list5.Count > 0)
            {
                list8.Add("待使用：" + string.Join("许可证, ", list5) + "许可证");
            }
            list4.Clear();
            list5.Clear();
            list3 = (NPC.unlockedSlimeBlueSpawn ? list4 : list5);
            list3.Add("呆瓜史莱姆");
            list3 = (NPC.unlockedSlimeGreenSpawn ? list4 : list5);
            list3.Add("冷酷史莱姆");
            list3 = (NPC.unlockedSlimeOldSpawn ? list4 : list5);
            list3.Add("年长史莱姆");
            list3 = (NPC.unlockedSlimePurpleSpawn ? list4 : list5);
            list3.Add("笨拙史莱姆");
            list3 = (NPC.unlockedSlimeRainbowSpawn ? list4 : list5);
            list3.Add("唱将史莱姆");
            list3 = (NPC.unlockedSlimeRedSpawn ? list4 : list5);
            list3.Add("粗暴史莱姆");
            list3 = (NPC.unlockedSlimeYellowSpawn ? list4 : list5);
            list3.Add("神秘史莱姆");
            list3 = (NPC.unlockedSlimeCopperSpawn ? list4 : list5);
            list3.Add("侍卫史莱姆");
            if (list5.Count > 0)
            {
                list8.Add("待解锁：" + string.Join(", ", list5));
            }
            player.SendInfoMessage(string.Join("\n", list8));
        }

        private static void FindNPC(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                player.SendInfoMessage("/npc find 指令用法：");
                player.SendInfoMessage("/npc find <id/名称>, 查询指定NPC的信息");
                return;
            }
            List<NPC> nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
            if (nPCByIdOrName.Count == 0)
            {
                args.Player.SendErrorMessage("找不到对应的 NPC");
                return;
            }
            if (nPCByIdOrName.Count > 1)
            {
                args.Player.SendMultipleMatchError(nPCByIdOrName.Select((NPC n) => $"{n.FullName}({n.type})"));
                return;
            }
            int num = CoundNPCByID(nPCByIdOrName[0].netID);
            if (num > 0)
            {
                int num2 = FindNPCByID(nPCByIdOrName[0].netID);
                NPC val = Main.npc[num2];
                player.SendSuccessMessage($"名称：{nPCByIdOrName[0].FullName}({val.netID}) {((num > 1) ? ("x" + num) : "")}\n位置：{Utils.GetLocationDesc(val)}(/tppos {(int)(val.position.X / 16f)} {(int)(val.position.Y / 16f)})");
            }
            else
            {
                player.SendSuccessMessage("未找到 " + nPCByIdOrName[0].FullName);
            }
        }

        private static bool ToggleNPC(TSPlayer op, string param)
        {
            string text = "";
            switch (param)
            {
                case "list":
                    {
                        int[] array = new int[19]
                        {
                            369, 107, 124, 353, 550, 588, 108, 441, 637, 638,
                            656, 670, 678, 679, 680, 681, 682, 683, 684
                        };
                        List<string> list = new List<string>();
                        int num = 0;
                        int[] array2 = array;
                        foreach (int num2 in array2)
                        {
                            string value = ((num != 0 && num % 5 == 0) ? "\n" : "");
                            list.Add($"{value}{NPCIDHelper.GetNameByID(num2)}={num2}");
                            num++;
                        }
                        op.SendInfoMessage("支持切换的NPC拯救/购买状态的有: ");
                        op.SendInfoMessage("{0}", string.Join(", ", list));
                        break;
                    }
                case "渔夫":
                case "angler":
                case "369":
                case "沉睡渔夫":
                case "376":
                    NPC.savedAngler = !NPC.savedAngler;
                    text = (NPC.savedAngler ? "沉睡渔夫 已解救" : "渔夫 已标记为 未解救");
                    break;
                case "哥布林工匠":
                case "107":
                case "受缚哥布林":
                case "goblin":
                case "105":
                    NPC.savedGoblin = !NPC.savedGoblin;
                    text = (NPC.savedGoblin ? "受缚哥布林 已解救" : "哥布林工匠 已标记为 未解救");
                    break;
                case "机械师":
                case "123":
                case "受缚机械师":
                case "124":
                case "mech":
                case "mechanic":
                    NPC.savedMech = !NPC.savedMech;
                    text = (NPC.savedMech ? "受缚机械师 已解救" : "机械师 已标记为 未解救");
                    break;
                case "发型师":
                case "353":
                case "受缚发型师":
                case "354":
                case "stylist":
                    NPC.savedStylist = !NPC.savedStylist;
                    text = (NPC.savedStylist ? "被网住的发型师 已解救" : "发型师 已标记为 未解救");
                    break;
                case "酒馆老板":
                case "550":
                case "昏迷男子":
                case "579":
                case "酒保":
                case "bartender":
                case "tavernkeep":
                case "tavern":
                    NPC.savedBartender = !NPC.savedBartender;
                    text = (NPC.savedBartender ? "昏迷男子 已解救" : "酒馆老板 已标记为 未解救");
                    break;
                case "高尔夫球手":
                case "588":
                case "589":
                case "高尔夫":
                case "golfer":
                    NPC.savedGolfer = !NPC.savedGolfer;
                    text = (NPC.savedGolfer ? "高尔夫球手 已解救" : "高尔夫球手 已标记为 未解救");
                    break;
                case "巫师":
                case "106":
                case "受缚巫师":
                case "108":
                case "wizard":
                    NPC.savedWizard = !NPC.savedWizard;
                    text = (NPC.savedWizard ? "受缚巫师 已解救" : "巫师 已标记为 未解救");
                    break;
                case "税收官":
                case "441":
                case "痛苦亡魂":
                case "534":
                case "tax":
                case "tax collector":
                    NPC.savedTaxCollector = !NPC.savedTaxCollector;
                    text = (NPC.savedTaxCollector ? "痛苦亡魂 已净化" : "税收官 已标记为 未解救");
                    break;
                case "猫":
                case "637":
                case "cat":
                    NPC.boughtCat = !NPC.boughtCat;
                    text = (NPC.boughtCat ? "猫咪许可证 已生效" : "猫咪许可证 已标记为 未使用");
                    break;
                case "狗":
                case "638":
                case "dog":
                    NPC.boughtDog = !NPC.boughtDog;
                    text = (NPC.boughtDog ? "狗狗许可证 已生效" : "狗狗许可证 已标记为 未使用");
                    break;
                case "兔子":
                case "兔兔":
                case "656":
                case "兔":
                case "bunny":
                case "rabbit":
                    NPC.boughtBunny = !NPC.boughtBunny;
                    text = (NPC.boughtBunny ? "兔兔许可证 已生效" : "兔兔许可证 已标记为 未使用");
                    break;
                case "呆瓜史莱姆":
                case "670":
                case "呆瓜":
                    NPC.unlockedSlimeBlueSpawn = !NPC.unlockedSlimeBlueSpawn;
                    text = UFlag(NPC.unlockedSlimeBlueSpawn, "呆瓜史莱姆");
                    break;
                case "冷酷史莱姆":
                case "678":
                case "冷酷":
                    NPC.unlockedSlimeGreenSpawn = !NPC.unlockedSlimeGreenSpawn;
                    text = UFlag(NPC.unlockedSlimeGreenSpawn, "冷酷史莱姆");
                    break;
                case "年长史莱姆":
                case "679":
                case "年长":
                    NPC.unlockedSlimeOldSpawn = !NPC.unlockedSlimeOldSpawn;
                    text = UFlag(NPC.unlockedSlimeOldSpawn, "年长史莱姆");
                    break;
                case "笨拙史莱姆":
                case "680":
                case "笨拙":
                    NPC.unlockedSlimePurpleSpawn = !NPC.unlockedSlimePurpleSpawn;
                    text = UFlag(NPC.unlockedSlimePurpleSpawn, "笨拙史莱姆");
                    break;
                case "唱将史莱姆":
                case "681":
                case "唱将":
                    NPC.unlockedSlimeRainbowSpawn = !NPC.unlockedSlimeRainbowSpawn;
                    text = UFlag(NPC.unlockedSlimeRainbowSpawn, "唱将史莱姆");
                    break;
                case "粗暴史莱姆":
                case "682":
                case "粗暴":
                    NPC.unlockedSlimeRedSpawn = !NPC.unlockedSlimeRedSpawn;
                    text = UFlag(NPC.unlockedSlimeRedSpawn, "粗暴史莱姆");
                    break;
                case "神秘史莱姆":
                case "683":
                case "神秘":
                    NPC.unlockedSlimeYellowSpawn = !NPC.unlockedSlimeYellowSpawn;
                    text = UFlag(NPC.unlockedSlimeYellowSpawn, "神秘史莱姆");
                    break;
                case "侍卫史莱姆":
                case "684":
                case "侍卫":
                    NPC.unlockedSlimeCopperSpawn = !NPC.unlockedSlimeCopperSpawn;
                    text = UFlag(NPC.unlockedSlimeCopperSpawn, "侍卫史莱姆");
                    break;
                default:
                    return false;
            }
            if (!string.IsNullOrEmpty(text))
            {
                TSPlayer.All.SendData((PacketTypes)7);
                op.SendSuccessMessage(text);
            }
            return true;
            static string UFlag(bool _value, string _str)
            {
                return _value ? (_str + " 已解锁") : (_str + " 已标记为 未解锁");
            }
        }

        private static void ClearNPC(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            if (args.Parameters.Count == 0)
            {
                Help();
                return;
            }
            string text = args.Parameters[0].ToLowerInvariant();
            string text2 = text;
            if (!(text2 == "help"))
            {
                if (text2 == "enemy")
                {
                    int num = 0;
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].active && !Main.npc[i].friendly)
                        {
                            Main.npc[i].active = false;
                            Main.npc[i].type = 0;
                            TSPlayer.All.SendData((PacketTypes)23, "", i);
                            num++;
                        }
                    }
                    args.Player.SendSuccessMessage($"已清除 {num}个 敌怪");
                    return;
                }
                List<NPC> nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
                if (nPCByIdOrName.Count == 0)
                {
                    args.Player.SendErrorMessage("找不到对应的 NPC");
                }
                else if (nPCByIdOrName.Count > 1)
                {
                    args.Player.SendMultipleMatchError(nPCByIdOrName.Select((NPC n) => $"{n.FullName}({n.type})"));
                }
                else
                {
                    args.Player.SendSuccessMessage("清理了 {0} 个 {1}", ClearNPCByID(nPCByIdOrName[0].netID), nPCByIdOrName[0].FullName);
                }
            }
            else
            {
                Help();
            }
            void Help()
            {
                args.Player.SendInfoMessage("/npc clear 指令用法：");
                args.Player.SendInfoMessage("/npc clear <id/名称>, 清除指定NPC");
                args.Player.SendInfoMessage("/npc clear enemy, 清除所有敌怪，保留友善NPC");
            }
        }

        private static void UniqueNPC(CommandArgs args)
        {
            List<int> list = new List<int>();
            int num = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC)
                {
                    int type = Main.npc[i].type;
                    if (list.Contains(type))
                    {
                        Main.npc[i].active = false;
                        Main.npc[i].type = 0;
                        TSPlayer.All.SendData((PacketTypes)23, "", i);
                        num++;
                    }
                    else
                    {
                        list.Add(type);
                    }
                }
            }
            if (num > 0)
            {
                args.Player.SendSuccessMessage($"已清除 {num} 个重复的NPC");
            }
            else
            {
                args.Player.SendInfoMessage("没有可清除的 重复的NPC");
            }
        }

        public static void Relive(CommandArgs args)
        {
            TSPlayer player = args.Player;
            List<int> relive = GetRelive();
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC && relive.Contains(Main.npc[i].type))
                {
                    relive.Remove(Main.npc[i].type);
                }
            }
            List<string> list = new List<string>();
            foreach (int item in relive)
            {
                NPC val = new NPC();
                val.SetDefaults(item, default);
                TSPlayer.Server.SpawnNPC(val.type, val.FullName, 1, player.TileX, player.TileY, 5, 2);
                if (list.Count != 0 && list.Count % 10 == 0)
                {
                    list.Add("\n" + val.FullName);
                }
                else
                {
                    list.Add(val.FullName);
                }
            }
            if (relive.Count > 0)
            {
                string msg = $"{player.Name} 复活了 {relive.Count}个 NPC:\n{string.Join("、", list)}";
                TSPlayer.All.SendInfoMessage(msg);
                if (!player.RealPlayer)
                {
                    player.SendInfoMessage(msg);
                }
            }
            else
            {
                player.SendInfoMessage("入住过的NPC都活着");
            }
        }

        public static List<int> GetRelive()
        {
            List<int> list = new List<int>();
            List<int> list2 = new List<int>
        {
            17, 18, 19, 20, 22, 38, 54, 107, 108, 124,
            160, 178, 207, 208, 209, 227, 228, 229, 353, 369,
            441, 550, 588, 633, 663, 637, 638, 656, 670, 678,
            679, 680, 681, 682, 683, 684
        };
            if (Main.xMas)
            {
                list2.Add(142);
            }
            foreach (int item in list2)
            {
                if (DidDiscoverBestiaryEntry(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public static bool DidDiscoverBestiaryEntry(int npcId)
        {
            return (int)Main.BestiaryDB.FindEntryByNPCID(npcId).UIInfoProvider.GetEntryUICollectionInfo().UnlockState > 0;
        }

        public static void TPHereNPC(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                player.SendInfoMessage("/npc tphere 指令用法：");
                player.SendInfoMessage("/npc th <id/名称>, 将指定NPC传到你身边");
                player.SendInfoMessage("/npc th town, 将所有城镇NPC传到你身边");
                return;
            }
            string text = args.Parameters[0].ToLowerInvariant();
            string text2 = text;
            string text3 = text2;
            Vector2 val = default;
            if (player.RealPlayer)
            {
                val = new Vector2(args.TPlayer.position.X, args.TPlayer.position.Y);
            }
            else
            {
                val = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            }
            if (text == "town")
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC val2 = Main.npc[i];
                    if (val2.active && val2.townNPC && val2.netID != 453 && val2.netID != 368 && val2.netID != 37)
                    {
                        val2.Teleport(val, 0, 0);
                    }
                }
                if (player.RealPlayer)
                {
                    player.SendSuccessMessage("已将所有城镇NPC传到你身边");
                }
                else
                {
                    player.SendSuccessMessage("已将所有城镇NPC传回出生点");
                }
                return;
            }
            bool flag = false;
            if (int.TryParse(text, out var result))
            {
                flag = true;
            }
            else
            {
                int iDByName = NPCIDHelper.GetIDByName(text);
                if (iDByName > 0)
                {
                    result = iDByName;
                    flag = true;
                }
            }
            int num = -1;
            for (int j = 0; j < 200; j++)
            {
                if (Main.npc[j].active)
                {
                    if (Main.npc[j].TypeName.ToLowerInvariant() == text)
                    {
                        num = j;
                        break;
                    }
                    if (flag && Main.npc[j].netID == result)
                    {
                        num = j;
                        break;
                    }
                }
            }
            if (num == -1)
            {
                player.SendErrorMessage("找不到对应的 NPC");
                return;
            }
            NPC val3 = Main.npc[num];
            val3.Teleport(val, 0, 0);
            if (player.RealPlayer)
            {
                player.SendSuccessMessage("已将 {0} 传到你身边", val3.FullName);
            }
            else
            {
                player.SendSuccessMessage("已将 {0} 传回出生点", val3.FullName);
            }
        }

        private static int ClearNPCByID(int npcID)
        {
            int num = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].netID == npcID)
                {
                    Main.npc[i].active = false;
                    Main.npc[i].type = 0;
                    TSPlayer.All.SendData((PacketTypes)23, "", i);
                    num++;
                }
            }
            return num;
        }

        private static int FindNPCByID(int npcID)
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].netID == npcID)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int CoundNPCByID(int npcID)
        {
            int num = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].netID == npcID)
                {
                    num++;
                }
            }
            return num;
        }
    }
}