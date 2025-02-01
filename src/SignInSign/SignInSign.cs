using System.IO.Streams;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SignInSign;
[ApiVersion(2, 1)]
public class SignInSign : TerrariaPlugin
{
    #region 插件信息

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Description => GetString("告示牌登录交互插件 支持进服弹窗！");
    public override string Author => "Soofa 羽学 少司命";
    public override Version Version => new Version(1, 0, 8);

    #endregion

    #region 实例变量
    public static Configuration Config = Configuration.Reload();
    private static int SignID = -1;
    #endregion

    #region 注册与卸载钩子
    public SignInSign(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command(Command.SetupCmd, "setupsign", "gs", "告示"));
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize, -100); //优先级为倒数100 避免和CreateSpawn、SpawnInfra冲突
        GetDataHandlers.TileEdit.Register(OnEdit);
        GetDataHandlers.Sign.Register(OnSignChange);
        GetDataHandlers.SignRead.Register(OnSignRead);
        GeneralHooks.ReloadEvent += LoadConfig;
        Config = Configuration.Reload();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TShockAPI.Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == Command.SetupCmd);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnNetGreetPlayer);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
            GetDataHandlers.TileEdit.UnRegister(OnEdit);
            GetDataHandlers.Sign.UnRegister(OnSignChange);
            GetDataHandlers.SignRead.UnRegister(OnSignRead);
            GeneralHooks.ReloadEvent -= LoadConfig;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置文件创建与重读加载方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        //调用Configuration.cs文件Read和Write方法
        Config = Configuration.Read(Configuration.ConfigPath);
        Config.Write(Configuration.ConfigPath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage(GetString("[告示板登录]重新加载配置完毕。"));
        }
    }
    #endregion

    #region  游戏初始化后
    private static void OnGamePostInitialize(EventArgs args)
    {
        if (args == null)
        {
            return;
        }

        //获取告示牌是否符合坐标
        SignID = Utils.GetSignIdByPos(Main.spawnTileX, Main.spawnTileY - 3);

        //当告示牌ID为-1，通过服务器发送：创建告示牌的指令
        if (SignID == -1)
        {
            SignID = Utils.SpawnSign(Main.spawnTileX, Main.spawnTileY - 3);
        }
    }
    #endregion

    #region 玩家加入事件
    public static void OnNetGreetPlayer(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];

        //当对象、玩家为空 或告示牌ID小于0 大于999时返回，不做任何处理
        if (args == null || TShock.Players[args.Who] == null || SignID < 0 || SignID >= 999)
        {
            return;
        }

        //检查玩家是否处于登录状态则发包更新告示牌
        if (!player.IsLoggedIn || Config.SignEnable1 == player.IsLoggedIn)
        {
            player.SendData(PacketTypes.SignNew, "", SignID, args.Who);
        }
    }
    #endregion

    #region 阻止没有权限的人破坏告示牌
    private static void OnEdit(object? sender, GetDataHandlers.TileEditEventArgs e)
    {
        if (e == null || e.Player.HasPermission("sign.edit")) { return; }
        if (Main.tile[e.X, e.Y].type == 55 &&
            Math.Abs(e.X - Main.spawnTileX) < 10 &&
            Math.Abs(e.Y - Main.spawnTileY) < 10 &&
            e.Player.Active)
        {
            e.Player.SendTileSquareCentered(e.X, e.Y, 4);
            e.Player.SendMessage($"{Config.SignText3}", color: Microsoft.Xna.Framework.Color.Yellow);
            e.Handled = true;
        }
    }
    #endregion

    #region 注册登录相关
    public static void OnSignChange(object? sender, GetDataHandlers.SignEventArgs args)
    {
        args.Data.Seek(0, SeekOrigin.Begin);
        int signId = args.Data.ReadInt16();
        int posX = args.Data.ReadInt16();
        int posY = args.Data.ReadInt16();
        var newText = args.Data.ReadString();

        if (args.Player == null
            || args == null
            || signId != SignID
            || signId < 0
            || signId >= Main.sign.Length
            || Main.sign[signId] == null
            || Config.SignEnable == false)
        {
            return;
        }

        #region 帮玩家执行登录指令与记录密码
        var password = Utils.ReadPassword(newText);
        if (TShock.UserAccounts.GetUserAccountByName(args.Player.Name) == null)
        {
            TShockAPI.Commands.HandleCommand(args.Player, $"/register {password}");
        }
        else
        {
            TShockAPI.Commands.HandleCommand(args.Player, $"/login {password}");
        }

        if (Config.PassInfo == true)
        {
            TShock.Log.ConsoleInfo(GetString($"玩家【{args.Player.Name}】的密码为：{password}"));//写入一份到Tshock自己的Logs文件里
            var MiMaPath = Path.Combine(TShock.SavePath, "告示牌登录玩家密码", ""); //写入日志的路径
            Directory.CreateDirectory(MiMaPath); // 创建日志文件夹
            var FileName = $"告示牌登录 {DateTime.Now:yyyy-MM-dd}.txt"; //给日志名字加上日期
            File.AppendAllLines(Path.Combine(MiMaPath, FileName), new[]
            {
                GetString($"{DateTime.Now:u} 玩家【{args.Player.Name}】的密码为：{password}")
            }); //写入日志log
        }
        #endregion

        //检查区域保护,暂时还没有任何作用
        var region = TShock.Regions.InAreaRegion(posX, posY);
        if (region.Any() && !region.First().Owner.Equals(args.Player.Name)) { return; }

        Main.sign[signId].text = SignInSign.Config.SignText;
        TSPlayer.All.SendData(PacketTypes.SignNew, number: signId);
        args.Handled = true;
    }
    #endregion

    #region  玩家主动点击告示牌触发的方法
    public static void OnSignRead(object? sender, GetDataHandlers.SignReadEventArgs args)
    {
        //当是否允许点击告示牌为false，则返回不做任何处理
        if (args.Player == null || Config.SignEnable2 == false || !args.Player.IsLoggedIn)
        {
            args.Handled = true;
        }
        else
        {
            if (Config.SignEnable3 == true)
            {
                args.Player!.SendMessage($"{Config.SignText2}", color: Microsoft.Xna.Framework.Color.Yellow);
            }

            //执行指令方法
            Cmd(args.Player);

            foreach (var BuffID in Config.BuffID)
            {
                args.Player.SetBuff(BuffID, Config.BuffTime * 3600, false);
            }

            foreach (var ItemID in Config.ItemID)
            {
                args.Player.GiveItem(ItemID, Config.ItemStack, 0);
            }

            //当点击告示牌是否传送为true,将玩家传送到指定坐标（仅对已登录玩家有效）
            if (Config.Teleport == true)
            {
                if (Config.Teleport_X <= 0 || Config.Teleport_Y <= 0)
                {
                    args.Player!.SendMessage(
                        GetString($"[告示牌登录]请使用 [c/F25E61:/gs s] 设置传送坐标，当前坐标为：{Config.Teleport_X},{Config.Teleport_Y} \n") +
                        GetString($"指令 [c/F25E61:/gs s] 的权限名为：signinsign.tp"), color: Microsoft.Xna.Framework.Color.Yellow);
                }
                else
                {
                    args.Player.Teleport(x: Config.Teleport_X * 16, y: Config.Teleport_Y * 16, style: Config.Style);
                }
            }
        }
    }
    #endregion

    #region 用超管组身份帮玩家执行指令方法
    private static void Cmd(TSPlayer plr)
    {
        var group = plr.Group;
        try
        {
            plr.Group = new SuperAdminGroup();
            foreach (var cmd in Config.CmdList)
            {
                Commands.HandleCommand(plr, cmd);
            }
        }
        finally
        {
            plr.Group = group;
        }
    }
    #endregion

}