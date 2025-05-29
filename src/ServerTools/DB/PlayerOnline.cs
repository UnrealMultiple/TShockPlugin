using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using System.Data;

namespace ServerTools.DB;

[Table("OnlineDuration")]
public class PlayerOnline : RecordBase<PlayerOnline>
{
    [PrimaryKey]
    [NotNull]
    [Column("username")]
    public string Name { get; set; } = string.Empty;

    [Column("duration")]
    public int Duration { get; set; }


    private static Context? _context;

    public static Context Instance => _context ??= Db.Context<PlayerOnline>();

    public static PlayerOnline? GetPlayerOnline(string name)
    {
        return Instance.Records.FirstOrDefault(x => x.Name == name);
    }

    public static List<PlayerOnline> GetOnlineRank()
    {
        return Instance.Records.OrderByDescending(x => x.Duration).ToList();
    }

    public static bool Add(string Name, int duration)
    {
        var online = GetPlayerOnline(Name);
        if (online == null)
        {
            online = new PlayerOnline { Name = Name, Duration = duration };
            Instance.Insert(online);
            return true;
        }
        else
        {
            online.Duration += duration;
            Instance.Update(online);
            return false;
        }
    }

}
