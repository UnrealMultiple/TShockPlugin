using LinqToDB;
using LinqToDB.Mapping;
using Newtonsoft.Json;

namespace CaiBotLite.Moulds;

[Table("cai_statistic")]
public class CaiCharacterInfo
{
    [PrimaryKey] [Column("account_name")] public string AccountName = null!;

    [Column("online_minute")] public int OnlineMinute; // 在线时间

    [Column("death")] public int Death; // 死亡次数

    [NotColumn] private DateTime _lastUpdate = DateTime.Now;

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
        Console.WriteLine(JsonConvert.SerializeObject(characterInfo));
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
            var affected = db.Update(bossKill);
            if (affected == 0)
            {
                db.Insert(bossKill);
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