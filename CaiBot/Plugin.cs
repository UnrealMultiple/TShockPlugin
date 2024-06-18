//代码来源：https://github.com/chi-rei-den/PluginTemplate/blob/master/src/PluginTemplate/Program.cs
//恋恋的TShock插件模板，有改动（为了配合章节名）
//来自棱镜的插件教程

using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;


namespace CaiBotPlugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        //定义插件的作者名称
        public override string Author => "Cai,羽学";

        //插件的一句话描述
        public override string Description => "CaiBot机器人的适配插件";

        //插件的名称
        public override string Name => "CaiBotPlugin";

        public static readonly Version VersionNum = new Version(2024, 6, 19, 1); //日期+版本号(0,1,2...)
        public override Version Version
        {
            get { return VersionNum; }
        }

        //插件的构造器
        public Plugin(Main game) : base(game)
        {
        }
        public static int code = -1;

        public static ClientWebSocket ws = new ClientWebSocket();

        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            string resourceName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{new AssemblyName(args.Name).Name}.dll";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new NullReferenceException("无法加载程序集:" + args.Name);
            byte[] assemblyData = new byte[stream.Length];
            stream.Read(assemblyData, 0, assemblyData.Length);
            return Assembly.Load(assemblyData);
        }
        public override void Initialize()
        {
            Config.Read();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData += MessageBuffer_InvokeGetData;
            ServerApi.Hooks.NetGetData.Register(this, Login.OnGetData, int.MaxValue);
            ServerApi.Hooks.GamePostInitialize.Register(this, GenCode);
            Task task = Task.Run(async () =>
            {
                while (true)
                {

                    try
                    {
                        ws = new ClientWebSocket();
                        while (Config.config.Token == "")
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                        if (Terraria.Program.LaunchParameters.ContainsKey("-cailocalbot"))
                            await ws.ConnectAsync(new Uri("ws://127.0.0.1:22333/bot/" + Config.config.Token), CancellationToken.None);
                        else
                            await ws.ConnectAsync(new Uri("ws://api.terraria.ink:22333/bot/" + Config.config.Token), CancellationToken.None);

                        // 连接成功，发送和接收消息的逻辑
                        // ...


                        // 关闭 WebSocket 连接
                        //await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
                        while (true)
                        {
                            byte[] buffer = new byte[1024];
                            WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            if (Terraria.Program.LaunchParameters.ContainsKey("-caidebug"))
                                TShock.Log.ConsoleInfo($"[CaiAPI]收到BOT数据包: {receivedData}");
                            MessageHandle.HandleMessageAsync(receivedData);

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

                    // 等待一段时间后再次尝试连接
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

            });
        }

        private void GenCode(EventArgs args)
        {
            if (Config.config.Token != "")
            {
                return;
            }
            Random rnd = new Random();
            code = rnd.Next(10000000, 99999999);
            TShock.Log.ConsoleError($"[CaiBot]您的服务器绑定码为: {code}");
        }

        private bool MessageBuffer_InvokeGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
        {
            //Console.WriteLine(1);
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
                if (data == code.ToString())
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



        //插件卸载时执行的代码
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
}
