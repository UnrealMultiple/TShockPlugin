using Economics.Skill.Model;
using Economics.Skill.Setting;
using MySql.Data.MySqlClient;
using System.Data;
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

        public TSPlayer? Player => Core.Economics.ServerPlayers.Find(x => x.Name == this.Name);

        public SkillContext? Skill => Config.Instance.GetSkill(this.ID);

        public int SkillCD = 0;

        public void ResetCD()
        {
            this.SkillCD = this.Skill == null ? 100 : this.Skill.SkillSpark.CD;
        }

        public PlayerSkill(int id, string name, int binditem)
        {
            this.ID = id;
            this.Name = name;
            this.BindItem = binditem;
        }
    }

    private readonly IDbConnection database;

    internal List<PlayerSkill> PlayerSkills = new();
    public PlayerSKillManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("Skill",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8 },
            new SqlColumn("Name", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("BindItem", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.ReadAll();
    }

    private void ReadAll()
    {
        using var reader = this.database.QueryReader("SELECT * FROM Skill");
        while (reader.Read())
        {
            var username = reader.Get<string>("Name");
            var BindItem = reader.Get<int>("BindItem");
            var Index = reader.Get<int>("ID");
            this.PlayerSkills.Add(new(Index, username, BindItem));
        }
    }

    public List<PlayerSkill> QuerySkill(string Name)
    {
        return this.PlayerSkills.FindAll(skill => skill.Name == Name);
    }

    public List<PlayerSkill> QuerySkillByItem(string Name, int itemId)
    {
        return this.PlayerSkills.FindAll(skill => skill.Name == Name && skill.BindItem == itemId);
    }

    public bool HasSkill(string Name, int index)
    {
        return this.PlayerSkills.Any(x => x.Name == Name && x.ID == index);
    }

    public bool HasSkill(string Name, int bindItem, int index)
    {
        return this.PlayerSkills.Any(x => x.Name == Name && x.ID == index && x.BindItem == bindItem);
    }

    public bool HasSkill(int index)
    {
        return this.PlayerSkills.Any(x => x.ID == index);
    }

    public void Add(string Name, int bindItem, int index)
    {
        this.database.Query("INSERT INTO `Skill` (`Name`, `BindItem`, `ID`) VALUES (@0, @1, @2)", Name, bindItem, index);
        this.PlayerSkills.Add(new(index, Name, bindItem));
    }

    public void Remove(string Name, int index)
    {
        this.database.Query("DELETE FROM Skill WHERE Name = @0 and ID = @1", Name, index);
        this.PlayerSkills.RemoveAll(x => x.Name == Name && x.ID == index);
    }

    public void ClearTable()
    {
        if (this.database.GetSqlType() == SqlType.Sqlite)
        {
            this.database.Query("delete from Skill");
        }
        else
        {
            this.database.Query("TRUNCATE Table Skill");
        }
        this.PlayerSkills.Clear();
    }
}