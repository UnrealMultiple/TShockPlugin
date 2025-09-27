using ChattyBridge.Message;
using LazyAPI;
using Newtonsoft.Json.Linq;
using Rests;
using System.Reflection;
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
    public override Version Version => new Version(1, 0, 1, 5);

    private readonly HttpClient _client  = new ();

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.addRestCommands.Add(new RestCommand("/chat", HandleMsg));
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
        }

        base.Dispose(disposing);
    }


    private static object HandleMsg(RestRequestArgs args)
    {
        var msg = args.Parameters["msg"];
        TShock.Log.ConsoleDebug($"ChattyBridge Receive: {msg}");
        if (args.Parameters["verify"] != Config.Instance.Verify)
        {
            return new RestObject("403") { Response = "ChattyBridge Token Verify Error!" };
        }
        try
        {
            var json = JObject.Parse(msg);
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
            return new RestObject("500") { Response = $"An error occurred in the processing of the message: {ex.Message}" };
        }
        return new RestObject("200") { Response = "Message Send Successfully!" };
    }

    private void SendMsg(string msg)
    {
        Task.Run(async () =>
        {
            foreach (var host in Config.Instance.RestHost)
            {
                try
                {
                    var url = $"http://{host}/chat";
                    await this.HttpGet(url, new Dictionary<string, string>
                    {
                        { "msg", msg },
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

    private async Task HttpGet(string url, Dictionary<string, string> payload)
    {
        var urlBuilder = new UriBuilder(url);
        var param = HttpUtility.ParseQueryString(urlBuilder.Query);
        foreach (var (key, value) in payload)
        {
            param[key] = value;
        }
        urlBuilder.Query = param.ToString();
        var response = await this._client.GetAsync(urlBuilder.ToString());
        try
        {
            TShock.Log.ConsoleDebug($"ChattyBridge Send: {payload["msg"]}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            TShock.Log.ConsoleError($"[ChattyBridge] Error: {e.Message}");
        }
        finally
        {
            TShock.Log.ConsoleDebug($"ChattyBridage Response: {await response.Content.ReadAsStringAsync()}");
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