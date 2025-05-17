using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PacketsStop;

[ApiVersion(2, 1)]
public class PacketsStop : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 7);
    public override string Author => "羽学 少司命";
    public override string Description => GetString("拦截指定玩家的数据包");
    #endregion

    #region 注册与卸载
    public PacketsStop(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        Packets = GetPackets();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData, int.MaxValue);
        TShockAPI.Commands.ChatCommands.Add(new Command("packetstop.use", Commands.Command, "pksp", "packetstop"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Command);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void ReloadConfig(ReloadEventArgs args = null!)
    {
        LoadConfig();
        Packets = GetPackets();
        args.Player.SendInfoMessage(GetString("[数据包拦截]重新加载配置完毕。"));
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
    }
    #endregion

    #region 处理数据包方法
    private static HashSet<PacketTypes> Packets = new HashSet<PacketTypes>();
    private void OnGetData(GetDataEventArgs args)
    {
        var plr = TShock.Players[args.Msg.whoAmI];

        //如果插件没开启 则返回
        if (!Config.Enabled)
        {
            return;
        }

        //如果数据包在拦截列表中且玩家名在拦截名单中
        if (Packets.Contains(args.MsgID) && Config.PlayerList.Contains(plr.Name))
        {
            //直接处理数据包
            args.Handled = true;
        }
        else 
        {
            //过滤数据包
            this.HandlePacket(plr, args.MsgID);
        }
    }
    #endregion

    #region 过滤数据包方法
    private readonly Dictionary<string, Dictionary<PacketTypes, DateTime>> CountDict = new Dictionary<string, Dictionary<PacketTypes, DateTime>>();
    private void HandlePacket(TSPlayer plr, PacketTypes type)
    {
        if (plr.Name != null)
        {
            var now = DateTime.Now;
            if (!this.CountDict.TryGetValue(plr.Name, out var dict))
            {
                dict = new Dictionary<PacketTypes, DateTime>();
                this.CountDict[plr.Name] = dict;
            }
            if (dict.TryGetValue(type, out var lastTime))
            {
                if ((now - lastTime).TotalMilliseconds >= 1000.0)
                {
                    dict[type] = now;
                }
            }
            else
            {
                dict[type] = now;
            }
        }
    }
    #endregion

    #region 获取数据包名
    //在配置文件中指定想要拦截数据包包名，
    //GetPackets方法会把这些包名转换成对应的PacketTypes枚举值并存储起来，
    //以便在OnGetData方法中进行数据包处理时使用
    private static HashSet<PacketTypes> GetPackets()
    {
        var Packets = new HashSet<PacketTypes>();
        foreach (var name in Config.Packets)
        {
            if (Enum.TryParse(name, out PacketTypes type))
            {
                Packets.Add(type);
            }
            else
            {
                TShock.Log.ConsoleError(GetString($"无法识别的数据包类型名称: {name}"));
            }
        }
        return Packets;
    }
    #endregion
}