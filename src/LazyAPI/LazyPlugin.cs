using LazyAPI.Commands;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace LazyAPI;

public abstract class LazyPlugin : TerrariaPlugin
{
    public override string Name => this.GetType().Namespace;

    protected LazyPlugin(Main game) : base(game)
    {
        this.AutoLoad();
        if (this.restToLoad.Count > 0)
            ServerApi.Hooks.GameInitialize.Register(this, args =>
            {
                foreach (var type in this.restToLoad)
                foreach (var name in type.GetCustomAttributes<RestAttribute>(false).SelectMany(attr => attr.alias))
                {
                    RestHelper.Register(type, name, this);
                }

                this.restToLoad.Clear();
            });
    }

    public override void Initialize()
    {
    }

    private readonly List<Type> restToLoad = new ();
    
    internal void AutoLoad()
    {
        foreach (var type in this.GetType().Assembly.GetTypes())
        {
            if (type.IsDefined(typeof(ConfigAttribute), false))
            {
                var name = type.BaseType.GetMethod("Load").Invoke(null, new object[0]); ;
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