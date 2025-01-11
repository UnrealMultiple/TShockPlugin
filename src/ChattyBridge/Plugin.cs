using ChattyBridge.Message;
using LazyAPI;
using Newtonsoft.Json.Linq;
using Rests;
using System.Net;
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
    public override Version Version => new (1, 0, 1, 1);

    private readonly HttpClient _client  = new ();

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        TShock.RestApi.Register("/chat", HandleMsg);
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


    private static object HandleMsg(RestRequestArgs args)
    {
        var msg = args.Parameters["msg"];
        var isVer = args.Parameters["verify"] == Config.Instance.Verify;
        if (!isVer)
        {
            return new RestObject("403");
        }
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
                        var joinMsg = json.ToObject<PlayerJoinMessage>()!;
                        TShock.Utils.Broadcast(string.Format(Config.Instance.MessageFormat.JoinFormat, joinMsg.ServerName, joinMsg.Name), joinMsg.RGB[0], joinMsg.RGB[1], joinMsg.RGB[2]);
                        break;
                    }

                    case "player_leave":
                    {
                        var leaveMsg = json.ToObject<PlayerLeaveMessage>()!;
                        TShock.Utils.Broadcast(string.Format(Config.Instance.MessageFormat.LeaveFormat, leaveMsg.ServerName, leaveMsg.Name), leaveMsg.RGB[0], leaveMsg.RGB[1], leaveMsg.RGB[2]);
                        break;
                    }
                    case "player_chat":
                    {
                        var chatMsg = json.ToObject<PlayerChatMessage>()!;
                        TShock.Utils.Broadcast(string.Format(Config.Instance.MessageFormat.ChatFormat, chatMsg.ServerName, chatMsg.Name, chatMsg.Text), chatMsg.RGB[0], chatMsg.RGB[1], chatMsg.RGB[2]);
                        break;
                    }
                    default:
                        TShock.Log.ConsoleError(GetString($"[聊天桥] 接收到未知类型:{type}"));
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.ToString());
            return new RestObject("500");
        }
        return new RestObject("200");
    }

    private void SendMsg(string msg)
    {
        Task.Run(() =>
        {
            var baseStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
            foreach (var host in Config.Instance.RestHost)
            {
                try
                {
                    var url = $"http://{host}/chat";
                    this.HttpGet(url, new Dictionary<string, string>
                    {
                        { "msg", baseStr },
                        { "verify", Config.Instance.Verify }
                    });
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError(GetString($"[聊天桥] 信息发送失败，目标地址:{host}\n错误信息:{ex}"));
                }
            }
        });
    }

    private void HttpGet(string url, Dictionary<string, string> payload)
    {
        var urlBuilder = new UriBuilder(url);
        var param = HttpUtility.ParseQueryString(urlBuilder.Query);
        foreach (var (key, value) in payload)
        {
            param[key] = value;
        }
        urlBuilder.Query = param.ToString();
        var response =  this._client.Send(new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString()));
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            case HttpStatusCode.Unauthorized:
                 TShock.Log.ConsoleError(GetString($"[聊天桥] 访问目标服务器验证失败:{url},请检查你的令牌是否配置正确!"));
                 break;
            case HttpStatusCode.InternalServerError:
                TShock.Log.ConsoleError(GetString($"[聊天桥] 目标服务器处理请求出错:{url}!"));
                break;
                    
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        var msg = new PlayerLeaveMessage(player).ToJson();
        this.SendMsg(msg);
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        var msg = new PlayerJoinMessage(player).ToJson();
        this.SendMsg(msg);
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        if (!player.IsLoggedIn)
        {
            return;
        }

        if (!Config.Instance.ForwardCommand &&
            (args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)
            || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier)))
        {
            return;
        }

        var msg = new PlayerChatMessage(player, args.Text).ToJson();
        this.SendMsg(msg);
    }
}