//代码来源：https://github.com/chi-rei-den/PluginTemplate/blob/master/src/PluginTemplate/Program.cs
//恋恋的TShock插件模板，有改动（为了配合章节名）
//来自棱镜的插件教程

using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;


namespace CaiBot;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "Cai,羽学";
    public override string Description => "CaiBot机器人的适配插件";
    public override string Name => "CaiBotPlugin";
    public static readonly Version VersionNum = new(2024, 7, 24, 2); //日期+版本号(0,1,2...)
    public override Version Version => VersionNum;

    //插件的构造器
    public Plugin(Main game) : base(game)
    {
    }

    public static int InitCode = -1;

    public static ClientWebSocket WebSocket = new();
    
    public Task WsTask;
    public Task HeartBeat;

    #region 加载前置

    private Assembly CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        string resourceName =
            $"{Assembly.GetExecutingAssembly().GetName().Name}.{new AssemblyName(args.Name).Name}.dll";
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new NullReferenceException("无法加载程序集:" + args.Name);
        byte[] assemblyData = new byte[stream.Length];
        stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
    }

    #endregion

    public override void Initialize()
    {
        Config.Read();
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        On.OTAPI.Hooks.MessageBuffer.InvokeGetData += MessageBuffer_InvokeGetData;
        ServerApi.Hooks.NetGetData.Register(this, Login.OnGetData, int.MaxValue);
        ServerApi.Hooks.GamePostInitialize.Register(this, GenCode);
        WsTask = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    WebSocket = new ClientWebSocket();
                    while (Config.config.Token == "") await Task.Delay(TimeSpan.FromSeconds(5));

                    if (Terraria.Program.LaunchParameters.ContainsKey("-cailocalbot"))
                        await WebSocket.ConnectAsync(new Uri("ws://127.0.0.1:22333/bot/" + Config.config.Token),
                            CancellationToken.None);
                    else
                        await WebSocket.ConnectAsync(new Uri("ws://api.terraria.ink:22333/bot/" + Config.config.Token),
                            CancellationToken.None);
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        WebSocketReceiveResult result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                            CancellationToken.None);
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        if (Terraria.Program.LaunchParameters.ContainsKey("-caidebug"))
                            TShock.Log.ConsoleInfo($"[CaiAPI]收到BOT数据包: {receivedData}");
                        await MessageHandle.HandleMessageAsync(receivedData);
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleInfo($"[CaiAPI]CaiBot断开连接...");
                    if (Terraria.Program.LaunchParameters.ContainsKey("-caidebug"))
                        TShock.Log.ConsoleError(ex.ToString());
                    else
                        TShock.Log.ConsoleError("链接失败原因: " + ex.Message);
                }

                await Task.Delay(5000);
            }
        });
        HeartBeat = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(60000);
                try
                {
                    if (WebSocket.State == WebSocketState.Open)
                    {
                        Dictionary<string, string> heartBeat = new()
                        {
                            { "type", "HeartBeat" }
                        };
                        await MessageHandle.SendDateAsync(JsonConvert.SerializeObject(heartBeat));
                    }
                }
                catch
                {
                    TShock.Log.ConsoleInfo("[CaiBot]心跳包发送失败!");
                }
            }
        });
    }

    private void GenCode(EventArgs args)
    {
        if (!string.IsNullOrEmpty(Config.config.Token)) return;
        InitCode = new Random().Next(10000000, 99999999);
        TShock.Log.ConsoleError($"[CaiBot]您的服务器绑定码为: {InitCode}");
    }

    private bool MessageBuffer_InvokeGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig,
        MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length,
        ref int messageType, int maxPackets)
    {
        if (messageType == 217)
        {
            if (!string.IsNullOrEmpty(Config.config.Token))
            {
                NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable("exist"));
                return false;
            }

            instance.ResetReader();
            instance.reader.BaseStream.Position = start + 1;
            string data = instance.reader.ReadString();
            string token = Guid.NewGuid().ToString();
            if (data == InitCode.ToString())
            {
                NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable(token));
                Config.config.Token = token;
                Config.config.Write();
            }
            else
            {
                NetMessage.SendData(2, instance.whoAmI, -1, NetworkText.FromFormattable("code"));
            }
        }


        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= MessageBuffer_InvokeGetData;
            ServerApi.Hooks.NetGetData.Deregister(this, Login.OnGetData);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, GenCode);
        }

        base.Dispose(disposing);
    }
}