using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace ZHIPlayerManager;

public partial class ZHIPM
{
    /// <summary>
    /// 备份数据库类
    /// </summary>
    public class ZplayerDB
    {
        private readonly IDbConnection database;

        private readonly string tableName = "";


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public ZplayerDB(IDbConnection db)
        {
            this.database = db;
            this.tableName = "Zhipm_PlayerBackUp";
            var table = new SqlTable(this.tableName, new SqlColumn("AccAndSlot", MySqlDbType.Text, 255)
            {
                Primary = true
            }, new SqlColumn("Account", MySqlDbType.Int32), new SqlColumn("Name", MySqlDbType.Text), new SqlColumn("Health", MySqlDbType.Int32), new SqlColumn("MaxHealth", MySqlDbType.Int32), new SqlColumn("Mana", MySqlDbType.Int32), new SqlColumn("MaxMana", MySqlDbType.Int32), new SqlColumn("Inventory", MySqlDbType.Text), new SqlColumn("extraSlot", MySqlDbType.Int32), new SqlColumn("spawnX", MySqlDbType.Int32), new SqlColumn("spawnY", MySqlDbType.Int32), new SqlColumn("skinVariant", MySqlDbType.Int32), new SqlColumn("hair", MySqlDbType.Int32), new SqlColumn("hairDye", MySqlDbType.Int32), new SqlColumn("hairColor", MySqlDbType.Int32), new SqlColumn("pantsColor", MySqlDbType.Int32), new SqlColumn("shirtColor", MySqlDbType.Int32), new SqlColumn("underShirtColor", MySqlDbType.Int32), new SqlColumn("shoeColor", MySqlDbType.Int32), new SqlColumn("hideVisuals", MySqlDbType.Int32), new SqlColumn("skinColor", MySqlDbType.Int32), new SqlColumn("eyeColor", MySqlDbType.Int32), new SqlColumn("questsCompleted", MySqlDbType.Int32), new SqlColumn("usingBiomeTorches", MySqlDbType.Int32), new SqlColumn("happyFunTorchTime", MySqlDbType.Int32), new SqlColumn("unlockedBiomeTorches", MySqlDbType.Int32), new SqlColumn("currentLoadoutIndex", MySqlDbType.Int32), new SqlColumn("ateArtisanBread", MySqlDbType.Int32), new SqlColumn("usedAegisCrystal", MySqlDbType.Int32), new SqlColumn("usedAegisFruit", MySqlDbType.Int32), new SqlColumn("usedArcaneCrystal", MySqlDbType.Int32), new SqlColumn("usedGalaxyPearl", MySqlDbType.Int32), new SqlColumn("usedGummyWorm", MySqlDbType.Int32), new SqlColumn("usedAmbrosia", MySqlDbType.Int32), new SqlColumn("unlockedSuperCart", MySqlDbType.Int32), new SqlColumn("enabledSuperCart", MySqlDbType.Int32));
            var queryBuilder = this.database.GetSqlType() != SqlType.Sqlite ? new MysqlQueryCreator() : (IQueryBuilder) new SqliteQueryCreator();
            queryBuilder.CreateTable(table);
            var sqlTableCreator = new SqlTableCreator(this.database, queryBuilder);
            sqlTableCreator.EnsureTableStructure(table);
        }


