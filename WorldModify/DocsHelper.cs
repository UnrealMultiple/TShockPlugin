using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using static Terraria.Localization.GameCulture;

namespace WorldModify
{
    public class DocsHelper
    {
        public static void GenDocs(TSPlayer op)
        {
            Utils.CreateSaveDir();
            bool flag = true;
            GameCulture activeCulture = Language.ActiveCulture;
            if (activeCulture.LegacyId != 7)
            {
                LanguageManager.Instance.SetLanguage(GameCulture.FromCultureName((CultureName)7));
                flag = false;
            }
            List<string> list = new List<string>
        {
            Utils.CombinePath("[wm]物品清单.txt"),
            Utils.CombinePath("[wm]修饰语清单.txt"),
            Utils.CombinePath("[wm]NPC清单.txt"),
            Utils.CombinePath("[wm]Buff清单.txt"),
            Utils.CombinePath("[wm]射弹清单.txt"),
            Utils.CombinePath("[wm]墙清单.txt")
        };
            DumpItems(list[0]);
            DumpPrefixes(list[1]);
            DumpNPCs(list[2]);
            DumpBuffs(list[3]);
            DumpProjectiles(list[4]);
            DumpWalls(list[5]);
            op.SendInfoMessage("已生成参考文档:\n" + string.Join("\n", list));
            if (flag)
            {
                LanguageManager.Instance.SetLanguage(activeCulture);
            }
        }

        private static void DumpItems(string path)
        {
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_002c: Expected O, but got Unknown
            Regex regex = new Regex("\\n");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,名称,描述");
            for (int i = 1; i < ItemID.Count; i++)
            {
                Item val = new Item();
                val.SetDefaults(i);
                string text = "";
                for (int j = 0; j < val.ToolTip.Lines; j++)
                {
                    text = text + val.ToolTip.GetLine(j) + "\n";
                }
                StringBuilder stringBuilder2 = stringBuilder;
                StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 3, stringBuilder2);
                handler.AppendFormatted(i);
                handler.AppendLiteral(",");
                handler.AppendFormatted(regex.Replace(val.Name, " "));
                handler.AppendLiteral(",");
                handler.AppendFormatted(regex.Replace(text, " ").TrimEnd());
                stringBuilder2.AppendLine(ref handler);
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DumpNPCs(string path)
        {
            //IL_0019: Unknown result type (might be due to invalid IL or missing references)
            //IL_001f: Expected O, but got Unknown
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            //IL_0029: Unknown result type (might be due to invalid IL or missing references)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,名称");
            for (int i = -65; i < NPCID.Count; i++)
            {
                NPC val = new NPC();
                val.SetDefaults(i, default(NPCSpawnParams));
                if (!string.IsNullOrEmpty(val.FullName))
                {
                    StringBuilder stringBuilder2 = stringBuilder;
                    StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
                    handler.AppendFormatted(i);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(val.FullName);
                    stringBuilder2.AppendLine(ref handler);
                }
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DumpBuffs(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,名称,描述");
            for (int i = 0; i < BuffID.Count; i++)
            {
                if (!string.IsNullOrEmpty(Lang.GetBuffName(i)))
                {
                    StringBuilder stringBuilder2 = stringBuilder;
                    StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 3, stringBuilder2);
                    handler.AppendFormatted(i);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(Lang.GetBuffName(i));
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(Lang.GetBuffDescription(i));
                    stringBuilder2.AppendLine(ref handler);
                }
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DumpPrefixes(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,名称");
            for (int i = 0; i < PrefixID.Count; i++)
            {
                string value = ((object)Lang.prefix[i]).ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    StringBuilder stringBuilder2 = stringBuilder;
                    StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
                    handler.AppendFormatted(i);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(value);
                    stringBuilder2.AppendLine(ref handler);
                }
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DumpProjectiles(string path)
        {
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            //IL_001e: Expected O, but got Unknown
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,射弹名称");
            for (int i = 0; i < ProjectileID.Count; i++)
            {
                Projectile val = new Projectile();
                val.SetDefaults(i);
                if (!string.IsNullOrEmpty(val.Name))
                {
                    StringBuilder stringBuilder2 = stringBuilder;
                    StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
                    handler.AppendFormatted(i);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(val.Name);
                    stringBuilder2.AppendLine(ref handler);
                }
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }

        private static void DumpWalls(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("备注：本表的名称参照了wiki。");
            stringBuilder.AppendLine("参考：https://terraria.wiki.gg/zh/wiki/Wall_IDs");
            stringBuilder.AppendLine("id,墙体名称,颜色");
            ResHelper.LoadWall();
            foreach (KeyValuePair<int, WallProp> wall in ResHelper.Walls)
            {
                stringBuilder.AppendLine(wall.Value.ToString());
            }
            File.WriteAllText(path, stringBuilder.ToString());
        }
    }
}
