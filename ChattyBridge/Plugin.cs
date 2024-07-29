using ChattyBridge.Message;
using Newtonsoft.Json.Linq;
using Rests;
using System.Reflection;
using System.Text;
using System.Web;
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

    public override Version Version => new(1, 0, 0, 3);

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
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        GeneralHooks.ReloadEvent += LoadConfig;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            GeneralHooks.ReloadEvent -= LoadConfig;
        }

        base.Dispose(disposing);
    }

    public static void LoadConfig(ReloadEventArgs? args = null)
    {
        if (File.Exists(Config.PATH))
        {
            try
            {
                Config = Config.Read();
                Config.Write();
            }
            catch (Exception ex)
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
        var isVer = args.Parameters["verify"] == Config.Verify;
        if (!string.IsNullOrEmpty(msg) && isVer)
        {
            try
            {
                var sourceMsg = Encoding.UTF8.GetString(Convert.FromBase64String(msg));
                var json = JObject.Parse(sourceMsg);
                if (json.TryGetValue("type", out var type))
                {
                    switch (type.ToString())
                    {
                        case "player_join":
                            {
                                var jobj = json.ToObject<PlayerJoinMessage>()!;
                                TShock.Utils.Broadcast(string.Join(Config.MessageFormat.JoinFormat, jobj.ServerName, jobj.Name), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                                break;
                            }

                        case "player_leave":
                            {
                                var jobj = json.ToObject<PlayerLeaveMessage>()!;
                                TShock.Utils.Broadcast(string.Join(Config.MessageFormat.LeaveFormat, jobj.ServerName, jobj.Name), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                                break;
                            }
                        case "player_chat":
                            {
                                var jobj = json.ToObject<PlayerChatMessage>()!;
                                TShock.Utils.Broadcast(string.Join(Config.MessageFormat.ChatFormat, jobj.ServerName, jobj.Name, jobj.Text), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
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
            var baseStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
            foreach (var host in Config.RestHost)
            {
                try
                {
                    var url = $"http://{host}{RestAPI}";
                    HttpGet(url, new Dictionary<string, string>()
                    {
                        { "msg", baseStr },
                        { "verify", Config.Verify }
                    });
                }
                catch
                {
                    TShock.Log.ConsoleError($"[聊天桥] 信息发送失败，目标地址:{host}");
                }
            }
        });
    }

    public void HttpGet(string url, Dictionary<string, string> playload)
    {
        var urlBuild = new UriBuilder(url);
        var param = HttpUtility.ParseQueryString(urlBuild.Query);
        foreach (var (key, value) in playload)
            param[key] = value;
        urlBuild.Query = param.ToString();
        Client.Send(new HttpRequestMessage(HttpMethod.Get, url));
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;
        var msg = new PlayerLeaveMessage(player).ToJson();
        SendMessage(msg);
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;
        var msg = new PlayerJoinMessage(player).ToJson();
        SendMessage(msg);
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;
        if (!Config.ForwardCommamd &&
            (args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)
            || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier)))
            return;
        var msg = new PlayerChatMessage(player, args.Text).ToJson();
        SendMessage(msg);
    }
}
