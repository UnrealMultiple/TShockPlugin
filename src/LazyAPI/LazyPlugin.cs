using LazyAPI.Attributes;
using LazyAPI.Commands;
using LazyAPI.Utility;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace LazyAPI;

public abstract class LazyPlugin : TerrariaPlugin
{
    public override string Name
    {
        get
        {
            var type = this.GetType();
            return type.Namespace ?? type.Name;
        }
    }

    protected LazyPlugin(Main game) : base(game)
    {
        this.AutoLoad();
        if (this.restToLoad.Count > 0)
        {
            ServerApi.Hooks.GameInitialize.Register(this, args =>
            {
                foreach (var (type, name) in this.restToLoad.SelectMany(type => type.GetCustomAttributes<RestAttribute>(false).SelectMany(attr => attr.alias).Select(name => (type, name))))
                {
                    RestHelper.Register(type, name, this);
                }

                this.restToLoad.Clear();
            });
        }
    }

    public override void Initialize()
    {
    }

    private readonly List<Type> restToLoad = new();

    internal void AutoLoad()
    {
        foreach (var type in this.GetType().Assembly.GetTypes())
        {
            if (type.IsDefined(typeof(ConfigAttribute), false))
            {
                var method = type.BaseType!.GetMethod("Load") ?? throw new MissingMethodException($"method 'Load()' is missing inside the lazy loaded config class '{this.Name}'");
                var name = method.Invoke(null, Array.Empty<object>());
                Console.WriteLine(GetString($"[{this.Name}] config registered: {name}"));
            }
            else if (type.IsDefined(typeof(CommandAttribute), false))
            {
                var names = CommandHelper.Register(type);
                Console.WriteLine(GetString($"[{this.Name}] command registered: {string.Join(",", names)}"));
            }
            else if (type.IsDefined(typeof(RestAttribute), false))
            {
                this.restToLoad.Add(type);
            }
        }
    }
}