        /// <summary>
        /// 从数据库读取一个玩家的备份存档槽，第slot个，此方法已对exist设置过
        /// </summary>
        /// <param name="player">一个没有什么意义的玩家</param>
        /// <param name="acctid">需要的那个玩家的账号ID</param>
        /// <param name="slot">第几个存档槽</param>
        /// <returns></returns>
        public PlayerData ReadZPlayerDB(TSPlayer player, int acctid, int slot = 1)
        {
            var playerData = new PlayerData(true)
            {
                exists = false
            };
            try
            {
                using var queryResult = this.database.QueryReader("SELECT * FROM " + this.tableName + " WHERE AccAndSlot=@0", acctid + "-" + slot);
                if (queryResult.Read())
                {
                    playerData.exists = true;
                    playerData.health = queryResult.Get<int>("Health");
                    playerData.maxHealth = queryResult.Get<int>("MaxHealth");
                    playerData.mana = queryResult.Get<int>("Mana");
                    playerData.maxMana = queryResult.Get<int>("MaxMana");
                    var list = queryResult.Get<string>("Inventory").Split('~').Select(NetItem.Parse)
                        .ToList();
                    if (list.Count < NetItem.MaxInventory)
                    {
                        list.InsertRange(67, new NetItem[2]);
                        list.InsertRange(77, new NetItem[2]);
                        list.InsertRange(87, new NetItem[2]);
                        list.AddRange(new NetItem[NetItem.MaxInventory - list.Count]);
                    }

                    playerData.inventory = list.ToArray();
                    playerData.extraSlot = queryResult.Get<int>("extraSlot");
                    playerData.spawnX = queryResult.Get<int>("spawnX");
                    playerData.spawnY = queryResult.Get<int>("spawnY");
                    playerData.skinVariant = queryResult.Get<int?>("skinVariant");
                    playerData.hair = queryResult.Get<int?>("hair");
                    playerData.hairDye = (byte) queryResult.Get<int>("hairDye");
                    playerData.hairColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("hairColor"));
                    playerData.pantsColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("pantsColor"));
                    playerData.shirtColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("shirtColor"));
                    playerData.underShirtColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("underShirtColor"));
                    playerData.shoeColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("shoeColor"));
                    playerData.hideVisuals = TShock.Utils.DecodeBoolArray(queryResult.Get<int?>("hideVisuals"));
                    playerData.skinColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("skinColor"));
                    playerData.eyeColor = TShock.Utils.DecodeColor(queryResult.Get<int?>("eyeColor"));
                    playerData.questsCompleted = queryResult.Get<int>("questsCompleted");
                    playerData.usingBiomeTorches = queryResult.Get<int>("usingBiomeTorches");
                    playerData.happyFunTorchTime = queryResult.Get<int>("happyFunTorchTime");
                    playerData.unlockedBiomeTorches = queryResult.Get<int>("unlockedBiomeTorches");
                    playerData.currentLoadoutIndex = queryResult.Get<int>("currentLoadoutIndex");
                    playerData.ateArtisanBread = queryResult.Get<int>("ateArtisanBread");
                    playerData.usedAegisCrystal = queryResult.Get<int>("usedAegisCrystal");
                    playerData.usedAegisFruit = queryResult.Get<int>("usedAegisFruit");
                    playerData.usedArcaneCrystal = queryResult.Get<int>("usedArcaneCrystal");
                    playerData.usedGalaxyPearl = queryResult.Get<int>("usedGalaxyPearl");
                    playerData.usedGummyWorm = queryResult.Get<int>("usedGummyWorm");
                    playerData.usedAmbrosia = queryResult.Get<int>("usedAmbrosia");
                    playerData.unlockedSuperCart = queryResult.Get<int>("unlockedSuperCart");
                    playerData.enabledSuperCart = queryResult.Get<int>("enabledSuperCart");
                }
                return playerData;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ReadZPlayerDB ") + ex);
                return playerData;
            }
        }


        /// <summary>
        /// 将这个玩家的备份数据精准写入到第几个槽
        /// </summary>
        /// <param name="player"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool WriteZPlayerDB(TSPlayer player, int slot)
        {
            if (!player.IsLoggedIn)
            {
                return false;
            }
            if (slot > config.MaxBackupsPerPlayer || slot < 1)
            {
                return false;
            }
            var playerData = player.PlayerData;
            playerData.CopyCharacter(player);
            //如果没读到，就是不存在，那么写入
            if (!this.ReadZPlayerDB(player, player.Account.ID, slot).exists)
            {
                try
                {
                    this.database.Query("INSERT INTO " + this.tableName + " (AccAndSlot, Account, Name, Health, MaxHealth, Mana, MaxMana, Inventory, extraSlot, spawnX, spawnY, skinVariant, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, hideVisuals, skinColor, eyeColor, questsCompleted, usingBiomeTorches, happyFunTorchTime, unlockedBiomeTorches, currentLoadoutIndex,ateArtisanBread, usedAegisCrystal, usedAegisFruit, usedArcaneCrystal, usedGalaxyPearl, usedGummyWorm, usedAmbrosia, unlockedSuperCart, enabledSuperCart) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20, @21, @22, @23, @24, @25, @26, @27, @28, @29, @30, @31, @32, @33, @34, @35);", player.Account.ID + "-" + slot, player.Account.ID, player.Account.Name, player.TPlayer.statLife, player.TPlayer.statLifeMax, player.TPlayer.statMana, player.TPlayer.statManaMax, string.Join("~", playerData.inventory), player.TPlayer.extraAccessory ? 1 : 0, player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.skinVariant, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor)!, TShock.Utils.EncodeColor(player.TPlayer.pantsColor)!, TShock.Utils.EncodeColor(player.TPlayer.shirtColor)!, TShock.Utils.EncodeColor(player.TPlayer.underShirtColor)!, TShock.Utils.EncodeColor(player.TPlayer.shoeColor)!, TShock.Utils.EncodeBoolArray(player.TPlayer.hideVisibleAccessory)!, TShock.Utils.EncodeColor(player.TPlayer.skinColor)!, TShock.Utils.EncodeColor(player.TPlayer.eyeColor)!, player.TPlayer.anglerQuestsFinished, player.TPlayer.UsingBiomeTorches ? 1 : 0, player.TPlayer.happyFunTorchTime ? 1 : 0, player.TPlayer.unlockedBiomeTorches ? 1 : 0, player.TPlayer.CurrentLoadoutIndex, player.TPlayer.ateArtisanBread ? 1 : 0, player.TPlayer.usedAegisCrystal ? 1 : 0, player.TPlayer.usedAegisFruit ? 1 : 0, player.TPlayer.usedArcaneCrystal ? 1 : 0, player.TPlayer.usedGalaxyPearl ? 1 : 0, player.TPlayer.usedGummyWorm ? 1 : 0, player.TPlayer.usedAmbrosia ? 1 : 0, player.TPlayer.unlockedSuperCart ? 1 : 0, player.TPlayer.enabledSuperCart ? 1 : 0);
                    return true;
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError(GetString("错误：WriteZPlayerDB ") + ex);
                    return false;
                }
            }
            //如果读到了，就是更新
            try
            {
                this.database.Query("UPDATE " + this.tableName + " SET Name = @0, Health = @1, MaxHealth = @2, Mana = @3, MaxMana = @4, Inventory = @5, Account = @6, spawnX = @7, spawnY = @8, hair = @9, hairDye = @10, hairColor = @11, pantsColor = @12, shirtColor = @13, underShirtColor = @14, shoeColor = @15, hideVisuals = @16, skinColor = @17, eyeColor = @18, questsCompleted = @19, skinVariant = @20, extraSlot = @21, usingBiomeTorches = @22, happyFunTorchTime = @23, unlockedBiomeTorches = @24, currentLoadoutIndex = @25, ateArtisanBread = @26, usedAegisCrystal = @27, usedAegisFruit = @28, usedArcaneCrystal = @29, usedGalaxyPearl = @30, usedGummyWorm = @31, usedAmbrosia = @32, unlockedSuperCart = @33, enabledSuperCart = @34 WHERE AccAndSlot = @35;", player.Account.Name, player.TPlayer.statLife, player.TPlayer.statLifeMax, player.TPlayer.statMana, player.TPlayer.statManaMax, string.Join("~", playerData.inventory), player.Account.ID, player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor)!, TShock.Utils.EncodeColor(player.TPlayer.pantsColor)!, TShock.Utils.EncodeColor(player.TPlayer.shirtColor)!, TShock.Utils.EncodeColor(player.TPlayer.underShirtColor)!, TShock.Utils.EncodeColor(player.TPlayer.shoeColor)!, TShock.Utils.EncodeBoolArray(player.TPlayer.hideVisibleAccessory)!, TShock.Utils.EncodeColor(player.TPlayer.skinColor)!, TShock.Utils.EncodeColor(player.TPlayer.eyeColor)!, player.TPlayer.anglerQuestsFinished, player.TPlayer.skinVariant, player.TPlayer.extraAccessory ? 1 : 0, player.TPlayer.UsingBiomeTorches ? 1 : 0, player.TPlayer.happyFunTorchTime ? 1 : 0, player.TPlayer.unlockedBiomeTorches ? 1 : 0, player.TPlayer.CurrentLoadoutIndex, player.TPlayer.ateArtisanBread ? 1 : 0, player.TPlayer.usedAegisCrystal ? 1 : 0, player.TPlayer.usedAegisFruit ? 1 : 0, player.TPlayer.usedArcaneCrystal ? 1 : 0, player.TPlayer.usedGalaxyPearl ? 1 : 0, player.TPlayer.usedGummyWorm ? 1 : 0, player.TPlayer.usedAmbrosia ? 1 : 0, player.TPlayer.unlockedSuperCart ? 1 : 0, player.TPlayer.enabledSuperCart ? 1 : 0, player.Account.ID + "-" + slot);
                return true;
            }
            catch (Exception ex2)
            {
                TShock.Log.ConsoleError(GetString("错误：WriteZPlayerDB 2 ") + ex2);
                return false;
            }
        }


        /// <summary>
        /// 获取这个用户目前几个备份槽了
        /// </summary>
        /// <param name="player">没意义的玩家</param>
        /// <param name="acctid">需要搜索的该玩家的ID</param>
        /// <param name="text">返回这些槽的name</param>
        /// <returns></returns>
        public int getZPlayerDBMaxSlot(TSPlayer player, int acctid, out List<string> text)
        {
            var num = 0;
            text = new List<string>();
            try
            {
                using (var queryResult = this.database.QueryReader("SELECT * FROM " + this.tableName + " WHERE Account=@0", acctid))
                {
                    while (queryResult.Read())
                    {
                        num++;
                        text.Add(queryResult.Get<string>("AccAndSlot"));
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：getZPlayerDBMaxSlot ") + ex);
            }
            return num;
        }


        /// <summary>
        /// 添加一个用户的备份槽，自动将存档槽像后排，排到大于上限时自动删除
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool AddZPlayerDB(TSPlayer player)
        {
            if (player == null || !player.IsLoggedIn)
            {
                return false;
            }
            var num = this.getZPlayerDBMaxSlot(player, player.Account.ID, out var text);
            if (num < config.MaxBackupsPerPlayer)
            {
                try
                {
                    for (var i = num + 1; i > 1; i--)
                    {
                        this.database.Query("UPDATE " + this.tableName + " SET AccAndSlot = @0 WHERE AccAndSlot = @1;", player.Account.ID + "-" + i, player.Account.ID + "-" + (i - 1));
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError(GetString("错误：AddZPlayerDB ") + ex);
                    return false;
                }
                return this.WriteZPlayerDB(player, 1);
            }

            try
            {
                for (var c = 1; c < config.MaxBackupsPerPlayer; c++)
                {
                    text.RemoveAll(x => x.Equals(player.Account.ID + "-" + c));
                }
                foreach (var str in text)
                {
                    this.database.Query("DELETE FROM " + this.tableName + " WHERE AccAndSlot = @0;", str);
                }
            }
            catch (Exception ex2)
            {
                TShock.Log.ConsoleError(GetString("错误：AddZPlayerDB 1 ") + ex2);
                return false;
            }
            return this.AddZPlayerDB(player);
        }


        /// <summary>
        /// 清理所有人的备份存档
        /// </summary>
        /// <param name="zdb"></param>
        /// <returns></returns>
        public bool ClearALLZPlayerDB(ZplayerDB zdb)
        {
            try
            {
                this.database.Query("DROP TABLE " + this.tableName);
                zdb = new ZplayerDB(TShock.DB);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString("错误：ClearALLZPlayerDB ") + ex);
                return false;
            }
        }


        /// <summary>
        /// 清理该用户的备份存档
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool ClearZPlayerDB(int account)
        {
            try
            {
                this.database.Query("DELETE FROM " + this.tableName + " WHERE Account = @0;", account);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ClearZPlayerDB ") + ex);
                return false;
            }
        }
    }


    /// <summary>
    /// 额外数据库类
    /// </summary>
    public class ZplayerExtraDB
    {
        private readonly IDbConnection database;
        private readonly string tableName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public ZplayerExtraDB(IDbConnection db)
        {
            this.database = db;
            this.tableName = "Zhipm_PlayerExtra";
            var table = new SqlTable(this.tableName, new SqlColumn("Account", MySqlDbType.Int32)
            {
                Primary = true
            }, new SqlColumn("Name", MySqlDbType.Text), new SqlColumn("time", MySqlDbType.Int64), new SqlColumn("backuptime", MySqlDbType.Int32), new SqlColumn("killNPCnum", MySqlDbType.Int32), new SqlColumn("killBossID", MySqlDbType.Text), new SqlColumn("killRareNPCID", MySqlDbType.Text), new SqlColumn("point", MySqlDbType.Int64), new SqlColumn("hideKillTips", MySqlDbType.Int32), new SqlColumn("hidePointTips", MySqlDbType.Int32), new SqlColumn("deathCount", MySqlDbType.Int32));
            var queryBuilder = this.database.GetSqlType() != SqlType.Sqlite ? new MysqlQueryCreator() : (IQueryBuilder) new SqliteQueryCreator();
            queryBuilder.CreateTable(table);
            var sqlTableCreator = new SqlTableCreator(this.database, queryBuilder);
            sqlTableCreator.EnsureTableStructure(table);
        }


        /// <summary>
        /// 从数据库中读取一个用户的额外数据库数据
        /// </summary>
        /// <param name="account">账户id,不是游戏内索引</param>
        /// <returns></returns>
        public ExtraData? ReadExtraDB(int account)
        {
            ExtraData? extraData = null;
            try
            {
                using (var queryResult = this.database.QueryReader("SELECT * FROM " + this.tableName + " WHERE Account = @0", account))
                {
                    if (queryResult.Read())
                    {
                        extraData = new ExtraData
                        {
                            Account = account,
                            Name = queryResult.Get<string>("Name"),
                            time = queryResult.Get<long>("time"),
                            backuptime = queryResult.Get<int>("backuptime"),
                            killNPCnum = queryResult.Get<int>("killNPCnum"),
                            killBossID = KillNpcStringToDictionary(queryResult.Get<string>("killBossID")),
                            killRareNPCID = KillNpcStringToDictionary(queryResult.Get<string>("killRareNPCID")),
                            point = queryResult.Get<long>("point"),
                            hideKillTips = queryResult.Get<int>("hideKillTips") != 0,
                            hidePointTips = queryResult.Get<int>("hidePointTips") != 0,
                            deathCount = queryResult.Get<int>("deathCount")
                        };
                    }
                }
                return extraData;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ReadExtraDB ") + ex);
                return null;
            }
        }


        /// <summary>
        /// 获取这个玩家游戏的时间，从数据库里获取
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public long getPlayerExtraDBTime(int account)
        {
            var extraData = this.ReadExtraDB(account);
            return extraData == null ? 0L : extraData.time;
        }


        /// <summary>
        /// 将这个用户的额外数据写入数据库
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ed"></param>
        /// <returns></returns>
        public bool WriteExtraDB(ExtraData ed)
        {
            //如果数据库中没有这个数据，那么新增一个
            if (this.ReadExtraDB(ed.Account) == null)
            {
                try
                {
                    this.database.Query("INSERT INTO " + this.tableName + " (Account, Name, time, backuptime, killNPCnum, killBossID, killRareNPCID, point, hideKillTips, hidePointTips, deathCount) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);", ed.Account, ed.Name, ed.time, ed.backuptime, ed.killNPCnum, DictionaryToKillNpcString(ed.killBossID), DictionaryToKillNpcString(ed.killRareNPCID), ed.point, ed.hideKillTips ? 1 : 0, ed.hidePointTips ? 1 : 0, ed.deathCount);
                    return true;
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(GetString("错误：WriteExtraDB ") + ex);
                    return false;
                }
            }
            //否则就更新数据
            try
            {
                this.database.Query("UPDATE " + this.tableName + " SET Name = @0, time = @1, backuptime = @3, killNPCnum = @4, killBossID = @5, killRareNPCID = @6, point = @7, hideKillTips = @8, hidePointTips = @9, deathCount = @10 WHERE Account = @2;", ed.Name, ed.time, ed.Account, ed.backuptime, ed.killNPCnum, DictionaryToKillNpcString(ed.killBossID), DictionaryToKillNpcString(ed.killRareNPCID), ed.point, ed.hideKillTips ? 1 : 0, ed.hidePointTips ? 1 : 0, ed.deathCount);
                return true;
            }
            catch (Exception ex2)
            {
                TShock.Log.ConsoleError(GetString("错误：WriteExtraDB 2 ") + ex2);
                return false;
            }
        }


        /// <summary>
        /// 清理所有用户的额外数据库，从数据库里删除
        /// </summary>
        /// <param name="zedb"></param>
        /// <returns></returns>
        public bool ClearALLZPlayerExtraDB(ZplayerExtraDB zedb)
        {
            try
            {
                this.database.Query("DROP TABLE " + this.tableName);
                zedb = new ZplayerExtraDB(TShock.DB);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ClearALLZPlayerExtraDB ") + ex);
                return false;
            }
        }


        /// <summary>
        /// 清理某个账户的额外数据库，从数据库里删除
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool ClearZPlayerExtraDB(int account)
        {
            try
            {
                this.database.Query("DELETE FROM " + this.tableName + " WHERE Account = @0;", account);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ClearZPlayerExtraDB ") + ex);
                return false;
            }
        }


        /// <summary>
        /// 获取当前额外数据库的所有成员，按照顺序类型排序，注意某些数据无法排序会按照Account默认排列
        /// </summary>
        /// <param name="extraDataDate"> 你想要排序的类型 </param>
        /// <param name="asc"> 是否升序，若为否就是降序 </param>
        /// <returns></returns>
        public List<ExtraData> ListAllExtraDB(ExtraDataDate extraDataDate = ExtraDataDate.Account, bool asc = true)
        {
            var list = new List<ExtraData>();
            var sqlmeg = "SELECT * FROM " + this.tableName;
            switch (extraDataDate)
            {
                case ExtraDataDate.Account:
                case ExtraDataDate.hidePointTips:
                case ExtraDataDate.hidekillTips:
                case ExtraDataDate.killBossID:
                case ExtraDataDate.killRareNPCID:
                    sqlmeg += " ORDER BY Account"; break;
                case ExtraDataDate.Name:
                    sqlmeg += " ORDER BY Name"; break;
                case ExtraDataDate.time:
                    sqlmeg += " ORDER BY time"; break;
                case ExtraDataDate.backuptime:
                    sqlmeg += " ORDER BY backuptime"; break;
                case ExtraDataDate.killNPCnum:
                    sqlmeg += " ORDER BY killNPCnum"; break;
                case ExtraDataDate.point:
                    sqlmeg += " ORDER BY point"; break;
                case ExtraDataDate.deathCount:
                    sqlmeg += " ORDER BY deathCount"; break;
            }
            if (asc)
            {
                sqlmeg += " ASC";
            }
            else
            {
                sqlmeg += " DESC";
            }

            try
            {
                using (var queryResult = this.database.QueryReader(sqlmeg))
                {
                    while (queryResult.Read())
                    {
                        list.Add(new ExtraData
                        {
                            Account = queryResult.Get<int>("Account"),
                            Name = queryResult.Get<string>("Name"),
                            time = queryResult.Get<long>("time"),
                            backuptime = queryResult.Get<int>("backuptime"),
                            killNPCnum = queryResult.Get<int>("killNPCnum"),
                            killBossID = KillNpcStringToDictionary(queryResult.Get<string>("killBossID")),
                            killRareNPCID = KillNpcStringToDictionary(queryResult.Get<string>("killRareNPCID")),
                            point = queryResult.Get<long>("point"),
                            hideKillTips = queryResult.Get<int>("hideKillTips") != 0,
                            hidePointTips = queryResult.Get<int>("hidePointTips") != 0,
                            deathCount = queryResult.Get<int>("deathCount")
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString("错误：ListAllExtraDB ") + ex);

                return list;
            }
        }
    }

    /// <summary>
    /// 额外数据库的类
    /// </summary>
    public class ExtraData
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        public int Account;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 在线总时长，单位秒
        /// </summary>
        public long time;
        /// <summary>
        /// 备份间隔，单位分钟
        /// </summary>
        public int backuptime;
        /// <summary>
        /// 击杀生物数
        /// </summary>
        public int killNPCnum;
        /// <summary>
        /// 击杀boss的id统计，id -> 击杀数
        /// </summary>
        public Dictionary<int, int> killBossID;
        /// <summary>
        /// 击杀罕见生物的id统计，id -> 击杀数
        /// </summary>
        public Dictionary<int, int> killRareNPCID;
        /// <summary>
        /// 点数
        /// </summary>
        public long point;
        /// <summary>
        /// 隐藏击杀 npc +1 的字
        /// </summary>
        public bool hideKillTips;
        /// <summary>
        /// 隐藏点数 +1$ 的字
        /// </summary>
        public bool hidePointTips;
        /// <summary>
        /// 死亡次数
        /// </summary>
        public int deathCount;

        //不写入数据库的变量
        /// <summary>
        /// 上次的死亡地点
        /// </summary>
        public Vector2 deadPos;


        public ExtraData()
        {
            this.Account = -1;
            this.Name = string.Empty;
            this.time = 0;
            this.backuptime = 0;
            this.killNPCnum = 0;
            this.killBossID = new Dictionary<int, int>();
            this.killRareNPCID = new Dictionary<int, int>();
            this.point = 0L;
            this.hideKillTips = false;
            this.hidePointTips = false;
            this.deathCount = 0;
            this.deadPos = Vector2.Zero;
        }

        public ExtraData(int Account, string Name, long time, int backuptime, int killNPCnum, long point, bool hideKillTips, bool hidePointTips)
        {
            this.Account = Account;
            this.Name = Name;
            this.time = time;
            this.backuptime = backuptime;
            this.killNPCnum = killNPCnum;
            this.killBossID = new Dictionary<int, int>();
            this.killRareNPCID = new Dictionary<int, int>();
            this.point = point;
            this.hideKillTips = hideKillTips;
            this.hidePointTips = hidePointTips;
            this.deathCount = 0;
            this.deadPos = Vector2.Zero;
        }
    }


    /// <summary>
    /// ExtraData类内成员名字的枚举
    /// </summary>
    public enum ExtraDataDate
    {
        Account, Name, time, backuptime, killNPCnum, killBossID, killRareNPCID, point, hidekillTips, hidePointTips, deathCount
    }
}