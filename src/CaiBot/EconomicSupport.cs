using Economics.RPG;
using Economics.RPG.Model;
using EconomicsAPI.Configured;
using EconomicsAPI.DB;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using TerrariaApi.Server;

namespace CaiBot;

public static class EconomicSupport
{
    public static bool GetCoinsSupport;
    public static bool GetLevelNameSupport;
    public static bool GetSkillSupport;
    private static Func<List<CustomizeCurrency>> _getCurrencyOptionFunc = null!;
    private static Func<string, string, CurrencyManager.PlayerCurrency> _getUserCurrencyFunc = null!;
    private static Func<string> _OldgetCurrencyNameFunc = null!;
    private static Func<string, long> _OldgetUserCurrencyFunc = null!;
    private static Func<string, string> _getLevelNameFunc = null!;
    private static Func<string, dynamic> _querySkillFunc = null!;

    public static void Init()
    {
        var pluginContainer = ServerApi.Plugins.Where(x => x.Plugin.Name == "EconomicsAPI").FirstOrDefault();
        if (pluginContainer is not null)
        {
            do
            {
                var economicsType = pluginContainer.Plugin.GetType();

                var settingProperty = economicsType.GetProperty(nameof(EconomicsAPI.Economics.Setting));
                if (settingProperty is null)
                {
                    break;
                }

                if (pluginContainer.Plugin.Version > new Version(1, 0 ,2, 0))
                {
                    var currencyNameField = settingProperty.PropertyType.GetField(nameof(EconomicsAPI.Economics.Setting.CustomizeCurrencys));
                    if (currencyNameField is null)
                    {
                        break;
                    }

                    var func = new DynamicMethod("GetCurrencyOption", typeof(List<CustomizeCurrency>), Type.EmptyTypes);
                    var iL = func.GetILGenerator();
                    iL.Emit(OpCodes.Call, settingProperty.GetMethod!);
                    iL.Emit(OpCodes.Ldfld, currencyNameField);
                    iL.Emit(OpCodes.Ret);
                    _getCurrencyOptionFunc = func.CreateDelegate<Func<List<CustomizeCurrency>>>();
                    var currencyManagerProperty = economicsType.GetProperty(nameof(EconomicsAPI.Economics.CurrencyManager));
                    if (currencyManagerProperty is null)
                    {
                        break;
                    }

                    var paramTypes = new[] { typeof(string), typeof(string) };
                    var getUserCurrencyMethod = currencyManagerProperty.PropertyType.GetMethod(nameof(EconomicsAPI.Economics.CurrencyManager.GetUserCurrency), paramTypes);
                    if (getUserCurrencyMethod is null)
                    {
                        break;
                    }

                    func = new DynamicMethod("GetUserCurrency", typeof(CurrencyManager.PlayerCurrency), paramTypes);
                    iL = func.GetILGenerator();
                    iL.Emit(OpCodes.Call, currencyManagerProperty.GetMethod!);
                    iL.Emit(OpCodes.Ldarg_0);
                    iL.Emit(OpCodes.Ldarg_1);
                    iL.Emit(OpCodes.Callvirt, getUserCurrencyMethod);
                    iL.Emit(OpCodes.Ret);
                    _getUserCurrencyFunc = func.CreateDelegate<Func<string, string, CurrencyManager.PlayerCurrency>>();
                }
                else
                {
                    var currencyNameField = settingProperty.PropertyType.GetField("CurrencyName");
                    if (currencyNameField is null)
                    {
                        break;
                    }

                    var func = new DynamicMethod("GetCurrencyName", typeof(string), Type.EmptyTypes);
                    var iL = func.GetILGenerator();
                    iL.Emit(OpCodes.Call, settingProperty.GetMethod!);
                    iL.Emit(OpCodes.Ldfld, currencyNameField);
                    iL.Emit(OpCodes.Ret);
                    _OldgetCurrencyNameFunc = func.CreateDelegate<Func<string>>();
                    var currencyManagerProperty = economicsType.GetProperty(nameof(EconomicsAPI.Economics.CurrencyManager));
                    if (currencyManagerProperty is null)
                    {
                        break;
                    }

                    var paramTypes = new[] { typeof(string) };
                    var getUserCurrencyMethod = currencyManagerProperty.PropertyType.GetMethod(nameof(EconomicsAPI.Economics.CurrencyManager.GetUserCurrency), paramTypes);
                    if (getUserCurrencyMethod is null)
                    {
                        break;
                    }

                    func = new DynamicMethod("GetUserCurrency", typeof(long), paramTypes);
                    iL = func.GetILGenerator();
                    iL.Emit(OpCodes.Call, currencyManagerProperty.GetMethod!);
                    iL.Emit(OpCodes.Ldarg_0);
                    iL.Emit(OpCodes.Callvirt, getUserCurrencyMethod);
                    iL.Emit(OpCodes.Ret);
                    _OldgetUserCurrencyFunc = func.CreateDelegate<Func<string, long>>();
                }

                GetCoinsSupport = true;
            } while (false);
        }

        pluginContainer = ServerApi.Plugins.Where(x => x.Plugin.Name == "Economics.RPG").FirstOrDefault();
        if (pluginContainer is not null)
        {
            do
            {
                var economicsRPGType = pluginContainer.Plugin.GetType();

                var playerLevelManagerProperty = economicsRPGType.GetProperty(nameof(RPG.PlayerLevelManager));
                if (playerLevelManagerProperty is null)
                {
                    break;
                }

                var paramTypes = new[] { typeof(string) };
                var getLevelMethod = playerLevelManagerProperty.PropertyType.GetMethod(nameof(RPG.PlayerLevelManager.GetLevel), paramTypes);
                if (getLevelMethod is null)
                {
                    break;
                }

                var nameProperty = getLevelMethod.ReturnType.GetProperty(nameof(Level.Name));
                if (nameProperty is null)
                {
                    break;
                }

                var func = new DynamicMethod("GetLevelName", typeof(string), paramTypes);
                var iL = func.GetILGenerator();
                iL.Emit(OpCodes.Call, playerLevelManagerProperty.GetMethod!);
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Callvirt, getLevelMethod);
                iL.Emit(OpCodes.Callvirt, nameProperty.GetMethod!);
                iL.Emit(OpCodes.Ret);
                _getLevelNameFunc = func.CreateDelegate<Func<string, string>>();

                GetLevelNameSupport = true;
            } while (false);
        }

