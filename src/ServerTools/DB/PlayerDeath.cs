using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using System.Data;

namespace ServerTools.DB;

[Table("Death")]
public class PlayerDeath : RecordBase<PlayerDeath>
{
    [PrimaryKey]
    [NotNull]
    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [Column("Count")]
    public int Count { get; set; }


    private static Context? _context;

    public static Context Instance => _context ??= Db.Context<PlayerDeath>();

    public static PlayerDeath? GetPlayerDeath(string name)
    {
        return Instance.Records.FirstOrDefault(x => x.Name == name);
    }

    public static List<PlayerDeath> GetDeathRank()
    {
        return [.. Instance.Records.OrderByDescending(x => x.Count)];
    }

    public static void Add(string name)
    {
        var record = GetPlayerDeath(name);
        if (record == null)
        {
            record = new PlayerDeath { Name = name, Count = 1 };
            Instance.Insert(record);
        }
        else
        {
            record.Count++;
            Instance.Update(record);
        }
    }
}