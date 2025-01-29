using Newtonsoft.Json;
using System.Collections;
using Terraria;
using TShockAPI;


namespace ProgressControl;

public class Config
{
    public static string configPath = Path.Combine(TShock.SavePath, "ProgressControl.json");


    #region 写入配置格式方法
    public static Config LoadConfigFile()
    {
        if (!Directory.Exists(TShock.SavePath))
        {
            Directory.CreateDirectory(TShock.SavePath);
        }
        if (!File.Exists(configPath))
        {
            var NewConfig = new Config
            {
                NpcKillCountForAutoReset = new Dictionary<int, ArrayList>()
           { { 398, new ArrayList { 3, "pco reset hand","bc 击杀月总3次自动重置" } } },
                CommandForBeforeResetting = new HashSet<string>()
            {
            "clall",
            "zout all",
            "wat clearall",
            "pbreload",
            "pco copy",
            "礼包 重置",
            "礼包重置",
            "pvp reset",
            "派系 reset",
            "bwl reload",
            "task clear",
            "task reset",
            "rpg reset",
            "bank reset",
            "deal reset",
            "skill reset",
            "level reset",
            "replenreload",
            "重读多重限制",
            "重读阶段库存",
            "clearbuffs all",
            "重读物品超数量封禁",
            "重读自定义怪物血量",
            "重读禁止召唤怪物表",
            "zresetallplayers",
            "clearallplayersplus",
            "reload"
            },
                DeleteSQLiteForBeforeResetting = new HashSet<string>()
            {
            "HousingDistrict",
            "TerrariaRobot死亡统计",
            "Warps",
            "渡劫表",
            "RememberedPos",
            "Zhipm_PlayerBackUp",
            "Zhipm_PlayerExtra",
            "Research",
            "使用日志",
            "区域执行指令",
            "Economics",
            "Economicsskill",
            "Regions",
            "RPG",
            "Skill",
            "Permabuff",
            "Permabuffs",
            "Onlineduration",
            "Onlybaniplist",
            "Stronger",
            "Synctable",
            "Task",
            "TaskKillnpc",
            "TaskTallk",
            "OnlineDuration",
            "WeaponPlusDBcostCoin",
            "WeaponPlusdbbasedOnEconomics"
            },
                DeleteFileForBeforeResetting = new HashSet<string>(){
            "tshock/backups/*.bak",
            "tshock/logs/*.log",
            "tshock/Watcher/logs/*.log",
            "tshock/检查背包/检查日志/*.txt",},
                ProgressLockTimeForStartServerDate = new Dictionary<string, double>(){
            {"史莱姆王", 0},
            {"克苏鲁之眼", 0},
            {"世界吞噬者" ,24},
            {"克苏鲁之脑" ,24},
            {"蜂后" ,42},
            {"巨鹿" ,36},
            {"骷髅王" ,48},
            {"血肉墙" ,72},
            {"史莱姆皇后" ,84},
            {"双子魔眼" ,96},
            {"毁灭者" ,102},
            {"机械骷髅王" ,108},
            {"猪龙鱼公爵" ,120},
            {"世纪之花" ,132},
            {"光之女皇" ,138},
            {"石巨人" ,150},
            {"拜月教教徒" ,162},
            {"四柱" ,164},
            {"月亮领主" ,170}},
                Command_PcoCopy_CopyPath = new HashSet<string>() { "world/SFE4.wld" }
            };
            File.WriteAllText(configPath, JsonConvert.SerializeObject(NewConfig, Formatting.Indented));
        }
        Config config;
        try
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath))!;
        }
        catch
        {
            TSPlayer.All.SendWarningMessage(GetString("ProgressControl.json 反序列化失败，可能有填写错误或配置文件需要更新，已对配置文件作了更新处理"));
            Console.WriteLine(GetString("ProgressControl.json 反序列化失败，可能有填写错误或配置文件需要更新，已对配置文件作了更新处理"));
            config = PControl.config;
            PControl.config.SaveConfigFile();
        }
        return config;
    }
    #endregion

    /// <summary>
    /// 保存
    /// </summary>
    public void SaveConfigFile()
    {
        File.WriteAllText(configPath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    /// <summary>
    /// 将原版的tshock的config.json从内存中写入文件一次
    /// </summary>
    public static void SaveTConfig()
    {
        TShock.Config.Write(Path.Combine(TShock.SavePath, "config.json"));
    }

    public Config()
    {
        this.StartServerDate = DateTime.Now;
        this.OpenAutoReset = false;
        this.HowLongTimeOfAotuResetServer = 176;
        this.ResetTSCharacter = true;
        this.DeleteWorldForReset = true;
        this.NpcKillCountForAutoReset = new Dictionary<int, ArrayList>();
        this.CommandForBeforeResetting = new HashSet<string>();
        this.DeleteSQLiteForBeforeResetting = new HashSet<string>();
        this.DeleteFileForBeforeResetting = new HashSet<string>();
        this.MapSizeForAfterReset = 3;
        this.MapDifficultyForAfterReset = 2;
        this.WorldSeedForAfterReset = "";
        this.WorldNameForAfterReset = "SFE4";
        this.ExpectedUsageWorldFileNameForAotuReset = new HashSet<string>();
        this.AfterResetPeople = 32;
        this.AfterResetPort = "7777";
        this.AfterResetServerPassword = "";
        this.WorldPath = "./world/";

        this.LasetServerRestartDate = DateTime.Now;
        this.AutoRestartServer = false;
        this.HowLongTimeOfRestartServer = 0;
        this.AfterRestartPeople = 32;
        this.AfterRestartPort = "7777";
        this.AfterRestartServerPassword = "";
        this.CommandForBeforeRestart = new HashSet<string>();

        this.OpenAutoControlProgressLock = false;
        this.ProgressLockTimeForStartServerDate = new Dictionary<string, double>();
        this.CustomNPCIDLockTimeForStartServerDate = new Dictionary<int, double> { };

        this.LasetAutoCommandDate = DateTime.Now;
        this.OpenAutoCommand = false;
        this.HowLongTimeOfAutoCommand = 0;
        this.AutoCommandList = new HashSet<string> { };
        this.AutoCommandOfBroadcast = true;
        this.CheckPerm = true;
        this.ServerLogWriterEnabledForAotuResetting = false;
        this.Command_PcoDelFile_DeletePath = new HashSet<string>();
        this.Command_PcoCopy_CopyPath = new HashSet<string>();
        this.Command_PcoCopy_PastePath = "tshock/Zhipm/";
        this.Command_PcoCopy_CoverFile = true;
    }

    //重置计划"
    [JsonProperty("开服日期", Order = -15)]
    public DateTime StartServerDate;
    [JsonProperty("是否启用自动重置世界", Order = -15)]
    public bool OpenAutoReset;
    [JsonProperty("多少小时后开始自动重置世界", Order = -15)]
    public double HowLongTimeOfAotuResetServer;

    [JsonProperty("重置是否重置玩家数据", Order = -14)]
    public bool ResetTSCharacter;
    [JsonProperty("重置前是否删除地图", Order = -14)]
    public bool DeleteWorldForReset;
    [JsonProperty("NPC死亡次数触发执行指令", Order = -14)]
    public Dictionary<int, ArrayList> NpcKillCountForAutoReset;
    [JsonProperty("重置前执行的指令", Order = -14)]
    public HashSet<string> CommandForBeforeResetting;
    [JsonProperty("重置前删除哪些数据库表", Order = -14)]
    public HashSet<string> DeleteSQLiteForBeforeResetting;
    [JsonProperty("重置前删除哪些文件或文件夹", Order = -14)]
    public HashSet<string> DeleteFileForBeforeResetting;

    [JsonProperty("重置后的地图大小_小1_中2_大3", Order = -13)]
    public int MapSizeForAfterReset;
    [JsonProperty("重置后的地图难度_普通0_专家1_大师2_旅途3", Order = -13)]
    public int MapDifficultyForAfterReset;
    [JsonProperty("重置后的地图种子", Order = -13)]
    public string WorldSeedForAfterReset;
    [JsonProperty("重置后的地图名称", Order = -13)]
    public string WorldNameForAfterReset;
    [JsonProperty("你提供用于重置的地图名称", Order = -13)]
    public HashSet<string> ExpectedUsageWorldFileNameForAotuReset;
    [JsonProperty("地图存放目录", Order = -13)]
    public string WorldPath;

    [JsonProperty("重置后的最多在线人数", Order = -12)]
    public int AfterResetPeople;
    [JsonProperty("重置后的端口", Order = -12)]
    public string AfterResetPort;
    [JsonProperty("重置后的服务器密码", Order = -12)]
    public string AfterResetServerPassword;

    //重启计划
    [JsonProperty("上次重启服务器的日期", Order = -11)]
    public DateTime LasetServerRestartDate;
    [JsonProperty("是否启用自动重启服务器", Order = -11)]
    public bool AutoRestartServer;
    [JsonProperty("多少小时后开始自动重启服务器", Order = -11)]
    public double HowLongTimeOfRestartServer;
    [JsonProperty("重启后的最多在线人数", Order = -11)]
    public int AfterRestartPeople;
    [JsonProperty("重启后的端口", Order = -11)]
    public string AfterRestartPort;
    [JsonProperty("重启后的服务器密码", Order = -11)]
    public string AfterRestartServerPassword;
    [JsonProperty("重启前执行的指令", Order = -11)]
    public HashSet<string> CommandForBeforeRestart;

    //Boss进度控制计划
    [JsonProperty("是否自动控制NPC进度", Order = -10)]
    public bool OpenAutoControlProgressLock;
    [JsonProperty("Boss封禁时长距开服日期", Order = -10)]
    public Dictionary<string, double> ProgressLockTimeForStartServerDate;
    [JsonProperty("NPC封禁时长距开服日期_ID和单位小时", Order = -10)]
    public Dictionary<int, double> CustomNPCIDLockTimeForStartServerDate;

    //指令使用计划
    [JsonProperty("上次自动执行指令的日期", Order = -9)]
    public DateTime LasetAutoCommandDate;
    [JsonProperty("是否启用自动执行指令", Order = -9)]
    public bool OpenAutoCommand;
    [JsonProperty("多少小时后开始自动执行指令", Order = -9)]
    public double HowLongTimeOfAutoCommand;
    [JsonProperty("自动执行的指令_不需要加斜杠", Order = -9)]
    public HashSet<string> AutoCommandList;
    [JsonProperty("执行指令时是否发广播", Order = -9)]
    public bool AutoCommandOfBroadcast;
    [JsonProperty("越权检查", Order = -9)]
    public bool CheckPerm;
    [JsonProperty("是否关闭ServerLog写入功能(Windows千万别开)", Order = -9)]
    public bool ServerLogWriterEnabledForAotuResetting;
    [JsonProperty("指令功能_删除哪些文件或文件夹", Order = -8)]
    public HashSet<string> Command_PcoDelFile_DeletePath;
    [JsonProperty("指令功能_要复制的文件或文件夹", Order = -8)]
    public HashSet<string> Command_PcoCopy_CopyPath;
    [JsonProperty("指令功能_复制目标目录", Order = -8)]
    public string Command_PcoCopy_PastePath { get; set; }
    [JsonProperty("指令功能_文件是否允许覆盖", Order = -8)]
    public bool Command_PcoCopy_CoverFile { get; set; } = true;

    /// <summary>
    /// 地图的文件夹目录
    /// </summary>
    /// <returns></returns>
    public string path()
    {
        return string.IsNullOrWhiteSpace(this.WorldPath) ? Main.WorldPath : this.WorldPath;
    }


    /// <summary>
    /// 为防止地图的文件夹目录里对即将要创造的地图名称重名，进行后缀加数字
    /// </summary>
    /// <param name="name">要创造的地图的名字</param>
    /// <param name="willDelete">将要删掉的地图的名字，删掉发生在创造地图前</param>
    /// <returns></returns>
    public string AddNumberFile(string? name, string willDelete = "")
    {   //尝试给重复地图编号，其实原版有自动编号的，但是自动编号的地图名称和地图数据的内部名称不一致，你自己手动开服就分不清了
        var count = 1;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "World";
        }

        while (true)
        {
            if (File.Exists(this.path() + "/" + name + (count == 1 ? "" : count) + ".wld") && (name + (count == 1 ? "" : count)) != willDelete)
            {
                count++;
            }
            else
            {
                return (name + (count == 1 ? "" : count)) != willDelete ? name + (count == 1 ? "" : count) : willDelete;
            }
        }
    }
}