        pluginContainer = ServerApi.Plugins.Where(x => x.Plugin.Name == "Economics.Skill").FirstOrDefault();
        if (pluginContainer is not null)
        {
            do
            {
                var economicsSkillType = pluginContainer.Plugin.GetType();
                var playerSkillManagerProperty = economicsSkillType.GetProperty("PlayerSKillManager", BindingFlags.NonPublic | BindingFlags.Static);
                if (playerSkillManagerProperty is null)
                {
                    break;
                }

                var paramTypes = new[] { typeof(string) };
                var querySkillMethod = playerSkillManagerProperty.PropertyType.GetMethod("QuerySkill", paramTypes);
                if (querySkillMethod is null)
                {
                    break;
                }

                var func = new DynamicMethod("QuerySkill", typeof(object), paramTypes);
                var iL = func.GetILGenerator();
                iL.Emit(OpCodes.Call, playerSkillManagerProperty.GetMethod!);
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Callvirt, querySkillMethod);
                iL.Emit(OpCodes.Ret);
                _querySkillFunc = func.CreateDelegate<Func<string, dynamic>>();

                GetSkillSupport = true;
            } while (false);
        }
    }

    public static bool IsSupported(string feature)
    {
        return feature switch
        {
            nameof(GetCoins) => GetCoinsSupport,
            nameof(GetLevelName) => GetLevelNameSupport,
            nameof(GetSkill) => GetSkillSupport,
            _ => false
        };
    }

    public static string GetCoins(string name)
    {
        ThrowIfNotSupported();
        if (_OldgetUserCurrencyFunc != null)
        {
            return $"{_OldgetCurrencyNameFunc()}x{_OldgetUserCurrencyFunc(_OldgetCurrencyNameFunc())}";
        }

        var option = _getCurrencyOptionFunc();
        var sb = new StringBuilder();
        foreach (var pair in option)
        {
            var current = _getUserCurrencyFunc(name, pair.Name);
            sb.Append($"{current.CurrencyType}x{current.Number}");
        }

        return sb.ToString().Trim();
    }

    public static string GetLevelName(string name)
    {
        ThrowIfNotSupported();
        var levelName = _getLevelNameFunc(name);
        return $"职业:{(string.IsNullOrEmpty(levelName) ? "无" : levelName)}";
    }

    public static string GetSkill(string name)
    {
        ThrowIfNotSupported();
        var obj = _querySkillFunc(name);
        IEnumerable<dynamic> skill = Enumerable.Cast<dynamic>(obj);
        var msg = skill.Any() ? string.Join(',', skill.Select(obj => obj.Skill is null ? "无效技能" : (string) obj.Skill.Name)) : "无";
        return $"技能:{msg}";
    }

    private static void ThrowIfNotSupported([CallerMemberName] string memberName = "")
    {
        if (!IsSupported(memberName))
        {
            throw new NotSupportedException(memberName);
        }
    }
}