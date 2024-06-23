using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using WorldModify;


namespace WorldModify
{
    internal class BestiaryHelper
    {
        public static string SaveFile;

        private static List<int> kills = new List<int>();

        private static List<int> sights = new List<int>();

        private static List<int> chats = new List<int>
    {
        369, 37, 22, 17, 20, 633, 368, 227, 38, 19,
        207, 453, 229, 107, 18, 54, 550, 124, 637, 638,
        228, 353, 441, 108, 160, 178, 656, 208, 588, 209,
        663, 142, 678, 679, 680, 681, 682, 683, 684, 670
    };

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                HelpTxt();
                return;
            }
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "unlock":
                    Unlock(op);
                    return;
                case "reset":
                    Reset(op);
                    return;
                case "import":
                    Import(op);
                    return;
                case "backup":
                    Backup();
                    op.SendSuccessMessage("备份完成（" + SaveFile + "）");
                    return;
                case "help":
                    HelpTxt();
                    return;
            }
            if (int.TryParse(args.Parameters[0], out var result))
            {
                if (result > 0 && result < NPCID.Count)
                {
                    UnlockOne(result, op);
                    return;
                }
                op.SendErrorMessage($"NPC ID 只能在 1~{ItemID.Count} 范围内");
                return;
            }
            List<NPC> nPCByName = TShock.Utils.GetNPCByName(args.Parameters[0]);
            if (nPCByName.Count == 0)
            {
                args.Player.SendErrorMessage("无效的NPC名称!");
            }
            else if (nPCByName.Count > 1)
            {
                args.Player.SendMultipleMatchError(nPCByName.Select((NPC i) => $"{i.FullName}({i.netID})"));
            }
            else
            {
                UnlockOne(nPCByName[0].netID, op);
            }
            void HelpTxt()
            {
                op.SendInfoMessage("/wm bestiary 指令用法：");
                op.SendInfoMessage("/wm be unlock，解锁 全怪物图鉴");
                op.SendInfoMessage("/wm be <id/名称>，解锁 单条记录");
                op.SendInfoMessage("/wm be reset，重置 怪物图鉴");
                op.SendInfoMessage("/wm be import，导入 怪物图鉴");
                op.SendInfoMessage("/wm be backup，备份 怪物图鉴 到 csv文件，解锁和重置前会自动备份");
            }
        }

        private static async void Unlock(TSPlayer op)
        {
            await Task.Run(delegate
            {

                Backup();
                if (kills.Count == 0)
                {
                    ReadEnmeyAndCritter();
                }
                foreach (int kill in kills)
                {
                    UnlockKill(kill);
                }
                foreach (int sight in sights)
                {
                    UnlockSight(sight);
                }
                foreach (int chat in chats)
                {
                    UnlockChat(chat);
                }
                TSPlayer.All.SendData((PacketTypes)7);
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        Main.BestiaryTracker.OnPlayerJoining(i);
                    }
                }
                BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
                op.SendSuccessMessage($"怪物图鉴 已全部解锁 ;-) {bestiaryProgressReport.CompletionAmountTotal}/{bestiaryProgressReport.EntriesTotal}");
            });
        }

        private static void UnlockOne(int id, TSPlayer op)
        {
            if (kills.Count == 0)
            {
                ReadEnmeyAndCritter();
            }
            bool flag = false;
            if (kills.Contains(id))
            {
                flag = UnlockKill(id);
            }
            else if (sights.Contains(id))
            {
                flag = UnlockSight(id);
            }
            else if (chats.Contains(id))
            {
                flag = UnlockChat(id);
            }
            if (flag)
            {
                TSPlayer.All.SendData((PacketTypes)7);
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        Main.BestiaryTracker.OnPlayerJoining(i);
                    }
                }
                op.SendSuccessMessage("已将 " + TShock.Utils.GetNPCById(id).FullName + " 加入怪物图鉴");
            }
            else
            {
                op.SendSuccessMessage("此条目已经解锁过了");
            }
        }

        private static bool UnlockKill(int id)
        {
            string bestiaryCreditId = GetBestiaryCreditId(id);
            Dictionary<string, int> killCountsByNpcId = Main.BestiaryTracker.Kills._killCountsByNpcId;
            if (killCountsByNpcId.ContainsKey(bestiaryCreditId))
            {
                if (killCountsByNpcId[bestiaryCreditId] < 50)
                {
                    killCountsByNpcId[bestiaryCreditId] = 50;
                    return true;
                }
                return false;
            }
            killCountsByNpcId.Add(bestiaryCreditId, 50);
            return true;
        }

        private static bool UnlockSight(int id)
        {
            string bestiaryCreditId = GetBestiaryCreditId(id);
            if (!Main.BestiaryTracker.Sights._wasNearPlayer.Contains(bestiaryCreditId))
            {
                Main.BestiaryTracker.Sights._wasNearPlayer.Add(bestiaryCreditId);
                return true;
            }
            return false;
        }

        private static bool UnlockChat(int id)
        {
            string bestiaryCreditId = GetBestiaryCreditId(id);
            if (!Main.BestiaryTracker.Chats._chattedWithPlayer.Contains(bestiaryCreditId))
            {
                Main.BestiaryTracker.Chats._chattedWithPlayer.Add(bestiaryCreditId);
                return true;
            }
            return false;
        }

        private static async void Reset(TSPlayer op)
        {
            await Task.Run(delegate
            {
                Backup();
                Main.BestiaryTracker.Reset();
                TSPlayer.All.SendData((PacketTypes)7);
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        Main.BestiaryTracker.OnPlayerJoining(i);
                    }
                }
                op.SendSuccessMessage("怪物图鉴 已清空，重进游戏后生效");
            });
        }

        private static void Backup()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, int> item in Main.BestiaryTracker.Kills._killCountsByNpcId)
            {
                if (item.Key == "DeerclopsLeg")
                {
                    continue;
                }
                int nPCId = GetNPCId(item.Key);
                if (nPCId == 0)
                {
                    continue;
                }
                StringBuilder stringBuilder2;
                StringBuilder.AppendInterpolatedStringHandler handler;
                switch (nPCId)
                {
                    case 195:
                        if (!Main.BestiaryTracker.Kills._killCountsByNpcId.ContainsKey("Nymph"))
                        {
                            stringBuilder2 = stringBuilder;
                            StringBuilder stringBuilder4 = stringBuilder2;
                            handler = new StringBuilder.AppendInterpolatedStringHandler(3, 3, stringBuilder2);
                            handler.AppendFormatted(196);
                            handler.AppendLiteral(",");
                            handler.AppendFormatted(item.Value);
                            handler.AppendLiteral(",");
                            handler.AppendFormatted(Lang.GetNPCName(nPCId));
                            handler.AppendLiteral("\n");
                            stringBuilder4.Append(ref handler);
                        }
                        break;
                    case 196:
                        if (!Main.BestiaryTracker.Kills._killCountsByNpcId.ContainsKey("LostGirl"))
                        {
                            stringBuilder2 = stringBuilder;
                            StringBuilder stringBuilder3 = stringBuilder2;
                            handler = new StringBuilder.AppendInterpolatedStringHandler(3, 3, stringBuilder2);
                            handler.AppendFormatted(195);
                            handler.AppendLiteral(",");
                            handler.AppendFormatted(item.Value);
                            handler.AppendLiteral(",");
                            handler.AppendFormatted(Lang.GetNPCName(nPCId));
                            handler.AppendLiteral("\n");
                            stringBuilder3.Append(ref handler);
                        }
                        break;
                }
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder5 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(3, 3, stringBuilder2);
                handler.AppendFormatted(nPCId);
                handler.AppendLiteral(",");
                handler.AppendFormatted(item.Value);
                handler.AppendLiteral(",");
                handler.AppendFormatted(Lang.GetNPCName(nPCId));
                handler.AppendLiteral("\n");
                stringBuilder5.Append(ref handler);
            }
            foreach (string item2 in Main.BestiaryTracker.Sights._wasNearPlayer)
            {
                int nPCId = GetNPCId(item2);
                StringBuilder stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder6 = stringBuilder2;
                StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 2, stringBuilder2);
                handler.AppendFormatted(GetNPCId(item2));
                handler.AppendLiteral(",");
                handler.AppendFormatted(Lang.GetNPCName(nPCId));
                handler.AppendLiteral("\n");
                stringBuilder6.Append(ref handler);
            }
            foreach (string item3 in Main.BestiaryTracker.Chats._chattedWithPlayer)
            {
                int nPCId = GetNPCId(item3);
                StringBuilder stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder7 = stringBuilder2;
                StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 2, stringBuilder2);
                handler.AppendFormatted(GetNPCId(item3));
                handler.AppendLiteral(",");
                handler.AppendFormatted(Lang.GetNPCName(nPCId));
                handler.AppendLiteral("\n");
                stringBuilder7.Append(ref handler);
            }
            Utils.SafeSave(SaveFile, stringBuilder.ToString());
        }

        private static async void Import(TSPlayer op)
        {
            if (!File.Exists(SaveFile))
            {
                op.SendInfoMessage(SaveFile + "文件不存在，无法导入，解锁/清空 全怪物图鉴 会自动生成该文件。");
                return;
            }
            await Task.Run(delegate
            {
                op.SendInfoMessage("正在导入，请稍等……");
                if (kills.Count == 0)
                {
                    ReadEnmeyAndCritter();
                }
                int num = 0;
                string text = "";
                string[] array = File.ReadAllLines(SaveFile);
                foreach (string text2 in array)
                {
                    string[] array2 = text2.Split(',');
                    if (int.TryParse(array2[0], out var result))
                    {
                        int result2 = 0;
                        if (array2.Length > 1 && kills.Contains(result) && int.TryParse(array2[1], out result2))
                        {
                            text = GetBestiaryCreditId(result);
                            if (Main.BestiaryTracker.Kills._killCountsByNpcId.ContainsKey(text))
                            {
                                Main.BestiaryTracker.Kills._killCountsByNpcId[text] = result2;
                            }
                            else
                            {
                                Main.BestiaryTracker.Kills._killCountsByNpcId.Add(text, result2);
                            }
                            num++;
                        }
                        else if (sights.Contains(result))
                        {
                            text = GetBestiaryCreditId(result);
                            if (!Main.BestiaryTracker.Sights._wasNearPlayer.Contains(text))
                            {
                                Main.BestiaryTracker.Sights._wasNearPlayer.Add(text);
                            }
                            num++;
                        }
                        else if (chats.Contains(result))
                        {
                            text = GetBestiaryCreditId(result);
                            if (!Main.BestiaryTracker.Chats._chattedWithPlayer.Contains(text))
                            {
                                Main.BestiaryTracker.Chats._chattedWithPlayer.Add(text);
                            }
                            num++;
                        }
                    }
                }
                BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
                string value = Terraria.Utils.PrettifyPercentDisplay(bestiaryProgressReport.CompletionPercent, "P2");
                op.SendSuccessMessage($"已处理 {num} 条数据。怪物图鉴进度： {value} {bestiaryProgressReport.CompletionAmountTotal}/{bestiaryProgressReport.EntriesTotal}");
            });
        }

        private static string GetBestiaryCreditId(int netID)
        {
            return ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[netID];
        }

        private static int GetNPCId(string key)
        {
            return ContentSamples.NpcNetIdsByPersistentIds[key];
        }

        private static void ReadEnmeyAndCritter()
        {
            HashSet<int> exclusions = GetExclusions();
            foreach (KeyValuePair<int, NPC> item in ContentSamples.NpcsByNetId)
            {
                if (!exclusions.Contains(item.Key) && !item.Value.isLikeATownNPC)
                {
                    if (item.Value.CountsAsACritter)
                    {
                        sights.Add(item.Key);
                    }
                    else
                    {
                        kills.Add(item.Key);
                    }
                }
            }
        }

        private static HashSet<int> GetExclusions()
        {
            HashSet<int> hashSet = new HashSet<int>();
            List<int> list = new List<int>();
            foreach (KeyValuePair<int, NPCID.Sets.NPCBestiaryDrawModifiers> item in NPCID.Sets.NPCBestiaryDrawOffset)
            {
                if (item.Value.Hide)
                {
                    list.Add(item.Key);
                }
            }
            foreach (int item2 in list)
            {
                hashSet.Add(item2);
            }
            return hashSet;
        }
    }
}