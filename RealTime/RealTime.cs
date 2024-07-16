using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace RealTime
{
    public delegate void ChangeCloud();
    [ApiVersion(2, 1)]
    public class RealTime : TerrariaPlugin
    {
        public override string Author => "十七";
        public override string Description => "同步现实时间";
        public override string Name => "RealTime";
        public override Version Version => new Version(2, 5, 0, 0);
        public RealTime(Main game) : base(game)
        {
        }
        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            On.Terraria.Main.UpdateTime += NPCS;
            GetDataHandlers.PlayerTeam += Team;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                On.Terraria.Main.UpdateTime -= NPCS;
                GetDataHandlers.PlayerTeam -= Team;
            }
            base.Dispose(disposing);
        }

        private void Team(object o, GetDataHandlers.PlayerTeamEventArgs args)//队伍判断
        {
            if (Main.bloodMoon == true || Main.eclipse == true || Main.pumpkinMoon == true || Main.snowMoon == true)
            {
                args.Player.SetTeam(0);
                args.Handled = true;
                args.Player.SendInfoMessage("事件禁止切换队伍。");
            }
        }

        private void NPCS(On.Terraria.Main.orig_UpdateTime orig)//通过拦截时间更新，然后让其在非白天情况下npc入住
        {
            orig();
            if (!Main.dayTime)
            {
                Main.UpdateTime_SpawnTownNPCs();
            }
        }

        public ChangeCloud GetRandom(ChangeCloud[] arr)//自定义随机数定义
        {
            Random ran = new Random();
            int n = ran.Next(arr.Length);
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
            if (lastBloodMoon ^ Main.bloodMoon)
            {
                if (Main.bloodMoon)
                {
                    //血月开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(20);
                }
            }
            lastBloodMoon = Main.bloodMoon;
            if (Main.bloodMoon == true)
            {
                TShock.Players.Where(p => p != null).ToList().ForEach(p=>
                {
                    if (p.TPlayer.hostile == false)
                    {
                        p.SetTeam(0);
                        p.TPlayer.hostile = true;
                        p.SendData(PacketTypes.TogglePvp, "", p.Index);
                        p.SendInfoMessage("血月的邪恶影响会阻止你的PvP关闭。");
                    }
                });                
                if (DateTime.Now >= time)
                {
                    Main.bloodMoon = false;
                }
            }
            #endregion
            #region 日食
            if (lastEclipse ^ Main.eclipse)
            {
                if (Main.eclipse)
                {
                    //日食开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(20);
                }
            }
            lastEclipse = Main.eclipse;
            if (Main.eclipse == true)
            {
                TShock.Players.Where(p => p != null).ToList().ForEach(p =>
                {
                    if (p.TPlayer.hostile == false)
                    {
                        p.TPlayer.hostile = true;
                        p.SetTeam(0);
                        p.SendData(PacketTypes.TogglePvp, "", p.Index);
                        p.SendInfoMessage("日食的邪恶影响会阻止你的PvP关闭。");
                    }
                });
                if (DateTime.Now >= time)
                {
                    Main.eclipse = false;
                }
            }
            #endregion
            #region 万圣节
            if (lastPumpkinMoon ^ Main.pumpkinMoon)
            {
                if (Main.pumpkinMoon)
                {
                    //万圣节开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(40);
                }
            }
            lastPumpkinMoon = Main.pumpkinMoon;
            if (Main.pumpkinMoon == true)
            {
                if (DateTime.Now >= time)
                {
                    TShock.Players.Where(p => p != null).ToList().ForEach(p =>
                    {
                        if (p.TPlayer.hostile == false)
                        {
                            p.TPlayer.hostile = true;
                            p.SetTeam(0);
                            p.SendData(PacketTypes.TogglePvp, "", p.Index);
                            p.SendInfoMessage("万圣节的邪恶影响会阻止你的PvP关闭。");
                        }
                    });
                    Main.pumpkinMoon = false;
                }
            }
            #endregion
            #region 霜月
            if (lastSnowMoon ^ Main.snowMoon)
            {
                if (Main.snowMoon)
                {
                    //霜月开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(40);
                }
            }
            lastSnowMoon = Main.snowMoon;
            if (Main.snowMoon == true)
            {
                if (DateTime.Now >= time)
                {
                    TShock.Players.Where(p => p != null).ToList().ForEach(p =>
                    {
                        if (p.TPlayer.hostile == false)
                        {
                            p.TPlayer.hostile = true;
                            p.SetTeam(0);
                            p.SendData(PacketTypes.TogglePvp, "", p.Index);
                            p.SendInfoMessage("霜月的邪恶影响会阻止你的PvP关闭。");
                        }
                    });
                    Main.snowMoon = false;
                }
            }
            #endregion
            #region 真实时间
            i++;
            if (i == 480)//真实时间
            {
                DateTime dt = DateTime.Now;
                dt.ToShortTimeString().ToString();
                decimal d = int.Parse(dt.Hour.ToString()) + int.Parse(dt.Minute.ToString()) / 60.0m;
                d -= 4.50m;
                if (d < 0.00m)
                {
                    d += 24.00m;
                }
                if (d >= 15.00m)
                {
                    TSPlayer.Server.SetTime(false, (double)((d - 15.00m) * 3600.0m));
                }
                else
                {
                    TSPlayer.Server.SetTime(true, (double)(d * 3600.0m));
                }
                i = 0;
            }
            #endregion
            #region npc生成、月相、天气、渔夫任务刷新
            y++;
            if (y == 86400)//npc生成 月相、天气、渔夫任务刷新
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
                Main.AnglerQuestSwap();//更换渔夫任务
                TSPlayer.All.SendInfoMessage("渔夫任务已更换");
                if ((DateTime.Now.Hour >= 19 && DateTime.Now.Hour <= 24) || (0 <= DateTime.Now.Hour && DateTime.Now.Hour <= 4))
                {
                    if (0 <= Main.moonPhase + 1 && Main.moonPhase + 1 <= 7)
                    {
                        string msg = GetMoon(Main.moonPhase);
                        Main.moonPhase += 1;
                        NetMessage.SendData(7);//发月相包
                        TSPlayer.All.SendInfoMessage("月相已更换为：{0}", msg);
                    }
                    else
                    {
                        Main.moonPhase = 0;
                        NetMessage.SendData(7);//发月相包
                        TSPlayer.All.SendInfoMessage("月相已更换为：满月");
                    }
                }
                else
                {
                    Random ran = new Random();
                    int num = ran.Next(1, 50); //1-50随机数
                    ChangeCloud[] arr =
                    {
                        ()=>Main.cloudBGActive=num,
                        ()=>Main.cloudBGActive=0,
                        ()=>Main.numClouds=0,
                        ()=>Main.numClouds=10,
                        ()=>Main.numClouds=95,
                        ()=>Main.numClouds=60,
                        ()=>Main.StartRain(), //下雨
                        ()=>Main.StopRain(), //雨停止
                        ()=>Main.windSpeedCurrent=num,//风速
                        ()=>Main.windSpeedTarget = num,
                        ()=>Main.maxRaining=0.3f,
                        ()=>Main.maxRaining=1,
                        ()=>Main.maxRaining=0.1f,
                        ()=>Terraria.GameContent.Events.Sandstorm.StartSandstorm(),//开始沙尘暴
                        ()=>Terraria.GameContent.Events.Sandstorm.StopSandstorm() //停止沙尘暴                       
                        };
                    GetRandom(arr)();
                    NetMessage.SendData(7);
                    TSPlayer.All.SendInfoMessage("天气已变更" );
                }
                y = 0;
            }
            #endregion
        }

        private String GetMoon(int index)
        {
            String[] arr = new string[] { "亏凸月", "下弦月", "残月", "新月", "娥眉月", "上弦月", "盈凸月" };
            if (index == -1 || index + 1 > arr.Length)
                return "[c/FF66FF:未知]";
            else return arr[index];
        }
    }
}
