using Economics.Skill.Attributes;
using Economics.Skill.Model;
using Jint;
using Microsoft.Xna.Framework;
using System.Linq.Expressions;
using System.Reflection;
using Terraria;
using TShockAPI;

namespace Economics.Skill.JSInterpreter;

public class Interpreter
{
    private static readonly Engine Engine = new Engine((o) =>
    {
        o.AllowClr(typeof(EconomicsAPI.Economics).Assembly,
            typeof(TShock).Assembly,
            typeof(Task).Assembly,
            typeof(List<>).Assembly,
            typeof(Main).Assembly);
        o.AddExtensionMethods(typeof(EconomicsAPI.Extensions.Vector2Ext),
            typeof(EconomicsAPI.Extensions.GameProgress),
            typeof(Terraria.Utils),
            typeof(EconomicsAPI.Extensions.PlayerExt),
            typeof(EconomicsAPI.Extensions.NpcExt),
            typeof(Enumerable),
            typeof(EconomicsAPI.Extensions.TSPlayerExt));
    });

    public static readonly string ScriptsDir = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "SkillScripts");
    static Interpreter()
    {
        if (!Directory.Exists(ScriptsDir))
        {
            Directory.CreateDirectory(ScriptsDir);
        }
    }

    /// <summary>
    /// 加载预定义JS函数
    /// </summary>
    public static void LoadFunction()
    {
        foreach (var method in typeof(JSFunctions).GetMethods())
        {
            var func = method.GetCustomAttribute<JavaScriptFunction>();
            if (method.IsStatic && func != null)
            {
                var tyep = Expression.GetDelegateType(method.GetParameters().Select(x => x.ParameterType).Append(method.ReturnType).ToArray());
                Engine.SetValue(func.Name, method.CreateDelegate(tyep, null));
            }
        }
    }

    public static void ExecuteScript(SkillContext skill, TSPlayer player, Vector2 pos, Vector2 vel, int index = -1)
    {
        if (skill.JsScript == null || string.IsNullOrEmpty(skill.JsScript.Script))
        {
            return;
        }
        try
        {
            Engine.Evaluate(skill.JsScript.Script);
            Engine.Invoke("main", skill, player, pos, vel, index);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(skill.JsScript.FilePathOrUri + "执行错误：" + ex);
        }
    }

}