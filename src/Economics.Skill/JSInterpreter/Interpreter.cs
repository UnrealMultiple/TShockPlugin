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
    public static readonly string ScriptsDir = Path.Combine(Core.Economics.SaveDirPath, "SkillScripts");
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
    public static void LoadFunction(Engine engine)
    {
        foreach (var method in typeof(JSFunctions).GetMethods())
        {
            var func = method.GetCustomAttribute<JavaScriptFunction>();
            if (method.IsStatic && func != null)
            {
                var tyep = Expression.GetDelegateType(method.GetParameters().Select(x => x.ParameterType).Append(method.ReturnType).ToArray());
                engine.SetValue(func.Name, method.CreateDelegate(tyep, null));
            }
        }
    }

    public static void ExecuteScript(SkillContext skill, TSPlayer player, Vector2 pos, Vector2 vel, int index = -1)
    {
        if (skill.JsScript == null || string.IsNullOrEmpty(skill.JsScript.Script))
        {
            return;
        }
        using var engine = new Engine((o) =>
        {
            o.AllowClr(typeof(Core.Economics).Assembly,
                typeof(TShock).Assembly,
                typeof(Task).Assembly,
                typeof(List<>).Assembly,
                typeof(Main).Assembly);
            o.AddExtensionMethods(typeof(Core.Extensions.Vector2Extension),
                typeof(Core.Extensions.GameProgress),
                typeof(Terraria.Utils),
                typeof(Core.Extensions.PlayerExtension),
                typeof(Core.Extensions.NpcExtension),
                typeof(Enumerable),
                typeof(Core.Extensions.TSPlayerExtension));
        });
        LoadFunction(engine);
        try
        { 
            engine.Evaluate(skill.JsScript.Script);
            engine.Invoke("main", skill, player, pos, vel, index);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(skill.JsScript.FilePathOrUri + "执行错误：" + ex);
        }
    }
}