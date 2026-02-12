using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static DamageRuleLoot.StrikeNPC;
using static DamageRuleLoot.Tool;

namespace DamageRuleLoot;

[ApiVersion(2, 1)]
public class DamageRuleLoot : TerrariaPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学 西江小子";
    public override Version Version => new Version(1, 3, 5);
    public override string Description => GetString("根据输出排名榜决定是否掉落宝藏袋的惩罚，并对各个BOSS进行相对的伤害规则处理");
    #endregion

    #region 注册与释放
    public DamageRuleLoot(Main game) : base(game) { }
    internal static StrikeNPC Strike = new();
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKill);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnMechQueen);
        On.Terraria.NPC.StrikeNPC += this.OnStrikeNPC;
        On.Terraria.NPC.StrikeNPC += this.AddDamage;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKill);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnMechQueen);
            On.Terraria.NPC.StrikeNPC -= this.OnStrikeNPC;
            On.Terraria.NPC.StrikeNPC -= this.AddDamage;
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void ReloadConfig(ReloadEventArgs args = null!)
    {
        LoadConfig();
        args.Player.SendInfoMessage(GetString("[伤害规则掉落]重新加载配置完毕。"));
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        WriteName();
        Config.Write();
    }
    #endregion

    #region 伤怪建表法+暴击计数法
    private double OnStrikeNPC(On.Terraria.NPC.orig_StrikeNPC orig, NPC self, int Damage, float knockBack, int hitDirection, bool crit, bool noEffect, bool fromNet,int owner, Entity entity)
    {
        var damage = orig(self, Damage, knockBack, hitDirection, crit, noEffect, fromNet, owner, entity);
        var strike = StrikeNPC.strikeNPC.Find(x => x.npcIndex == self.whoAmI && x.npcID == self.netID);

        if (fromNet && entity is Player plr)
        {
            if (strike != null && strike.npcName != string.Empty)
            {
                if (strike.PlayerOrDamage.ContainsKey(plr.name))
                {
                    if (crit)
                    {
                        strike.PlayerOrDamage[plr.name] += Damage;
                        strike.AllDamage += Damage;

                        if (Config.CritInfo)
                        {
                            TSPlayer.All.SendInfoMessage(GetString($"[c/FBF069:【暴击】] 玩家:[c/F06576:{plr.name}] ") +
                                GetString($"对象:[c/AEA3E4:{self.FullName}] 满血:[c/FBF069:{self.lifeMax}] ") +
                                GetString($"血量:[c/6DDA6D:{self.life}] 伤害:[c/F06576:{damage}] ") +
                                GetString($"暴击数:[c/FBF069:{CritTracker.GetCritCount(plr.name)}]"), 202, 221, 222);
                        }

                        CritTracker.UpdateCritCount(plr.name);
                    }
                    strike.PlayerOrDamage[plr.name] += Damage;
                    strike.AllDamage += Damage;
                }
                else
                {
                    strike.PlayerOrDamage.Add(plr.name, Damage);
                    strike.AllDamage += Damage;
                }
            }

            //不是城镇npc 雕像怪 假人才创建数据
            else if (!self.townNPC || !self.SpawnedFromStatue || self.netID != 488)
            {
                var snpc = new StrikeNPC()
                {
                    npcID = self.netID,
                    npcIndex = self.whoAmI,
                    npcName = self.FullName,
                };
                snpc.PlayerOrDamage.Add(plr.name, Damage);
                snpc.AllDamage += Damage;
                StrikeNPC.strikeNPC.Add(snpc);
            }
        }
        return damage;
    }
    #endregion

    #region 打怪伤BOSS法
    private double AddDamage(On.Terraria.NPC.orig_StrikeNPC orig, NPC self, int Damage, float knockBack, int hitDirection, bool crit, bool noEffect, bool fromNet,int owner, Entity entity)
    {
        var damage = orig(self, Damage, knockBack, hitDirection, crit, noEffect, fromNet, owner,entity);
        if (fromNet && entity is Player plr)
        {
            //不是雕像怪
            if (!self.SpawnedFromStatue)
            {
                if ((Config.Sharkron && self.netID == 372) || self.netID == 373)
                {
                    //获取猪鲨id
                    var strike = StrikeNPC.strikeNPC.Find(x => x.npcID == 370);
                    General(self, Damage, plr, strike, 20000f);
                }

                //判定为FTW和天顶世界的火焰小鬼与饿鬼
                if ((Config.FireImp && (Main.getGoodWorld || Main.zenithWorld) &&
                self.netID == 24) || self.netID == 115 || self.netID == 116)
                {
                    //获取肉山id
                    var strike = StrikeNPC.strikeNPC.Find(x => x.npcID == 113);
                    General(self, Damage, plr, strike, 3000f);
                }

                //判定为机械骷髅王四肢
                if ((Config.Prime && self.netID == 128) || self.netID == 129 ||
                    self.netID == 130 || self.netID == 131)
                {
                    //获取机械骷髅王id
                    var strike = StrikeNPC.strikeNPC.Find(x => x.npcID == 127);
                    General(self, Damage, plr, strike, 5000f);
                }

                //判定自定义
                if (Config.CustomTransfer)
                {
                    foreach (var Custom in Config.TList)
                    {
                        foreach (var B in Custom.NPCB)
                        {
                            if (self.netID == B)
                            {
                                var strike = StrikeNPC.strikeNPC.Find(x => x.npcID == Custom.NPCA);

                                if (strike == null)
                                {
                                    continue;
                                }

                                if (Custom.Crit)
                                {
                                    if (Damage >= Custom.Damage && Damage <= Custom.Damage2
                                        && Main.npc[strike.npcIndex].life > Custom.LifeLimit)
                                    {
                                        TransformDamage(self, Damage, plr, Custom, strike);
                                    }
                                }
                                else
                                {
                                    if (Damage >= Custom.Damage && Damage <= Custom.Damage2
                                        && Main.npc[strike.npcIndex].life > Custom.LifeLimit && !crit)
                                    {
                                        TransformDamage(self, Damage, plr, Custom, strike);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        return damage;
    }
    #endregion

    #region 对各BOSS伤害特殊处理后播报
    public static Dictionary<string, double> Destroyer = new Dictionary<string, double>(); //毁灭者
    public static Dictionary<string, double> FleshWall = new Dictionary<string, double>(); //肉山
    public static Dictionary<string, double> Eaterworld = new Dictionary<string, double>(); //世吞
    public static Dictionary<string, double> Retinazer = new Dictionary<string, double>(); // 激光眼
    public static Dictionary<string, double> Spazmatism = new Dictionary<string, double>(); // 魔焰眼
    public static Dictionary<string, double> CustomDicts = new Dictionary<string, double>(); // 自定义转移伤害的BOSS
    private void OnNpcKill(NpcKilledEventArgs args)
    {
        var strike = StrikeNPC.strikeNPC.Find(x => x.npcIndex == args.npc.whoAmI && x.npcID == args.npc.netID)!;

        if (!Config.Enabled || strike == null || !strike.PlayerOrDamage.Any())
        {
            return;
        }

        //自定义转移伤害统计
        if (Config.CustomTransfer)
        {
            foreach (var Custom in Config.TList)
            {
                var boss = StrikeNPC.strikeNPC.Find(x => x.npcID == Custom.NPCA);

                if (args.npc.netID != Custom.NPCA || boss == null)
                {
                    continue;
                }

                foreach (var sss in strikeNPC)
                {
                    if (sss.npcID == Custom.NPCA)
                    {
                        foreach (var ss in boss.PlayerOrDamage)
                        {
                            UpdateDict(CustomDicts, ss.Key, ss.Value);
                        }
                    }

                    if (Custom.SettlementDamage)
                    {
                        foreach (var B in Custom.NPCB)
                        {
                            if (sss.npcID == B)
                            {
                                foreach (var ss in boss.PlayerOrDamage)
                                {
                                    UpdateDict(CustomDicts, ss.Key, ss.Value);
                                }
                            }
                        }
                    }
                }

                var num = CustomDicts.Values.Sum();

                if (Custom.Mess)
                {
                    SendKillMessage(args.npc.FullName, CustomDicts, num);
                }

                strikeNPC.RemoveAll(x => x.npcID == Custom.NPCA || Custom.NPCB.Contains(x.npcID) || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                CustomDicts.Clear();
                return;
            }
        }

        //毁灭者的处理
        if (args.npc.netID == 134)
        {
            if (Main.zenithWorld && Config.MechQueen)
            {
                return;
            }

            foreach (var sss in strikeNPC)
            {
                if (sss.npcID == 134 || sss.npcID == 135 || sss.npcID == 136)
                {
                    foreach (var ss in sss.PlayerOrDamage)
                    {
                        UpdateDict(Destroyer, ss.Key, ss.Value);
                    }
                }
            }

            var sum = Destroyer.Values.Sum();
            SendKillMessage(args.npc.FullName, Destroyer, sum);
            Destroyer.Clear();
            strikeNPC.RemoveAll(x => x.npcID == 134 || x.npcID == 136 || x.npcID == 135 ||
            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
            return;
        }

        //肉山和嘴巴
        else if (args.npc.netID == 113)
        {
            foreach (var sss in strikeNPC)
            {
                if (sss.npcID == 113 || sss.npcID == 114)
                {
                    foreach (var ss in sss.PlayerOrDamage)
                    {
                        UpdateDict(FleshWall, ss.Key, ss.Value);
                    }
                }

                //如果是For the worthy或天顶种子，把小鬼和饿鬼的伤害加算到肉山身上（并排除雕像怪）
                else if (Config.FireImp && (Main.getGoodWorld || Main.zenithWorld) &&
                    !args.npc.SpawnedFromStatue && (sss.npcID == 24 || sss.npcID == 115 || sss.npcID == 116))
                {
                    foreach (var ss in sss.PlayerOrDamage)
                    {
                        UpdateDict(FleshWall, ss.Key, ss.Value);
                    }
                }
            }
            var sum = FleshWall.Values.Sum();
            SendKillMessage(GetString("血肉墙"), FleshWall, sum);
            FleshWall.Clear();
            strikeNPC.RemoveAll(x => x.npcID == 113 || x.npcID == 114 ||
            x.npcID == 24 || x.npcID == 115 || x.npcID == 116 ||
            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
            return;
        }

        // 双子魔眼
        else if (args.npc.netID == 125 || args.npc.netID == 126)
        {
            if (Main.zenithWorld && Config.MechQueen)
            {
                return;
            }

            foreach (var sss in strikeNPC)
            {
                if ((args.npc.netID == 125 && (sss.npcID == 125 || sss.npcID == 126)) ||
                    (args.npc.netID == 126 && (sss.npcID == 125 || sss.npcID == 126)))
                {
                    foreach (var ss in sss.PlayerOrDamage)
                    {
                        if (args.npc.netID == 125)
                        {
                            UpdateDict(Retinazer, ss.Key, ss.Value);
                        }
                        else if (args.npc.netID == 126)
                        {
                            UpdateDict(Spazmatism, ss.Key, ss.Value);
                        }
                    }
                }
            }

            if ((args.npc.netID == 125 && Spazmatism.Count > 0) || (args.npc.netID == 126 && Retinazer.Count > 0))
            {
                SendKillMessage(GetString("双子魔眼"), CombineDamages(Retinazer, Spazmatism), GetCombineDamages(CombineDamages(Retinazer, Spazmatism)));
                ClearDictionaries(Retinazer, Spazmatism);
            }
            strikeNPC.RemoveAll(x => x.npcID == 125 || x.npcID == 126 ||
            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
            return;
        }

        //其他生物，对被击杀的生物进行计数
        for (var i = 0; i < strikeNPC.Count; i++)
        {
            if (strikeNPC[i].npcIndex == args.npc.whoAmI && strikeNPC[i].npcID == args.npc.netID)
            {
                switch (strikeNPC[i].npcID)
                {
                    //黑长直
                    case 13:
                    case 14:
                    case 15:
                    {
                        var flag = true;
                        foreach (var n in Main.npc)
                        {
                            if (n.whoAmI != args.npc.whoAmI && n.active &&
                               (n.netID == 13 || n.netID == 14 || n.netID == 15))
                            {
                                flag = false;
                                break;
                            }
                        }

                        foreach (var ss in strikeNPC[i].PlayerOrDamage)
                        {
                            UpdateDict(Eaterworld, ss.Key, ss.Value);
                        }

                        if (flag)
                        {
                            var sum = Eaterworld.Values.Sum();
                            SendKillMessage(args.npc.FullName, Eaterworld, sum);
                            strikeNPC.RemoveAll(x => x.npcID == 13 || x.npcID == 14 || x.npcID == 15 ||
                            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            Eaterworld.Clear();
                            return;
                        }
                    }
                    break;

                    //荷兰飞船的处理，特殊点：本体不可被击中，在其他炮塔全死亡后计入击杀
                    case 492:
                    {
                        var flag = true;
                        flag = CombDmg(args, i, flag, 491, new int[] { 492 }, 80000f);

                        if (flag)
                        {
                            var airship = strikeNPC.Find(x => x.npcID == 491);
                            if (airship == null)
                            {
                                strikeNPC.RemoveAll(x => x.npcID == 491 || x.npcID == 492 ||
                                x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                                return;
                            }
                            SendKillMessage(airship.npcName, airship.PlayerOrDamage, airship.AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 491 || x.npcID == 492 ||
                            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //火星飞碟的处理，特殊点：本体在炮塔死亡后计入击杀
                    case 392:
                    case 393:
                    case 394:
                    case 395:
                    {
                        var strike2 = strikeNPC.Find(x => x.npcID == 395);
                        CombDmg2(i, ref strike2, 395, new int[] { 392, 393, 394 }, 81000f);

                        if (strikeNPC[i].npcID == 395)
                        {
                            SendKillMessage(GetString("火星飞碟"), strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 392 || x.npcID == 393 || x.npcID == 394 ||
                            x.npcID == 395 || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //猪鲨的处理，把鲨鱼龙的伤害统计到猪鲨本体身上
                    case 370:
                    case 372:
                    case 373:
                    {
                        var strike2 = strikeNPC.Find(x => x.npcID == 370);
                        CombDmg3(Config.Sharkron, i, ref strike2, 370, new int[] { 372, 373 }, 10000f);
                        if (strikeNPC[i].npcID == 370)
                        {
                            SendKillMessage(GetString("猪龙鱼公爵"), strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 370 || x.npcID == 372 || x.npcID == 373 ||
                            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //月球领主的处理，特殊点，本体可被击中，但肢体会假死，击中肢体也应该算入本体中
                    case 398:
                    {
                        var strikenpcs = strikeNPC.FindAll(x => x.npcID == 397 || x.npcID == 396);
                        if (strikenpcs.Count > 0)
                        {
                            foreach (var v in strikenpcs)
                            {
                                foreach (var vv in v.PlayerOrDamage)
                                {
                                    if (strikeNPC[i].PlayerOrDamage.ContainsKey(vv.Key))
                                    {
                                        strikeNPC[i].PlayerOrDamage[vv.Key] += vv.Value;
                                        strikeNPC[i].AllDamage += vv.Value;
                                    }
                                    else
                                    {
                                        strikeNPC[i].PlayerOrDamage.Add(vv.Key, vv.Value);
                                        strikeNPC[i].AllDamage += vv.Value;
                                    }
                                }
                            }
                        }
                        SendKillMessage(GetString("月亮领主"), strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                        strikeNPC.RemoveAll(x => x.npcID == 398 || x.npcID == 397 || x.npcID == 396 ||
                        x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                        return;
                    }

                    //机械骷髅王的处理，特殊点，本体可能被击中，其他肢体可能会死
                    case 127:
                    case 128:
                    case 129:
                    case 130:
                    case 131:
                    {
                        if (Main.zenithWorld && Config.MechQueen)
                        {
                            return;
                        }

                        var strike2 = strikeNPC.Find(x => x.npcID == 127);
                        CombDmg3(Config.Sharkron, i, ref strike2, 127, new int[] { 128, 129, 130, 131 }, 300000f);
                        if (strikeNPC[i].npcID == 127)
                        {
                            SendKillMessage(args.npc.FullName, strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 127 || x.npcID == 128 || x.npcID == 129 ||
                            x.npcID == 130 || x.npcID == 131 || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //骷髅王的处理 把肢体受伤计算加入到本体头部中
                    case 35:
                    case 36:
                    {
                        var strike2 = strikeNPC.Find(x => x.npcID == 35);
                        CombDmg2(i, ref strike2, 35, new int[] { 36 }, 16000f);
                        if (strikeNPC[i].npcID == 35)
                        {
                            SendKillMessage(args.npc.FullName, strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 35 || x.npcID == 36 ||
                            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //石巨人的特殊处理 本体以外的肢体的伤害计算加到本体上
                    case 245:
                    case 246:
                    case 247:
                    case 248:
                    {
                        var strike2 = strikeNPC.Find(x => x.npcID == 245);
                        CombDmg2(i, ref strike2, 245, new int[] { 246, 247, 248 }, 120000f);
                        if (strikeNPC[i].npcID == 245)
                        {
                            SendKillMessage(args.npc.FullName, strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 245 || x.npcID == 246 || x.npcID == 247 ||
                            x.npcID == 248 || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;

                    //克苏鲁之脑的特殊处理
                    case 266:
                    case 267:
                    {
                        var strike2 = strikeNPC.Find(x => x.npcID == 266);
                        CombDmg2(i, ref strike2, 266, new int[] { 267 }, 125000f);
                        if (strikeNPC[i].npcID == 266)
                        {
                            SendKillMessage(args.npc.FullName, strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                            strikeNPC.RemoveAll(x => x.npcID == 266 || x.npcID == 267 ||
                            x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                            return;
                        }
                    }
                    break;
                    default:
                    {

                        if ((Main.zenithWorld && Config.MechQueen &&
                            args.npc.netID == 125) || args.npc.netID == 126 || args.npc.netID == 134 ||
                            args.npc.netID == 135 || args.npc.netID == 136 || args.npc.netID == 139)
                        {
                            continue;
                        }

                        if (Config.CustomTransfer)
                        {
                            foreach (var Custom in Config.TList)
                            {
                                if (args.npc.netID == Custom.NPCA)
                                {
                                    continue;
                                }
                            }
                        }

                        if (args.npc.boss || args.npc.netID == 551 || args.npc.netID == 668 || Config.Expand.Contains(args.npc.netID))
                        {
                            SendKillMessage(args.npc.FullName, strikeNPC[i].PlayerOrDamage, strikeNPC[i].AllDamage);
                        }
                        strikeNPC.RemoveAt(i);
                        strikeNPC.RemoveAll(x => x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                        return;
                    }
                }
            }

            if (i >= 0 && (strikeNPC[i].npcID != Main.npc[strikeNPC[i].npcIndex].netID || !Main.npc[strikeNPC[i].npcIndex].active))
            {
                strikeNPC.RemoveAt(i);
                i--;
            }
        }
    }
    #endregion

    #region 美杜莎
    public static Dictionary<string, double> MechQueen = new Dictionary<string, double>();
    private void OnMechQueen(NpcKilledEventArgs args)
    {
        if (!Config.Enabled || !Config.MechQueen)
        {
            return;
        }

        if (NPC.IsMechQueenUp || Main.zenithWorld)
        {
            for (var i = 0; i < strikeNPC.Count; i++)
            {
                if (strikeNPC[i].npcIndex == args.npc.whoAmI && strikeNPC[i].npcID == args.npc.netID)
                {
                    switch (strikeNPC[i].npcID)
                    {
                        case 125:
                        case 126:
                        case 127:
                        case 134:
                        {
                            //标识一直开启，
                            var flag = true;

                            //伤害统计法
                            foreach (var sss in strikeNPC)
                            {
                                if (sss.npcID == 125 || sss.npcID == 126 || sss.npcID == 127 || sss.npcID == 134)
                                {
                                    foreach (var ss in strikeNPC[i].PlayerOrDamage)
                                    {
                                        UpdateDict(MechQueen, ss.Key, ss.Value);
                                    }
                                }
                            }

                            //循环到没有活着的这些NPC则视为美杜莎死亡，标识自动通过
                            foreach (var n in Main.npc)
                            {
                                //如果当前NPC (n) 不是被杀死的NPC (args.npc) 并且还活着,则关闭标识
                                if (n.whoAmI != args.npc.whoAmI && n.active && IDGroup(n))
                                {
                                    flag = false;
                                    break;
                                }
                            }

                            //以上NPC全死 则发送伤害榜
                            if (flag)
                            {
                                double num = 0;
                                foreach (var Mech in MechQueen)
                                {
                                    num += Mech.Value;
                                }
                                SendKillMessage(GetString("美杜莎"), MechQueen, num);
                                strikeNPC.RemoveAll(x =>
                                x.npcID == 125 || x.npcID == 126 || x.npcID == 127 || x.npcID == 134 ||
                                x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                                MechQueen.Clear();
                                return;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    //美杜莎构成ID
    public static bool IDGroup(NPC nPC)
    {
        int[] id = { 125, 126, 127, 134 };
        return id.Contains(nPC.netID);
    }

    #endregion

}