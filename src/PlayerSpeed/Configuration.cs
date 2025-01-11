using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace PlayerSpeed;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    #region 实例变量
    [LocalizedPropertyName(CultureType.Chinese, "开关", Order = 0)]
    [LocalizedPropertyName(CultureType.English, "Enabled")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "次数", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "Count")]
    public int Count { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "间隔", Order = 2)]
    [LocalizedPropertyName(CultureType.English, "Range")]
    public double Range { get; set; } = 2000;

    [LocalizedPropertyName(CultureType.Chinese, "冷却", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "CoolTime")]
    public int CoolTime { get; set; } = 25;

    [LocalizedPropertyName(CultureType.Chinese, "速度", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "Speed")]
    public float Speed { get; set; } = 20.0f;

    [LocalizedPropertyName(CultureType.Chinese, "高度", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "Height")]
    public float Height { get; set; } = 5.0f;

    [LocalizedPropertyName(CultureType.Chinese, "播报", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "Mess")]
    public bool Mess { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "冲刺", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "Dash")]
    public bool Dash { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "冲刺速度倍数", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "DashMultiple")]
    public float DashMultiple { get; set; } = 1.5f;

    [LocalizedPropertyName(CultureType.Chinese, "跳跃", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "Jump")]
    public bool Jump { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "跳跃下降除于倍数", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "JumpMultiple")]
    public float JumpMultiple { get; set; } = 5.0f;

    [LocalizedPropertyName(CultureType.Chinese, "跳跃加速物品", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "JumpHasArmorItem")]
    public List<int>? ArmorItem { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "自动进度", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "KilledBoss")]
    public bool KilledBoss { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "自动进度表", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "BossList")]
    public List<BossData> BossList { get; set; } = new List<BossData>();
    #endregion

    #region 进度表结构
    public class BossData
    {
        [LocalizedPropertyName(CultureType.Chinese, "怪物名称", Order = -12)]
        [LocalizedPropertyName(CultureType.English, "Name")]
        public string Name { get; set; } = "";

        [LocalizedPropertyName(CultureType.Chinese, "击败状态", Order = -11)]
        [LocalizedPropertyName(CultureType.English, "Enabled")]
        public bool Enabled { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "设置速度", Order = -10)]
        [LocalizedPropertyName(CultureType.English, "Speed")]
        public float Speed { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "设置高度", Order = -9)]
        [LocalizedPropertyName(CultureType.English, "Height")]
        public float Height { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "使用次数", Order = -8)]
        [LocalizedPropertyName(CultureType.English, "Count")]
        public int Count { get; set; } = 5;

        [LocalizedPropertyName(CultureType.Chinese, "冷却时间", Order = -7)]
        [LocalizedPropertyName(CultureType.English, "CoolTime")]
        public int CoolTime { get; set; } = 25;

        [LocalizedPropertyName(CultureType.Chinese, "怪物ID", Order = -6)]
        [LocalizedPropertyName(CultureType.English, "NPCID")]
        public int[] ID { get; set; }

        public BossData(string name, bool enabled, int count, int coolTime, float speed, float height, int[] id)
        {
            this.Name = name ?? "";
            this.Enabled = enabled;
            this.Speed = speed;
            this.Height = height;
            this.Count = count;
            this.CoolTime = coolTime;
            this.ID = id ?? new int[] { 1 };
        }
    }
    #endregion

    #region 预设参数方法
    protected override string Filename => "PlayerSpeed";
    protected override void SetDefault()
    {
        this.ArmorItem = new List<int>() { 5107, 4989 };

        this.BossList = new List<BossData>
        {
            new BossData("",false,1,60,20f,2.5f,new int []{ 4,50 }),
            new BossData("",false,2,45,25f,5f,new int []{ 13,266 }),
            new BossData("",false,3,30,30f,10f,new int []{ 113 }),
            new BossData("",false,4,15,40f,15f, new int[] { 125, 126, 127, 134 })
        };
    }
    #endregion

}