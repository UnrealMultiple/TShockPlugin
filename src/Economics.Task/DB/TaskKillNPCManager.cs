using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Task.DB;

internal class TaskKillNPCManager
{
    private readonly IDbConnection database;

    private static readonly Dictionary<string, Dictionary<int, int>> UserKillNpcs = new();
    public TaskKillNPCManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("TaskKillNPC",
            new SqlColumn("NPCID", MySqlDbType.Int32),
            new SqlColumn("Name", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("Count", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.GetPlayerkill();
    }

    private void GetPlayerkill()
    {
        UserKillNpcs.Clear();
        using var read = this.database.QueryReader("select * from `TaskKillNPC`");
        while (read.Read())
        {
            var NPCID = read.Get<int>("NPCID");
            var Name = read.Get<string>("Name");
            var Count = read.Get<int>("Count");
            if (!UserKillNpcs.TryGetValue(Name, out var data) || data == null)
            {
                UserKillNpcs[Name] = new();
            }

            if (UserKillNpcs[Name].ContainsKey(NPCID))
            {
                UserKillNpcs[Name][NPCID] += Count;
            }
            else
            {
                UserKillNpcs[Name][NPCID] = Count;
            }
        }
    }

    public void AddKillNpc(string Name, int Npcid)
    {
        if (UserKillNpcs.TryGetValue(Name, out var npc))
        {
            if (npc == null)
            {
                UserKillNpcs[Name] = new Dictionary<int, int> { { Npcid, 1 } };
                this.Write(Name, Npcid, 1);
            }
            else
            {
                if (npc.ContainsKey(Npcid))
                {
                    UserKillNpcs[Name][Npcid] += 1;
                    this.Update(Name, Npcid, UserKillNpcs[Name][Npcid]);
                }
                else
                {
                    UserKillNpcs[Name][Npcid] = 1;
                    this.Write(Name, Npcid, 1);
                }
            }
        }
        else
        {
            UserKillNpcs[Name] = new Dictionary<int, int> { { Npcid, 1 } };
            this.Write(Name, Npcid, 1);
        }
    }

    public bool KillNpcsByID(string Name, int NPCid, int count)
    {
        if (UserKillNpcs.TryGetValue(Name, out var killnpcs) && killnpcs != null)
        {
            if (killnpcs.TryGetValue(NPCid, out var num))
            {
                return num >= count;
            }
        }
        return false;
    }

    public int GetKillNpcsCountByID(string Name, int NPCid)
    {
        if (UserKillNpcs.TryGetValue(Name, out var killnpcs) && killnpcs != null)
        {
            if (killnpcs.TryGetValue(NPCid, out var num))
            {
                return num;
            }
        }
        return 0;
    }

    public bool Update(string Name, int NPCID, int Count)
    {
        var reader = this.database.Query("UPDATE `TaskKillNPC` SET `Count` = @0 WHERE `Name` = @1 AND `NPCID` = @2", Count, Name, NPCID);
        return reader == 1;
    }
    public bool Write(string Name, int NPCID, int Count)
    {
        var reader = this.database.Query("INSERT INTO `TaskKillNPC` (`Name`, `NPCID`,`Count`) VALUES (@0, @1, @2)", Name, NPCID, Count);
        return reader == 1;
    }

    public void Remove(string Name, int NPCID)
    {
        this.database.Query("DELETE FROM `TaskKillNPC` WHERE `Name` = @0 AND `NPCID` = @1", Name, NPCID);
    }

    public void RemoveAll()
    {
        if (this.database.GetSqlType() == SqlType.Sqlite)
        {
            this.database.Query("delete from TaskKillNPC");
        }
        else
        {
            this.database.Query("TRUNCATE Table TaskKillNPC");
        }
        UserKillNpcs.Clear();
    }

    public void RemoveNpcKill(string Name)
    {
        if (UserKillNpcs.TryGetValue(Name, out var data) && data != null)
        {
            foreach (var info in data)
            {
                this.Remove(Name, info.Key);
            }
        }
        UserKillNpcs.Remove(Name);
    }

}