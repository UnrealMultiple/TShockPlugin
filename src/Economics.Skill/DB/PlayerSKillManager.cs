using Economics.Skill.Model;
using Economics.Skill.Setting;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Skill.DB;

public class PlayerSKillManager
{
    public class PlayerSkill(int id, string name, int binditem, int Level = 1)
    {
        public int ID { get; } = id;

        public string Name { get; } = name;

        public int BindItem { get; } = binditem;

        public int Level { get; set; } = Level;

        public TSPlayer? Player => Core.Economics.ServerPlayers.Find(x => x.Name == this.Name);

        public SkillContext? Skill => Config.Instance.GetSkill(this.ID);

        public int SkillCD = 0;

        public void ResetCD()
        {
            this.SkillCD = this.Skill == null ? 100 : this.Skill.SkillSpark.CD;
        }
    }

    private readonly IDbConnection database;

    internal List<PlayerSkill> PlayerSkills = [];
    public PlayerSKillManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("Skill",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8 },
            new SqlColumn("Name", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("BindItem", MySqlDbType.Int32) { Length = 255 },
            new SqlColumn("Level", MySqlDbType.Int32) { Length = 8 }
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
            var Level = reader.Get<int>("Level");
            this.PlayerSkills.Add(new(Index, username, BindItem, Level));
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

    public void Add(string Name, int bindItem, int index, int Level)
    {
        this.database.Query("INSERT INTO `Skill` (`Name`, `BindItem`, `ID`, `Level`) VALUES (@0, @1, @2, @3)", Name, bindItem, index, Level);
        this.PlayerSkills.Add(new(index, Name, bindItem, Level));
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

    internal void UpdateLevel(PlayerSkill playerSkill)
    {
        playerSkill.Level++;
        this.database.Query("UPDATE Skill SET Level = @0 WHERE Name = @1 AND ID = @2", playerSkill.Level, playerSkill.Name, playerSkill.ID);
    }
}