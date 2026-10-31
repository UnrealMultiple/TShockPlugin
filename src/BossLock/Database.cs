using MySql.Data.MySqlClient;
using System.Data;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace BossLock;

public static class Database
{
    private static IDbConnection Db => TShock.DB;
    public static void Init()
    {
        var tableCreator = new SqlTableCreator(Db, Db.GetSqlQueryBuilder());

        tableCreator.EnsureTableStructure(new SqlTable("boss_lock",
            new SqlColumn("npc_id", MySqlDbType.Int32),
            new SqlColumn("time", MySqlDbType.Text)));
    }

    public static string? LoadBoss(int npcId)
    {
        string? latest = null;
        foreach (var member in BossGroup.Members(npcId))
        {
            using var result = Db.QueryReader("SELECT time FROM boss_lock WHERE npc_id = @0;", member);
            while (result.Read())
            {
                var time = result.Get<string>("time");
                if (latest == null || string.CompareOrdinal(time, latest) > 0) { latest = time; }
            }
        }
        return latest;
    }

    public static void SendAllBoss(CommandArgs args)
    {
        using var result = Db.QueryReader("SELECT * FROM boss_lock;");
        List<Lock> dataInfos = [];
        while (result.Read())
        {
            dataInfos.Add(new Lock
            {
                Time = result.Get<string>("time"),
                BossId = result.Get<int>("npc_id")

            });
        }
        dataInfos = dataInfos.GroupBy(i => BossGroup.Root(i.BossId)).Select(g => new Lock { BossId = g.Key, Time = g.Max(i => i.Time)! }).ToList();
        dataInfos.Sort((x, y) => DateTime.ParseExact(x.Time, "yyyy-MM-dd-HH:mm:ss", null).CompareTo(DateTime.ParseExact(y.Time, "yyyy-MM-dd-HH:mm:ss", null)));
        args.Player.SendInfoMessage(GetString("BossLock配置列表:"));
        foreach (var i in dataInfos)
        {
            args.Player.SendInfoMessage(GetString($"{Lang.GetNPCNameValue(i.BossId)}({i.BossId})解锁时间为:[c/32FF82:{ Utils.TimeFormat(i.Time)}]"));
        }
    }
    
    public static Dictionary<int, string> GetAllLocked()
    {
        using var result = Db.QueryReader("SELECT * FROM boss_lock;");
        Dictionary<int, string> locks = [];
        while (result.Read())
        {
            var time = result.Get<string>("time");
            if (DateTime.ParseExact(time, "yyyy-MM-dd-HH:mm:ss", null) > DateTime.Now)
            {
                var root = BossGroup.Root(result.Get<int>("npc_id"));
                if (!locks.TryGetValue(root, out var previous) || string.CompareOrdinal(time, previous) > 0)
                {
                    locks[root] = time;
                }
            }
        }
        return locks.ToDictionary(entry => entry.Key, entry => Utils.TimeFormat(entry.Value));
    }
        
