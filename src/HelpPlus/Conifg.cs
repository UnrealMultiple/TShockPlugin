using Newtonsoft.Json;

namespace UserCheck;

public class Config
{
    private const string ConfigPath = "tshock/HelpPlus.json";

    public static Config Settings = new();
    
    [JsonProperty("每页行数")] 
    public int PageSize = 30;

    [JsonProperty("每行字数")] 
    public int WithSize = 120;
    
    [JsonProperty("按照字母顺序")] 
    public bool OrderByLetter;
    
    [JsonProperty("简短提示开关")] public bool DisPlayShort = true;
    
    [JsonProperty("简短提示对应")]
    public Dictionary<string, string> ShortCommands;
    public Config()
    {
        this.ShortCommands = new Dictionary<string, string>
        {
            { "user", "用户管理" },
            { "login", "登录" },
            { "logout", "登出" },
            { "password", "修改密码" },
            { "register", "注册" },
            { "accountinfo", "账号信息" },
            { "ban", "封禁" },
            { "broadcast", "广播" },
            { "displaylogs", "显示日志" },
            { "group", "组管理" },
            { "itemban", "物品封禁" },
            { "projban", "弹幕封禁" },
            { "tileban", "图格封禁" },
            { "su", "临时超管" },
            { "sudo", "以超管权限运行" },
            { "userinfo", "玩家信息" },
            { "region", "区域管理" },
            { "kick", "踢" },
            { "mute", "禁言" },
            { "overridessc", "覆盖SSC" },
            { "savessc", "保存SSC" },
            { "uploadssc", "上传SSC" },
            { "tempgroup", "临时组" },
            { "annoy", "打扰" },
            { "rocket", "上天" },
            { "firework", "烟火" },
            { "checkupdates", "检查更新" },
            { "off", "关服并保存" },
            { "off-nosave", "关服不保存" },
            { "reload", "重载配置" },
            { "serverpassword", "服务器密码" },
            { "version", "版本" },
            { "whitelist", "白名单" },
            { "give", "给物品" },
            { "item", "给自己物品" },
            { "butcher", "击杀生物" },
            { "renamenpc", "重命名城镇NPC" },
            { "maxspawns", "单人最大刷怪" },
            { "spawnboss", "召唤BOSS" },
            { "spawnmob", "召唤生物" },
            { "spawnrate", "刷怪间隔" },
            { "clearangler", "重置渔夫任务" },
            { "home", "回家" },
            { "spawn", "传送至世界出生点" },
            { "tp", "传送" },
            { "tphere", "传送玩家至身边" },
            { "tpnpc", "传送至生物" },
            { "tppos", "传送至坐标" },
            { "pos", "显示坐标" },
            { "tpallow", "传送保护" },
            { "worldmode", "修改世界难度" },
            { "antibuild", "建筑保护" },
            { "grow", "种植" },
            { "forcehalloween", "强制万圣节" },
            { "forcexmas", "强制圣诞节" },
            { "worldevent", "事件" },
            { "hardmode", "切换困难模式" },
            { "protectspawn", "出生点保护" },
            { "save", "保存世界" },
            { "setspawn", "设置世界出生点" },
            { "setdungeon", "设置地牢" },
            { "settle", "平衡液体" },
            { "time", "时间" },
            { "wind", "风速" },
            { "worldinfo", "地图信息" },
            { "buff", "给自己Buff" },
            { "clear", "清理" },
            { "gbuff", "给BUFF" },
            { "godmode", "上帝模式" },
            { "heal", "治疗" },
            { "kill", "杀死" },
            { "me", "发送消息" },
            { "party", "队内消息" },
            { "reply", "回复私聊" },
            { "rest", "REST管理" },
            { "slap", "伤害" },
            { "serverinfo", "服务器信息" },
            { "warp", "传送点" },
            { "whisper", "私聊" },
            { "wallow", "私聊保护" },
            { "dump-reference-data", "生成帮助文档" },
            { "sync", "同步" },
            { "respawn", "复活" },
            { "aliases", "命令别名" },
            { "help", "帮助" },
            { "motd", "进服提示" },
            { "playing", "在线人数" },
            { "rules", "规则" },

            { "分界线", "下面是Cai自己服务器的命令" },

            // { "
            // ", "重置" },
            { "rs", "重置设置" },
            { "ghost", "幽灵模式" },
            { "maxplayers", "最大玩家数" },
            { "runas", "以某用户执行" },
            { "exportcharacter", "导出角色" },
            { "whynot", "权限查询" },
            { "setlang", "设置语言" },
            { "resetcharacte", "重置玩家角色" },
            { "searchitem", "查找物品" },
            { "searchchest", "查找箱子" },
            { "tpchest", "传送至箱子" },
            { "chestinfo", "箱子信息" },
            { "tpall", "传送至所有人" },
            { "tpallchest", "传送至所有箱子" },
            { "removechestitem", "移除箱子物品" },
            { "removeitem", "移除玩家物品" },
            { "key", "钥匙兑换" },
            { "load", "插件热重载" },
            { "statustext", "计分板开关" },
            { "veinminer", "连锁挖矿" },
            { "replen", "资源补充" },
            { "replenreload", "重载资源补充" },
            { "lookbag", "查背包" },
            { "playermanager", "玩家管理" },
            { "vote", "投票" },
            { "worldmodify", "地图修改" },
            { "moonphase", "月相" },
            { "moonstyle", "月亮样式" },
            { "bossmanage", "BOSS管理" },
            { "npcmanage", "NPC管理" },
            { "creativemode", "创造模式" },
            { "generatemap", "生成地图图片" },
            { "history", "图格历史" },
            { "prunehist", "还原图格" },
            { "reenact", "撤销" },
            { "rollback", "还原玩家修改" },
            { "tpahere", "请求传送玩家至身边" },
            { "tpa", "请求传送" },
            { "back", "回到死亡点" },
            { "permabuff", "永久BUFF" },
            { "buffcheck", "列出永久BUFF" },
            { "gpermabuff", "给永久BUFF" },
            { "regionbuff", "区域BUFF" },
            { "globalbuff", "全局BUFF" },
            { "clearbuffs", "移除永久BUFF" },
            { "scommand", "查命令" },
            { "sperm", "查权限" },
            { "sgcommand", "查命令可用组" },
            { "pluginlist", "查非TShock权限" },
            { "invsee", "修改背包" },
            { "house", "圈地" },
            { "分界线2", "常用插件" },
            { "query", "查询货币" },
            { "es", "倍率设置" },
            { "bank", "银行" },
            { "level", "等级设置" },
            { "rank", "升级" },
            { "reset", "重置职业" },
            { "skill", "技能" },
            { "task", "任务" },
            { "pco", "计划书" },
            { "plus", "强化武器" },
            { "clearallplayersplus", "重置强化武器" },
            { "zhelp", "ZPM帮助" },
            { "zsave", "备份存档" },
            { "zvisa", "查看存档" },
            { "zsaveauto", "自动备份" },
            { "zback", "回档" },
            { "zclone", "克隆存档" },
            { "zmodify", "修改玩家" },
            { "zfre", "冻结玩家" },
            { "zunfre", "解冻玩家" },
            { "zresetdb", "删除玩家备份" },
            { "zresetex", "重置额外数据" },
            { "zreset", "重置角色" },
            { "zresetallplayers", "重置所有角色" },
            { "vi", "查背包(排序)" },
            { "vid", "查背包" },
            { "vs", "查状态" },
            { "zsort", "排行" },
            { "zban", "封禁" },
            { "zhide", "隐藏弹字" },
            { "zclear", "清理" },
            { "shop", "商店" },
            { "deal", "交易" },
            { "igen", "快速构建" },
            { "relive", "复活NPC" },
            { "bossinfo", "进度查询" },
            { "zout", "导出存档" }
        };
    }

    /// <summary>
    /// 将配置文件写入硬盘
    /// </summary>
    internal void Write()
    {
        using FileStream fileStream = new (ConfigPath, FileMode.Create, FileAccess.Write, FileShare.Write);
        using StreamWriter streamWriter = new (fileStream);
        streamWriter.Write(JsonConvert.SerializeObject(this, JsonSettings));
    }

    /// <summary>
    /// 从硬盘读取配置文件
    /// </summary>
    internal static void Read()
    {
        Config result;
        if (!File.Exists(ConfigPath))
        {
            result = new Config();
            result.Write();
        }
        else
        {
            using FileStream fileStream = new (ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new (fileStream);
            result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd(), JsonSettings)!;
        }

        Settings = result;
    }

    private static readonly JsonSerializerSettings JsonSettings = new () { Formatting = Formatting.Indented, ObjectCreationHandling = ObjectCreationHandling.Replace };
}