using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace AutoBroadcast;
public class Broadcast
{
    private int _interval;
    private readonly List<int> _delays = new();
    
    public Broadcast()
    {
        this._interval = this.Interval;
    }
    
    [LocalizedPropertyName(CultureType.Chinese, "广播名称")]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public string Name  = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "启用")] 
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled;

    [LocalizedPropertyName(CultureType.Chinese, "广播消息")]
    [LocalizedPropertyName(CultureType.English, "Msg")]
    public string[] Messages = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "RGB颜色")]
    [LocalizedPropertyName(CultureType.English, "ColorRGB")]
    public int[] ColorRgb = new int[3];

    [JsonIgnore]
    public byte[] ColorRgbBytes => this.ColorRgb.Select(i => i > 255 ? (byte)255 : (byte)i).ToArray();


    [LocalizedPropertyName(CultureType.Chinese, "时间间隔")] 
    [LocalizedPropertyName(CultureType.English, "Interval")]
    public int Interval;

    [LocalizedPropertyName(CultureType.Chinese, "延迟执行")]
    [LocalizedPropertyName(CultureType.English, "Delay")]
    public int StartDelay;

    [LocalizedPropertyName(CultureType.Chinese, "广播组")]
    [LocalizedPropertyName(CultureType.English, "Groups")]
    public string[] Groups = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发词语")]
    [LocalizedPropertyName(CultureType.English, "TriggerWords")]
    public string[] TriggerWords  = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发整个组")] 
    [LocalizedPropertyName(CultureType.English, "TriggerToWholeGroup")]
    public bool TriggerToWholeGroup;
    
    /// <summary>
    /// 触发关键词的广播
    /// </summary>
    public void RunTriggerWords(TSPlayer plr)
    {
        
        if (this.Groups.Length > 0)
        {
            if (this.Groups.Contains(plr.Group.Name))
            {
                return;
            }

            if (this.TriggerToWholeGroup)
            {
                Utils.BroadcastToGroups(this.Groups, this.Messages, this.ColorRgbBytes);
            }
            else
            {
                Utils.BroadcastToPlayer(plr, this.Messages, this.ColorRgbBytes);
            }

        }
        else
        {
            if (this.TriggerToWholeGroup)
            {
                Utils.BroadcastToGroups(this.Groups, this.Messages, this.ColorRgbBytes);
            }
            else
            {
                Utils.BroadcastToPlayer(plr, this.Messages, this.ColorRgbBytes);
            }
        }
        
    }
    
    /// <summary>
    /// 直接广播
    /// </summary>
    public void Run()
    {
        if (this.Groups.Length > 0)
        {
            Utils.BroadcastToGroups(this.Groups, this.Messages, this.ColorRgbBytes);
        }
        else
        {
            Utils.BroadcastToAll(this.Messages, this.ColorRgbBytes);
        }
    }
    /// <summary>
    /// 延迟广播
    /// </summary>
    public void RunDelay()
    {
        if (this.StartDelay <= 0)
        {
            this.Run();
        }
        else
        {
            this._delays.Add(this.StartDelay);
        }
    }

    /// <summary>
    /// 更新广播计时器
    /// </summary>
    public void SecondUpdate()
    {
        
        for (var i = 0; i < this._delays.Count; i++)
        {
            this._delays[i]--;
            if (this._delays[i] <= 0)
            {
                this.Run();
            }
        }
        
        this._delays.RemoveAll(x=>x <= 0);
        
        this._interval--;
        if (this._interval <= 0)
        {
            this.RunDelay();
            this._interval = this.Interval;
        }
        
    }

}