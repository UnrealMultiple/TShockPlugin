using Newtonsoft.Json.Linq;
using On.OTAPI;
using CaiBot;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using PacketWriter = CaiBot.PacketWriter;
using Program = Terraria.Program;

namespace CaiBotLite;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public static readonly Version VersionNum = new (2025, 3, 15, 1); //日期+版本号(0,1,2...)
    internal static int InitCode = -1;
    public static bool DebugMode;
    private static bool _stopWebsocket;
    internal static ClientWebSocket WebSocket
    {
        get => PacketWriter.WebSocket;
        set => PacketWriter.WebSocket = value;
    }

    public Plugin(Main game) : base(game)
    {
    }

    public override string Author => "Cai,羽学,西江";
    public override string Description => "CaiBot官方机器人的适配插件";
    public override string Name => "CaiBotLitePlugin";
    public override Version Version => VersionNum;


    public override void Initialize()
    {
        DebugMode = Program.LaunchParameters.ContainsKey("-caidebug");
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        Commands.ChatCommands.Add(new Command("caibotlite.admin", this.CaiBotCommand, "caibotlite"));
        Config.Settings.Read();
        Config.Settings.Write();
        ServerApi.Hooks.NetGetData.Register(this, Login.OnGetData, int.MaxValue);
        ServerApi.Hooks.GamePostInitialize.Register(this, GenBindCode);
        Hooks.MessageBuffer.InvokeGetData += Login.MessageBuffer_InvokeGetData; 
        GeneralHooks.ReloadEvent += GeneralHooksOnReloadEvent;
        MapGenerator.Init();
        EconomicSupport.Init();
        PacketWriter.Init(true, DebugMode);
        Task.Factory.StartNew(StartCaiApi, TaskCreationOptions.LongRunning);
        Task.Factory.StartNew(StartHeartBeat, TaskCreationOptions.LongRunning);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method.DeclaringType?.Assembly == asm);
            ServerApi.Hooks.NetGetData.Deregister(this, Login.OnGetData);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, GenBindCode);
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
            Hooks.MessageBuffer.InvokeGetData -= Login.MessageBuffer_InvokeGetData;
            MapGenerator.Dispose();
            _stopWebsocket = true;
            WebSocket.Dispose();
        }

        base.Dispose(disposing);
    }
    
    private static void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        Config.Settings.Read();
        e.Player.SendSuccessMessage("[CaiBotLite]配置文件已重载 :)");
    }
    
    private static async Task? StartHeartBeat()
    {
        while (!_stopWebsocket)
        {
            await Task.Delay(TimeSpan.FromSeconds(60));
            try
            {
                if (WebSocket.State == WebSocketState.Open)
                {
                    var packetWriter = new PacketWriter();
                    packetWriter.SetType("HeartBeat")
                        .Send();
                }
            }
            catch
            {
                TShock.Log.ConsoleInfo("[CaiBotLite]心跳包发送失败!");
            }
        }
    }

    private static async Task? StartCaiApi()
    {
        while (!_stopWebsocket)
        {
            try
            {
                WebSocket = new ClientWebSocket();
                while (string.IsNullOrEmpty(Config.Settings.Token))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    HttpClient client = new ();
                    client.Timeout = TimeSpan.FromSeconds(5.0);
                    var response = client.GetAsync($"https://api.terraria.ink:22338/bot/get_token?code={InitCode}")
                        .Result;
                    if (response.StatusCode != HttpStatusCode.OK || Config.Settings.Token != "")
                    {
                        continue;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    var token = json["token"]!.ToString();
                    Config.Settings.Token = token;
                    Config.Settings.Write();
                    TShock.Log.ConsoleInfo("[CaiBotLite]被动绑定成功!");
                }


                await WebSocket.ConnectAsync(new Uri($"wss://api.terraria.ink:22338/bot/" + Config.Settings.Token), CancellationToken.None);


                while (true)
                {
                    var buffer = new byte[1024];
                    var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (DebugMode)
                    {
                        TShock.Log.ConsoleInfo($"[CaiBotLite]收到BOT数据包: {receivedData}");
                    }

                    _ = CaiBotApi.HandleMessageAsync(receivedData);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo("[CaiBotLite]CaiBot断开连接...");
                if (DebugMode)
                {
                    TShock.Log.ConsoleError(ex.ToString());
                }
                else
                {
                    TShock.Log.ConsoleError("链接失败原因: " + ex.Message);
                }
            }

            await Task.Delay(5000);
        }
    }

    

    private void CaiBotCommand(CommandArgs args)
    {
        var plr = args.Player;

        void ShowHelpText()
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, plr, out var pageNumber))
            {
                return;
            }

            List<string> lines = new () 
            { 
                "/caibot debug CaiBot调试开关", 
                "/caibot code 生成并且展示验证码", 
                "/caibot info 显示CaiBot的一些信息",
                "/caibot unbind 主动解除绑定",
                "/caibot test Cai保留用于测试的命令,乱用可能会爆掉"
            };

            PaginationTools.SendPage(
                plr, pageNumber, lines,
                new PaginationTools.Settings { HeaderFormat = GetString("帮助 ({0}/{1})："), FooterFormat = GetString("输入 {0}caibot help {{0}} 查看更多").SFormat(Commands.Specifier) }
            );
        }

        if (args.Parameters.Count == 0)
        {
            ShowHelpText();
            return;
        }


        switch (args.Parameters[0].ToLowerInvariant())
        {
            case "test":
                Console.WriteLine("你怎么知道Cai喜欢留一个测试命令?");
                break;
            // 帮助
            case "help":
                ShowHelpText();
                return;

            default:
                ShowHelpText();
                break;

            case "信息":
            case "info":
                plr.SendInfoMessage($"[CaiBot信息]\n" +
                                    $"插件版本: v{VersionNum}\n" +
                                    $"WebSocket状态: {WebSocket.State}\n" +
                                    $"绑定QQ群: {(Config.Settings.GroupNumber == 0L ? "未绑定|未连接" : Config.Settings.GroupNumber)}\n" +
                                    $"绑定状态: {Config.Settings.Token != ""}\n" +
                                    $"Debug模式: {DebugMode}\n" +
                                    $"Economic API支持: {EconomicSupport.GetCoinsSupport}\n" +
                                    $"Economic RPG支持: {EconomicSupport.GetLevelNameSupport}\n" +
                                    $"Economic Skill支持: {EconomicSupport.GetSkillSupport}\n"
                );
                break;
            case "调试":
            case "debug":
                DebugMode = !DebugMode;
                plr.SendInfoMessage($"[CaiBotLite]调试模式已{(DebugMode ? "开启" : "关闭")}!");
                break;
            case "验证码":
            case "code":
                if (!string.IsNullOrEmpty(Config.Settings.Token))
                {
                    plr.SendInfoMessage("[CaiBotLite]服务器已绑定无法生成验证码!");
                    return;
                }

                GenBindCode(EventArgs.Empty);
                plr.SendInfoMessage("[CaiBotLite]验证码已生成,请在后台查看喵~");
                break;
            
            case "解绑":
            case "unbind":
                if (string.IsNullOrEmpty(Config.Settings.Token))
                {
                    plr.SendInfoMessage("[CaiBotLite]服务器没有绑定任何群哦!");
                    return;
                }
                Config.Settings.Token = string.Empty;
                Config.Settings.Write();
                WebSocket.Dispose();
                GenBindCode(EventArgs.Empty);
                plr.SendInfoMessage("[CaiBotLite]验证码已生成,请在后台查看喵~");
                break;
        }
    }
    


    public static void GenBindCode(EventArgs args)
    {
        if (!string.IsNullOrEmpty(Config.Settings.Token))
        {
            return;
        }

        InitCode = new Random().Next(10000000, 99999999);
        TShock.Log.ConsoleError($"[CaiBotLite]您的服务器绑定码为: {InitCode}");
    }


    #region 加载前置

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName =
            $"{Assembly.GetExecutingAssembly().GetName().Name}.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        var assemblyData = new byte[stream.Length];
        _ = stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
    }

    #endregion
}
