using Economics.RPG;
using Economics.RPG.Model;
using Economics.Skill.DB;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TerrariaApi.Server;

namespace CaiBot;

public static class EconomicSupport
{
    private static bool _isOldCoinsSupport;

    private static Func<object> _getPlayerSKillManagerFunc = null!;

    private static Func<string> _getCurrencyNameFunc = null!;
    private static Func<string, long> _getUserCurrencyFunc = null!;
    public static bool GetCoinsSupport { get; private set; }
    public static bool GetLevelNameSupport { get; private set; }
    public static bool GetSkillSupport { get; private set; }

    public static void Init()
    {
        var pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "EconomicsAPI");
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

                if (pluginContainer.Plugin.Version < new Version(2, 0, 0, 3))
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
                    _getCurrencyNameFunc = func.CreateDelegate<Func<string>>();
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
                    _getUserCurrencyFunc = func.CreateDelegate<Func<string, long>>();

                    _isOldCoinsSupport = true;
                    GetCoinsSupport = true;
                }
                else
                {
                    GetCoinsSupport = true;
                }
            } while (false);
        }

        pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "Economics.RPG");
        if (pluginContainer is not null)
        {
            do
            {
                var economicsRpgType = pluginContainer.Plugin.GetType();

                var playerLevelManagerProperty = economicsRpgType.GetProperty(nameof(RPG.PlayerLevelManager));
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

                GetLevelNameSupport = true;
            } while (false);
        }

        pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "Economics.Skill");
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

                var func = new DynamicMethod(nameof(_getPlayerSKillManagerFunc), typeof(object), Type.EmptyTypes);
                var iL = func.GetILGenerator();
                iL.Emit(OpCodes.Call, playerSkillManagerProperty.GetMethod!);
                iL.Emit(OpCodes.Ret);
                _getPlayerSKillManagerFunc = func.CreateDelegate<Func<object>>();

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
        if (_isOldCoinsSupport)
        {
            return $"{_getCurrencyNameFunc()}:{_getUserCurrencyFunc(name)}";
        }

        // 新方法是必要的，防止解析报错
        return GetNewCoins(name);
    }

    private static string GetNewCoins(string name)
    {
        return string.Join('\n', EconomicsAPI.Economics.Setting.CustomizeCurrencys.Select(x => EconomicsAPI.Economics.CurrencyManager.GetUserCurrency(name, x.Name)).Select(static x => $"{x.CurrencyType}x{x.Number}"));
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
        var manager = (PlayerSKillManager) _getPlayerSKillManagerFunc.Invoke();
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