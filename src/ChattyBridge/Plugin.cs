using ChattyBridge.Message;
using LazyAPI;
using Newtonsoft.Json.Linq;
using Rests;
using System.Reflection;
using System.Text;
using System.Web;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChattyBridge;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 0, 0, 5);

    private HttpClient Client { get; set; }

    public const string RestAPI = "/chat";

    public Plugin(Main game) : base(game)
    {
        this.Client = new HttpClient();
    }

    public override void Initialize()
    {
        TShock.RestApi.Register(RestAPI, this.Receive);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ((List<RestCommand>) typeof(Rest).GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(TShock.RestApi)!)
            .RemoveAll(x => x.Name == "/chat");
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
        }

        base.Dispose(disposing);
    }


    private object Receive(RestRequestArgs args)
    {
        var msg = args.Parameters["msg"];
        var isVer = args.Parameters["verify"] == Config.Instance.Verify;
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
                            TShock.Utils.Broadcast(string.Join(Config.Instance.MessageFormat.JoinFormat, jobj.ServerName, jobj.Name), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                            break;
                        }

                        case "player_leave":
                        {
                            var jobj = json.ToObject<PlayerLeaveMessage>()!;
                            TShock.Utils.Broadcast(string.Join(Config.Instance.MessageFormat.LeaveFormat, jobj.ServerName, jobj.Name), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                            break;
                        }
                        case "player_chat":
                        {
                            var jobj = json.ToObject<PlayerChatMessage>()!;
                            TShock.Utils.Broadcast(string.Join(Config.Instance.MessageFormat.ChatFormat, jobj.ServerName, jobj.Name, jobj.Text), jobj.RGB[0], jobj.RGB[1], jobj.RGB[2]);
                            break;
                        }
                        default:
                            TShock.Log.ConsoleError(GetString($"接收到未知类型:{type}"));
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
            foreach (var host in Config.Instance.RestHost)
            {
                try
                {
                    var url = $"http://{host}{RestAPI}";
                    this.HttpGet(url, new Dictionary<string, string>()
                    {
                        { "msg", baseStr },
                        { "verify", Config.Instance.Verify }
                    });
                }
                catch
                {
                    TShock.Log.ConsoleError(GetString($"[聊天桥] 信息发送失败，目标地址:{host}"));
                }
            }
        });
    }

    public void HttpGet(string url, Dictionary<string, string> playload)
    {
        var urlBuild = new UriBuilder(url);
        var param = HttpUtility.ParseQueryString(urlBuild.Query);
        foreach (var (key, value) in playload)
        {
            param[key] = value;
        }

        urlBuild.Query = param.ToString();
        this.Client.Send(new HttpRequestMessage(HttpMethod.Get, url));
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        var msg = new PlayerLeaveMessage(player).ToJson();
        this.SendMessage(msg);
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        var msg = new PlayerJoinMessage(player).ToJson();
        this.SendMessage(msg);
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        if (!Config.Instance.ForwardCommamd &&
            (args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)
            || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier)))
        {
            return;
        }

        var msg = new PlayerChatMessage(player, args.Text).ToJson();
        this.SendMessage(msg);
    }
}