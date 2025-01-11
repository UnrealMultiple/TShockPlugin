using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Microsoft.Xna.Framework;

namespace ModifyWeapons;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    #region 实例变量
    protected override string Filename => "ModifyWeapons";
    [LocalizedPropertyName(CultureType.Chinese, "插件开关", Order = 0)]
    [LocalizedPropertyName(CultureType.English, "Enabled")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "初始重读次数", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "ReadCount")]
    public int ReadCount { get; set; } = 2;

    [LocalizedPropertyName(CultureType.Chinese, "只给指定名字物品", Order = 2)]
    [LocalizedPropertyName(CultureType.English, "OnlyGiveItemName")]
    public bool OnlyGiveItemName { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "给完物品的延迟指令", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "Alone")]
    public bool Alone { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "延迟指令毫秒", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "AloneTimer")]
    public float AloneTimer { get; set; } = 500.0f;

    [LocalizedPropertyName(CultureType.Chinese, "延迟指令表", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "AloneList")]
    public HashSet<string> AloneList { get; set; } = new HashSet<string>();

    [LocalizedPropertyName(CultureType.Chinese, "自动重读", Order = 6)]
    [LocalizedPropertyName(CultureType.English, "Auto")]
    public int Auto { get; set; } = 1;

    [LocalizedPropertyName(CultureType.Chinese, "触发重读指令检测表", Order = 7)]
    [LocalizedPropertyName(CultureType.English, "Text")]
    public HashSet<string> Text { get; set; } = new HashSet<string>();

    [LocalizedPropertyName(CultureType.Chinese, "清理修改武器(丢出或放箱子会消失)", Order = 8)]
    [LocalizedPropertyName(CultureType.English, "ClearItem")]
    public bool ClearItem = true;

    [LocalizedPropertyName(CultureType.Chinese, "免清表", Order = 9)]
    [LocalizedPropertyName(CultureType.English, "ExemptItems")]
    public int[] ExemptItems { get; set; } = new int[] { 1 };

    [LocalizedPropertyName(CultureType.Chinese, "进服只给管理建数据", Order = 10)]
    [LocalizedPropertyName(CultureType.English, "OnlyAdminCreateData")]
    public bool OnlyAdminCreateData { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "增加重读次数的冷却秒数", Order = 11)]
    [LocalizedPropertyName(CultureType.English, "ReadTime")]
    public float ReadTime { get; set; } = 1800;

    [LocalizedPropertyName(CultureType.Chinese, "启用公用武器", Order = 12)]
    [LocalizedPropertyName(CultureType.English, "PublicWeapons")]
    public bool PublicWeapons { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "同步数据秒数", Order = 13)]
    [LocalizedPropertyName(CultureType.English, "SyncTime")]
    public int SyncTime { get; set; } = 15;

    [LocalizedPropertyName(CultureType.Chinese, "公用武器播报标题", Order = 14)]
    [LocalizedPropertyName(CultureType.English, "Title")]
    public string Title { get; set; } = "羽学开荒服 ";

    [LocalizedPropertyName(CultureType.Chinese, "公用武器表", Order = 15)]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public List<ItemData>? ItemDatas { get; set; }
    #endregion

    #region 预设参数方法
    protected override void SetDefault()
    {
        this.Text = new HashSet<string> { "deal", "shop", "fishshop", "fs" };
        this.AloneList = new HashSet<string> { "/mw read" };
        this.ItemDatas = new List<ItemData>()
        {
            new ItemData("",96,1,82,30,1,5.5f,10,15,10,9,0,97,default),
            new ItemData("",800,1,82,35,1,2.5f,10,15,5,6,0,97,default),
        };
    }
    #endregion

    #region 公用武器数据结构
    public class ItemData
    {
        [LocalizedPropertyName(CultureType.Chinese, "名称")]
        [LocalizedPropertyName(CultureType.English, "Name")]
        public string Name { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "物品ID")]
        [LocalizedPropertyName(CultureType.English, "ItemID")]
        public int type { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "物品数量")]
        [LocalizedPropertyName(CultureType.English, "Stack")]
        public int stack { get; set; }


        [LocalizedPropertyName(CultureType.Chinese, "物品前缀")]
        [LocalizedPropertyName(CultureType.English, "Prefix")]
        public byte prefix { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "伤害")]
        [LocalizedPropertyName(CultureType.English, "damage")]
        public int damage { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "大小")]
        [LocalizedPropertyName(CultureType.English, "scale")]
        public float scale { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "击退")]
        [LocalizedPropertyName(CultureType.English, "knockBack")]
        public float knockBack { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "用速")]
        [LocalizedPropertyName(CultureType.English, "useTime")]
        public int useTime { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "攻速")]
        [LocalizedPropertyName(CultureType.English, "useAnimation")]
        public int useAnimation { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "弹幕")]
        [LocalizedPropertyName(CultureType.English, "shoot")]
        public int shoot { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "弹速")]
        [LocalizedPropertyName(CultureType.English, "shootSpeed")]
        public float shootSpeed { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "弹药")]
        [LocalizedPropertyName(CultureType.English, "ammo")]
        public int ammo { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "发射器")]
        [LocalizedPropertyName(CultureType.English, "useAmmo")]
        public int useAmmo { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "颜色")]
        [LocalizedPropertyName(CultureType.English, "color")]
        public Color color { get; set; }

        public ItemData(string name, int type, int stack, byte prefix, int damage, float scale, float knockBack,
            int useTime, int useAnimation, int shoot, float shootSpeed, int ammo, int useAmmo, Color color = default)
        {
            this.Name = name ?? "";
            this.type = type;
            this.stack = stack;
            this.prefix = prefix;
            this.damage = damage;
            this.scale = scale;
            this.knockBack = knockBack;
            this.useTime = useTime;
            this.useAnimation = useAnimation;
            this.shoot = shoot;
            this.shootSpeed = shootSpeed;
            this.ammo = ammo;
            this.useAmmo = useAmmo;
            this.color = color;
        }
    }
    #endregion



}