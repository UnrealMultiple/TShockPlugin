using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI.DB;

namespace PvPer
{
    public class DbManager
    {
        private static IDbConnection _db;

        public DbManager(IDbConnection db)
        {
            _db = db;

            var sqlCreator = new SqlTableCreator(db, new SqliteQueryCreator());

            sqlCreator.EnsureTableStructure(new SqlTable("Players",
                new SqlColumn("AccountID", MySqlDbType.Int32) { Primary = true, Unique = true },
                new SqlColumn("Kills", MySqlDbType.Int32),
                new SqlColumn("Deaths", MySqlDbType.Int32),
                new SqlColumn("WinStreak", MySqlDbType.Int32) // 添加WinStreak列
                ));
        }

        public bool InsertPlayer(int accountID, int kills = 0, int deaths = 0, int winStreak =0)
        {
            return _db.Query("INSERT INTO Players (AccountID, Kills, Deaths, WinStreak) VALUES (@0, @1, @2, @3)", accountID, kills, deaths, winStreak) != 0;
        }

        public bool SavePlayer(DPlayer player)
        {
            return _db.Query("UPDATE Players SET Kills = @1, Deaths = @2, WinStreak = @3 WHERE AccountID = @0", player.AccountID, player.Kills, player.Deaths, player.WinStreak) != 0;
        }

        public DPlayer GetDPlayer(int accountID)
        {
            using var reader = _db.QueryReader("SELECT * FROM Players WHERE AccountID = @0", accountID);
            while (reader.Read())
            {
                return new DPlayer(reader.Get<int>("AccountID"), reader.Get<int>("Kills"), reader.Get<int>("Deaths"), reader.Get<int>("WinStreak"));
            }
            throw new NullReferenceException();
        }

        public List<DPlayer> GetAllDPlayers()
        {
            List<DPlayer> list = new List<DPlayer>();
            using var reader = _db.QueryReader("SELECT * FROM Players");
            while (reader.Read())
            {
                list.Add(new DPlayer(reader.Get<int>("AccountID"), reader.Get<int>("Kills"), reader.Get<int>("Deaths"), reader.Get<int>("WinStreak"))); // 添加WinStreak参数
            }
            return list;
        }

        // 清空玩家数据表中的所有记录（羽学加）
        public static bool ClearData()
        {
            // 删除Players表中的所有记录
            return _db.Query("DELETE FROM Players") != 0;
        }
    }
}