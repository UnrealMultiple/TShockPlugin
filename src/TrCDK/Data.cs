using Microsoft.Data.Sqlite;
using LinqToDB.Mapping;
using LazyAPI.Database;
using LinqToDB;

namespace TrCDK;

[Table("CDK")]
public class CDK : RecordBase<CDK>
{
    [Column("name"), PrimaryKey, NotNull]
    public string Name { get; set; } = string.Empty;

    [Column("usetime")]
    public int Usetime { get; set; }

    [Column("utiltime")]
    public long Utiltime { get; set; }

    [Column("grouplimit")]
    public string Grouplimit { get; set; } = string.Empty;

    [Column("playerlimit")]
    public string Playerlimit { get; set; } = string.Empty;

    [Column("used")]
    public string Used { get; set; } = string.Empty;

    [Column("cmds")]
    public string Cmds { get; set; } = string.Empty;

    private static readonly Context _context = Db.Context<CDK>("CDK");

    public static bool Insert(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Cmds)
    {
        _context.Records.Append(new CDK
        {
            Name = CDKname,
            Usetime = Usetime,
            Utiltime = Utiltime,
            Grouplimit = Grouplimit,
            Playerlimit = Playerlimit,
            Used = "",
            Cmds = Cmds
        });
        return true;
    }

    public static void Update(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Used, string Cmds)
    {
        var cdk = _context.Records.FirstOrDefault(c => c.Name == CDKname);
        if (cdk != null)
        {
            cdk.Usetime = Usetime;
            cdk.Utiltime = Utiltime;
            cdk.Grouplimit = Grouplimit;
            cdk.Playerlimit = Playerlimit;
            cdk.Used = Used;
            cdk.Cmds = Cmds;
            _context.Update(cdk);
        }
    }

    public static CDK? GetData(string name)
    {
        return _context.Records.FirstOrDefault(c => c.Name == name);
    }

    public static CDK[] GetAllData()
    {
        return [.. _context.Records];
    }

    public static bool DelCDK(string name)
    {
        var cdk = _context.Records.FirstOrDefault(c => c.Name == name);
        if (cdk != null)
        {
            _context.Delete(cdk);
            return true;
        }
        return false;
    }
}