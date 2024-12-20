using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using On.OTAPI;
using Rests;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using Program = Terraria.Program;

namespace CaiBot;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public static readonly Version VersionNum = new (2024, 12, 20, 1); //日期+版本号(0,1,2...)
    public static int InitCode = -1;
    public static bool LocalMode;
    public static bool DebugMode;
    private static bool _stopWebsocket;
    public static ClientWebSocket WebSocket = new ();
    private static readonly Task WebSocketTask = Task.CompletedTask;
    private static readonly CancellationTokenSource TokenSource = new ();

    public Plugin(Main game) : base(game)
    {
    }

    public override string Author => "Cai,羽学,西江";
    public override string Description => "CaiBot机器人的适配插件";
    public override string Name => "CaiBotPlugin";
    public override Version Version => VersionNum;
    
    public override void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        Commands.ChatCommands.Add(new Command("CaiBot.Admin", this.CaiBotCommand, "caibot"));
        Config.Read();
        Config.config.Write();
        LocalMode = Program.LaunchParameters.ContainsKey("-cailocalbot");
        DebugMode = Program.LaunchParameters.ContainsKey("-caidebug");
        BanManager.OnBanPostAdd += this.OnBanInsert;
        Hooks.MessageBuffer.InvokeGetData += this.MessageBuffer_InvokeGetData;
        Hooks.MessageBuffer.InvokeGetData += Login.MessageBuffer_InvokeGetData;
        ServerApi.Hooks.NetGetData.Register(this, Login.OnGetData, int.MaxValue);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat, int.MaxValue);
        PlayerHooks.PlayerPostLogin += this.PlayerHooksOnPlayerPostLogin;
        PlayerHooks.PlayerLogout += this.PlayerHooksOnPlayerLogout;
        ServerApi.Hooks.GamePostInitialize.Register(this, this.GenCode);
        Task.Run(async () =>
        {
            while (!_stopWebsocket)
            {
                try
                {
                    WebSocket = new ClientWebSocket();
                    while (string.IsNullOrEmpty(Config.config.Token))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        HttpClient client = new ();
                        client.Timeout = TimeSpan.FromSeconds(5.0);
                        var response = client.GetAsync($"http://api.terraria.ink:22334/bot/get_token?" +
                                                       $"code={InitCode}")
                            .Result;
                        //TShock.Log.ConsoleInfo($"[CaiAPI]尝试被动绑定,状态码:{response.StatusCode}");
                        if (response.StatusCode == HttpStatusCode.OK && Config.config.Token == "")
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var json = JObject.Parse(responseBody);
                            var token = json["token"]!.ToString();
                            Config.config.Token = token;
                            Config.config.Write();
                            TShock.Log.ConsoleInfo("[CaiAPI]被动绑定成功!");
                        }
                    }

                    if (LocalMode)
                    {
                        await WebSocket.ConnectAsync(new Uri("ws://127.0.0.1:22334/bot/" + Config.config.Token),
                            CancellationToken.None);
                        TShock.Log.ConsoleWarn("[CaiAPI]你正在使用CaiBot本地模式,请确保你已本地部署CaiBot!");
                    }
                    else
                    {
                        await WebSocket.ConnectAsync(new Uri("ws://api.terraria.ink:22334/bot/" + Config.config.Token),
                            CancellationToken.None);
                    }

                    while (true)
                    {
                        var buffer = new byte[1024];
                        var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                            CancellationToken.None);
                        var receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        if (DebugMode)
                        {
                            TShock.Log.ConsoleInfo($"[CaiAPI]收到BOT数据包: {receivedData}");
                        }

                        _ = MessageHandle.HandleMessageAsync(receivedData);
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleInfo("[CaiAPI]CaiBot断开连接...");
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
        }, TokenSource.Token);
        Task.Run(async () =>
        {
            while (!_stopWebsocket)
            {
                await Task.Delay(60000);
                try
                {
                    if (WebSocket.State == WebSocketState.Open)
                    {
                        Dictionary<string, string> heartBeat = new () { { "type", "HeartBeat" } };
                        await MessageHandle.SendDateAsync(JsonConvert.SerializeObject(heartBeat));
                    }
                }
                catch
                {
                    TShock.Log.ConsoleInfo("[CaiBot]心跳包发送失败!");
                }
            }
        }, TokenSource.Token);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method.DeclaringType?.Assembly == asm);
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
            Hooks.MessageBuffer.InvokeGetData -= this.MessageBuffer_InvokeGetData;
            Hooks.MessageBuffer.InvokeGetData -= Login.MessageBuffer_InvokeGetData;
            BanManager.OnBanPostAdd -= this.OnBanInsert;
            ServerApi.Hooks.NetGetData.Deregister(this, Login.OnGetData);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.GenCode);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            PlayerHooks.PlayerPostLogin -= this.PlayerHooksOnPlayerPostLogin;
            PlayerHooks.PlayerLogout -= this.PlayerHooksOnPlayerLogout;
            _stopWebsocket = true;
            WebSocket.Dispose();
            if (!WebSocketTask.IsCompleted)
            {
                TokenSource.Cancel();
                TokenSource.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var plr = TShock.Players[args.Who];

        if (Config.config.WhiteList && plr is { IsLoggedIn: false })
        {
            args.Handled = true;
        }
        
        if (!Config.config.SyncChatFromServer || plr == null || !plr.IsLoggedIn || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier) || string.IsNullOrEmpty(args.Text))
        {
            return;
        }

        var result = new RestObject
        {
            { "type", "chat" },
            { "chat", string.Format(Config.config.ServerChatFormat, plr.Name, args.Text, plr.Group.Name, plr.Group.Prefix, EconomicSupport.IsSupported("GetLevelName") ? EconomicSupport.GetLevelName(plr.Account.Name).Replace("职业:", "") : "不支持") }, //[Server]玩家名:内容" 额外 {2}:玩家组名 {3}:玩家聊天前缀 {4}:Ec职业名
            { "group", Config.config.GroupNumber }
        };
        _ = MessageHandle.SendDateAsync(JsonConvert.SerializeObject(result));
    }

    private void PlayerHooksOnPlayerPostLogin(PlayerPostLoginEventArgs e)
    {
        var plr = e.Player;
        if (!Config.config.SyncChatFromServer || plr == null)
        {
            return;
        }

        var result = new RestObject
        {
            { "type", "chat" },
            { "chat", string.Format(Config.config.JoinServerFormat, plr.Name, plr.Group.Name, plr.Group.Prefix, EconomicSupport.IsSupported("GetLevelName") ? EconomicSupport.GetLevelName(plr.Account.Name).Replace("职业:", "") : "不支持") }, //[Server]玩家名:内容" 额外 {2}:玩家组名 {3}:玩家聊天前缀 {4}:Ec职业名
            { "group", Config.config.GroupNumber }
        };
        _ = MessageHandle.SendDateAsync(JsonConvert.SerializeObject(result));
    }

    private void PlayerHooksOnPlayerLogout(PlayerLogoutEventArgs e)
    {
        var plr = e.Player;
        if (!Config.config.SyncChatFromServer || plr == null)
        {
            return;
        }

        var result = new RestObject
        {
            { "type", "chat" },
            { "chat", string.Format(Config.config.ExitServerFormat, plr.Name, plr.Group.Name, plr.Group.Prefix, EconomicSupport.IsSupported("GetLevelName") ? EconomicSupport.GetLevelName(plr.Account.Name).Replace("职业:", "") : "不支持") }, //[Server]玩家名:内容" 额外 {2}:玩家组名 {3}:玩家聊天前缀 {4}:Ec职业名
            { "group", Config.config.GroupNumber }
        };
        _ = MessageHandle.SendDateAsync(JsonConvert.SerializeObject(result));
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

            List<string> lines = new () { "/caibot debug CaiBot调试开关", "/caibot code 生成并且展示验证码", "/caibot info 显示CaiBot的一些信息" };

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
                                    $"绑定QQ群: {(Config.config.GroupNumber == 0L ? "未绑定|未连接" : Config.config.GroupNumber)}\n" +
                                    $"本地模式: {LocalMode}\n" +
                                    $"绑定状态: {Config.config.Token != ""}\n" +
                                    $"Debug模式: {DebugMode}\n" +
                                    $"Economic API支持: {EconomicSupport.GetCoinsSupport}\n" +
                                    $"Economic RPG支持: {EconomicSupport.GetLevelNameSupport}\n" +
                                    $"Economic Skill支持: {EconomicSupport.GetSkillSupport}\n"
                );
                break;
            case "调试":
            case "debug":
                DebugMode = !DebugMode;
                plr.SendInfoMessage($"[CaiBot]调试模式已{(DebugMode ? "开启" : "关闭")}!");
                break;
            case "验证码":
            case "code":
                if (!string.IsNullOrEmpty(Config.config.Token))
                {
                    plr.SendInfoMessage("[CaiBot]服务器已绑定无法生成验证码!");
                    return;
                }

                this.GenCode(EventArgs.Empty);
                plr.SendInfoMessage("[CaiBot]验证码已生成,请在后台查看喵~");
                break;
        }
    }

    private void OnBanInsert(object? sender, BanEventArgs e)
    {
        if (e.Ban.Identifier.StartsWith(Identifier.Name.Prefix) || e.Ban.Identifier.StartsWith(Identifier.Account.Prefix))
        {
            var name = e.Ban.Identifier.Replace(Identifier.Name.Prefix, "").Replace(Identifier.Account.Prefix, "");
            var expireTime = e.Ban.GetPrettyExpirationString();
            var result = new RestObject
            {
                { "type", "post_ban_add" },
                { "name", name },
                { "reason", e.Ban.Reason },
                { "admin", e.Ban.BanningUser },
                { "expire_time", expireTime == "Never" ? "永久封禁" : expireTime }
            };
            _ = MessageHandle.SendDateAsync(JsonConvert.SerializeObject(result));
        }
    }


    private void GenCode(EventArgs args)
    {
        EconomicSupport.Init();
        if (!string.IsNullOrEmpty(Config.config.Token))
        {
            return;
        }

        InitCode = new Random().Next(10000000, 99999999);
        TShock.Log.ConsoleError($"[CaiBot]您的服务器绑定码为: {InitCode}");
    }

    private bool MessageBuffer_InvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig,
        MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length,
        ref int messageType, int maxPackets)
    {
        if (messageType == 217)
        {
            if (!string.IsNullOrEmpty(Config.config.Token))
            {
                NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable("exist"));
                TShock.Log.ConsoleInfo("[CaiAPI]试图绑定已绑定服务器!");
                return false;
            }

            instance.ResetReader();
            instance.reader.BaseStream.Position = start + 1;
            var data = instance.reader.ReadString();
            var token = Guid.NewGuid().ToString();
            if (data == InitCode.ToString())
            {
                NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable(token));
                Config.config.Token = token;
                Config.config.Write();
                TShock.Log.ConsoleInfo("[CaiAPI]主动绑定成功!");
                return false;
            }

            NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable("code"));
        }
        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }
    
    
    #region 加载前置

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName =
            $"{Assembly.GetExecutingAssembly().GetName().Name}.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            var assemblyData = new byte[stream.Length];
            _ = stream.Read(assemblyData, 0, assemblyData.Length);
            return Assembly.Load(assemblyData);
        }

        return null;
    }

    #endregion
}