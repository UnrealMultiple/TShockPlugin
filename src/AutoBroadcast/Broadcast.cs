using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using System.Text.Json;
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
    
    public void RunTriggerWords(TSPlayer plr)
    {
        if (this.TriggerToWholeGroup)
        {
            Utils.BroadcastToGroups(this.Groups, this.Messages, this.ColorRGB);
        }
        else
        {
            Utils.BroadcastToPlayer(plr, this.Messages, this.ColorRGB);
        }
    }
    
    public void Run()
    {
        if (this.Groups.Length > 0)
        {
            Utils.BroadcastToGroups(this.Groups, this.Messages, this.ColorRGB);
        }
        else
        {
            Utils.BroadcastToAll(this.Messages, this.ColorRGB);
        }
    }

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
    /// <returns>如果计时器已经跑完, 就开始</returns>
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

    [LocalizedPropertyName(CultureType.Chinese, "广播名称")]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public string Name { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播消息")]
    [LocalizedPropertyName(CultureType.English, "Msg")]
    public string[] Messages { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "RGB颜色")]
    [LocalizedPropertyName(CultureType.English, "Color")]
    public byte[] ColorRGB { get; set; } = new byte[3];

    [LocalizedPropertyName(CultureType.Chinese, "时间间隔")]
    [LocalizedPropertyName(CultureType.English, "Interval")]
    public int Interval { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "延迟执行")]
    [LocalizedPropertyName(CultureType.English, "Delay")]
    public int StartDelay { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "广播组")]
    [LocalizedPropertyName(CultureType.English, "Groups")]
    public string[] Groups { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发词语")]
    [LocalizedPropertyName(CultureType.English, "TriggerWords")]
    public string[] TriggerWords { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发整个组")]
    [LocalizedPropertyName(CultureType.English, "TriggerToWholeGroup")]
    public bool TriggerToWholeGroup { get; set; } = false;
}