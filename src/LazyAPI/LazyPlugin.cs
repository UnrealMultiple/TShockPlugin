using LazyAPI.Attributes;
using LazyAPI.Commands;
using LazyAPI.ConfigFiles;
using LazyAPI.Utility;
using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace LazyAPI;

public abstract class LazyPlugin : TerrariaPlugin
{
    private readonly List<GeneralHooks.ReloadEventD> addReloadEvents = new();
    protected readonly List<TShockAPI.Command> addCommands = new();
    protected readonly List<RestCommand> addRestCommands = new();
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
                    this.addRestCommands.AddRange(RestHelper.Register(type, name, this));
                }

                this.restToLoad.Clear();
            });
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach(var handler in this.addReloadEvents)
            {
                GeneralHooks.ReloadEvent -= handler;
            }
            foreach(var cmd in this.addCommands)
            {
                TShockAPI.Commands.ChatCommands.Remove(cmd);    
            }
            foreach (var cmd in this.addRestCommands)
            {
                Utility.Utils.Unregister(TShockAPI.TShock.RestApi, cmd);
            }
        }
        base.Dispose(disposing);
    }

    private readonly List<Type> restToLoad = new();

    internal void AutoLoad()
    {
        foreach (var type in this.GetType().Assembly.GetTypes())
        {
            if (type.IsDefined(typeof(ConfigAttribute), false))
            {
                var method = type.BaseType!.GetMethod(nameof(JsonConfigBase<SimpleJsonConfig>.Load)) ?? throw new MissingMethodException($"method 'Load()' is missing inside the lazy loaded config class '{this.Name}'");
                var name = method.Invoke(null, Array.Empty<object>());
                if(type.GetProperty(nameof(JsonConfigBase<SimpleJsonConfig>.ReloadEvent))?.GetValue(null) is GeneralHooks.ReloadEventD reloadEvent)
                {
                    this.addReloadEvents.Add(reloadEvent);
                }
                Console.WriteLine(GetString($"[{this.Name}] config registered: {name}"));
            }
            else if (type.IsDefined(typeof(CommandAttribute), false))
            {
                var cmd = CommandHelper.Register(type);
                this.addCommands.Add(cmd);
                Console.WriteLine(GetString($"[{this.Name}] command registered: {string.Join(",", cmd.Names)}"));
            }
            else if (type.IsDefined(typeof(RestAttribute), false))
            {
                this.restToLoad.Add(type);
            }
        }
    }
}