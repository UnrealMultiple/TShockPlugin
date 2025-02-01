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
    
    public override Version Version => new (1, 1, 2);

    private DateTime _lastUpdate = DateTime.Now;

    public AutoBroadcast(Main game) : base(game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerChat.Register(this, OnChat, int.MinValue); //最低优先级，这样不需要处理命令
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
        }
        base.Dispose(disposing);
    }

    /*
     * 每一秒运行一次
     * 更新所有广播的计时器
     */
    private void OnUpdate(EventArgs args)
    {
        
        if (!((DateTime.Now - this._lastUpdate).TotalSeconds >= 1)) 
        {
            return;
        }
        
        this._lastUpdate = DateTime.Now;
        
        foreach (var broadcast in AutoBroadcastConfig.Instance.Broadcasts)
        {
            if (!broadcast.Enabled || broadcast.Interval==0) //不更新未启用和计时间隔为0的广播
            {
                continue;
            }
            broadcast.SecondUpdate();
        }
    }

    /*
     * 聊天关键词触发广播
     * 当聊天关键词匹配时触发广播
     */
    private static void OnChat(ServerChatEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        
        if (plr == null)
        {
            return;
        }
        
        foreach (var broadcast in AutoBroadcastConfig.Instance.Broadcasts)
        {
            if (!broadcast.Enabled)
            {
                continue;
            }
            
            if (broadcast.TriggerWords.Any(word => args.Text.Contains(word))) //检查消息内是否含有关键词
            {
                broadcast.RunTriggerWords(plr);
            }
        }
    }
    
}