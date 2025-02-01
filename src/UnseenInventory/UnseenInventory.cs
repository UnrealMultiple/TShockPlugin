using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI.Hooks;


namespace UnseenInventory;

[ApiVersion(2, 1)]
public class UnseenInventory : TerrariaPlugin
{

    public override string Author => "肝帝熙恩";
    public override string Description => GetString("允许服务器端生成“无法获取”的物品");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 3);
    public static Configuration Config = null!;

    public UnseenInventory(Main game) : base(game)
    {
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        OnUpdate();
        args.Player.SendSuccessMessage(GetString("[{0}] 重新加载配置完毕。"), nameof(UnseenInventory));
    }

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        OnUpdate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
        }
        base.Dispose(disposing);
    }

    private static void OnUpdate()
    {

        foreach (var item in Config.AllowList)
        {
            Array.Clear(ItemID.Sets.Deprecated, item, 1);
        }
    }

}