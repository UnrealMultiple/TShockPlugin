using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using TShockAPI;

namespace CaiBot;

[Serializable]
public class PacketWriter : Dictionary<string, object>
{
    public static bool Debug;
    public static bool IsLiteMessage;
    public static ClientWebSocket WebSocket = null!;
    private readonly long _groupId;
    private readonly string _groupOpenId;
    private readonly string _msgId;
    private readonly long _at;

    public static void Init(bool isLiteMessage,bool debug = false)
    {
        IsLiteMessage = isLiteMessage;
        Debug = debug;
    }
    
    
    public PacketWriter()
    {
        this._groupId = 0L;
        this._at = 0L;
        this._groupOpenId = "";
        this._msgId = "";
    }
    
    public PacketWriter(long groupId,long at = 0L)
    {
        this._groupId = groupId;
        this._at = at;
        this._groupOpenId = "";
        this._msgId = "";
    }
    
    public PacketWriter(string groupOpenId, string msgId)
    {
        this._groupId = 0L;
        this._at = 0L;
        this._groupOpenId = groupOpenId;
        this._msgId = msgId;
    }
    
    

    public PacketWriter Write(string key, object value)
    {
        this.Add(key, value);
        return this;
    }
    public PacketWriter SetType(string type)
    {
        this.Add("type",type);
        return this;
    }
    public void Send()
    {
        var botPrefix = IsLiteMessage ? "CaiBotLite" : "CaiBot";
        try
        {
            if (IsLiteMessage)
            {
                if (this._groupOpenId != "")
                {
                    this.Add("group", this._groupOpenId);
                }
                if (this._msgId != "")
                {
                    this.Add("msg_id", this._msgId);
                }
            }
            else
            {
                if (this._groupId != 0)
                {
                    this.Add("group", this._groupId);
                }
                if (this._at != 0L)
                {
                    this.Add("at", this._at);
                }
            }
            var message = JsonConvert.SerializeObject(this, Formatting.None);
            if (Debug)
            {
                TShock.Log.ConsoleInfo($"[{botPrefix}]发送BOT数据包：{message}");
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = WebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        catch (Exception e)
        {
            TShock.Log.ConsoleInfo($"[{botPrefix}]发送数据包时发生错误：{e}");
        }
    }
    
    public new object this[string key]
    {
        get => this.TryGetValue(key, out var obj);
        set
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
            }
            else
            {
                base[key] = value;
            }
        }
    }
}