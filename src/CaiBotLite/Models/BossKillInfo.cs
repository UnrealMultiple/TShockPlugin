using LinqToDB.Mapping;

namespace CaiBotLite.Models;

[Table("cai_boss_kill")]
public class BossKillInfo
{
    [PrimaryKey]
    [Identity]
    [Column("id")]
    public int Id;

    [Column("account_name")]
    public string AccountName = null!;

    [Column("boss_id")]
    public int BossId;

    [Column("kill_counts")]
    public int KillCounts;
}