    public static void SendAllBossSimple(CommandArgs args)
    {
        using var result = Db.QueryReader("SELECT * FROM boss_lock;");
        List<Lock> dataInfos = [];
        while (result.Read())
        {
            dataInfos.Add(new Lock
            {
                Time = result.Get<string>("Time"),
                BossId = result.Get<int>("npc_id")

            });
        }
        dataInfos = dataInfos.GroupBy(i => BossGroup.Root(i.BossId)).Select(g => new Lock { BossId = g.Key, Time = g.Max(i => i.Time)! }).ToList();
        dataInfos.Sort((x, y) => DateTime.ParseExact(x.Time, "yyyy-MM-dd-HH:mm:ss", null).CompareTo(DateTime.ParseExact(y.Time, "yyyy-MM-dd-HH:mm:ss", null)));
        args.Player.SendInfoMessage(GetString("[BossLock列表]"));
        foreach (var i in dataInfos)
        {
            args.Player.SendWarningMessage($"[c/AF4BFF:{Lang.GetNPCNameValue(i.BossId)}] -> [c/32FF82:{Utils.TimeFormat(i.Time)}]");
        }
    }
    public static void SendLastestBoss(TSPlayer plr)
    {
        using var result = Db.QueryReader("SELECT * FROM boss_lock;");
        List<Lock> dataInfos = [];
        while (result.Read())
        {
            dataInfos.Add(new Lock
            {
                Time = result.Get<string>("time"),
                BossId = result.Get<int>("npc_id")

            });
        }
        var emj = plr.RealPlayer ? "[i:43]" : "📝"; 
        dataInfos = dataInfos.GroupBy(i => BossGroup.Root(i.BossId)).Select(g => new Lock { BossId = g.Key, Time = g.Max(i => i.Time)! }).ToList();
        dataInfos.Sort((x, y) => DateTime.ParseExact(x.Time, "yyyy-MM-dd-HH:mm:ss", null).CompareTo(DateTime.ParseExact(y.Time, "yyyy-MM-dd-HH:mm:ss", null)));
        for (var i =0;i<dataInfos.Count;i++)
        {
            if (DateTime.ParseExact(dataInfos[i].Time, "yyyy-MM-dd-HH:mm:ss", null) > DateTime.Now)
            {
                plr.SendWarningMessage(emj + (i - 1 < 0 ? 
                    GetString($"[c/AF4BFF:{Lang.GetNPCNameValue(dataInfos[i].BossId)}]的解锁时间为:") +
                    GetString($"[c/32FF82:{Utils.TimeFormat(dataInfos[i].Time)}] [c/FFFFFF:(/locklist 查看详细)]") :
                    GetString($"[c/AF4BFF:{Lang.GetNPCNameValue(dataInfos[i-1].BossId)}]已解锁! [c/AF4BFF:{Lang.GetNPCNameValue(dataInfos[i].BossId)}]的解锁时间为:") +
                    GetString($"[c/32FF82:{Utils.TimeFormat(dataInfos[i].Time)}] [c/FFFFFF:(/locklist 查看详细)]")));
                return;
            }
        }
    }

    public static void AddBoss(int npcId,string time)
    {
        Db.Query("INSERT INTO boss_lock (npc_id, time) VALUES (@0, @1);", npcId, time);
    }
        
    public static void UpdateTime(int npcId, string newTime)
    {
        WriteRows(new Dictionary<int, string> { [BossGroup.Root(npcId)] = newTime }, false);
    }

    internal static void ReplaceAll(IReadOnlyDictionary<int, string> rows) => WriteRows(rows, true);

    private static void WriteRows(IReadOnlyDictionary<int, string> rows, bool replaceAll)
    {
        // 使用独立连接，避免事务状态影响 TShock/其他插件共享的连接。
        using var connection = (IDbConnection)Activator.CreateInstance(Db.GetType())!;
        connection.ConnectionString = Db.ConnectionString;
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        if (replaceAll)
        {
            command.CommandText = "DELETE FROM boss_lock;";
            command.ExecuteNonQuery();
        }
        else
        {
            command.CommandText = "DELETE FROM boss_lock WHERE npc_id = @id;";
            var memberId = command.CreateParameter();
            memberId.ParameterName = "@id";
            memberId.DbType = DbType.Int32;
            command.Parameters.Add(memberId);
            foreach (var member in rows.Keys.SelectMany(BossGroup.Members).Distinct())
            {
                memberId.Value = member;
                command.ExecuteNonQuery();
            }
        }
        foreach (var row in rows)
        {
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO boss_lock (npc_id, time) VALUES (@id, @time);";
            var id = command.CreateParameter(); id.ParameterName = "@id"; id.Value = row.Key;
            var time = command.CreateParameter(); time.ParameterName = "@time"; time.Value = row.Value;
            command.Parameters.Add(id); command.Parameters.Add(time);
            command.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public static void ClearDb()
    {
        Db.Query("DELETE FROM boss_lock;");
    }

    private class Lock
    {
        public int BossId;
        public string Time = "";
    }

}
