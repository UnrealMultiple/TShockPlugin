using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Task.DB;

public class TaskTallkManager
{
    private readonly IDbConnection database;

    private static readonly Dictionary<string, HashSet<int>> UserTallks = new();
    public TaskTallkManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("TaskTallk",
            new SqlColumn("NPCID", MySqlDbType.Int32),
            new SqlColumn("Name", MySqlDbType.Text) { Length = 500 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.GetPlayerTallk();
    }

    private void GetPlayerTallk()
    {
        UserTallks.Clear();
        using var read = this.database.QueryReader("select * from `TaskTallk`");
        while (read.Read())
        {
            var NPCID = read.Get<int>("NPCID");
            var Name = read.Get<string>("Name");
            if (!UserTallks.TryGetValue(Name, out var data) || data == null)
            {
                UserTallks[Name] = new();
            }

            UserTallks[Name].Add(NPCID);
        }
    }

    public void AddTallkNPC(string Name, int NPCID)
    {
        if (UserTallks.TryGetValue(Name, out var npcs))
        {
            if (npcs == null)
            {
                UserTallks[Name] = new();
            }

            if (!UserTallks[Name].Contains(NPCID))
            {
                this.Write(Name, NPCID);
                UserTallks[Name].Add(NPCID);
            }
        }
        else
        {
            this.Write(Name, NPCID);
            UserTallks[Name] = new() { NPCID };
        }
    }

    public void RemoveTallkNPC(string Name)
    {
        if (UserTallks.TryGetValue(Name, out var data) && data != null)
        {
            foreach (var info in data)
            {
                this.Remove(Name, info);
            }
        }
        UserTallks.Remove(Name);
    }

    public bool TallkNpcByID(string Name, int npcid)
    {
        return UserTallks.TryGetValue(Name, out var npcs) && npcs != null ? npcs.Contains(npcid) : false;
    }

    public void RemoveAll()
    {
        if (this.database.GetSqlType() == SqlType.Sqlite)
        {
            this.database.Query("delete from TaskTallk");
        }
        else
        {
            this.database.Query("TRUNCATE Table TaskTallk");
        }
        UserTallks.Clear();
    }

    public bool Write(string Name, int NPCID)
    {
        var reader = this.database.Query("INSERT INTO `TaskTallk` (`Name`, `NPCID`) VALUES (@0, @1)", Name, NPCID);
        return reader == 1;
    }

    public void Remove(string Name, int NPCID)
    {
        this.database.Query("DELETE FROM `TaskTallk` WHERE `Name` = @0 AND `NPCID` = @1", Name, NPCID);
    }
}