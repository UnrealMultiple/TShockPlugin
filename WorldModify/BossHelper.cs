using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using TShockAPI;
using WorldModify;


namespace WorldModify
{
    internal class BossHelper
    {
        public static void Manage(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                player.SendInfoMessage("/boss info, 查看boss进度");
                player.SendInfoMessage("/boss sb, sb 召唤指令备注（SpawnBoss boss召唤指令）");
                player.SendInfoMessage("/boss <boss名>, 切换boss击败状态");
                player.SendInfoMessage("/boss list, 查看支持切换击败状态的boss名");
                return;
            }
            string text = args.Parameters[0].ToLowerInvariant();
            switch (text)
            {
                case "sb":
                case "spawn":
                    ShowSpawnTips(args);
                    return;
                case "info":
                    BossInfo(args);
                    return;
            }
            if (!ToggleBoss(player, text))
            {
                player.SendErrorMessage("语法不正确！");
            }
        }

        private static void ShowSpawnTips(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (PaginationTools.TryParsePageNumber(args.Parameters, 1, player, out var pageNumber))
            {
                List<string> list = new List<string>
            {
                "史莱姆王, {0}ks, {0}\"king slime\", {0}king",
                "克苏鲁之眼, {0}eoc, {0}\"eye of cthulhu\", {0}eye（时间会调到晚上）",
                "克苏鲁之脑, {0}boc, {0}\"brain of cthulhu\", {0}brain（不在 猩红之地 会脱战）",
                "世界吞噬怪, {0}eow, {0}\"eater of worlds\", {0}eater（不在 腐化之地 会脱战）",
                "蜂王, {0}qb, {0}\"queen bee\"",
                "骷髅王, {0}skeletron（时间会调到晚上）",
                "鹿角怪, {0}deerclops",
                "血肉墙, {0}wof, {0}\"wall of flesh\"（得在地狱）",
                "史莱姆皇后, {0}qs, {0}\"queen slime\"",
                "机械骷髅王, {0}prime",
                "双子魔眼, {0}twins（时间会调到晚上）",
                "毁灭者, {0}destroyer（时间会调到晚上）",
                "世纪之花, {0}plantera",
                "石巨人, {0}golem",
                "光之女皇, {0}eol, {0}\"empress of light\", {0}empress（不会将时间调到晚上）",
                "猪龙鱼公爵, {0}duke, {0}\"duke fishron\", {0}fishron",
                "哀木, {0}\"mourning wood\"（不会将时间调到晚上，白天会脱战）",
                "南瓜王, {0}pumpking（不会将时间调到晚上，白天会脱战）",
                "常绿尖叫怪, {0}everscream（不会将时间调到晚上，白天会脱战）",
                "冰雪女王, {0}\"ice queen\"（不会将时间调到晚上，白天会脱战）",
                "圣诞坦克, {0}santa（不会将时间调到晚上，白天会脱战）",
                "荷兰飞盗船, {0}\"flying dutchman\", {0}flying, {0}dutchman",
                $"火星飞碟, {0}\"martian saucer\", {Commands.Specifier}sm 395",
                "双足翼龙, {0}betsy（可召唤多只，玩家死亡不脱战）",
                "日耀柱, {0}\"solar pillar\"",
                "星旋柱, {0}\"vortex pillar\"",
                "星云柱, {0}\"nebula pillar\"",
                "星尘柱, {0}\"stardust pillar\"",
                "拜月教邪教徒, {0}lc, {0}\"lunatic cultist\", {0}lunatic, {0}cultist",
                "月亮领主, {0}moon, {0}\"moon lord\", {0}ml"
            };
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = string.Format(list[i], Commands.Specifier + "sb ");
                }
                PaginationTools.SendPage(player, pageNumber, list, new PaginationTools.Settings
                {
                    HeaderFormat = Commands.Specifier + "spawnboss 指令 贴士 ({0}/{1})：",
                    FooterFormat = "输入 {0}boss sb {{0}} 查看更多".SFormat(Commands.Specifier)
                });
            }
        }

        private static bool ToggleBoss(TSPlayer op, string param)
        {
            switch (param)
            {
                case "list":
                    {
                        string[] value = new string[8] { "史莱姆王", "克苏鲁之眼", "鹿角怪", "世界吞噬怪", "克苏鲁之脑", "蜂王", "骷髅王", "血肉墙" };
                        string[] value2 = new string[10] { "毁灭者", "双子魔眼", "机械骷髅王", "世纪之花", "石巨人", "史莱姆皇后", "光之女皇", "猪龙鱼公爵", "拜月教邪教徒", "月亮领主" };
                        string[] value3 = new string[13]
                        {
                "哥布林军队", "海盗入侵", "火星暴乱", "哀木", "南瓜王", "冰雪女王", "常绿尖叫怪", "圣诞坦克", "日耀柱", "星旋柱",
                "星云柱", "星尘柱", "双足翼龙"
                        };
                        op.SendInfoMessage("支持切换的BOSS击败状态的有");
                        op.SendInfoMessage("肉前：{0}", string.Join(", ", value));
                        op.SendInfoMessage("肉后：{0}", string.Join(", ", value2));
                        op.SendInfoMessage("事件：{0}", string.Join(", ", value3));
                        break;
                    }
                case "史莱姆王":
                case "史莱姆国王":
                case "king slime":
                case "king":
                case "ks":
                    NPC.downedSlimeKing = !NPC.downedSlimeKing;
                    ShowResult(NPC.downedSlimeKing, "史莱姆王");
                    break;
                case "鹿角怪":
                case "deerclops":
                case "deer":
                case "独眼巨鹿":
                case "巨鹿":
                    NPC.downedDeerclops = !NPC.downedDeerclops;
                    ShowResult(NPC.downedDeerclops, "鹿角怪");
                    break;
                case "克苏鲁之眼":
                case "克眼":
                case "eye of cthulhu":
                case "eye":
                case "eoc":
                    NPC.downedBoss1 = !NPC.downedBoss1;
                    ShowResult(NPC.downedBoss1, "克苏鲁之眼");
                    break;
                case "世界吞噬怪":
                case "世吞":
                case "黑长直":
                case "克苏鲁之脑":
                case "克脑":
                case "brain of cthulhu":
                case "boc":
                case "brain":
                case "eater of worlds":
                case "eow":
                case "eater":
                case "boss2":
                    {
                        NPC.downedBoss2 = !NPC.downedBoss2;
                        string name = "";
                        if (Main.ActiveWorldFileData.HasCrimson && Main.ActiveWorldFileData.HasCorruption)
                        {
                            name = "世界吞噬怪 或 克苏鲁之脑";
                        }
                        else if (Main.ActiveWorldFileData.HasCrimson)
                        {
                            name = "克苏鲁之脑";
                        }
                        else if (Main.ActiveWorldFileData.HasCorruption)
                        {
                            name = "世界吞噬怪";
                        }
                        ShowResult(NPC.downedBoss2, name);
                        break;
                    }
                case "骷髅王":
                case "skeletron":
                case "boss3":
                    NPC.downedBoss3 = !NPC.downedBoss3;
                    ShowResult(NPC.downedBoss3, "骷髅王");
                    break;
                case "蜂王":
                case "蜂后":
                case "queen bee":
                case "qb":
                    NPC.downedQueenBee = !NPC.downedQueenBee;
                    ShowResult(NPC.downedQueenBee, "蜂王");
                    break;
                case "血肉墙":
                case "血肉之墙":
                case "肉山":
                case "wall of flesh":
                case "wof":
                    if (Main.hardMode)
                    {
                        Main.hardMode = false;
                        TSPlayer.All.SendData((PacketTypes)7);
                        op.SendSuccessMessage("已标记 血肉墙 为未击败（困难模式 已关闭）");
                    }
                    else if (!TShock.Config.Settings.DisableHardmode)
                    {
                        WorldGen.StartHardmode();
                        op.SendSuccessMessage("已标记 血肉墙 为已击败（困难模式 已开启）");
                    }
                    break;
                case "毁灭者":
                case "铁长直":
                case "destroyer":
                    NPC.downedMechBoss1 = !NPC.downedMechBoss1;
                    ShowResult(NPC.downedMechBoss1, "毁灭者");
                    break;
                case "双子魔眼":
                case "twins":
                    NPC.downedMechBoss2 = !NPC.downedMechBoss2;
                    ShowResult(NPC.downedMechBoss2, "双子魔眼");
                    TSPlayer.All.SendData((PacketTypes)7);
                    break;
                case "机械骷髅王":
                case "skeletron prime":
                case "prime":
                    NPC.downedMechBoss3 = !NPC.downedMechBoss3;
                    ShowResult(NPC.downedMechBoss3, "机械骷髅王");
                    break;
                case "世纪之花":
                case "plantera":
                    NPC.downedPlantBoss = !NPC.downedPlantBoss;
                    ShowResult(NPC.downedPlantBoss, "世纪之花");
                    break;
                case "石巨人":
                case "golem":
                    NPC.downedGolemBoss = !NPC.downedGolemBoss;
                    ShowResult(NPC.downedGolemBoss, "石巨人");
                    break;
                case "史莱姆皇后":
                case "史莱姆女王":
                case "史莱姆王后":
                case "queen slime":
                case "qs":
                    NPC.downedQueenSlime = !NPC.downedQueenSlime;
                    ShowResult(NPC.downedQueenSlime, "史莱姆皇后");
                    break;
                case "光之女皇":
                case "光女":
                case "光之女神":
                case "光之皇后":
                case "empress of light":
                case "empress":
                case "eol":
                    NPC.downedEmpressOfLight = !NPC.downedEmpressOfLight;
                    ShowResult(NPC.downedEmpressOfLight, "光之女皇");
                    break;
                case "猪龙鱼公爵":
                case "猪鲨":
                case "duke fishron":
                case "duke":
                case "fishron":
                    NPC.downedFishron = !NPC.downedFishron;
                    ShowResult(NPC.downedFishron, "猪龙鱼公爵");
                    break;
                case "拜月教邪教徒":
                case "拜月教":
                case "邪教徒":
                case "lunatic cultist":
                case "lunatic":
                case "cultist":
                case "lc":
                    NPC.downedAncientCultist = !NPC.downedAncientCultist;
                    ShowResult(NPC.downedAncientCultist, "拜月教邪教徒");
                    break;
                case "月亮领主":
                case "月总":
                case "moon lord":
                case "moon":
                case "ml":
                    NPC.downedMoonlord = !NPC.downedMoonlord;
                    ShowResult(NPC.downedMoonlord, "月亮领主");
                    break;
                case "哥布林军队":
                case "哥布林":
                case "goblin":
                case "goblins":
                    NPC.downedGoblins = !NPC.downedGoblins;
                    ShowResult(NPC.downedGoblins, "哥布林军队");
                    break;
                case "海盗入侵":
                case "荷兰飞盗船":
                case "海盗船":
                case "pirate":
                case "pirates":
                case "flying dutchman":
                case "flying":
                case "dutchman":
                    NPC.downedPirates = !NPC.downedPirates;
                    ShowResult(NPC.downedPirates, "海盗入侵");
                    break;
                case "火星暴乱":
                case "火星人入侵":
                case "火星飞碟":
                case "ufo":
                case "martian saucer":
                case "martian":
                case "martians":
                    NPC.downedMartians = !NPC.downedMartians;
                    ShowResult(NPC.downedMartians, "火星暴乱");
                    break;
                case "哀木":
                case "mourning wood":
                case "wood":
                case "halloween tree":
                case "ht":
                    NPC.downedHalloweenTree = !NPC.downedHalloweenTree;
                    ShowResult(NPC.downedHalloweenTree, "哀木");
                    break;
                case "南瓜王":
                case "pumpking":
                case "halloween king":
                case "hk":
                    NPC.downedHalloweenKing = !NPC.downedHalloweenKing;
                    ShowResult(NPC.downedHalloweenKing, "南瓜王");
                    break;
                case "冰雪女王":
                case "冰雪皇后":
                case "ice queen":
                    NPC.downedChristmasIceQueen = !NPC.downedChristmasIceQueen;
                    ShowResult(NPC.downedChristmasIceQueen, "冰雪女王");
                    break;
                case "常绿尖叫怪":
                case "everscream":
                    NPC.downedChristmasTree = !NPC.downedChristmasTree;
                    ShowResult(NPC.downedChristmasTree, "常绿尖叫怪");
                    break;
                case "圣诞坦克":
                case "santa":
                case "santa-nk1":
                case "tank":
                    NPC.downedChristmasSantank = !NPC.downedChristmasSantank;
                    ShowResult(NPC.downedChristmasSantank, "圣诞坦克");
                    break;
                case "日耀柱":
                case "日耀":
                case "日曜柱":
                case "日曜":
                case "solar pillar":
                case "solar":
                    NPC.downedTowerSolar = !NPC.downedTowerSolar;
                    ShowResult(NPC.downedTowerSolar, "日曜柱");
                    break;
                case "星旋柱":
                case "星旋":
                case "vortex pillar":
                case "vortex":
                    NPC.downedTowerVortex = !NPC.downedTowerVortex;
                    ShowResult(NPC.downedTowerVortex, "星旋柱");
                    break;
                case "星云柱":
                case "星云":
                case "nebula pillar":
                case "nebula":
                    NPC.downedTowerNebula = !NPC.downedTowerNebula;
                    ShowResult(NPC.downedTowerNebula, "星云柱");
                    break;
                case "星尘柱":
                case "星尘":
                case "stardust pillar":
                case "stardust":
                    NPC.downedTowerStardust = !NPC.downedTowerStardust;
                    ShowResult(NPC.downedTowerStardust, "星尘柱");
                    break;
                case "betsy":
                    DD2Event.DownedInvasionT3 = !DD2Event.DownedInvasionT3;
                    ShowResult(DD2Event.DownedInvasionT3, "双足翼龙");
                    return false;
                default:
                    return false;
            }
            return true;
            void ShowResult(bool _vaule, string _name)
            {
                TSPlayer.All.SendData((PacketTypes)7);
                if (_vaule)
                {
                    op.SendSuccessMessage("已标记 " + _name + " 为已击败");
                }
                else
                {
                    op.SendSuccessMessage("已标记 " + _name + " 为未击败");
                }
            }
        }

        public static void BossInfo(CommandArgs args)
        {
            args.Player.SendInfoMessage(string.Join("\n", ShowBossInfo()));
        }

        private static List<string> ShowBossInfo(bool isSuperAdmin = false)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            List<string> list3 = new List<string>();
            List<string> list4 = new List<string>();
            string boss2Name = "";
            list2.Add(Utils.CFlag(NPC.downedSlimeKing, "史莱姆王"));
            list2.Add(Utils.CFlag(NPC.downedBoss1, "克苏鲁之眼"));
            string text = "";
            if (Main.drunkWorld)
                boss2Name = "世界吞噬怪 或 克苏鲁之脑";
            else
                boss2Name = WorldGen.crimson ? "克苏鲁之脑" : "世界吞噬怪";
            list2.Add(Utils.CFlag(NPC.downedDeerclops, "鹿角怪"));
            list2.Add(Utils.CFlag(NPC.downedBoss3, "骷髅王"));
            list2.Add(Utils.CFlag(NPC.downedQueenBee, "蜂王"));
            list2.Add(Utils.CFlag(Main.hardMode, "血肉墙"));
            list3.Add(Utils.CFlag(NPC.downedMechBoss1, "毁灭者"));
            list3.Add(Utils.CFlag(NPC.downedMechBoss2, "双子魔眼"));
            list3.Add(Utils.CFlag(NPC.downedMechBoss3, "机械骷髅王"));
            list3.Add(Utils.CFlag(NPC.downedPlantBoss, "世纪之花"));
            list3.Add(Utils.CFlag(NPC.downedGolemBoss, "石巨人"));
            list3.Add(Utils.CFlag(NPC.downedQueenSlime, "史莱姆皇后"));
            list3.Add(Utils.CFlag(NPC.downedEmpressOfLight, "光之女皇"));
            list3.Add(Utils.CFlag(NPC.downedFishron, "猪龙鱼公爵"));
            list3.Add(Utils.CFlag(NPC.downedAncientCultist, "拜月教邪教徒"));
            list3.Add(Utils.CFlag(NPC.downedMoonlord, "月亮领主"));
            list4.Add(Utils.CFlag(NPC.downedGoblins, "哥布林军队"));
            list4.Add(Utils.CFlag(NPC.downedPirates, "海盗入侵"));
            list4.Add(Utils.CFlag(NPC.downedMartians, "火星暴乱"));
            list4.Add(Utils.CFlag(NPC.downedHalloweenTree, "哀木"));
            list4.Add(Utils.CFlag(NPC.downedHalloweenKing, "南瓜王"));
            list4.Add(Utils.CFlag(NPC.downedChristmasIceQueen, "冰雪女王"));
            list4.Add(Utils.CFlag(NPC.downedChristmasTree, "常绿尖叫怪"));
            list4.Add(Utils.CFlag(NPC.downedChristmasSantank, "圣诞坦克"));
            list4.Add(Utils.CFlag(NPC.downedTowerSolar, "日耀柱"));
            list4.Add(Utils.CFlag(NPC.downedTowerVortex, "星旋柱"));
            list4.Add(Utils.CFlag(NPC.downedTowerNebula, "星云柱"));
            list4.Add(Utils.CFlag(NPC.downedTowerStardust, "星尘柱"));
            return new List<string>
        {
            string.Format("肉前：{0}", string.Join(", ", list2)),
            string.Format("肉后：{0}", string.Join(", ", list3)),
            string.Format("事件：{0}", string.Join(", ", list4))
        };
        }
    }
}