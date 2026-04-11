using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RealTime;

public delegate void ChangeCloud();

[ApiVersion(2, 1)]
public class RealTime : TerrariaPlugin
{
    #region 基础部分
    public override string Author => "十七";
    public override string Description => GetString("同步现实时间");
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 7, 0, 7);
    private static readonly Random rand = new Random();
    private int realTimeSyncTimer = 0;   // 同步时间计时器
    private int npcWeatherTimer = 0;     // NPC/渔夫/月相计时器
    private int weatherChangeTimer = 0;  // 天气变化计时器
    private int secondTimer = 0;         // 每秒触发一次的计时器
    // 将事件的 DateTime 替换为简单的秒数倒计时
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
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        On.Terraria.Main.UpdateTime += this.NPCS;
        GetDataHandlers.PlayerTeam += this.Team;
        On.Terraria.WorldGen.UnspawnTravelNPC += this.BlockUnspawn;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            On.Terraria.Main.UpdateTime -= this.NPCS;
            GetDataHandlers.PlayerTeam -= this.Team;
            On.Terraria.WorldGen.UnspawnTravelNPC -= this.BlockUnspawn;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 禁止旅商离开
    private void BlockUnspawn(On.Terraria.WorldGen.orig_UnspawnTravelNPC orig)
    {
        return;
    }
    #endregion

    #region 禁止事件期间切换队伍
    private void Team(object? o, GetDataHandlers.PlayerTeamEventArgs args)
    {
        if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
        {
            args.Player.SetTeam(0);
            args.Handled = true;
            args.Player.SendInfoMessage(GetString("事件禁止切换队伍。"));
        }
    }
    #endregion

    #region NPC夜间入住
    private void NPCS(On.Terraria.Main.orig_UpdateTime orig)
    {
        orig();
        if (!Main.dayTime)
        {
            Main.UpdateTime_SpawnTownNPCs(true);
        }
    }
    #endregion

    #region 事件PvP逻辑
    // 将频繁调用的强制PvP逻辑独立，现在它只会每秒被调用1次，而不是每秒60次
    private void EnforceEventPvP(string warningMessage)
    {
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
        //每秒触发一次的逻辑（60帧）: 用于倒计时和 PvP 检查
        this.secondTimer++;
        if (this.secondTimer >= 60)
        {
            this.secondTimer = 0;

            #region 血月倒计时与强制PvP
            if (this.lastBloodMoon ^ Main.bloodMoon)
            {
                if (Main.bloodMoon) this.bloodMoonSecondsLeft = 20 * 60; // 20分钟换算为秒
            }
            this.lastBloodMoon = Main.bloodMoon;
            if (Main.bloodMoon)
            {
                EnforceEventPvP(GetString("血月的邪恶影响会阻止你的PvP关闭。"));
                this.bloodMoonSecondsLeft--;
                if (this.bloodMoonSecondsLeft <= 0)
                {
                    Main.bloodMoon = false;
                    NetMessage.SendData(7);
                }
            }
            #endregion

            #region 日食倒计时与强制PvP
            if (this.lastEclipse ^ Main.eclipse)
            {
                if (Main.eclipse) this.eclipseSecondsLeft = 20 * 60;
            }
            this.lastEclipse = Main.eclipse;
            if (Main.eclipse)
            {
                EnforceEventPvP(GetString("日食的邪恶影响会阻止你的PvP关闭。"));
                this.eclipseSecondsLeft--;
                if (this.eclipseSecondsLeft <= 0)
                {
                    Main.eclipse = false;
                    NetMessage.SendData(7);
                }
            }
            #endregion

            #region 万圣节倒计时与强制PvP
            if (this.lastPumpkinMoon ^ Main.pumpkinMoon)
            {
                if (Main.pumpkinMoon) this.pumpkinMoonSecondsLeft = 40 * 60;
            }
            this.lastPumpkinMoon = Main.pumpkinMoon;
            if (Main.pumpkinMoon)
            {
                EnforceEventPvP(GetString("万圣节的邪恶影响会阻止你的PvP关闭。"));
                this.pumpkinMoonSecondsLeft--;
                if (this.pumpkinMoonSecondsLeft <= 0)
                {
                    Main.pumpkinMoon = false;
                    NetMessage.SendData(7);
                }
            }
            #endregion

            #region 霜月倒计时与强制PvP
            if (this.lastSnowMoon ^ Main.snowMoon)
            {
                if (Main.snowMoon) this.snowMoonSecondsLeft = 40 * 60;
            }
            this.lastSnowMoon = Main.snowMoon;
            if (Main.snowMoon)
            {
                EnforceEventPvP(GetString("霜月的邪恶影响会阻止你的PvP关闭。"));
                this.snowMoonSecondsLeft--;
                if (this.snowMoonSecondsLeft <= 0)
                {
                    Main.snowMoon = false;
                    NetMessage.SendData(7);
                }
            }
            #endregion
        }

        #region 真实时间
        //每480帧 / 约8秒触发
        this.realTimeSyncTimer++;
        if (this.realTimeSyncTimer >= 480)
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
        #endregion

        #region npc生成、月相、天气、渔夫任务刷新
        //每86400帧/约24分钟
        this.npcWeatherTimer++;
        if (this.npcWeatherTimer >= 86400)
        {
            bool oldManAlive = false;
            bool cultistAlive = false;
            for (int j = 0; j < Main.maxNPCs; j++)//检查所有NPC是否存在老人或教徒
            {
                var npc = Main.npc[j];
                if (npc != null && npc.active)
                {
                    if (npc.netID == 37) oldManAlive = true;
                    if (npc.netID == 439) cultistAlive = true;
                }
            }
            if (!oldManAlive && !NPC.downedBoss3)//如果没有老人且没有打过骷髅王，就生成老人
                TSPlayer.Server.SpawnNPC(37, "老人", 1, Main.dungeonX, Main.dungeonY, 50, 20);
            if (!cultistAlive && !NPC.downedMoonlord && NPC.downedGolemBoss)//如果没有教徒且打过石巨人但没打过月球领主，就生成教徒
                TSPlayer.Server.SpawnNPC(439, "教徒", 1, Main.dungeonX, Main.dungeonY, 50, 20);

            Chest.SetupTravelShop();//更新旅商商品
            NetMessage.SendData(72);
            Main.AnglerQuestSwap();//更新渔夫任务
            TSPlayer.All.SendInfoMessage(GetString("渔夫任务和旅商商品已更换"));

            int currentHour = DateTime.Now.Hour; 
            if (currentHour >= 19 || currentHour <= 4)
            {
                int nextMoon = (Main.moonPhase + 1) % 8;
                Main.moonPhase = nextMoon;
                NetMessage.SendData(7);
                TSPlayer.All.SendInfoMessage(GetString($"月相已更换为：{this.GetMoon(nextMoon)}"));
            }
            this.npcWeatherTimer = 0;
        }
        #endregion

        #region 天气刷新
        //每36000帧/约10分钟
        this.weatherChangeTimer++;
        if (this.weatherChangeTimer >= 36000)
        {
            // 先清空所有天气
            Main.StopRain();
            Terraria.GameContent.Events.Sandstorm.StopSandstorm();
            Main.StopSlimeRain();
            //风速重置
            Main.windSpeedTarget = 0f;
            Main.windSpeedCurrent = 0f;
            int r = rand.Next(100);//随机生成0-99的数来决定天气
            string weatherName;
            if (r < 35)
            {
                Main.numClouds = 0;
                weatherName = GetString("晴天");
            }
            else if (r < 60)
            {
                Main.numClouds = 60;
                Main.windSpeedTarget = 0.2f;
                weatherName = GetString("多云");
            }
            else if (r < 75)
            {
                Main.StartRain();
                Main.maxRaining = 0.3f;
                Main.numClouds = rand.Next(60, 100);
                weatherName = GetString("小雨");
            }
            else if (r < 88)
            {
                Main.StartRain();
                Main.maxRaining = 0.6f;
                Main.numClouds = 70;
                weatherName = GetString("中雨");
            }
            else if (r < 96)
            {
                Main.StartRain();
                Main.maxRaining = 1.0f;
                Main.windSpeedTarget = 0.8f;
                Main.windSpeedCurrent = 0.8f;
                Main.numClouds = rand.Next(80, 100);
                weatherName = GetString("暴雨/雷暴");
            }
            else
            {
                Terraria.GameContent.Events.Sandstorm.StartSandstorm();
                Main.windSpeedTarget = 0.7f;
                Main.windSpeedCurrent = 0.7f;
                weatherName = GetString("沙尘暴");
            }

            NetMessage.SendData(7);
            TSPlayer.All.SendInfoMessage(GetString($"当前天气：{weatherName}"));
            this.weatherChangeTimer = 0;
        }
        #endregion
    }
    #endregion

    #region 月相文本
    private string GetMoon(int phase)
    {
        switch (phase)
        {
            case 0: return GetString("满月");
            case 1: return GetString("亏凸月");
            case 2: return GetString("下弦月");
            case 3: return GetString("残月");
            case 4: return GetString("新月");
            case 5: return GetString("娥眉月");
            case 6: return GetString("上弦月");
            case 7: return GetString("盈凸月");
            default: return GetString("未知");
        }
    }
    #endregion
}
