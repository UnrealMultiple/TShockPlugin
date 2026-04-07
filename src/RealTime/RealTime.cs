using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RealTime;

public delegate void ChangeCloud();
[ApiVersion(2, 1)]
public class RealTime : TerrariaPlugin
{
    public override string Author => "十七";
    public override string Description => GetString("同步现实时间");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 7, 0, 5);
    //private Hook? unspawnHook;
    private static readonly Random rand = new Random();
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

    private void BlockUnspawn(On.Terraria.WorldGen.orig_UnspawnTravelNPC orig)//拦截旅商被删除的函数，旅商永久驻留
    {
        return;
    }

    private void Team(object? o, GetDataHandlers.PlayerTeamEventArgs args)//队伍判断
    {
        if (Main.bloodMoon == true || Main.eclipse == true || Main.pumpkinMoon == true || Main.snowMoon == true)
        {
            args.Player.SetTeam(0);
            args.Handled = true;
            args.Player.SendInfoMessage(GetString("事件禁止切换队伍。"));
        }
    }

    private void NPCS(On.Terraria.Main.orig_UpdateTime orig)//通过拦截时间更新，然后让其在非白天情况下npc入住
    {
        orig();
        if (!Main.dayTime)
        {
            Main.UpdateTime_SpawnTownNPCs(true);
        }
    }

    public ChangeCloud GetRandom(ChangeCloud[] arr)//自定义随机数定义
    {
        var ran = new Random();
        var n = ran.Next(arr.Length);
        return arr[n];
    }

    int i = 0;
    int y = 0;
    int q = 0;
    bool lastBloodMoon = false;
    bool lastEclipse = false;
    bool lastPumpkinMoon = false;
    bool lastSnowMoon = false;
    DateTime time = DateTime.Now;
    private void OnGameUpdate(EventArgs args)
    {
        #region 血月
        if (this.lastBloodMoon ^ Main.bloodMoon)
        {
            if (Main.bloodMoon)
            {
                //血月开启瞬间
                this.time = DateTime.Now + TimeSpan.FromMinutes(20);
            }
        }
        this.lastBloodMoon = Main.bloodMoon;
        if (Main.bloodMoon == true)
        {
            TShock.Players.Where(p => p != null).ToList().ForEach(p =>
            {
                if (p.TPlayer.hostile == false)
                {
                    p.SetTeam(0);
                    p.TPlayer.hostile = true;
                    p.SendData(PacketTypes.TogglePvp, "", p.Index);
                    p.SendInfoMessage(GetString("血月的邪恶影响会阻止你的PvP关闭。"));
                }
            });
            if (DateTime.Now >= this.time)
            {
                Main.bloodMoon = false;
            }
        }
        #endregion
        #region 日食
        if (this.lastEclipse ^ Main.eclipse)
        {
            if (Main.eclipse)
            {
                //日食开启瞬间
                this.time = DateTime.Now + TimeSpan.FromMinutes(20);
            }
        }
        this.lastEclipse = Main.eclipse;
        if (Main.eclipse == true)
        {
            TShock.Players.Where(p => p != null).ToList().ForEach(p =>
            {
                if (p.TPlayer.hostile == false)
                {
                    p.TPlayer.hostile = true;
                    p.SetTeam(0);
                    p.SendData(PacketTypes.TogglePvp, "", p.Index);
                    p.SendInfoMessage(GetString("日食的邪恶影响会阻止你的PvP关闭。"));
                }
            });
            if (DateTime.Now >= this.time)
            {
                Main.eclipse = false;
            }
        }
        #endregion
        #region 万圣节
        if (this.lastPumpkinMoon ^ Main.pumpkinMoon)
        {
            if (Main.pumpkinMoon)
            {
                //万圣节开启瞬间
                this.time = DateTime.Now + TimeSpan.FromMinutes(40);
            }
        }
        this.lastPumpkinMoon = Main.pumpkinMoon;
        if (Main.pumpkinMoon == true)
        {
            if (DateTime.Now >= this.time)
            {
                TShock.Players.Where(p => p != null).ToList().ForEach(p =>
                {
                    if (p.TPlayer.hostile == false)
                    {
                        p.TPlayer.hostile = true;
                        p.SetTeam(0);
                        p.SendData(PacketTypes.TogglePvp, "", p.Index);
                        p.SendInfoMessage(GetString("万圣节的邪恶影响会阻止你的PvP关闭。"));
                    }
                });
                Main.pumpkinMoon = false;
            }
        }
        #endregion
        #region 霜月
        if (this.lastSnowMoon ^ Main.snowMoon)
        {
            if (Main.snowMoon)
            {
                //霜月开启瞬间
                this.time = DateTime.Now + TimeSpan.FromMinutes(40);
            }
        }
        this.lastSnowMoon = Main.snowMoon;
        if (Main.snowMoon == true)
        {
            if (DateTime.Now >= this.time)
            {
                TShock.Players.Where(p => p != null).ToList().ForEach(p =>
                {
                    if (p.TPlayer.hostile == false)
                    {
                        p.TPlayer.hostile = true;
                        p.SetTeam(0);
                        p.SendData(PacketTypes.TogglePvp, "", p.Index);
                        p.SendInfoMessage(GetString("霜月的邪恶影响会阻止你的PvP关闭。"));
                    }
                });
                Main.snowMoon = false;
            }
        }
        #endregion
        #region 真实时间
        this.i++;
        if (this.i == 480)//真实时间
        {
            var dt = DateTime.Now;
            dt.ToShortTimeString().ToString();
            var d = int.Parse(dt.Hour.ToString()) + (int.Parse(dt.Minute.ToString()) / 60.0m);
            d -= 4.50m;
            if (d < 0.00m)
            {
                d += 24.00m;
            }
            if (d >= 15.00m)
            {
                TSPlayer.Server.SetTime(false, (double) ((d - 15.00m) * 3600.0m));
            }
            else
            {
                TSPlayer.Server.SetTime(true, (double) (d * 3600.0m));
            }
            this.i = 0;
        }
        #endregion
        #region npc生成、月相、天气、渔夫任务刷新
        this.y++; 
        if (this.y == 86400)//npc生成 月相、天气、渔夫任务刷新
        {
            var AllNPCS = Main.npc.Where(n => n != null);
            foreach (var TNPC in AllNPCS)
            {
                if (TNPC.netID == 37)
                {
                    if (!TNPC.active)//遍历所有NPC，判断这个NPC是否存在
                    {
                        if (!NPC.downedBoss3)//判断有没有击败骷髅王
                        {
                            TSPlayer.Server.SpawnNPC(37, "老人", 200, Main.dungeonX, Main.dungeonY, 50, 20);
                        }
                    }
                }
                if (TNPC.netID == 439)
                {
                    if (!TNPC.active)
                    {
                        if (!NPC.downedMoonlord && NPC.downedGolemBoss)//没有击败月总但击败了石头人
                        {
                            TSPlayer.Server.SpawnNPC(439, "教徒", 200, Main.dungeonX, Main.dungeonY, 50, 20);
                        }
                    }
                }
            }
            Chest.SetupTravelShop();
            NetMessage.SendData(72);
            Main.AnglerQuestSwap();//更换渔夫任务
            TSPlayer.All.SendInfoMessage(GetString("渔夫任务和旅商商品已更换"));
            if (DateTime.Now.Hour >= 19 || DateTime.Now.Hour <= 4)
            {
                int nextMoon = (Main.moonPhase + 1) % 8;
                var msg = this.GetMoon(nextMoon);
                Main.moonPhase = nextMoon;
                NetMessage.SendData(7);
                TSPlayer.All.SendInfoMessage(GetString($"月相已更换为：{msg}"));
            }
            this.y = 0;
        }
        #endregion
        #region 天气刷新
        this.q++;
        if (this.q == 36000)//天气刷新
        {
            // 先清空所有天气
            Main.StopRain();
            Terraria.GameContent.Events.Sandstorm.StopSandstorm();
            Main.StopSlimeRain();
            // 风归零
            Main.windSpeedTarget = 0f;
            Main.windSpeedCurrent = 0f;
            int r = rand.Next(100); // 权重随机
            string weatherName = "";
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
            this.q = 0;
        }
        #endregion
    }
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
}
