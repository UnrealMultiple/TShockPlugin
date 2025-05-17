using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TimeRate;

[ApiVersion(2, 1)]
public class TimeRate : TerrariaPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 2, 3);
    public override string Description => GetString("使用指令修改时间加速");
    #endregion

    #region 注册与释放
    public TimeRate(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        On.Terraria.Main.UpdateTimeRate += this.UpdateTimeRate;
        TShockAPI.Commands.ChatCommands.Add(new Command("times.admin", Commands.times, "times", "时间加速"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            On.Terraria.Main.UpdateTimeRate -= this.UpdateTimeRate;
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.times);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void ReloadConfig(ReloadEventArgs args = null!)
    {
        LoadConfig();
        args.Player.SendInfoMessage(GetString("[时间加速插件]重新加载配置完毕。"));
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
    }
    #endregion

    #region 时间加速方法
    private void UpdateTimeRate(On.Terraria.Main.orig_UpdateTimeRate orig)
    {
        orig.Invoke();

        // 获取所有在线玩家
        var plr = TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn);
        var plrCount = plr.Count();

        //是否全部玩家睡觉
        var all = plr.All(plr => plr.TPlayer.sleeping.isSleeping);
        //是否任意1人睡觉
        var one = plr.Any(plr => plr.TPlayer.sleeping.isSleeping);

        //用标识决定是否发包优化时间流逝视觉
        var Update = false;

        if ((plrCount > 0 && (Config.Enabled || (all && Config.All) || (one && Config.One))) || plrCount == 0)
        {
            var Rate = plrCount > 0 && (Config.Enabled || (all && Config.All) || (one && Config.One)) ? Config.UpdateRate : 1;

            if (Terraria.Main.dayRate != Rate)
            {
                Update = true;
                Terraria.Main.dayRate = Rate;
            }
        }

        if (Update && Config.TimeSet_Packet)
        {
            TSPlayer.All.SendData((PacketTypes) 18);
        }
    }
    #endregion
}