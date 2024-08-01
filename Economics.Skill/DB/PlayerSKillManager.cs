using System.Data;
using Economics.Skill.Model;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Skill.DB;

public class PlayerSKillManager
{
    public class PlayerSkill
    {
        public int ID { get; }

        public string Name { get; }

        public int BindItem { get; }

        public TSPlayer? Player => EconomicsAPI.Economics.ServerPlayers.Find(x => x.Name == Name);

        public SkillContext? Skill => Economics.Skill.Skill.Config.GetSkill(ID);

        public int SkillCD = 0;

        public void ResetCD()
        {
            SkillCD = Skill == null ? 100 : Skill.SkillSpark.CD;
        }

        public PlayerSkill(int id, string name, int binditem)
        {
            ID = id;
            Name = name;
            BindItem = binditem;
        }
    }

    private readonly IDbConnection database;

    internal List<PlayerSkill> PlayerSkills = new();
    public PlayerSKillManager()
    {
        database = TShock.DB;
        var Skeleton = new SqlTable("Skill",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8 },
            new SqlColumn("Name", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("BindItem", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        ReadAll();
    }

    private void ReadAll()
    {
        using var reader = database.QueryReader("SELECT * FROM Skill");
        while (reader.Read())
        {
            string username = reader.Get<string>("Name");
            int BindItem = reader.Get<int>("BindItem");
            int Index = reader.Get<int>("ID");
            PlayerSkills.Add(new(Index, username, BindItem));
        }
    }

    public List<PlayerSkill> QuerySkill(string Name)
    {
        return PlayerSkills.FindAll(skill => skill.Name == Name);
    }

    public List<PlayerSkill> QuerySkillByItem(string Name, int itemId)
    {
        return PlayerSkills.FindAll(skill => skill.Name == Name && skill.BindItem == itemId);
    }

    public bool HasSkill(string Name, int index)
    {
        return PlayerSkills.Any(x => x.Name == Name && x.ID == index);
    }

    public bool HasSkill(string Name, int bindItem, int index)
    {
        return PlayerSkills.Any(x => x.Name == Name && x.ID == index && x.BindItem == bindItem);
    }

    public bool HasSkill(int index)
    {
        return PlayerSkills.Any(x => x.ID == index);
    }

    public void Add(string Name, int bindItem, int index)
    {
        database.Query("INSERT INTO `Skill` (`Name`, `BindItem`, `ID`) VALUES (@0, @1, @2)", Name, bindItem, index);
        PlayerSkills.Add(new(index, Name, bindItem));
    }

    public void Remove(string Name, int index)
    {
        database.Query("DELETE FROM Skill WHERE Name = @0 and ID = @1", Name, index);
        PlayerSkills.RemoveAll(x => x.Name == Name && x.ID == index);
    }

    public void ClearTable()
    {
        if (database.GetSqlType() == SqlType.Sqlite)
        {
            database.Query("delete from Skill");
        }
        else
        {
            database.Query("TRUNCATE Table Skill");
        }
        PlayerSkills.Clear();
    }
}
