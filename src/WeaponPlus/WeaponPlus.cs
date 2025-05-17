using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace WeaponPlus;

[ApiVersion(2, 1)]
public partial class WeaponPlus : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "z枳";
    public override string Description => GetString("允许在基础属性上强化任何武器, Allow any weapon to be strengthened on basic attributes");
    public override Version Version => new Version(1, 0, 0, 7);
    #endregion

    #region 实例变量
    public string configPath = Path.Combine(TShock.SavePath, "WeaponPlus.json");
    public static Config config = new Config();
    public static WPlayer[] wPlayers = new WPlayer[256];
    public static List<List<string>> LangTips = new List<List<string>>();
    public static WeaponPlusDB DB { get; set; } = null!;
    #endregion

    #region 注册卸载钩子
    public WeaponPlus(Main game) : base(game) { }
    public override void Initialize()
    {
        DB = new WeaponPlusDB(TShock.DB);
        this.NewLangTips();
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
        Commands.ChatCommands.Add(new Command("weaponplus.plus", this.PlusItem, "plus")
        {
            HelpText = LangTipsGet("输入 /plus    查看当前该武器的等级状态和升至下一级需要多少材料")
        });
        Commands.ChatCommands.Add(new Command("weaponplus.admin", this.ClearPlusItem, "clearallplayersplus")
        {
            HelpText = LangTipsGet("输入 /clearallplayersplus    将数据库中所有玩家的所有强化物品全部清理，管理员专属")
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LangTips.Clear();
            GeneralHooks.ReloadEvent -= LoadConfig;
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PlusItem || x.CommandDelegate == this.ClearPlusItem);
        }
    }
    #endregion

    #region 配置文件创建与重读加载方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        config = Config.Read(Config.configPath);
        config.Write(Config.configPath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage("[武器强化]重新加载配置完毕。");
        }
    }
    #endregion

    #region 创建提示语
    public void NewLangTips()
    {
        LangTips.Add(new List<string> {
            "几乎所有的武器和弹药都能强化，但是强化结果会无效化词缀，作为补偿，前三次强化价格降低 80%",
            "Almost all weapons and ammunition can be strengthened, but the strengthening results will invalidate the affixes. As compensation, the price of the first three enhancements will be reduced by 80%" });
        LangTips.Add(new List<string> {
            "强化绑定一类武器，即同 ID 武器，而不是单独的一个物品。强化与人物绑定，不可分享，扔出即失效，只在背包，猪猪等个人私有库存内起效。",
            "Strengthen the binding of a type of weapon, that is, the same ID weapon, rather than a single item. Strengthen the binding with the character, which cannot be shared. Throw it out and it will become invalid. It only works in the private inventory of backpacks, piggy bank and other individuals." });
        LangTips.Add(new List<string> {
            "当你不小心扔出或其他原因导致强化无效，请使用指令 /plus load 来重新获取。每次重新获取都会从当前背包中查找并强制拿出来重给，请注意捡取避免丢失。",
            "When you throw it out carelessly or the reinforcement is invalid for other reasons, please use the command </plus load> to retrieve it again. Each time you retrieve it, you will find it from the current backpack and force it to be taken out again. Please pay attention to picking up to avoid loss." });
        LangTips.Add(new List<string> {
            "重新获取时重给的物品是单独给予，不会被其他玩家捡走，每次进入服务器时会默认强制重新获取。",
            "The items to be re-acquired are given separately and will not be picked up by other players. Each time you enter the server, you will be forced to re-acquire by default." });
        LangTips.Add(new List<string> {
            "第一个物品栏是强化栏，指令只对该物品栏内的物品起效，强化完即可将武器拿走换至其他栏位，功能类似于哥布林的重铸槽。",
            "The first item column is the reinforcement column. The command only works on the items in this item column. After the reinforcement, the weapon can be taken away and replaced to another column. The function is similar to the recasting slot of Goblin." });
        LangTips.Add(new List<string> {
            "输入 /plus    查看当前该武器的等级状态和升至下一级需要多少材料",
            "Enter /plus     --to view the current level status of the weapon and how many materials are needed to upgrade to the next level" });
        LangTips.Add(new List<string> {
            "输入 /plus help    查看 plus 系列指令帮助",
            "Enter /plus help     --to view the help of the plus series of instructions" });
        LangTips.Add(new List<string> {
            "输入 /plus load    将当前身上所有已升级的武器重新获取",
            "Enter /plus load     --to reacquire all upgraded weapons on the current inventory" });
        LangTips.Add(new List<string> {
            "输入 /plus [damage/da/伤害] [up/down] [num]   升级/降级当前武器的伤害等级",
            "Enter /plus [damage/da] [up/down] [num]    --to upgrade/downgrade the damage level of the current weapon" });
        LangTips.Add(new List<string> {
            "输入 /plus [scale/sc/大小] [up/down] [num]  升级/降级当前武器或射弹的体积等级 ±5%",
            "Enter /plus [scale/sc] [up/down] [num]    --to upgrade/downgrade the volume level of the current weapon or projectile by ± 5%" });
        LangTips.Add(new List<string> {
            "输入 /plus [knockback/kn/击退] [up/down] [num]   升级/降级当前武器的击退等级 ±5%",
            "Enter /plus [knockback/kn] [up/down] [num]    --to upgrade/downgrade the knockback level of the current weapon by ± 5%" });
        LangTips.Add(new List<string> {
            "输入 /plus [usespeed/us/用速] [up/down] [num]   升级/降级当前武器的使用速度等级",
            "Enter /plus [usespeed/us] [up/down] [num]    --to upgrade/downgrade the speed level of the current weapon" });
        LangTips.Add(new List<string> {
            "输入 /plus [shootspeed/sh/飞速] [up/down] [num]   升级/降级当前武器的射弹飞行速度等级，影响鞭类武器范围±5%",
            "Enter /plus [shootspeed/sh] [up/down] [num]    --to upgrade/downgrade the projectile flying speed level of the current weapon, affecting the range of whip weapons by ± 5%" });
        LangTips.Add(new List<string> {
            "输入 /plus clear    清理当前武器的所有等级，可以回收一点消耗物",
            "Enter /plus clear     --to clear all levels of the current weapon, and you can recycle some consumables" });
        LangTips.Add(new List<string> {
            "输入 /clearallplayersplus    将数据库中所有玩家的所有强化物品全部清理，管理员专属",
            "Enter /clearallplayersplus     --to clear all enhancement items of all players in the database, exclusive to the administrator" });
        LangTips.Add(new List<string> {
            "该指令必须在游戏内使用",
            "This command must be used in the game" });
        LangTips.Add(new List<string> {
            "请在第一个物品栏内放入武器而不是其他什么东西或空",
            "Please put weapons in the first item column instead of anything else or empty" });
        LangTips.Add(new List<string> { "当前物品：", "Current item: " });
        LangTips.Add(new List<string> {
            "您当前的升级武器已重新读取",
            "Your current upgraded weapon has been re-read" });
        LangTips.Add(new List<string> {
            "当前武器没有任何等级，不用回炉重做",
            "The current weapon has no level, so you don't need to redo it" });
        LangTips.Add(new List<string> {
            "完全重置成功！钱币回收：",
            "Complete reset succeeded! Coin recovery: " });
        LangTips.Add(new List<string> { "升级成功", "Upgrade succeeded" });
        LangTips.Add(new List<string> { "共计消耗：", "Total consumption: " });
        LangTips.Add(new List<string> { "降级成功", "Degraded successfully" });
        LangTips.Add(new List<string> { "等级过低", "The grade is too low" });
        LangTips.Add(new List<string> {
            "当前该类型升级已达到上限，无法升级",
            "Currently, the upgrade of this type has reached the upper limit and cannot be upgraded" });
        LangTips.Add(new List<string> { "扣除钱币：", "Deduct coins: " });
        LangTips.Add(new List<string> { "当前剩余：", "Current remaining: " });
        LangTips.Add(new List<string> { "钱币不够！", "Not enough money!" });
        LangTips.Add(new List<string> {
            "所有玩家的所有强化数据全部清理成功！",
            "All enhancement data of all players have been cleared successfully!" });
        LangTips.Add(new List<string> { "强化数据清理失败！！!", "Enhanced data cleaning failed!!!" });
        LangTips.Add(new List<string> { "当前总等级：", "Current total level: " });
        LangTips.Add(new List<string> { "剩余强化次数：", "How many times can the weapon be strengthened: " });
        LangTips.Add(new List<string> { "次", "times" });
        LangTips.Add(new List<string> { "伤害等级：", "damage level: " });
        LangTips.Add(new List<string> { "大小等级：", "scale level: " });
        LangTips.Add(new List<string> { "击退等级：", "knockback level: " });
        LangTips.Add(new List<string> { "攻速等级：", "use time level: " });
        LangTips.Add(new List<string> { "射弹飞行速度等级：", "projectile speed level: " });
        LangTips.Add(new List<string> { "未升级过，无任何加成", "Not upgraded, no bonus" });
        LangTips.Add(new List<string> { "当前状态：", "Current status: " });
        LangTips.Add(new List<string> { "伤害", "damage" });
        LangTips.Add(new List<string> { "大小", "scale" });
        LangTips.Add(new List<string> { "击退", "knockback" });
        LangTips.Add(new List<string> { "攻速", "use time" });
        LangTips.Add(new List<string> { "射弹飞速", "projectile speed" });
        LangTips.Add(new List<string> { "伤害升至下一级需：", "Damage to the next level requires: " });
        LangTips.Add(new List<string> { "大小升至下一级需：", "Scale to the next level requires: " });
        LangTips.Add(new List<string> { "击退升至下一级需：", "KnockBack to the next level requires: " });
        LangTips.Add(new List<string> { "攻速升至下一级需：", "UseTime to the next level requires: " });
        LangTips.Add(new List<string> { "射弹飞速升至下一级需：", "Proiectile speed to the next level requires: " });
        LangTips.Add(new List<string> { "当前已满级", "The current level is full" });
        LangTips.Add(new List<string> { "已达到最大武器总等级", "The maximum total weapon level has been reached" });
        LangTips.Add(new List<string> { "SSC 未开启", "SSC is disable" });
        LangTips.Add(new List<string> { "请输入正整数", "Please enter a positive integer" });
    }
    #endregion


    #region 切换提示语语言方法
    public static string LangTipsGet(string str)
    {
        foreach (var langTip in LangTips)
        {
            if (langTip.Contains(str))
            {
                return config.EnableEnglish ? langTip[1] : langTip[0];
            }
        }
        return string.Empty;
    }
}
#endregion