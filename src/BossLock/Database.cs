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
        using var result = Db.QueryReader("SELECT * FROM boss_lock WHERE npc_id=@0;", npcId);
        return result.Read() ? result.Get<string>("time") : null;
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
                locks.Add(result.Get<int>("npc_id"), Utils.TimeFormat(time));
            }
        }
        return locks;
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
        Db.Query("UPDATE boss_lock SET time=@0 WHERE npc_id=@1;",newTime, npcId);
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