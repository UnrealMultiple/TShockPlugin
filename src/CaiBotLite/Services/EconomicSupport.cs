using CaiBotLite.Enums;
using CaiBotLite.Moulds;
using Economics.RPG;
using Economics.RPG.Model;
using Economics.Skill.DB;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TerrariaApi.Server;
using Economics;
using Economics.Skill;

namespace CaiBotLite.Services;

public static class EconomicSupport
{
    public static bool GetCoinsSupport { get; private set; }
    public static bool GetLevelNameSupport { get; private set; }
    public static bool GetSkillSupport { get; private set; }

    public static void Init()
    {
        var pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "Economics.Core");
        if (pluginContainer is not null)
        {
             GetCoinsSupport = true;
        }

        pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "Economics.RPG");
        if (pluginContainer is not null)
        {
           GetLevelNameSupport = true;
        }

        pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "Economics.Skill");
        if (pluginContainer is not null)
        {
           GetSkillSupport = true;
        }

        if (GetCoinsSupport)
        {
            Rank.RankTypeMappings.Add("货币", RankTypes.EconomicCoin);
        }
        
    }

    public static bool IsSupported(string feature)
    {
        return feature switch
        {
            nameof(GetCoins) => GetCoinsSupport,
            nameof(GetLevelName) => GetLevelNameSupport,
            nameof(GetSkill) => GetSkillSupport,
            nameof(GetCoinRank) => GetCoinsSupport,
            nameof(SupportCoins) => GetCoinsSupport,
            _ => false
        };
    }

    public static string GetCoins(string name)
    {
        ThrowIfNotSupported();
        return GetNewCoins(name);
    }

    public static List<string> SupportCoins
    {
        get
        {
            ThrowIfNotSupported();
            return Economics.Core.ConfigFiles.Setting.Instance.CustomizeCurrencys
                .Select(x => x.Name)
                .ToList();
        }
    }

    public static Rank GetCoinRank(string type)
    {
        ThrowIfNotSupported();
        return new Rank($"{type}排行" ,
            Economics.Core.Economics.CurrencyManager.GetCurrencies()
            .Where(c => c.CurrencyType == type)
            .OrderByDescending(c => c.Number)
            .ToDictionary(x => x.PlayerName, x => x.Number + x.CurrencyType));
    }

    private static string GetNewCoins(string name)
    {
        return string.Join('\n', Economics.Core.ConfigFiles.Setting.Instance.CustomizeCurrencys.Select(x => Economics.Core.Economics.CurrencyManager.GetUserCurrency(name, x.Name)).Select(static x => $"{x.CurrencyType}x{x.Number}"));
    }

    public static string GetLevelName(string name)
    {
        ThrowIfNotSupported();
        var levelName = RPG.PlayerLevelManager.GetLevel(name).Name;
        return $"职业:{(string.IsNullOrEmpty(levelName) ? "无" : levelName)}";
    }

    public static string GetSkill(string name)
    {
        ThrowIfNotSupported();
        var manager = Skill.PlayerSKillManager;
        var skills = manager.QuerySkill(name);
        return !skills.Any() ? "技能:无" : string.Join(',', skills.Select(obj => obj.Skill is null ? "无效技能" : obj.Skill.Name));
    }

    private static void ThrowIfNotSupported([CallerMemberName] string memberName = "")
    {
        if (!IsSupported(memberName))
        {
            throw new NotSupportedException(memberName);
        }
    }
}