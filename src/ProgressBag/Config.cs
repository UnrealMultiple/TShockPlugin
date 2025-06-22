using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using LazyAPI.Utility;

namespace ProgressBag;

public class Award
{
    public int netID = 22;

    public int prefix = 0;

    public int stack = 99;
}

public class Bag
{
    [LocalizedPropertyName(CultureType.Chinese, "礼包名称")]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public string Name { get; set; } = "新手礼包";

    [LocalizedPropertyName(CultureType.Chinese, "进度限制")]
    [LocalizedPropertyName(CultureType.English, "ProgressLimit")]
    public List<string> Limit { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "礼包奖励")]
    [LocalizedPropertyName(CultureType.English, "Award")]
    public List<Award> Award { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "执行命令")]
    [LocalizedPropertyName(CultureType.English, "Commands")]
    public List<string> Command { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "已领取玩家")]
    [LocalizedPropertyName(CultureType.English, "ReceivePlayers")]
    public List<string> Receive { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "可领取组")]
    [LocalizedPropertyName(CultureType.English, "Groups")]
    public List<string> Group { get; set; } = new();
}

[Config]
public class Config : JsonConfigBase<Config>
{
    [LocalizedPropertyName(CultureType.Chinese, "礼包")]
    [LocalizedPropertyName(CultureType.English, "Bags")]
    public List<Bag> Bag = new();

    protected override void SetDefault()
    {
        foreach (var (name, _) in GameProgress.DefaultProgressNames)
        {
            Bag bag = new();
            bag.Limit.Add(name);
            bag.Name = name + "礼包";
            bag.Award.Add(new Award());
            this.Bag.Add(bag);
        }
    }

    public void Reset()
    {
        foreach (var bag in this.Bag)
        {
            bag.Receive.Clear();
        }
        this.SaveTo();
    }
}