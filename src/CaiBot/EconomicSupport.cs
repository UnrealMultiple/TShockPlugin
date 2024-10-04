using TShockAPI;

namespace CaiBot;

public static class EconomicSupport
{
    public static string GetCoins(TSPlayer player)
    {
        return $"拥有{EconomicsAPI.Economics.Setting.CurrencyName}:{EconomicsAPI.Economics.CurrencyManager.GetUserCurrency(player.Name)}";
    }
    
    public static string GetLevelName(TSPlayer player)
    {
        return $"当前职业: {Economics.RPG.RPG.PlayerLevelManager.GetLevel(player.Name)}";
    }
    
    public static string GetSkill(TSPlayer player)
    {
        var skill = Economics.Skill.Skill.PlayerSKillManager.QuerySkill(player.Name);
        var msg = skill.Any() ? string.Join(",", skill.Select(x => x.Skill == null ? "无效技能" : x.Skill.Name)) : "无";
        return $"绑定技能:{msg}";
    }
}