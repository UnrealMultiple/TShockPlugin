using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using TShockAPI;
using TShockAPI.DB;



namespace WorldModify
{
    internal class ResearchHelper
    {
        public static string SaveFile;

        private static bool isTasking;

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                HelpTxt();
                return;
            }
            switch (args.Parameters[0].ToLower())
            {
                case "unlock":
                    if (isTasking)
                    {
                        op.SendSuccessMessage("有任务正在运行，请稍后再试！");
                    }
                    else
                    {
                        UnlockAll(op);
                    }
                    return;
                case "reset":
                    Reset(op);
                    return;
                case "clear":
                    Reset(op, superReset: true);
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
                if (result > 0 && result < ItemID.Count)
                {
                    UnlockOne(result, op);
                    return;
                }
                op.SendErrorMessage($"物品id 只能在 1~{ItemID.Count} 范围内");
                return;
            }
            List<Item> itemByName = TShock.Utils.GetItemByName(args.Parameters[0]);
            if (itemByName.Count == 0)
            {
                args.Player.SendErrorMessage("无效的物品名!");
            }
            else if (itemByName.Count > 1)
            {
                args.Player.SendMultipleMatchError(itemByName.Select((Item i) => $"{i.Name}({i.netID})"));
            }
            else
            {
                UnlockOne(itemByName[0].netID, op);
            }
            void HelpTxt()
            {
                op.SendInfoMessage("/wm research 指令用法：");
                op.SendInfoMessage("/wm re unlock, 解锁 全物品研究");
                op.SendInfoMessage("/wm re <id/名称>, 研究单个物品");
                op.SendInfoMessage("/wm re import, 导入 物品研究");
                op.SendInfoMessage("/wm re reset, 重置 物品研究");
                op.SendInfoMessage("/wm re clear, 清空 物品研究（所有地图）");
                op.SendInfoMessage("/wm re backup，备份 物品研究 到 csv文件，解锁和清空前会自动备份");
            }
        }

        private static void UnlockOne(int id, TSPlayer op)
        {
            if (!CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(id))
            {
                op.SendErrorMessage($"id={id}的物品无法研究。");
                return;
            }
            int num = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[id];
            TShock.ResearchDatastore.SacrificeItem(id, num, op);
            NetPacket val = NetCreativeUnlocksModule.SerializeItemSacrifice(id, num);
            NetManager.Instance.Broadcast(val, -1);
            op.SendErrorMessage($"{Lang.GetItemName(id)} 已研究。id:{id} 研究数:{num}");
        }

        private static async void UnlockAll(TSPlayer op)
        {
            await Task.Run(delegate
            {
                isTasking = true;
                Backup();
                op.SendInfoMessage("正在解锁，请稍等……");
                Dictionary<int, int> sacrificeCountNeededByItemId = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId;
                foreach (KeyValuePair<int, int> item in sacrificeCountNeededByItemId)
                {
                    TShock.ResearchDatastore.SacrificeItem(item.Key, item.Value, op);
                    NetPacket val = NetCreativeUnlocksModule.SerializeItemSacrifice(item.Key, item.Value);
                    NetManager.Instance.Broadcast(val, -1);
                }
                op.SendSuccessMessage($"已解锁 {sacrificeCountNeededByItemId.Count} 个物品研究");
                isTasking = false;
            });
        }

        private static void Backup()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<int, int> sacrificedItem in TShock.ResearchDatastore.GetSacrificedItems())
            {
                StringBuilder stringBuilder2 = stringBuilder;
                StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 3, stringBuilder2);
                handler.AppendFormatted(sacrificedItem.Key);
                handler.AppendLiteral(",");
                handler.AppendFormatted(sacrificedItem.Value);
                handler.AppendLiteral(",");
                handler.AppendFormatted(Lang.GetItemName(sacrificedItem.Key));
                handler.AppendLiteral("\n");
                stringBuilder2.Append(ref handler);
            }
            Utils.SafeSave(SaveFile, stringBuilder.ToString());
        }

        private static async void Import(TSPlayer op)
        {
            if (!File.Exists(SaveFile))
            {
                op.SendInfoMessage(SaveFile + "文件不存在，请在此文件的每一行填写好“物品id,物品数量”，然后再导入。");
                return;
            }
            await Task.Run(delegate
            {
                op.SendInfoMessage("正在导入，请稍等……");
                int num = 0;
                string[] array = File.ReadAllLines(SaveFile);
                foreach (string text in array)
                {
                    string[] array2 = text.Split(',');
                    if (array2.Length >= 2 && int.TryParse(array2[0], out var result) && int.TryParse(array2[0], out var result2))
                    {
                        TShock.ResearchDatastore.SacrificeItem(result, result2, op);
                        NetPacket val = NetCreativeUnlocksModule.SerializeItemSacrifice(result, result2);
                        NetManager.Instance.Broadcast(val, -1);
                        num++;
                    }
                }
                op.SendSuccessMessage($"已导入 {num} 个物品研究");
            });
        }

        private static async void Reset(TSPlayer op, bool superReset = false)
        {
            await Task.Run(delegate
            {
                IDbConnection dB = TShock.DB;
                IDbConnection olddb = dB;
                SqlTable table = new SqlTable("Research", new SqlColumn("WorldId", (MySqlDbType)3), new SqlColumn("PlayerId", (MySqlDbType)3), new SqlColumn("ItemId", (MySqlDbType)3), new SqlColumn("AmountSacrificed", (MySqlDbType)3), new SqlColumn("TimeSacrificed", (MySqlDbType)12));
                IQueryBuilder provider;
                if (dB.GetSqlType() != SqlType.Sqlite)
                {
                    IQueryBuilder queryBuilder = new MysqlQueryCreator();
                    provider = queryBuilder;
                }
                else
                {
                    IQueryBuilder queryBuilder = new SqliteQueryCreator();
                    provider = queryBuilder;
                }
                SqlTableCreator sqlTableCreator = new SqlTableCreator(dB, provider);
                try
                {
                    sqlTableCreator.EnsureTableStructure(table);
                }
                catch (DllNotFoundException)
                {
                    Console.WriteLine("Possible problem with your database - is Sqlite3.dll present?");
                    throw new Exception("Could not find a database library (probably Sqlite3.dll)");
                }
                string query = (superReset ? "DELETE FROM Research WHERE NOT WorldId = @0" : "DELETE FROM Research WHERE WorldId = @0");
                if (!superReset)
                {
                    Backup();
                }
                try
                {
                    olddb.Query(query, Main.worldID);
                }
                catch (Exception ex2)
                {
                    TShock.Log.Error(ex2.ToString());
                }
                if (superReset)
                {
                    op.SendInfoMessage("历史世界 的 物品研究 已清空");
                }
                else
                {
                    op.SendInfoMessage("当前世界 的 物品研究 已清空，重开服后有效！");
                }
            });
        }

        public static int GetSacrificeTotal()
        {
            return CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Count;
        }

        public static int GetSacrificeCompleted()
        {
            Dictionary<int, int> sacrificedItems = TShock.ResearchDatastore.GetSacrificedItems();
            int num = 0;
            int num3 = default(int);
            foreach (int key in sacrificedItems.Keys)
            {
                int num2 = sacrificedItems[key];
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(key, out num3);
                if (num2 >= num3)
                {
                    num++;
                }
            }
            return num;
        }
    }
}