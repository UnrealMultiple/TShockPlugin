using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace Plugin;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{

    #region 插件模版信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 0, 3);
    public override string Description => GetString("根据玩家血量计算死亡后会掉落多少生命水晶");
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        TShockAPI.GetDataHandlers.KillMe += OnPlayerDeath!;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TShockAPI.GetDataHandlers.KillMe -= OnPlayerDeath!;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 硬核死亡掉落方法
    private static void OnPlayerDeath(object sender, TShockAPI.GetDataHandlers.KillMeEventArgs args)
    {
        var plr = TShock.Players[args.Player.Index];
        var drop_amount = (plr.TPlayer.statLifeMax - 100) / 20;

        if (plr.Difficulty != 2 || drop_amount == 0)
        {
            return;
        }

        var itemIndex = Item.NewItem(null, (int) plr.X, (int) plr.Y, plr.TPlayer.width, plr.TPlayer.height, 29, drop_amount, true, 0, true);
    }
    #endregion

}