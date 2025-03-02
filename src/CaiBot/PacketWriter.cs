using System.Text.Encodings.Web;
using System.Text.Json;
using TShockAPI;

namespace CaiBot;

[Serializable]
public class PacketWriter : Dictionary<string, object>
{
    private readonly long _groupId;
    private readonly string _groupOpenId;
    private readonly bool _isLiteMessage;
    private readonly string _msgId;
    private readonly long _at;
    
    
    public PacketWriter(bool isLiteMessage)
    {
        this._isLiteMessage = isLiteMessage;
        this._groupId = 0L;
        this._at = 0L;
        this._groupOpenId = "";
        this._msgId = "";
    }
    
    public PacketWriter(long groupId,long at = 0L)
    {
        this._isLiteMessage = false;
        this._groupId = groupId;
        this._at = at;
        this._groupOpenId = "";
        this._msgId = "";
    }
    
    public PacketWriter(string groupOpenId, string msgId)
    {
        this._isLiteMessage = true;
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
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
        try
        {
            if (this._isLiteMessage)
            {
                if (this._groupOpenId != "")
                {
                    this.Add("group", this._groupId);
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
            
            _ = CaiBotApi.SendDateAsync(JsonSerializer.Serialize(this,options));
        }
        catch (Exception e)
        {
            TShock.Log.ConsoleInfo($"[CaiAPI]发送数据包时发生错误：{e}");
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