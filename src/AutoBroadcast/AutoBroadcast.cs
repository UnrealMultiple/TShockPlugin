using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoBroadcast;

[ApiVersion(2, 1)]
public class AutoBroadcast : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Author => "Scavenger,Cai";
    public override string Description => GetString("自动广播插件");
    
    public override Version Version => new (1, 1, 1);

    private DateTime _lastCheck = DateTime.UtcNow;

    public AutoBroadcast(Main game) : base(game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
        }
        base.Dispose(disposing);
    }
    
    public void OnUpdate(EventArgs args)
    {
        if (!((DateTime.UtcNow - this._lastCheck).TotalSeconds >= 1))
        {
            return;
        }
        this._lastCheck = DateTime.UtcNow;
        
        foreach (var broadcast in AutoBroadcastConfig.Instance.Broadcasts)
        {
            if (!broadcast.Enabled)
            {
                continue;
            }

            broadcast.SecondUpdate();
        }
    }
    public void OnChat(ServerChatEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }

        var plr = TShock.Players[args.Who];
        
        foreach (var broadcast in AutoBroadcastConfig.Instance.Broadcasts)
        {
            if (!broadcast.Enabled)
            {
                continue;
            }

            if (!broadcast.Groups.Contains(plr.Group.Name))
            {
                continue;
            }
            
            foreach (var word in broadcast.TriggerWords)
            {
                if (args.Text.Contains(word))
                {
                    broadcast.RunTriggerWords(plr);
                    break;
                }
            }
        }
    }
    
}