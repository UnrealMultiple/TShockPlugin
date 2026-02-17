using LinqToDB;
using LinqToDB.Mapping;

namespace CaiBotLite.Models;

[Table("cai_statistic")]
public class CaiCharacterInfo
{
    [PrimaryKey]
    [NotNull]
    [Column("account_name")]
    public string AccountName = null!;

    [Column("online_minute")]
    public int OnlineMinute; // 在线时间

    [Column("death")]
    public int Death; // 死亡次数

    [NotColumn]
    private DateTime _lastUpdate = DateTime.Now;

    [NotColumn] public List<BossKillInfo> BossKills { get; private set; } = [];

    public static CaiCharacterInfo? GetByName(string name)
    {
        using var db = Database.Db;

        var characterInfo = db
            .GetTable<CaiCharacterInfo>()
            .FirstOrDefault(c => c.AccountName == name);


        if (characterInfo == null)
        {
            return null;
        }

        var bossInfo = db
            .GetTable<BossKillInfo>()
            .Where(c => c.AccountName == name)
            .ToList();

        characterInfo.BossKills = bossInfo;
        return characterInfo;
    }

    public void CreatOrUpdate()
    {
        using var db = Database.Db;
        this.OnlineMinute += (int) (DateTime.Now - this._lastUpdate).TotalMinutes;
        this._lastUpdate = DateTime.Now;

        db.InsertOrReplace(this);

        foreach (var bossKill in this.BossKills)
        {
            var result = db.GetTable<BossKillInfo>().FirstOrDefault(x =>
                x.AccountName == bossKill.AccountName && x.BossId == bossKill.BossId
            );

            if (result is null)
            {
                db.Insert(bossKill);
            }
            else
            {
                result.KillCounts = bossKill.KillCounts;
                db.Update(result);
            }
        }
    }

    public static void CleanAll()
    {
        using var db = Database.Db;
        db.GetTable<CaiCharacterInfo>().Delete();
        db.GetTable<BossKillInfo>().Delete();
    }
}