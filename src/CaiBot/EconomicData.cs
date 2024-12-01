using System.Text.Json.Serialization;

namespace CaiBot;

public class EconomicData
{
    [JsonPropertyOrder(1)] public string Coins = "";

    [JsonPropertyOrder(2)] public string LevelName = "";

    [JsonPropertyOrder(3)] public string Skill = "";

    public static EconomicData GetEconomicData(string name)
    {
        EconomicData economicData = new ();
        if (EconomicSupport.GetCoinsSupport)
        {
            economicData.Coins = EconomicSupport.GetCoins(name);
        }

        if (EconomicSupport.GetLevelNameSupport)
        {
            economicData.LevelName = EconomicSupport.GetLevelName(name);
        }

        if (EconomicSupport.GetSkillSupport)
        {
            economicData.Skill = EconomicSupport.GetSkill(name);
        }

        return economicData;
    }
}