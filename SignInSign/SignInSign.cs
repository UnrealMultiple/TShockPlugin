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
    
    public override string Name => "告示牌登录 SignInSign";
    public override string Description => "告示牌登录交互插件 支持进服弹窗！";
    public override string Author => "Soofa 羽学 少司命";
    public override Version Version => new(1, 0, 3);

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
        TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command("signinsign.setup", Command.SetupCmd, "setupsign", "gs", "告示"));
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
        GetDataHandlers.TileEdit.Register(OnEdit);
        GetDataHandlers.Sign.Register(OnSignChange);
        GetDataHandlers.SignRead.Register(OnSignRead);
        GeneralHooks.ReloadEvent += LoadConfig;
        Config = Configuration.Reload();
    }

    public static void DisposeHandlers(TerrariaPlugin deregistrator)
    {
        ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);
        ServerApi.Hooks.GamePostInitialize.Deregister(deregistrator, OnGamePostInitialize);
        GetDataHandlers.TileEdit.UnRegister(OnEdit);
        GetDataHandlers.Sign.UnRegister(OnSignChange);
        GetDataHandlers.SignRead.UnRegister(OnSignRead);
        GeneralHooks.ReloadEvent -= LoadConfig;
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
            args.Player.SendSuccessMessage("[告示板登录]重新加载配置完毕。");
        }
    }
    #endregion

    #region  游戏初始化后
    private static void OnGamePostInitialize(EventArgs args)
    {
        if (args == null) return;

        //获取告示牌是否符合坐标
        SignID = Utils.GetSignIdByPos(Main.spawnTileX, Main.spawnTileY - 3);

        //当告示牌ID为-1，通过服务器发送：创建告示牌的指令
        if (SignID == -1)
            SignID = Utils.SpawnSign(Main.spawnTileX, Main.spawnTileY - 3);
    }
    #endregion

    #region 玩家加入事件
    public static void OnNetGreetPlayer(GreetPlayerEventArgs args)
    {
        TSPlayer player = TShock.Players[args.Who];

        //当对象、玩家为空 或告示牌ID小于0 大于999时返回，不做任何处理
        if (args == null || TShock.Players[args.Who] == null || SignID < 0 || SignID >= 999)
            return;

        //检查玩家是否处于登录状态则发包更新告示牌
        if (!player.IsLoggedIn || Config.SignEnable1 == player.IsLoggedIn)
            player.SendData(PacketTypes.SignNew, "", SignID, args.Who);
    }
    #endregion

    #region 阻止没有权限的人破坏告示牌
    private static void OnEdit(object? sender, GetDataHandlers.TileEditEventArgs e)
    {
        if (e == null || e.Player.HasPermission("sign.edit")) return;
        if (Main.tile[e.X, e.Y].type == 55 &&
            Math.Abs(e.X - Main.spawnTileX) < 10 &&
            Math.Abs(e.Y - Main.spawnTileY) < 10 &&
            e.Player.Active)
        {
            e.Player.SendTileSquareCentered(e.X, e.Y, 4);
            e.Player.SendMessage($"{Config.SignText3}", color: Microsoft.Xna.Framework.Color.Yellow);
            e.Handled = false;
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
        string newText = args.Data.ReadString();

        if (args.Player == null
            || args == null
            || signId != SignID
            || signId < 0
            || signId >= Main.sign.Length
            || Main.sign[signId] == null
            || Config.SignEnable == false)
            return;

        #region 帮玩家执行登录指令与记录密码
        string password = Utils.ReadPassword(newText);
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
            TShock.Log.ConsoleInfo($"玩家【{args.Player.Name}】的密码为：{password}");//写入一份到Tshock自己的Logs文件里
            string MiMaPath = Path.Combine(TShock.SavePath, "告示牌登录玩家密码", ""); //写入日志的路径
            Directory.CreateDirectory(MiMaPath); // 创建日志文件夹
            string FileName = $"告示牌登录 {DateTime.Now.ToString("yyyy-MM-dd")}.txt"; //给日志名字加上日期
            File.AppendAllLines(Path.Combine(MiMaPath, FileName), new string[] { DateTime.Now.ToString("u") + $" 玩家【{args.Player.Name}】的密码为：{password}" }); //写入日志log
        } 
        #endregion

        //检查区域保护,暂时还没有任何作用
        IEnumerable<TShockAPI.DB.Region> region = TShock.Regions.InAreaRegion(posX, posY);
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
        if (args.Player == null || Config.SignEnable2 == false) { args.Handled = true; }

        //否则
        else
        {
            if (Config.SignEnable3 == true)
            {
                args.Player!.SendMessage($"{Config.SignText2}", color: Microsoft.Xna.Framework.Color.Yellow);
            }

            // 从配置中读取CommandsOnSignRead列表，并依次执行每个命令
            foreach (var command in Config.CommandsOnSignRead)
            {
                // 执行命令，这里使用TSPlayer.Server执行命令意味着由服务器执行
                Commands.HandleCommand(TSPlayer.Server, command);
            }

            //遍历配置文件中的BUFFID，点击设置BUFF
            foreach (var BuffID in Config.BuffID)
            {
                args.Player.SetBuff(BuffID, Config.BuffTime * 3600, false);
            }

            //遍历配置文件中的物品ID，点击给予物品
            foreach (var ItemID in Config.ItemID)
            {
                args.Player.GiveItem(ItemID, Config.ItemStack, 0);
            }

            //当点击告示牌是否传送为true,将玩家传送到指定坐标（仅对已登录玩家有效）
            if (Config.Teleport == true || args.Player.IsLoggedIn)
            {
                if (Config.Teleport_X  <= 0 || Config.Teleport_Y <= 0)
                {
                    args.Player!.SendMessage($"[告示牌登录]请使用 [c/F25E61:/gs s] 设置传送坐标，当前坐标为：{Config.Teleport_X},{Config.Teleport_Y}", color: Microsoft.Xna.Framework.Color.Yellow);
                }
                else { args.Player.Teleport(x: Config.Teleport_X * 16, y: Config.Teleport_Y * 16, style: Config.Style); }
            }
        }
    }
    #endregion
}
