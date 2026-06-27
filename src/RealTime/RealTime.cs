using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace RealTime;

[ApiVersion(2, 1)]
public class RealTime : TerrariaPlugin
{
    #region 基础部分
    public override string Author => "十七、星梦";
    public override string Description => "同步现实时间";
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 8, 0, 0);
    private static readonly Random rand = new Random();

    private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "realtime.json");
    public static RealTimeConfig Config { get; private set; } = null!;

    private int realTimeSyncTimer = 0;
    private int npcWeatherTimer = 0;
    private int weatherChangeTimer = 0;
    private int secondTimer = 0;

    private int bloodMoonSecondsLeft = 0;
    private int eclipseSecondsLeft = 0;
    private int pumpkinMoonSecondsLeft = 0;
    private int snowMoonSecondsLeft = 0;

    private bool lastBloodMoon = false;
    private bool lastEclipse = false;
    private bool lastPumpkinMoon = false;
    private bool lastSnowMoon = false;

    public RealTime(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = RealTimeConfig.Load(ConfigPath);

        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        On.Terraria.Main.UpdateTime += this.NPCS;
        GetDataHandlers.PlayerTeam += this.Team;
        On.Terraria.WorldGen.UnspawnTravelNPC += this.BlockUnspawn;
        GeneralHooks.ReloadEvent += this.OnReload;

        Commands.ChatCommands.Add(new Command("realtime.admin", this.CmdRte, "rte")
        {
            HelpText = "查看和修改RealTime插件配置。用法: /rte [序列号] [值]"
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            On.Terraria.Main.UpdateTime -= this.NPCS;
            GetDataHandlers.PlayerTeam -= this.Team;
            On.Terraria.WorldGen.UnspawnTravelNPC -= this.BlockUnspawn;
            GeneralHooks.ReloadEvent -= this.OnReload;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 指令系统
    private static readonly Dictionary<int, string> ConfigIndexMap = new()
    {
        { 1, "同步现实时间" },
        { 2, "同步间隔秒" },
        { 3, "事件自动结束" },
        { 4, "血月日食持续时间分钟" },
        { 5, "霜月万圣节持续时间分钟" },
        { 6, "渔夫任务刷新" },
        { 7, "月相刷新" },
        { 8, "NPC综合刷新间隔分钟" },
        { 9, "老人自动召唤" },
        { 10, "拜月教徒自动召唤" },
        { 11, "旅商商品刷新" },
        { 12, "旅商常驻" },
        { 13, "随机天气" },
        { 14, "天气更新间隔分钟" },
        { 15, "夜间NPC入住" },
        { 16, "事件强制PVP" },
        { 17, "事件禁止切换队伍" }
    };

    private void CmdRte(CommandArgs args)
    {
        var player = args.Player;
        var parameters = args.Parameters;

        if (parameters.Count == 0)
        {
            ShowConfig(player);
            return;
        }

        if (parameters.Count == 1)
        {
            player.SendErrorMessage("用法: /rte [序列号] [值]");
            player.SendInfoMessage("示例: /rte 1 true");
            return;
        }

        string keyOrIndex = parameters[0];
        string value = parameters[1];

        string? configKey = null;

        if (int.TryParse(keyOrIndex, out int index))
        {
            if (ConfigIndexMap.TryGetValue(index, out configKey))
            {
            }
            else
            {
                player.SendErrorMessage($"[RealTime] 无效的序列号: {index}，有效范围: 1-{ConfigIndexMap.Count}");
                return;
            }
        }
        else
        {
            if (ConfigIndexMap.ContainsValue(keyOrIndex))
            {
                configKey = keyOrIndex;
            }
            else
            {
                player.SendErrorMessage($"[RealTime] 未知的配置项: {keyOrIndex}，请使用序列号 (1-{ConfigIndexMap.Count})");
                return;
            }
        }

        bool success = TrySetConfig(configKey!, value, out string feedback);
        if (success)
        {
            Config.Save(ConfigPath);
            player.SendSuccessMessage($"[RealTime] {feedback}");
        }
        else
        {
            player.SendErrorMessage($"[RealTime] {feedback}");
        }
    }

    private void ShowConfig(TSPlayer player)
    {
        player.SendInfoMessage("[c/FFD700:=== RealTime 当前配置 ===]");
        player.SendInfoMessage($"[c/90EE90:用法:] /rte [序列号] [值]");
        player.SendInfoMessage("[c/87CEEB:----------------------]");
        
        foreach (var kvp in ConfigIndexMap.OrderBy(x => x.Key))
        {
            string displayValue = GetConfigDisplayValue(kvp.Value);
            player.SendInfoMessage($"[c/FFFF00:({kvp.Key})] [c/87CEEB:{kvp.Value}:] {displayValue}");
        }
    }

    private string GetConfigDisplayValue(string key)
    {
        return key switch
        {
            "同步现实时间" => Bool(Config.EnableRealTimeSync),
            "同步间隔秒" => Config.RealTimeSyncInterval.ToString(),
            "事件自动结束" => Bool(Config.EnableEventAutoEnd),
            "血月日食持续时间分钟" => Config.BloodMoonEclipseDuration.ToString(),
            "霜月万圣节持续时间分钟" => Config.PumpkinMoonSnowMoonDuration.ToString(),
            "渔夫任务刷新" => Bool(Config.EnableAnglerQuest),
            "月相刷新" => Bool(Config.EnableMoonPhase),
            "NPC综合刷新间隔分钟" => Config.NpcRefreshInterval.ToString(),
            "老人自动召唤" => Bool(Config.EnableOldManSpawn),
            "拜月教徒自动召唤" => Bool(Config.EnableCultistSpawn),
            "旅商商品刷新" => Bool(Config.EnableTravelShopRefresh),
            "旅商常驻" => Bool(Config.EnableTravelNPCStay),
            "随机天气" => Bool(Config.EnableWeatherChange),
            "天气更新间隔分钟" => Config.WeatherChangeInterval.ToString(),
            "夜间NPC入住" => Bool(Config.EnableNightNpcSpawn),
            "事件强制PVP" => Bool(Config.EnableEventForcePvp),
            "事件禁止切换队伍" => Bool(Config.EnableEventTeamLock),
            _ => "未知"
        };
    }

    private static string Bool(bool b) => b ? "[c/00FF00:开启]" : "[c/FF0000:关闭]";

    private bool TrySetConfig(string key, string value, out string feedback)
    {
        try
        {
            switch (key)
            {
                case "同步现实时间":
                    Config.EnableRealTimeSync = ParseBool(value);
                    feedback = $"同步现实时间已{BoolText(Config.EnableRealTimeSync)}";
                    return true;
                case "同步间隔秒":
                    Config.RealTimeSyncInterval = ParseInt(value, 1, 3600);
                    feedback = $"同步间隔秒已设置为 {Config.RealTimeSyncInterval}";
                    return true;
                case "事件自动结束":
                    Config.EnableEventAutoEnd = ParseBool(value);
                    feedback = $"事件自动结束已{BoolText(Config.EnableEventAutoEnd)}";
                    return true;
                case "血月日食持续时间分钟":
                    Config.BloodMoonEclipseDuration = ParseInt(value, 1, 120);
                    feedback = $"血月日食持续时间已设置为 {Config.BloodMoonEclipseDuration} 分钟";
                    return true;
                case "霜月万圣节持续时间分钟":
                    Config.PumpkinMoonSnowMoonDuration = ParseInt(value, 1, 120);
                    feedback = $"霜月万圣节持续时间已设置为 {Config.PumpkinMoonSnowMoonDuration} 分钟";
                    return true;
                case "渔夫任务刷新":
                    Config.EnableAnglerQuest = ParseBool(value);
                    feedback = $"渔夫任务刷新已{BoolText(Config.EnableAnglerQuest)}";
                    return true;
                case "月相刷新":
                    Config.EnableMoonPhase = ParseBool(value);
                    feedback = $"月相刷新已{BoolText(Config.EnableMoonPhase)}";
                    return true;
                case "NPC综合刷新间隔分钟":
                    Config.NpcRefreshInterval = ParseInt(value, 1, 1440);
                    feedback = $"NPC综合刷新间隔已设置为 {Config.NpcRefreshInterval} 分钟";
                    return true;
                case "老人自动召唤":
                    Config.EnableOldManSpawn = ParseBool(value);
                    feedback = $"老人自动召唤已{BoolText(Config.EnableOldManSpawn)}";
                    return true;
                case "拜月教徒自动召唤":
                    Config.EnableCultistSpawn = ParseBool(value);
                    feedback = $"拜月教徒自动召唤已{BoolText(Config.EnableCultistSpawn)}";
                    return true;
                case "旅商商品刷新":
                    Config.EnableTravelShopRefresh = ParseBool(value);
                    feedback = $"旅商商品刷新已{BoolText(Config.EnableTravelShopRefresh)}";
                    return true;
                case "旅商常驻":
                    Config.EnableTravelNPCStay = ParseBool(value);
                    feedback = $"旅商常驻已{BoolText(Config.EnableTravelNPCStay)}";
                    return true;
                case "随机天气":
                    Config.EnableWeatherChange = ParseBool(value);
                    feedback = $"随机天气已{BoolText(Config.EnableWeatherChange)}";
                    return true;
                case "天气更新间隔分钟":
                    Config.WeatherChangeInterval = ParseInt(value, 1, 1440);
                    feedback = $"天气更新间隔已设置为 {Config.WeatherChangeInterval} 分钟";
                    return true;
                case "夜间NPC入住":
                    Config.EnableNightNpcSpawn = ParseBool(value);
                    feedback = $"夜间NPC入住已{BoolText(Config.EnableNightNpcSpawn)}";
                    return true;
                case "事件强制PVP":
                    Config.EnableEventForcePvp = ParseBool(value);
                    feedback = $"事件强制PVP已{BoolText(Config.EnableEventForcePvp)}";
                    return true;
                case "事件禁止切换队伍":
                    Config.EnableEventTeamLock = ParseBool(value);
                    feedback = $"事件禁止切换队伍已{BoolText(Config.EnableEventTeamLock)}";
                    return true;
                default:
                    feedback = "未知配置项，请检查拼写";
                    return false;
            }
        }
        catch (FormatException)
        {
            feedback = "值格式错误，布尔值请输入 true/false，数值请输入整数";
            return false;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            feedback = ex.Message;
            return false;
        }
    }

    private static bool ParseBool(string value)
    {
        return value.ToLower() switch
        {
            "true" or "1" or "开" or "是" or "yes" or "on" => true,
            "false" or "0" or "关" or "否" or "no" or "off" => false,
            _ => throw new FormatException()
        };
    }

    private static int ParseInt(string value, int min, int max)
    {
        int result = int.Parse(value);
        if (result < min || result > max)
            throw new ArgumentOutOfRangeException($"数值必须在 {min} ~ {max} 之间");
        return result;
    }

    private static string BoolText(bool b) => b ? "开启" : "关闭";
    #endregion

    #region 重载配置
    private void OnReload(ReloadEventArgs args)
    {
        Config = RealTimeConfig.Load(ConfigPath);
        args.Player?.SendSuccessMessage("[RealTime] 配置已重载。");
    }
    #endregion

    #region 禁止旅商离开
    private void BlockUnspawn(On.Terraria.WorldGen.orig_UnspawnTravelNPC orig)
    {
        if (!Config.EnableTravelNPCStay)
        {
            orig();
            return;
        }
        return;
    }
    #endregion

    #region 禁止事件期间切换队伍
    private void Team(object? o, GetDataHandlers.PlayerTeamEventArgs args)
    {
        if (!Config.EnableEventTeamLock) return;
        if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
        {
            args.Player.SetTeam(0);
            args.Handled = true;
            args.Player.SendInfoMessage("事件禁止切换队伍。");
        }
    }
    #endregion

    #region NPC夜间入住
    private void NPCS(On.Terraria.Main.orig_UpdateTime orig)
    {
        orig();
        if (!Config.EnableNightNpcSpawn) return;
        if (!Main.dayTime)
        {
            Main.UpdateTime_SpawnTownNPCs(true);
        }
    }
    #endregion

    #region 事件PvP逻辑
    private void EnforceEventPvP(string warningMessage)
    {
        if (!Config.EnableEventForcePvp) return;
        foreach (var p in TShock.Players)
        {
            if (p != null && p.Active && !p.TPlayer.hostile)
            {
                p.SetTeam(0);
                p.TPlayer.hostile = true;
                p.SendData(PacketTypes.TogglePvp, "", p.Index);
                p.SendInfoMessage(warningMessage);
            }
        }
    }
    #endregion

    #region 游戏更新逻辑
    private void OnGameUpdate(EventArgs args)
    {
        this.secondTimer++;
        if (this.secondTimer >= 60)
        {
            this.secondTimer = 0;

            if (Config.EnableEventAutoEnd)
            {
                if (this.lastBloodMoon ^ Main.bloodMoon)
                {
                    if (Main.bloodMoon) this.bloodMoonSecondsLeft = Config.BloodMoonEclipseDuration * 60;
                }
                this.lastBloodMoon = Main.bloodMoon;
                if (Main.bloodMoon)
                {
                    EnforceEventPvP("血月的邪恶影响会阻止你的PvP关闭。");
                    this.bloodMoonSecondsLeft--;
                    if (this.bloodMoonSecondsLeft <= 0)
                    {
                        Main.bloodMoon = false;
                        NetMessage.SendData(7);
                    }
                }

                if (this.lastEclipse ^ Main.eclipse)
                {
                    if (Main.eclipse) this.eclipseSecondsLeft = Config.BloodMoonEclipseDuration * 60;
                }
                this.lastEclipse = Main.eclipse;
                if (Main.eclipse)
                {
                    EnforceEventPvP("日食的邪恶影响会阻止你的PvP关闭。");
                    this.eclipseSecondsLeft--;
                    if (this.eclipseSecondsLeft <= 0)
                    {
                        Main.eclipse = false;
                        NetMessage.SendData(7);
                    }
                }

                if (this.lastPumpkinMoon ^ Main.pumpkinMoon)
                {
                    if (Main.pumpkinMoon) this.pumpkinMoonSecondsLeft = Config.PumpkinMoonSnowMoonDuration * 60;
                }
                this.lastPumpkinMoon = Main.pumpkinMoon;
                if (Main.pumpkinMoon)
                {
                    EnforceEventPvP("万圣节的邪恶影响会阻止你的PvP关闭。");
                    this.pumpkinMoonSecondsLeft--;
                    if (this.pumpkinMoonSecondsLeft <= 0)
                    {
                        Main.pumpkinMoon = false;
                        NetMessage.SendData(7);
                    }
                }

                if (this.lastSnowMoon ^ Main.snowMoon)
                {
                    if (Main.snowMoon) this.snowMoonSecondsLeft = Config.PumpkinMoonSnowMoonDuration * 60;
                }
                this.lastSnowMoon = Main.snowMoon;
                if (Main.snowMoon)
                {
                    EnforceEventPvP("霜月的邪恶影响会阻止你的PvP关闭。");
                    this.snowMoonSecondsLeft--;
                    if (this.snowMoonSecondsLeft <= 0)
                    {
                        Main.snowMoon = false;
                        NetMessage.SendData(7);
                    }
                }
            }
            else
            {
                this.lastBloodMoon = Main.bloodMoon;
                this.lastEclipse = Main.eclipse;
                this.lastPumpkinMoon = Main.pumpkinMoon;
                this.lastSnowMoon = Main.snowMoon;

                if (Main.bloodMoon) EnforceEventPvP("血月的邪恶影响会阻止你的PvP关闭。");
                if (Main.eclipse) EnforceEventPvP("日食的邪恶影响会阻止你的PvP关闭。");
                if (Main.pumpkinMoon) EnforceEventPvP("万圣节的邪恶影响会阻止你的PvP关闭。");
                if (Main.snowMoon) EnforceEventPvP("霜月的邪恶影响会阻止你的PvP关闭。");
            }
        }

        if (Config.EnableRealTimeSync)
        {
            this.realTimeSyncTimer++;
            if (this.realTimeSyncTimer >= Config.RealTimeSyncInterval * 60)
            {
                var dt = DateTime.Now;
                double d = dt.Hour + (dt.Minute / 60.0);
                d -= 4.5;
                if (d < 0.0) d += 24.0;

                if (d >= 15.0)
                    TSPlayer.Server.SetTime(false, (d - 15.0) * 3600.0);
                else
                    TSPlayer.Server.SetTime(true, d * 3600.0);

                this.realTimeSyncTimer = 0;
            }
        }

        this.npcWeatherTimer++;
        if (this.npcWeatherTimer >= Config.NpcRefreshInterval * 60 * 60)
        {
            if (Config.EnableOldManSpawn || Config.EnableCultistSpawn)
            {
                bool oldManAlive = false;
                bool cultistAlive = false;
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    var npc = Main.npc[j];
                    if (npc != null && npc.active)
                    {
                        if (npc.netID == 37) oldManAlive = true;
                        if (npc.netID == 439) cultistAlive = true;
                    }
                }
                if (Config.EnableOldManSpawn && !oldManAlive && !NPC.downedBoss3)
                    TSPlayer.Server.SpawnNPC(37, "老人", 1, Main.dungeonX, Main.dungeonY, 50, 20);
                if (Config.EnableCultistSpawn && !cultistAlive && !NPC.downedMoonlord && NPC.downedGolemBoss)
                    TSPlayer.Server.SpawnNPC(439, "教徒", 1, Main.dungeonX, Main.dungeonY, 50, 20);
            }

            if (Config.EnableTravelShopRefresh)
            {
                Chest.SetupTravelShop();
                NetMessage.SendData(72);
            }

            if (Config.EnableAnglerQuest)
            {
                Main.AnglerQuestSwap();
            }

            if (Config.EnableAnglerQuest || Config.EnableTravelShopRefresh)
            {
                TSPlayer.All.SendInfoMessage("渔夫任务和旅商商品已更换");
            }

            if (Config.EnableMoonPhase)
            {
                int currentHour = DateTime.Now.Hour;
                if (currentHour >= 19 || currentHour <= 4)
                {
                    int nextMoon = (Main.moonPhase + 1) % 8;
                    Main.moonPhase = nextMoon;
                    NetMessage.SendData(7);
                    TSPlayer.All.SendInfoMessage($"月相已更换为：{this.GetMoon(nextMoon)}");
                }
            }
            this.npcWeatherTimer = 0;
        }

        if (Config.EnableWeatherChange)
        {
            this.weatherChangeTimer++;
            if (this.weatherChangeTimer >= Config.WeatherChangeInterval * 60 * 60)
            {
                Main.StopRain();
                Terraria.GameContent.Events.Sandstorm.StopSandstorm();
                Main.StopSlimeRain();
                Main.windSpeedTarget = 0f;
                Main.windSpeedCurrent = 0f;
                int r = rand.Next(100);
                string weatherName;
                if (r < 35)
                {
                    Main.numClouds = 0;
                    weatherName = "晴天";
                }
                else if (r < 60)
                {
                    Main.numClouds = 60;
                    Main.windSpeedTarget = 0.2f;
                    weatherName = "多云";
                }
                else if (r < 75)
                {
                    Main.StartRain();
                    Main.maxRaining = 0.3f;
                    Main.numClouds = rand.Next(60, 100);
                    weatherName = "小雨";
                }
                else if (r < 88)
                {
                    Main.StartRain();
                    Main.maxRaining = 0.6f;
                    Main.numClouds = 70;
                    weatherName = "中雨";
                }
                else if (r < 96)
                {
                    Main.StartRain();
                    Main.maxRaining = 1.0f;
                    Main.windSpeedTarget = 0.8f;
                    Main.windSpeedCurrent = 0.8f;
                    Main.numClouds = rand.Next(80, 100);
                    weatherName = "暴雨/雷暴";
                }
                else
                {
                    Terraria.GameContent.Events.Sandstorm.StartSandstorm();
                    Main.windSpeedTarget = 0.7f;
                    Main.windSpeedCurrent = 0.7f;
                    weatherName = "沙尘暴";
                }

                NetMessage.SendData(7);
                TSPlayer.All.SendInfoMessage($"当前天气：{weatherName}");
                this.weatherChangeTimer = 0;
            }
        }
    }
    #endregion

    #region 月相文本
    private string GetMoon(int phase)
    {
        switch (phase)
        {
            case 0: return "满月";
            case 1: return "亏凸月";
            case 2: return "下弦月";
            case 3: return "残月";
            case 4: return "新月";
            case 5: return "娥眉月";
            case 6: return "上弦月";
            case 7: return "盈凸月";
            default: return "未知";
        }
    }
    #endregion
}