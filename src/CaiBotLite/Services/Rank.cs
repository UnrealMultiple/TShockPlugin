using CaiBotLite.Enums;
using CaiBotLite.Moulds;
using LinqToDB;
using LinqToDB.Data;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;

namespace CaiBotLite.Services;

public class Rank(string title, Dictionary<string, string> rankLines)
{
    [JsonProperty("title")]
    public string Title = title;
    
    [JsonProperty("rank_lines")]
    public Dictionary<string, string> RankLines = rankLines;
    public static List<NPC> GetBossByIdOrName(string idOrName)
    {
        return TShock.Utils.GetNPCByIdOrName(idOrName).Where(x => x.boss).ToList();
    }
    
    [JsonIgnore]
    internal static readonly Dictionary<string, RankTypes> RankTypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["boss"] = RankTypes.Boss,
        ["死亡"] = RankTypes.Death,
        ["在线"] = RankTypes.Online,
        ["钓鱼"] = RankTypes.Fishing
    };
    
    [JsonIgnore]
    public static List<string> SupportRankTypes => RankTypeMappings.Keys.ToList();

    
    public static RankTypes GetRankTypeByName(string name)
    {
        return RankTypeMappings.GetValueOrDefault(name, RankTypes.Unknown);
    }
    
    public static Rank GetFishingRank()
    {
        using var db = Database.Db;
        var result = db.Query<dynamic>("SELECT Username,questsCompleted FROM tsCharacter JOIN Users ON tsCharacter.Account == Users.ID");
        return new Rank("渔夫任务排行", result.OrderByDescending(x => (int)x.questsCompleted)
            .ToDictionary(x => (string)x.Username, x=> (int)x.questsCompleted + "次"));
    }
    

    public static Rank GetDeathRank()
    {
        using var db = Database.Db;
        return new Rank("死亡排行", db.GetTable<CaiCharacterInfo>()
            .OrderByDescending(x => x.Death)
            .ToDictionary(x => x.AccountName, x=> x.Death + "次"));
    }
    
    public static Rank GetOnlineRank()
    {
        using var db = Database.Db;
        return new Rank("在线排行", db.GetTable<CaiCharacterInfo>()
            .OrderByDescending(x => x.OnlineMinute)
            .ToDictionary(x => x.AccountName, x=> x.OnlineMinute + "分钟"));
    } 
    
    public static Rank GetBossRank(NPC npc)
    {
        using var db = Database.Db;
        return new Rank($"{npc.TypeName}の击杀排行",db.GetTable<BossKillInfo>()
            .Where(x => x.BossId == npc.type)
            .OrderByDescending(x => x.KillCounts)
            .ToDictionary(x => x.AccountName, x => x.KillCounts + "次"));
    }
    
}