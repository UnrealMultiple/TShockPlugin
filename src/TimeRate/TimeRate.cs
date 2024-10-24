using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TimeRate;

[ApiVersion(2, 1)]
public class TimeRate : TerrariaPlugin
{

    #region 插件信息
    public override string Name => "时间加速";
    public override string Author => "羽学";
    public override Version Version => new Version(1, 1, 0);
    public override string Description => "涡轮增压不蒸鸭";
    #endregion

    #region 注册与释放
    public TimeRate(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        On.Terraria.Main.UpdateTimeRate += this.UpdateTimeRate;
        TShockAPI.Commands.ChatCommands.Add(new Command("times.admin", Commands.times, "times", "时间加速"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= LoadConfig;
            On.Terraria.Main.UpdateTimeRate -= this.UpdateTimeRate;
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.times);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        Config = Configuration.Read();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[时间加速插件]重新加载配置完毕。"));
    }
    #endregion

    #region 时间加速方法
    private void UpdateTimeRate(On.Terraria.Main.orig_UpdateTimeRate orig)
    {
        orig.Invoke();

        // 获取所有在线玩家
        var plr = TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn);
        //是否全部玩家睡觉
        var all = plr.All(plr => plr.TPlayer.sleeping.isSleeping);
        //是否任意1人睡觉
        var one = plr.Any(plr => plr.TPlayer.sleeping.isSleeping);

        //用标识决定是否发包优化时间流逝视觉
        var Update = false;

        if (Config.Enabled || (all && Config.All) || (one && Config.One))
        {
            // 设置时间加速
            if (Terraria.Main.dayRate != Config.UpdateRate)
            {
                Update = true;
                Terraria.Main.dayRate = Config.UpdateRate;
            }
        }

        else // 设置默认时间速率
        {
            if (Terraria.Main.dayRate != 1)
            {
                Update = true;
                Terraria.Main.dayRate = 1;
            }
        }

        if (Update && Config.TimeSet_Packet)
        {
            TSPlayer.All.SendData((PacketTypes)18);
        }
    }
    #endregion
}