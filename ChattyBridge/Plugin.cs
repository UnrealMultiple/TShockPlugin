using ChattyBridge.Message;
using Newtonsoft.Json.Linq;
using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ChattyBridge;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    private HttpClient Client { get; set; }

    internal static Config Config { get; set; } = new();

    public const string RestAPI = "/chat";

    public Plugin(Main game) : base(game)
    {
        Client = new HttpClient();
    }

    public override void Initialize()
    {
        LoadConfig();
        TShock.RestApi.Register(RestAPI, Receive);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        GeneralHooks.ReloadEvent += (_) => LoadConfig();
    }

    public void LoadConfig()
    {
        if (File.Exists(Config.PATH))
        {
            try
            {
                Config = Config.Read();
                Config.Write();
            }
            catch(Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
            }
        }
        else
        {
            Config.Write();
        }
    }

    private object Receive(RestRequestArgs args)
    {
        var msg = args.Parameters["msg"];
        if (!string.IsNullOrEmpty(msg))
        {
            try
            { 
                var json = JObject.Parse(msg);
                if (json.TryGetValue("type", out var type))
                {
                    switch (type.ToString())
                    {
                        case "player_join":
                            {
                                var jobj = json.ToObject<PlayerJoinMessage>()!;
                                TShock.Utils.Broadcast($"[{jobj.ServerName}] {jobj.Name} 加入服务器", jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                                break;
                            }

                        case "player_leave":
                            {
                                var jobj = json.ToObject<PlayerLeaveMessage>()!;
                                TShock.Utils.Broadcast($"[{jobj.ServerName}] {jobj.Name} 离开服务器", jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                                break;
                            }
                        case "player_chat":
                            {
                                var jobj = json.ToObject<PlayerChatMessage>()!;
                                TShock.Utils.Broadcast($"[{jobj.ServerName}] {jobj.Name}: {jobj.Text}", jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                                break;
                            }
                        default:
                            TShock.Log.ConsoleError($"接收到未知类型:{type}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.ToString());
            }
            
        }
        return new RestObject("200");
    }

    private void SendMessage(string msg)
    {
        Task.Run(() =>
        {
            foreach (var host in Config.RestHost)
            {
                try
                {
                    var url = $"http://{host}{RestAPI}?msg={msg}";
                    Client.Send(new HttpRequestMessage(HttpMethod.Get, url));
                }
                catch
                {
                    TShock.Log.ConsoleError($"[聊天桥] 信息发送失败，目标地址:{host}");
                }
            }
        });
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;
        var msg = new PlayerLeaveMessage(player).ToJson();
        SendMessage(msg);
    }

    private void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null || args.Handled)
            return;
        var msg = new PlayerJoinMessage(player).ToJson();
        SendMessage(msg);
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null || args.Handled)
            return;
        var msg = new PlayerChatMessage(player, args.Text).ToJson();
        SendMessage(msg);
    }
}
