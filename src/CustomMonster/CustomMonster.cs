using Microsoft.Xna.Framework;
using System.Timers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TerrariaApi.Server;
using TShockAPI;
using static TShockAPI.GetDataHandlers;
using Main = Terraria.Main;
using NPC = Terraria.NPC;


namespace CustomMonster;

[ApiVersion(2, 1)]
public class TestPlugin : TerrariaPlugin
{
    #region 插件信息
    public override string Author => "GK 阁下 羽学";
    public override string Description => GetString("自定义怪物出没时的血量,当然不止这些！");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 4, 41);
    #endregion

    #region 全局变量
    public string ConfigName => "CustomMonster.json";
    public string ExtraPath => System.IO.Path.Combine(TShock.SavePath, "." + this.ConfigName);
    public string Path => System.IO.Path.Combine(TShock.SavePath, this.ConfigName);
    public string NPCKillPath => System.IO.Path.Combine(TShock.SavePath, "CustomMonster.txt");
    public int Beta = 0;
    public Configuration Config = new Configuration();
    private static readonly System.Timers.Timer Update = new System.Timers.Timer(10.0);
    public static bool ULock = false;
    public int LRespawnSeconds = -1;
    public int LRespawnBossSeconds = -1;
    public int TeamP = 0;
    public DateTime NPCKillDataTime { get; set; }
    public DateTime OServerDataTime { get; set; }
    private static List<LNKC>? LNkc { get; set; }
    private static List<LSMNPC>? LLSMNPCs { get; set; }
    public static LPrj[]? LPrjs { get; set; }
    public static LNPC[]? LNpcs { get; set; }
    #endregion

    #region 注册与释放
    public TestPlugin(Main game)
    : base(game)
    {
        this.Order = 1;
        LNpcs = new LNPC[201];
        LPrjs = new LPrj[1001];
        LNkc = new List<LNKC>();
        LLSMNPCs = new List<LSMNPC>();
        this.NPCKillDataTime = DateTime.UtcNow;
        this.OServerDataTime = DateTime.UtcNow;
    }

    public override void Initialize()
    {
        this.RC();
        this.RD();
        GetDataHandlers.KillMe += this.OnKillMe!;
        ServerApi.Hooks.GamePostInitialize.Register(this, this.PostInitialize);
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.NpcSpawn.Register(this, this.NpcSpawn);
        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        ServerApi.Hooks.NpcStrike.Register(this, this.NpcStrike);
        ServerApi.Hooks.NetSendData.Register(this, this.SendData);
        On.Terraria.NPC.SetDefaults += this.OnSetDefaults;
        On.Terraria.Projectile.Kill += this.OnProjectileKill;
        On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float +=
            this.Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float;
    }

    private void OnInitialize(EventArgs args)
    {
        Commands.ChatCommands.Add(new Command("CustomMonster", this.CRC, new string[] { "重读自定义怪物血量", "reload" })
        {
            HelpText = GetString("输入 /重读自定义怪物血量 会重新读取重读自定义怪物血量表")
        });
        Commands.ChatCommands.Add(new Command("CustomMonster", this.CMD, new string[] { "召唤自定义怪物血量怪物", "smc" })
        {
            HelpText = GetString("输入 /召唤自定义怪物血量怪物 怪物标志 会召唤指定标记的怪物(无视召唤条件)")
        });

        Commands.ChatCommands.Add(new Command("CustomMonster", this.CRS, "改怪物", "ggw"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (LNkc!)
            {
                this.SD(notime: true);
            }
            Update.Elapsed -= this.OnUpdate!;
            Update.Stop();
            GetDataHandlers.KillMe -= this.OnKillMe!;
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.NpcSpawn);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NpcKilled);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.PostInitialize);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.NpcStrike);
            ServerApi.Hooks.NetSendData.Deregister(this, this.SendData);
            On.Terraria.NPC.SetDefaults -= this.OnSetDefaults;
            On.Terraria.Projectile.Kill -= this.OnProjectileKill;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -=
                this.Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float;
        }
        base.Dispose(disposing);
    }

    public void PostInitialize(EventArgs e)
    {
        Update.Elapsed += this.OnUpdate!;
        Update.Start();
        if (this.Config.Advertisement)
        {
            var x = this.Beta > 0
                ? GetString($"Beta{this.Beta} 已启动! --------")
                : GetString("已启动! --------------");
            Console.WriteLine($" ------------ {this.Name} 版本:{this.Version} {x}");
            Console.WriteLine(GetString(" >>> 如果使用过程中出现什么问题可前往QQ群232109072交流反馈."));
            Console.WriteLine(GetString(" >>> 本插件免费请勿上当受骗,您可在QQ群232109072中获取最新插件."));
            if (this.Beta > 0)
            {
                Console.WriteLine(GetString(" >>> 抢先测试版,因配置项待定后续可能会修改,所以仅仅测试使用!!!!!!"));
            }
            Console.WriteLine(" ----------------------------------------------------------------");
        }
    }
    #endregion

    #region 隐藏多余配置项的指令方法 羽学加
    private void CRS(CommandArgs args)
    {
        this.CRC(args);
        this.Config = Configuration.Read(this.Path);

        this.Config.HideUselessConfig = !this.Config.HideUselessConfig;
        this.Config.Write(this.Path);

        args.Player.SendSuccessMessage(GetString("[自定义怪物血量] 隐藏配置项:") +
            (this.Config.HideUselessConfig
                ? GetString("已隐藏")
                : GetString("已显示")));
    }
    #endregion

    #region 指令与读取配置方法
    private void RC()
    {
        try
        {
            if (!File.Exists(this.Path))
            {
                TShock.Log.ConsoleError(GetString("未找到自定义怪物血量配置，已为您创建！修改配置后重启即可重新载入数据。"));
                Configuration.WriteExample(this.Path);
            }
            this.Config = Configuration.Read(this.Path);
            var version = new Version(this.Config.Version);
            if (version <= this.Version)
            {
                this.Config.Version = this.Version.ToString();
                this.Config.Write(this.Path);
            }
            else
            {
                TShock.Log.ConsoleError(GetString("[自定义怪物血量]您载入的配置文件插件版本高于本插件版本,配置可能无法正常使用,请升级插件后使用！"));
            }

            var extraDirPath = System.IO.Path.GetDirectoryName(this.ExtraPath);
            if (!Directory.Exists(extraDirPath))
            {
                Directory.CreateDirectory(extraDirPath!);
            }

            var directoryInfo = new DirectoryInfo(extraDirPath!);
            var files = directoryInfo.GetFiles("*" + this.ConfigName);
            for (var i = 0; i < files.Length; i++)
            {
                var fullName = files[i].FullName;
                Console.WriteLine(GetString("[自定义怪物血量] 读入额外配置:") + files[i].Name);
                try
                {
                    var config = Configuration.Read(fullName);
                    version = new Version(config.Version);
                    if (version <= this.Version)
                    {
                        config.Version = this.Version.ToString();
                        config.Write(fullName);
                    }
                    else
                    {
                        TShock.Log.ConsoleError(GetString($"[自定义怪物血量]您读入的额外配置 {files[i].Name} 插件版本高于本插件版本,配置可能无法正常使用,请升级插件后使用！"));
                    }
                    var num = config.MonsterGroup.Length;
                    var num2 = this.Config.MonsterGroup.Length;
                    if (num > 0)
                    {
                        var array = new MonsterGroup[num2 + num];
                        Array.Copy(this.Config.MonsterGroup, 0, array, 0, num2);
                        Array.Copy(config.MonsterGroup, 0, array, num2, num);
                        this.Config.MonsterGroup = array;
                    }
                    if (config.IgnoreMonsterTable.Count > 0)
                    {
                        this.Config.IgnoreMonsterTable = this.Config.IgnoreMonsterTable.Union(config.IgnoreMonsterTable).ToList();
                    }
                    Console.WriteLine(GetString($"[自定义怪物血量] 额外配置[{files[i].Name}]增添了{num}条配置"));
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError(GetString($"[自定义怪物血量] 额外配置[{files[i].Name}]错误:\n{ex}\n"));
                }
            }
        }
        catch (Exception ex2)
        {
            TShock.Log.ConsoleError(GetString($"[自定义怪物血量] 配置错误:\n{ex2}\n"));
        }
    }

    private void CRC(CommandArgs args)
    {
        this.RC();
        LLSMNPCs!.Clear();
        args.Player.SendSuccessMessage(args.Player.Name + GetString("[自定义怪物血量]配置重读完毕。"));
    }

    private void CMD(CommandArgs args)
    {
        if (args.Parameters.Count < 1 || args.Parameters.Count > 3)
        {
            args.Player.SendErrorMessage(GetString("格式错误,正确格式是:"));
            args.Player.SendErrorMessage(GetString(" /召唤自定义怪物血量怪物 怪物标志 "));
            args.Player.SendErrorMessage(GetString(" /召唤自定义怪物血量怪物 怪物标志 数量"));
            args.Player.SendErrorMessage(GetString(" /召唤自定义怪物血量怪物 怪物标志 X像素坐标 Y像素坐标"));
            return;
        }
        if (args.Parameters[0].Length == 0)
        {
            args.Player.SendErrorMessage(GetString("无效标志"));
            return;
        }
        var result = 1;
        if (args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out result))
        {
            args.Player.SendErrorMessage(GetString("数量错误,正确格式是: /召唤自定义怪物血量怪物 怪物标志 召唤数量"));
            return;
        }
        var result2 = -1;
        var result3 = -1;
        if (args.Parameters.Count == 3 && !int.TryParse(args.Parameters[1], out result2) && !int.TryParse(args.Parameters[2], out result3))
        {
            args.Player.SendErrorMessage(GetString("坐标错误,正确格式是: /召唤自定义怪物血量怪物 怪物标志 X像素坐标 Y像素坐标(无法指定肉山)"));
            return;
        }
        result = Math.Min(result, 200);
        var MonsterGroups = this.Config.MonsterGroup;
        foreach (var MonsterGroup in MonsterGroups)
        {
            if (!(MonsterGroup.Sign == args.Parameters[0]) || MonsterGroup.NPCID == 0)
            {
                continue;
            }
            var nPCById = TShock.Utils.GetNPCById(MonsterGroup.NPCID);
            if (nPCById == null)
            {
                args.Player.SendErrorMessage(GetString("无效的空NPCID!"));
            }
            else if (nPCById.type >= 1 && nPCById.type < NPCID.Count && nPCById.type != 113)
            {
                LLSMNPCs!.Add(new LSMNPC(nPCById.type, MonsterGroup.Sign));
                if (result2 < 0 || result3 < 0)
                {
                    TSPlayer.Server.SpawnNPC(nPCById.netID, nPCById.FullName, result, args.Player.TileX, args.Player.TileY, 50, 20);
                }
                else
                {
                    Sundry.LaunchProjectileSpawnNPC(nPCById.netID, result2, result3);
                }
                if (args.Silent)
                {
                    args.Player.SendSuccessMessage(GetString("召唤 {0} {1} 次."), new object[2] { nPCById.FullName, result });
                }
                else
                {
                    TSPlayer.All.SendSuccessMessage(GetString("{0} 召唤 {1} {2} 次."), new object[3]
                    {
                        args.Player.Name,
                        nPCById.FullName,
                        result
                    });
                }
            }
            else if (nPCById.type == 113)
            {
                if (Main.wofNPCIndex != -1 || args.Player.Y / 16f < Main.maxTilesY - 205)
                {
                    args.Player.SendErrorMessage(GetString("无法根据肉墙的当前状态或您的当前位置生成肉墙。"));
                    return;
                }
                LLSMNPCs!.Add(new LSMNPC(nPCById.type, MonsterGroup.Sign));
                NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
                if (args.Silent)
                {
                    args.Player.SendSuccessMessage(GetString("肉墙召唤成功."));
                    return;
                }
                TSPlayer.All.SendSuccessMessage(GetString("{0} 召唤了肉墙."), new object[1] { args.Player.Name });
            }
            else
            {
                args.Player.SendErrorMessage(GetString("错误的召唤过程."));
            }
            return;
        }
        args.Player.SendErrorMessage(GetString("没有这个标记的怪物"));
    }
    #endregion

    #region 纠正弹幕伤害方法
    private int Projectile_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
    {
        if (Owner == Main.myPlayer && this.Config.UnifiedMonsterProjectileDamageCorrection != 1f)
        {
            Damage = (int) (Damage * this.Config.UnifiedMonsterProjectileDamageCorrection);
        }
        return orig.Invoke(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
    }
    #endregion

    #region NPC设为默认
    private void OnSetDefaults(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, NPCSpawnParams spawnparams)
    {
        if (this.Config.UnifiedInitialMonsterPlayerCoefficient > 0 || this.Config.UnifiedInitialMonsterEnhancementCoefficient > 0f)
        {
            if (this.Config.UnifiedInitialMonsterPlayerCoefficient > 0)
            {
                spawnparams.playerCountForMultiplayerDifficultyOverride = this.Config.UnifiedInitialMonsterPlayerCoefficient;
                if (spawnparams.playerCountForMultiplayerDifficultyOverride > 1000)
                {
                    spawnparams.playerCountForMultiplayerDifficultyOverride = 1000;
                }
                if (this.Config.UnifiedInitialPlayerCoefficientNotLessThanTheNumberOfPlayers)
                {
                    var activePlayerCount = TShock.Utils.GetActivePlayerCount();
                    if (spawnparams.playerCountForMultiplayerDifficultyOverride < activePlayerCount)
                    {
                        spawnparams.playerCountForMultiplayerDifficultyOverride = activePlayerCount;
                    }
                }
            }
            if (this.Config.UnifiedInitialMonsterEnhancementCoefficient > 0f)
            {
                spawnparams.strengthMultiplierOverride = this.Config.UnifiedInitialMonsterEnhancementCoefficient;
                if (spawnparams.strengthMultiplierOverride > 1000f)
                {
                    spawnparams.strengthMultiplierOverride = 1000f;
                }
            }
        }
        orig.Invoke(self, Type, spawnparams);
    }
    #endregion

    #region 怪物生成事件
    private void NpcSpawn(NpcSpawnEventArgs args)
    {
        if (args.Handled || Main.npc[args.NpcId] == null || Main.npc[args.NpcId].netID == 0 || !Main.npc[args.NpcId].active)
        {
            return;
        }
        var flag = false;
        var activePlayerCount = TShock.Utils.GetActivePlayerCount();
        var lifeMax = Main.npc[args.NpcId].lifeMax;
        MonsterGroup? config = null;
        var maxtime = 0;
        var lNKC = getLNKC(Main.npc[args.NpcId].netID);
        var num = 0;
        var num2 = 0;
        var strengthMultiplier = Main.npc[args.NpcId].strengthMultiplier;
        var npc = Main.npc;
        foreach (var val in npc)
        {
            if (val.netID == Main.npc[args.NpcId].netID)
            {
                num++;
            }
        }
        var random = new Random();
        var MonsterGroups = this.Config.MonsterGroup;
        foreach (var MonGroup in MonsterGroups)
        {
            if (Main.npc[args.NpcId].netID != MonGroup.NPCID && MonGroup.NPCID != 0)
            {
                continue;
            }
            var num3 = -1;
            for (var k = 0; k < LLSMNPCs!.Count; k++)
            {
                if (LLSMNPCs[k].ID == MonGroup.NPCID || LLSMNPCs[k].M == MonGroup.Sign)
                {
                    num3 = k;
                    break;
                }
            }
            if (num3 != -1)
            {
                LLSMNPCs.RemoveAt(num3);
            }
            else
            {
                if ((MonGroup.NPCID == 0 && MonGroup.Rematching.Count() > 0 && !MonGroup.Rematching.Contains(Main.npc[args.NpcId].netID)) || (MonGroup.NPCID == 0 && MonGroup.AdditionalMatching.Count() > 0 && MonGroup.AdditionalMatching.Contains(Main.npc[args.NpcId].netID)))
                {
                    continue;
                }
                num2 = (MonGroup.StartServerTimeType == 1) ? ((int) (DateTime.UtcNow - this.OServerDataTime).TotalDays) : ((MonGroup.StartServerTimeType != 2) ? ((int) (DateTime.UtcNow - this.OServerDataTime).TotalHours) : ((int) (DateTime.UtcNow.Date - this.OServerDataTime.Date).TotalDays));
                if ((MonGroup.Difficulty.Length != 0 && !MonGroup.Difficulty.Contains(Main.GameMode)) || Sundry.SeedRequirement(MonGroup.MapSeed))
                {
                    continue;
                }
                if (MonGroup.KilledStack != 0)
                {
                    if (MonGroup.KilledStack > 0)
                    {
                        if (lNKC < MonGroup.KilledStack)
                        {
                            continue;
                        }
                    }
                    else if (lNKC >= Math.Abs(MonGroup.KilledStack))
                    {
                        continue;
                    }
                }
                if (MonGroup.QuantityCondition != 0)
                {
                    if (MonGroup.QuantityCondition > 0)
                    {
                        if (num < MonGroup.QuantityCondition)
                        {
                            continue;
                        }
                    }
                    else if (num >= Math.Abs(MonGroup.QuantityCondition))
                    {
                        continue;
                    }
                }
                if (MonGroup.PlayerCount != 0)
                {
                    if (MonGroup.PlayerCount > 0)
                    {
                        if (activePlayerCount < MonGroup.PlayerCount)
                        {
                            continue;
                        }
                    }
                    else if (activePlayerCount >= Math.Abs(MonGroup.PlayerCount))
                    {
                        continue;
                    }
                }
                if (MonGroup.StastServer != 0)
                {
                    if (MonGroup.StastServer > 0)
                    {
                        if (num2 < MonGroup.StastServer)
                        {
                            continue;
                        }
                    }
                    else if (num2 >= Math.Abs(MonGroup.StastServer))
                    {
                        continue;
                    }
                }
                if ((MonGroup.DayAndNight == -1 && Main.dayTime) || (MonGroup.DayAndNight == 1 && !Main.dayTime) || (MonGroup.Rain == -1 && Main.raining) || (MonGroup.Rain == 1 && !Main.raining) || (MonGroup.BloodMoon == -1 && Main.bloodMoon) || (MonGroup.BloodMoon == 1 && !Main.bloodMoon) || (MonGroup.Eclipse == -1 && Main.eclipse) || (MonGroup.Eclipse == 1 && !Main.eclipse) || (MonGroup.HardMode == -1 && Main.hardMode) || (MonGroup.HardMode == 1 && !Main.hardMode) || (MonGroup.DownedGolemBoss == -1 && NPC.downedGolemBoss) || (MonGroup.DownedGolemBoss == 1 && !NPC.downedGolemBoss) || (MonGroup.DownedMoonlord == -1 && NPC.downedMoonlord) || (MonGroup.DownedMoonlord == 1 && !NPC.downedMoonlord) || MonGroup.AppearanceRateNumerator <= 0 || MonGroup.AppearanceRateDenominator <= 0 || (MonGroup.AppearanceRateNumerator < MonGroup.AppearanceRateDenominator && random.Next(1, MonGroup.AppearanceRateDenominator + 1) > MonGroup.AppearanceRateNumerator) || Sundry.NPCKillRequirement(MonGroup.KilledNPC) || Sundry.MonsterRequirement(MonGroup.MonsterCondition, Main.npc[args.NpcId]))
                {
                    continue;
                }
            }
            if (this.Config.MonsterTimeLimit && (MonGroup.PersonSecondCoefficient != 0 || MonGroup.AppearanceSeconds != 0 || MonGroup.ServiceOpenSecond != 0 || MonGroup.KillCountSecond != 0))
            {
                var num4 = activePlayerCount * MonGroup.PersonSecondCoefficient;
                num4 += MonGroup.AppearanceSeconds;
                num4 += num2 * MonGroup.ServiceOpenSecond;
                num4 += (int) lNKC * MonGroup.KillCountSecond;
                if (num4 < 1)
                {
                    args.Handled = true;
                    Main.npc[args.NpcId].active = false;
                    Console.WriteLine(GetString($"{Main.npc[args.NpcId].FullName}定义时间过小被阻止生成"));
                    return;
                }
                maxtime = num4;
            }
            if (MonGroup.InitialPlayerCoefficient > 0 || MonGroup.InitialEnhancementCoefficient > 0f)
            {
                NPCSpawnParams val2 = default;
                if (MonGroup.InitialPlayerCoefficient > 0)
                {
                    val2.playerCountForMultiplayerDifficultyOverride = MonGroup.InitialPlayerCoefficient;
                    if (val2.playerCountForMultiplayerDifficultyOverride > 1000)
                    {
                        val2.playerCountForMultiplayerDifficultyOverride = 1000;
                    }
                }
                if (MonGroup.InitialEnhancementCoefficient > 0f)
                {
                    val2.strengthMultiplierOverride = MonGroup.InitialEnhancementCoefficient;
                    if (val2.strengthMultiplierOverride > 1000f)
                    {
                        val2.strengthMultiplierOverride = 1000f;
                    }
                }
                Main.npc[args.NpcId].SetDefaults(Main.npc[args.NpcId].type, val2);
                lifeMax = Main.npc[args.NpcId].lifeMax;
                strengthMultiplier = Main.npc[args.NpcId].strengthMultiplier;
            }
            if (MonGroup.InitialDamageCorrectionForMonsters == 1f)
            {
                var obj = Main.npc[args.NpcId];
                obj.takenDamageMultiplier *= MonGroup.InitialDamageCorrectionForMonsters;
            }
            if (MonGroup.SetAsBoss == -1)
            {
                Main.npc[args.NpcId].boss = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.SetAsBoss == 1)
            {
                Main.npc[args.NpcId].boss = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.ImmuneToLava == -1)
            {
                Main.npc[args.NpcId].lavaImmune = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.ImmuneToLava == 1)
            {
                Main.npc[args.NpcId].lavaImmune = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.ImmuneToTraps == -1)
            {
                Main.npc[args.NpcId].trapImmune = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.ImmuneToTraps == 1)
            {
                Main.npc[args.NpcId].trapImmune = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.CanPassThroughWalls == -1)
            {
                Main.npc[args.NpcId].noTileCollide = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.CanPassThroughWalls == 1)
            {
                Main.npc[args.NpcId].noTileCollide = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.IgnoreGravity == -1)
            {
                Main.npc[args.NpcId].noGravity = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.IgnoreGravity == 1)
            {
                Main.npc[args.NpcId].noGravity = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.MonsterGodMode == -1)
            {
                Main.npc[args.NpcId].immortal = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.MonsterGodMode == 1)
            {
                Main.npc[args.NpcId].immortal = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.AIMechanism >= 0 && MonGroup.AIMechanism != 27)
            {
                Main.npc[args.NpcId].aiStyle = MonGroup.AIMechanism;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.ModifyDefense)
            {
                Main.npc[args.NpcId].defDefense = MonGroup.MonsterDefense;
                Main.npc[args.NpcId].defense = MonGroup.MonsterDefense;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (MonGroup.PlayerCoefficient != 0 || MonGroup.NPCLife != 0 || MonGroup.ServiceOpenCoefficient != 0 || MonGroup.KillCountCoefficient != 0)
            {
                var num5 = activePlayerCount * MonGroup.PlayerCoefficient;
                num5 += MonGroup.NPCLife;
                num5 += num2 * MonGroup.ServiceOpenCoefficient;
                num5 += (int) lNKC * MonGroup.KillCountCoefficient;
                if (!MonGroup.OverrideOriginalLife)
                {
                    num5 += Main.npc[args.NpcId].lifeMax;
                }
                if (num5 < 1)
                {
                    num5 = 1;
                }
                if (Main.npc[args.NpcId].lifeMax < num5 || !MonGroup.NotLowerThanNormal)
                {
                    Main.npc[args.NpcId].lifeMax = num5;
                    flag = true;
                }
            }
            if (MonGroup.PlayerEnhancementCoefficient != 0f || MonGroup.EnhancementCoefficient != 0f || MonGroup.ServiceOpenEnhancementCoefficient != 0f || MonGroup.KillCountEnhancementCoefficient != 0f)
            {
                var num6 = activePlayerCount * MonGroup.PlayerEnhancementCoefficient;
                num6 += MonGroup.EnhancementCoefficient;
                num6 += num2 * MonGroup.ServiceOpenEnhancementCoefficient;
                num6 += (int) lNKC * MonGroup.KillCountEnhancementCoefficient;
                if (!MonGroup.OverrideOriginalEnhancement)
                {
                    num6 += Main.npc[args.NpcId].strengthMultiplier;
                }
                if (num6 > 1000f)
                {
                    num6 = 1000f;
                }
                if (num6 < 0f)
                {
                    num6 = 0f;
                }
                if ((!(Main.npc[args.NpcId].strengthMultiplier >= num6) || !MonGroup.NotLessThanNormal) && num6 > 0f)
                {
                    Main.npc[args.NpcId].strengthMultiplier = num6;
                    flag = true;
                }
            }
            if (MonGroup.CustomPrefix != "")
            {
                Main.npc[args.NpcId]._givenName = MonGroup.CustomPrefix;
                Main.npc[args.NpcId].netUpdate = true;
            }
            config = MonGroup;
            break;
        }
        if (config == null)
        {
            return;
        }
        if (!this.Config.IgnoreMonsterTable.Contains(Main.npc[args.NpcId].netID))
        {
            if (this.Config.UnifiedMonsterLifeMaxMultiplier != 1.0 && this.Config.UnifiedMonsterLifeMaxMultiplier > 0.0)
            {
                Main.npc[args.NpcId].lifeMax = (int) (Main.npc[args.NpcId].lifeMax * this.Config.UnifiedMonsterLifeMaxMultiplier);
                if (Main.npc[args.NpcId].lifeMax < 1)
                {
                    Main.npc[args.NpcId].lifeMax = 1;
                }
                flag = true;
            }
            if (Main.npc[args.NpcId].lifeMax < lifeMax && this.Config.UnifiedLifeNotLowerThanNormal)
            {
                Main.npc[args.NpcId].lifeMax = lifeMax;
                flag = false;
            }
            if (this.Config.UnifiedMonsterEnhancementFactor != 1.0 && this.Config.UnifiedMonsterEnhancementFactor > 0.0)
            {
                var num7 = (float) (Main.npc[args.NpcId].strengthMultiplier * this.Config.UnifiedMonsterEnhancementFactor);
                if (num7 > 1000f)
                {
                    num7 = 1000f;
                }
                if (num7 > 0f)
                {
                    Main.npc[args.NpcId].strengthMultiplier = num7;
                    flag = false;
                }
            }
            if (Main.npc[args.NpcId].strengthMultiplier < strengthMultiplier && this.Config.UnifiedReinforcementNotLowerThanNormal)
            {
                Main.npc[args.NpcId].strengthMultiplier = strengthMultiplier;
                flag = false;
            }
            if (this.Config.UnifiedMonsterImmuneLava == 1)
            {
                Main.npc[args.NpcId].lavaImmune = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (this.Config.UnifiedMonsterImmuneLava == -1)
            {
                Main.npc[args.NpcId].lavaImmune = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (this.Config.UnifiedMonsterImmuneTrap == 1)
            {
                Main.npc[args.NpcId].trapImmune = true;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (this.Config.UnifiedMonsterImmuneTrap == -1)
            {
                Main.npc[args.NpcId].trapImmune = false;
                Main.npc[args.NpcId].netUpdate = true;
            }
            if (this.Config.UnifiedDamageCorrectionForMonsters != 1f)
            {
                var obj2 = Main.npc[args.NpcId];
                obj2.takenDamageMultiplier *= this.Config.UnifiedDamageCorrectionForMonsters;
            }
        }
        if (flag)
        {
            Main.npc[args.NpcId].life = Main.npc[args.NpcId].lifeMax;
            Main.npc[args.NpcId].netUpdate = true;
        }
        lock (LNpcs!)
        {
            LNpcs[args.NpcId] = new LNPC(args.NpcId, activePlayerCount, lifeMax, config!, maxtime, num2, lNKC);
        }
    }
    #endregion

    #region 发送数据
    private void SendData(SendDataEventArgs args)
    {
        if (args.Handled || (int) args.MsgId != 65 || args.number5 != 0 || args.number != 0)
        {
            return;
        }

        var flag = false;
        var npc = Main.npc;
        foreach (var val in npc)
        {
            if (val == null || !val.active)
            {
                continue;
            }
            lock (LNpcs!)
            {
                var lNPC = LNpcs[val.whoAmI];
                if (lNPC == null || lNPC.Config == null || lNPC.BlockTeleporter != 1)
                {
                    continue;
                }
                flag = true;
                break;
            }
        }
        if (flag)
        {
            args.Handled = true;
        }
    }
    #endregion

    #region 伤害NPC事件
    private void NpcStrike(NpcStrikeEventArgs args)
    {
        if (args.Handled || args.Damage < 0 || args.Npc.netID == 0 || !args.Npc.active)
        {
            return;
        }
        lock (LNpcs!)
        {
            if (LNpcs[args.Npc.whoAmI] != null && LNpcs[args.Npc.whoAmI].Config != null)
            {
                LNpcs[args.Npc.whoAmI].Struck++;
            }
        }
    }
    #endregion

    #region 杀死NPC事件
    private void NpcKilled(NpcKilledEventArgs args)
    {
        if (args.npc.netID == 0)
        {
            return;
        }
        lock (LNpcs!)
        {
            if (LNpcs[args.npc.whoAmI] != null)
            {
                if (LNpcs[args.npc.whoAmI].Config != null)
                {
                    if (!LNpcs[args.npc.whoAmI].Config!.DoNotAnnounceInfo)
                    {
                        TShock.Utils.Broadcast(GetString($"攻略成功: {args.npc.FullName} 已被击败."), Convert.ToByte(130), Convert.ToByte(50), Convert.ToByte(230));
                    }
                    foreach (var item in LNpcs[args.npc.whoAmI].Config!.DeathAudio)
                    {
                        if (item.SoundID >= 0)
                        {
                            NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(args.npc.Center, 1, item.SoundID, item.SoundSize, item.HighPitch), -1, -1);
                        }
                    }
                    foreach (var item2 in LNpcs[args.npc.whoAmI].Config!.DeathCommands)
                    {
                        Commands.HandleCommand(TSPlayer.Server, TShock.Config.Settings.CommandSilentSpecifier + LNpcs[args.npc.whoAmI].ReplaceMarkers(item2));
                    }
                    if (LNpcs[args.npc.whoAmI].Config!.DeathBroadcast != "")
                    {
                        TShock.Utils.Broadcast((LNpcs[args.npc.whoAmI].Config!.DeathBroadcastHeadless ? "" : (args.npc.FullName + ": ")) + LNpcs[args.npc.whoAmI].Config!.DeathBroadcast, Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                    }
                    var lNKC = getLNKC(args.npc.netID);
                    var num = LNpcs[args.npc.whoAmI].Config!.DropGroupLimit;
                    var rd = new Random();
                    foreach (var item3 in LNpcs[args.npc.whoAmI].Config!.ExtraItemDrops)
                    {
                        if (num <= 0)
                        {
                            break;
                        }
                        if ((item3.Difficulty.Length != 0 && !item3.Difficulty.Contains(Main.GameMode)) || Sundry.SeedRequirement(item3.MapSeed))
                        {
                            continue;
                        }
                        if (item3.KilledStack != 0)
                        {
                            if (item3.KilledStack > 0)
                            {
                                if (lNKC < item3.KilledStack)
                                {
                                    continue;
                                }
                            }
                            else if (lNKC >= Math.Abs(item3.KilledStack))
                            {
                                continue;
                            }
                        }
                        if (item3.Strike != 0)
                        {
                            if (item3.Strike > 0)
                            {
                                if (LNpcs[args.npc.whoAmI].Struck < item3.Strike)
                                {
                                    continue;
                                }
                            }
                            else if (LNpcs[args.npc.whoAmI].Struck >= Math.Abs(item3.Strike))
                            {
                                continue;
                            }
                        }
                        if (item3.PlayerCount != 0)
                        {
                            if (item3.PlayerCount > 0)
                            {
                                if (LNpcs[args.npc.whoAmI].PlayerCount < item3.PlayerCount)
                                {
                                    continue;
                                }
                            }
                            else if (LNpcs[args.npc.whoAmI].PlayerCount >= Math.Abs(item3.PlayerCount))
                            {
                                continue;
                            }
                        }
                        if (item3.TimeConsume != 0)
                        {
                            if (item3.TimeConsume > 0)
                            {
                                if (LNpcs[args.npc.whoAmI].TiemN < item3.TimeConsume)
                                {
                                    continue;
                                }
                            }
                            else if (LNpcs[args.npc.whoAmI].TiemN >= Math.Abs(item3.TimeConsume))
                            {
                                continue;
                            }
                        }
                        if (item3.KilledCount != 0)
                        {
                            if (item3.KilledCount > 0)
                            {
                                if (LNpcs[args.npc.whoAmI].KillPlay < item3.KilledCount)
                                {
                                    continue;
                                }
                            }
                            else if (LNpcs[args.npc.whoAmI].KillPlay >= Math.Abs(item3.KilledCount))
                            {
                                continue;
                            }
                        }
                        if (item3.StastServer != 0)
                        {
                            if (item3.StastServer > 0)
                            {
                                if (LNpcs[args.npc.whoAmI].OSTime < item3.StastServer)
                                {
                                    continue;
                                }
                            }
                            else if (LNpcs[args.npc.whoAmI].OSTime >= Math.Abs(item3.StastServer))
                            {
                                continue;
                            }
                        }
                        if ((item3.DayAndNight == -1 && Main.dayTime) || (item3.DayAndNight == 1 && !Main.dayTime) || (item3.BloodMoon == -1 && Main.bloodMoon) || (item3.BloodMoon == 1 && !Main.bloodMoon) || (item3.Eclipse == -1 && Main.eclipse) || (item3.Eclipse == 1 && !Main.eclipse) || (item3.Rain == -1 && Main.raining) || (item3.Rain == 1 && !Main.raining) || (item3.HardMode == -1 && Main.hardMode) || (item3.HardMode == 1 && !Main.hardMode) || (item3.DownedGolemBoss == -1 && NPC.downedGolemBoss) || (item3.DownedGolemBoss == 1 && !NPC.downedGolemBoss) || (item3.DownedMoonlord == -1 && NPC.downedMoonlord) || (item3.DownedMoonlord == 1 && !NPC.downedMoonlord) || item3.Numerator <= 0 || item3.Denominator <= 0 || (item3.Numerator < item3.Denominator && rd.Next(1, item3.Denominator + 1) > item3.Numerator) || Sundry.NPCKillRequirement(item3.KilledNPC) || !LNpcs[args.npc.whoAmI].haveMarkers(item3.Indicator, args.npc) || Sundry.AIRequirement(item3.AI, args.npc) || Sundry.MonsterRequirement(item3.MonsterCondition, args.npc))
                        {
                            continue;
                        }
                        foreach (var item4 in item3.ItemDrop)
                        {
                            if (item4.ItemStack <= 0 || item4.ItemID <= 0)
                            {
                                continue;
                            }
                            if (item4.ItemPrefix >= 0)
                            {
                                if (item4.DropItemAlone)
                                {
                                    args.npc.DropItemInstanced(args.npc.Center, args.npc.Size, item4.ItemID, item4.ItemStack, true);
                                    continue;
                                }
                                var num2 = Item.NewItem(new EntitySource_DebugCommand(), args.npc.Center, args.npc.Size, item4.ItemID, item4.ItemStack, false, item4.ItemPrefix, false, false);
                                NetMessage.TrySendData(21, -1, -1, null, num2, 0f, 0f, 0f, 0, 0, 0);
                            }
                            else if (item4.DropItemAlone)
                            {
                                args.npc.DropItemInstanced(args.npc.Center, args.npc.Size, item4.ItemID, item4.ItemStack, true);
                            }
                            else
                            {
                                var num3 = Item.NewItem(new EntitySource_DebugCommand(), args.npc.Center, args.npc.Size, item4.ItemID, item4.ItemStack, false, 0, false, false);
                                NetMessage.TrySendData(21, -1, -1, null, num3, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                        if (item3.Broadcast != "")
                        {
                            TShock.Utils.Broadcast((item3.BroadcastHeadless ? "" : (args.npc.FullName + ": ")) + item3.Broadcast, Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                        }
                        if (item3.DontDrop)
                        {
                            break;
                        }
                        num--;
                    }
                    Sundry.SetMonsterMarkers(LNpcs[args.npc.whoAmI].Config!.DeathMonsterIndicatorModifications, args.npc, ref rd);
                    Sundry.HurtMonster(LNpcs[args.npc.whoAmI].Config!.DeathMonsterStrike, args.npc);
                    Sundry.LaunchProjectile(LNpcs[args.npc.whoAmI].Config!.DeathProjectile, args.npc, LNpcs[args.npc.whoAmI]);
                    foreach (var item5 in LNpcs[args.npc.whoAmI].Config!.DyingWordsMonsters)
                    {
                        if (item5.Value >= 1 && item5.Key != 0)
                        {
                            var num4 = Math.Min(item5.Value, 200);
                            var nPCById = TShock.Utils.GetNPCById(item5.Key);
                            if (nPCById != null && nPCById.type != 113 && nPCById.type != 0 && nPCById.type < NPCID.Count)
                            {
                                TSPlayer.Server.SpawnNPC(nPCById.type, nPCById.FullName, num4, Terraria.Utils.ToTileCoordinates(args.npc.Center).X, Terraria.Utils.ToTileCoordinates(args.npc.Center).Y, 3, 3);
                            }
                        }
                    }
                    if (LNpcs[args.npc.whoAmI].Config!.DeathBuffRange > 0)
                    {
                        var players = TShock.Players;
                        foreach (var val in players)
                        {
                            if (val == null || val.Dead || val.TPlayer.statLife < 1 || !Sundry.WithinRange(val.TPlayer.Center, args.npc.Center, LNpcs[args.npc.whoAmI].Config!.DeathBuffRange * 16))
                            {
                                continue;
                            }
                            foreach (var item6 in LNpcs[args.npc.whoAmI].Config!.DeathBuff)
                            {
                                if (item6.Value > 0)
                                {
                                    val.SetBuff(item6.Key, item6.Value * 60, false);
                                }
                            }
                        }
                    }
                }
                LNpcs[args.npc.whoAmI] = null!;
            }
        }
        this.addLNKC(args.npc.netID);
    }
    #endregion

    #region 计时器更新方法
    public void OnUpdate(object sender, ElapsedEventArgs e)
    {
        if (ULock)
        {
            return;
        }
        ULock = true;
        var now = DateTime.Now;
        var rd = new Random();
        Main.rand ??= new UnifiedRandom();
        try
        {
            var activePlayerCount = TShock.Utils.GetActivePlayerCount();
            var num = 0;
            var npc = Main.npc;
            foreach (var val in npc)
            {
                if (val != null && val.active)
                {
                    num++;
                }
            }
            var flag = false;
            var dict = new Dictionary<int, int>();
            var dict2 = dict;
            var npc2 = Main.npc;
            foreach (var val2 in npc2)
            {
                if (activePlayerCount <= 0 || val2 == null || !val2.active)
                {
                    continue;
                }
                if (val2.boss)
                {
                    flag = true;
                }
                lock (LNpcs!)
                {
                    var lNPC = LNpcs[val2.whoAmI];
                    if (lNPC != null && lNPC.Config != null)
                    {
                        dict2.Add(val2.whoAmI, lNPC.Config.EventWeight);
                    }
                }
            }
            var orderedEnumerable = dict2.OrderByDescending(delegate (KeyValuePair<int, int> objDic)
            {
                var keyValuePair = objDic;
                return keyValuePair.Value;
            });
            foreach (var item in orderedEnumerable)
            {
                if (activePlayerCount <= 0)
                {
                    continue;
                }
                var val3 = Main.npc[item.Key];
                if (val3 == null || !val3.active)
                {
                    continue;
                }
                if (Timeout(now))
                {
                    ULock = false;
                    return;
                }
                lock (LNpcs!)
                {
                    var lNPC2 = LNpcs[val3.whoAmI];
                    if (lNPC2 == null || lNPC2.Config == null)
                    {
                        continue;
                    }
                    if (lNPC2.PlayerCount < activePlayerCount)
                    {
                        lNPC2.PlayerCount = activePlayerCount;
                        if (this.Config.DynamicTimeLimit && (lNPC2.Config.PersonSecondCoefficient != 0 || lNPC2.Config.AppearanceSeconds != 0 || lNPC2.Config.ServiceOpenSecond != 0 || lNPC2.Config.KillCountSecond != 0))
                        {
                            var num2 = lNPC2.PlayerCount * lNPC2.Config.PersonSecondCoefficient;
                            num2 += lNPC2.Config.AppearanceSeconds;
                            var num3 = lNPC2.PlayerCount * lNPC2.Config.PersonSecondCoefficient;
                            num3 += lNPC2.Config.AppearanceSeconds;
                            num3 += lNPC2.OSTime * lNPC2.Config.ServiceOpenSecond;
                            num3 += (int) lNPC2.LKC * lNPC2.Config.KillCountSecond;
                            if (num3 < 1)
                            {
                                num3 = -1;
                            }
                            if (lNPC2.MaxTime != num3)
                            {
                                lNPC2.MaxTime = num3;
                                var value = num2 - num3;
                                if (num2 > num3)
                                {
                                    if (!lNPC2.Config.DoNotAnnounceInfo)
                                    {
                                        TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 受服务器人数增多影响攻略时间减少 {value} 秒剩{(int) (num3 - lNPC2.TiemN)}秒."), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(100));
                                    }
                                }
                                else if (num2 < num3 && !lNPC2.Config.DoNotAnnounceInfo)
                                {
                                    TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 受服务器人数增多影响攻略时间增加 {Math.Abs(value)}秒剩{(int) (num3 - lNPC2.TiemN)}秒."), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(100));
                                }
                            }
                        }
                        if (this.Config.DynamicLifeLimit && (lNPC2.Config.PlayerCoefficient != 0 || lNPC2.Config.NPCLife != 0 || lNPC2.Config.ServiceOpenCoefficient != 0 || lNPC2.Config.KillCountCoefficient != 0))
                        {
                            var num4 = activePlayerCount * lNPC2.Config.PlayerCoefficient;
                            num4 += lNPC2.Config.NPCLife;
                            num4 += lNPC2.OSTime * lNPC2.Config.ServiceOpenCoefficient;
                            num4 += (int) lNPC2.LKC * lNPC2.Config.KillCountCoefficient;
                            if (!lNPC2.Config.OverrideOriginalLife)
                            {
                                num4 += val3.lifeMax;
                            }
                            if (num4 < 1)
                            {
                                num4 = 1;
                            }
                            if (lNPC2.MaxLife < num4 || !lNPC2.Config.NotLowerThanNormal)
                            {
                                var lifeMax = val3.lifeMax;
                                var num5 = num4;
                                if (!this.Config.IgnoreMonsterTable.Contains(val3.netID))
                                {
                                    if (this.Config.UnifiedMonsterLifeMaxMultiplier != 1.0 && this.Config.UnifiedMonsterLifeMaxMultiplier > 0.0)
                                    {
                                        num5 = (int) (num5 * this.Config.UnifiedMonsterLifeMaxMultiplier);
                                        if (num5 < 1)
                                        {
                                            num5 = 1;
                                        }
                                    }
                                    if (this.Config.UnifiedLifeNotLowerThanNormal && num5 < lNPC2.MaxLife)
                                    {
                                        num5 = lNPC2.MaxLife;
                                    }
                                }
                                if (lifeMax != num5)
                                {
                                    var life = val3.life;
                                    var num6 = (int) (life * (num5 / (double) lifeMax));
                                    if (num6 < 1)
                                    {
                                        num6 = 1;
                                    }
                                    val3.lifeMax = num5;
                                    val3.life = num6;
                                    var value2 = life - num6;
                                    if (life > num6)
                                    {
                                        if (!lNPC2.Config.DoNotAnnounceInfo)
                                        {
                                            TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 受服务器人数增多影响怪物血量减少 {value2} 剩{val3.life}."), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(100));
                                        }
                                    }
                                    else if (life < num6 && !lNPC2.Config.DoNotAnnounceInfo)
                                    {
                                        TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 受服务器人数增多影响怪物血量增加 {Math.Abs(value2)}剩{val3.life}."), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(100));
                                    }
                                }
                            }
                        }
                    }
                    if (lNPC2.Config == null)
                    {
                        continue;
                    }
                    if (lNPC2.TiemN == 0f)
                    {
                        if (lNPC2.MaxTime != 0)
                        {
                            if (!lNPC2.Config.DoNotAnnounceInfo)
                            {
                                TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 现身,攻略时间为 {lNPC2.MaxTime} 秒,血量为 {val3.lifeMax},快快加入战斗吧!"), Convert.ToByte(130), Convert.ToByte(255), Convert.ToByte(170));
                            }
                        }
                        else if (!lNPC2.Config.DoNotAnnounceInfo)
                        {
                            TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 现身,血量为 {val3.lifeMax},快快加入战斗吧!"), Convert.ToByte(130), Convert.ToByte(255), Convert.ToByte(170));
                        }
                        foreach (var item2 in lNPC2.Config.EntryAudio)
                        {
                            if (item2.SoundID >= 0)
                            {
                                NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(val3.Center, 1, item2.SoundID, item2.SoundSize, item2.HighPitch), -1, -1);
                            }
                        }
                        foreach (var item3 in lNPC2.Config.EntryCommands)
                        {
                            Commands.HandleCommand(TSPlayer.Server, TShock.Config.Settings.CommandSilentSpecifier + item3);
                        }
                        if (lNPC2.Config.EntryBroadcast != "")
                        {
                            TShock.Utils.Broadcast((lNPC2.Config.EntryBroadcastHeadless ? "" : (val3.FullName + ": ")) + lNPC2.Config.EntryBroadcast, Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                        }
                        Sundry.SetMonsterMarkers(lNPC2.Config.EntryMonsterIndicatorModifications, val3, ref rd);
                        Sundry.HurtMonster(lNPC2.Config.EntryMonsterStrike, val3);
                        Sundry.LaunchProjectile(lNPC2.Config.EntryProjectile, val3, lNPC2);
                        foreach (var item4 in lNPC2.Config.FollowerMonsters)
                        {
                            if (item4.Value >= 1 && item4.Key != 0)
                            {
                                var num7 = Math.Min(item4.Value, 200);
                                var nPCById = TShock.Utils.GetNPCById(item4.Key);
                                if (nPCById != null && nPCById.type != 113 && nPCById.type != 0 && nPCById.type < NPCID.Count)
                                {
                                    TSPlayer.Server.SpawnNPC(nPCById.type, nPCById.FullName, num7, Terraria.Utils.ToTileCoordinates(val3.Center).X, Terraria.Utils.ToTileCoordinates(val3.Center).Y, 30, 15);
                                }
                            }
                        }
                        lNPC2.BuffR = lNPC2.Config.BuffRange;
                        lNPC2.RBuff = lNPC2.Config.SurroundingBuff;
                    }
                    lNPC2.Time++;
                    lNPC2.TiemN = lNPC2.Time / 100f;
                    if (lNPC2.MaxTime == 0)
                    {
                        goto IL_0e35;
                    }
                    var maxTime = lNPC2.MaxTime;
                    if ((double) lNPC2.TiemN == Math.Round(maxTime * 0.5) || (double) lNPC2.TiemN == Math.Round(maxTime * 0.7) || (double) lNPC2.TiemN == Math.Round(maxTime * 0.9))
                    {
                        maxTime -= (int) lNPC2.TiemN;
                        if (!lNPC2.Config.DoNotAnnounceInfo)
                        {
                            TShock.Utils.Broadcast(GetString($"注意: {val3.FullName} 剩余攻略时间 {maxTime} 秒."), Convert.ToByte(130), Convert.ToByte(255), Convert.ToByte(170));
                        }
                        goto IL_0e35;
                    }
                    if (!(lNPC2.TiemN >= maxTime))
                    {
                        goto IL_0e35;
                    }
                    if (!lNPC2.Config.DoNotAnnounceInfo)
                    {
                        TShock.Utils.Broadcast(GetString($"攻略失败: {val3.FullName} 已撤离."), Convert.ToByte(190), Convert.ToByte(150), Convert.ToByte(150));
                    }
                    var whoAmI = val3.whoAmI;
                    Main.npc[whoAmI] = new NPC();
                    NetMessage.SendData(23, -1, -1, NetworkText.Empty, whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    goto end_IL_022f;
                IL_0e35:
                    var num8 = 0;
                    var num9 = 0;
                    var num10 = 0;
                    var num11 = 0;
                    var num12 = 0;
                    var num13 = 0;
                    var num14 = 0;
                    var num15 = 0f;
                    var num16 = 0f;
                    var flag2 = false;
                    var num17 = 0;
                    var num18 = 0;
                    var num19 = 0;
                    var lNKC = getLNKC(val3.netID);
                    var flag3 = false;
                    var num20 = 0;
                    if (val3.lifeMax > 0)
                    {
                        num20 = val3.life * 100 / val3.lifeMax;
                    }
                    if (lNPC2.TiemN != lNPC2.LTime)
                    {
                        var TimeEventLimits = lNPC2.Config.TimeEventLimit;
                        foreach (var item5 in lNPC2.CTime!)
                        {
                            if (TimeEventLimits <= 0)
                            {
                                break;
                            }
                            if (item5.TriggerTimes <= 0 && item5.TriggerTimes != -1)
                            {
                                continue;
                            }
                            var num21 = (int) (item5.DelaySeconds * 100f);
                            var num22 = (int) (item5.ConsumeTime * 100f);
                            if (num22 <= 0)
                            {
                                continue;
                            }
                            if (item5.LoopExecution)
                            {
                                if ((lNPC2.Time - num21) % num22 != 0)
                                {
                                    continue;
                                }
                            }
                            else if (num22 != lNPC2.Time - num21)
                            {
                                continue;
                            }
                            if ((item5.Difficulty.Length != 0 && !item5.Difficulty.Contains(Main.GameMode)) || Sundry.SeedRequirement(item5.MapSeed))
                            {
                                continue;
                            }
                            if (item5.KilledStack != 0)
                            {
                                if (item5.KilledStack > 0)
                                {
                                    if (lNKC < item5.KilledStack)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNKC >= Math.Abs(item5.KilledStack))
                                {
                                    continue;
                                }
                            }
                            if (item5.MonsterCountCondition != 0)
                            {
                                if (item5.MonsterCountCondition > 0)
                                {
                                    if (num < item5.MonsterCountCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (num >= Math.Abs(item5.MonsterCountCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.HealthRatioCondition != 0)
                            {
                                if (item5.HealthRatioCondition > 0)
                                {
                                    if (num20 < item5.HealthRatioCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (num20 >= Math.Abs(item5.HealthRatioCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.HealthCondition != 0)
                            {
                                if (item5.HealthCondition > 0)
                                {
                                    if (val3.life < item5.HealthCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (val3.life >= Math.Abs(item5.HealthCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.HitCondition != 0)
                            {
                                if (item5.HitCondition > 0)
                                {
                                    if (lNPC2.Struck < item5.HitCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNPC2.Struck >= Math.Abs(item5.HitCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.PlayerCount != 0)
                            {
                                if (item5.PlayerCount > 0)
                                {
                                    if (lNPC2.PlayerCount < item5.PlayerCount)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNPC2.PlayerCount >= Math.Abs(item5.PlayerCount))
                                {
                                    continue;
                                }
                            }
                            if (item5.TimeSpentCondition != 0)
                            {
                                if (item5.TimeSpentCondition > 0)
                                {
                                    if (lNPC2.TiemN < item5.TimeSpentCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNPC2.TiemN >= Math.Abs(item5.TimeSpentCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.IDCondition != 0 && item5.IDCondition != val3.netID)
                            {
                                continue;
                            }
                            if (item5.KillCondition != 0)
                            {
                                if (item5.KillCondition > 0)
                                {
                                    if (lNPC2.KillPlay < item5.KillCondition)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNPC2.KillPlay >= Math.Abs(item5.KillCondition))
                                {
                                    continue;
                                }
                            }
                            if (item5.StastServer != 0)
                            {
                                if (item5.StastServer > 0)
                                {
                                    if (lNPC2.OSTime < item5.StastServer)
                                    {
                                        continue;
                                    }
                                }
                                else if (lNPC2.OSTime >= Math.Abs(item5.StastServer))
                                {
                                    continue;
                                }
                            }
                            if ((item5.DayAndNight == -1 && Main.dayTime) || (item5.DayAndNight == 1 && !Main.dayTime) || (item5.BloodMoon == -1 && Main.bloodMoon) || (item5.BloodMoon == 1 && !Main.bloodMoon) || (item5.Eclipse == -1 && Main.eclipse) || (item5.Eclipse == 1 && !Main.eclipse) || (item5.Rain == -1 && Main.raining) || (item5.Rain == 1 && !Main.raining) || (item5.HardMode == -1 && Main.hardMode) || (item5.HardMode == 1 && !Main.hardMode) || (item5.DownedGolemBoss == -1 && NPC.downedGolemBoss) || (item5.DownedGolemBoss == 1 && !NPC.downedGolemBoss) || (item5.DownedMoonlord == -1 && NPC.downedMoonlord) || (item5.DownedMoonlord == 1 && !NPC.downedMoonlord))
                            {
                                continue;
                            }
                            if (item5.XAxisCondition != 0)
                            {
                                if (item5.XAxisCondition > 0)
                                {
                                    if (val3.Center.X < (item5.XAxisCondition << 4))
                                    {
                                        continue;
                                    }
                                }
                                else if (val3.Center.X >= Math.Abs(item5.XAxisCondition << 4))
                                {
                                    continue;
                                }
                            }
                            if (item5.YAxisCondition != 0)
                            {
                                if (item5.YAxisCondition > 0)
                                {
                                    if (val3.Center.Y < item5.YAxisCondition << 4)
                                    {
                                        continue;
                                    }
                                }
                                else if (val3.Center.Y >= Math.Abs(item5.YAxisCondition << 4))
                                {
                                    continue;
                                }
                            }
                            if ((item5.FacingDirectionCondition == 1 && (val3.direction != 1 || val3.directionY != 0)) || (item5.FacingDirectionCondition == 2 && (val3.direction != 1 || val3.directionY != 1)) || (item5.FacingDirectionCondition == 3 && (val3.direction != 0 || val3.directionY != 1)) || (item5.FacingDirectionCondition == 4 && (val3.direction != -1 || val3.directionY != 1)) || (item5.FacingDirectionCondition == 5 && (val3.direction != -1 || val3.directionY != 0)) || (item5.FacingDirectionCondition == 6 && (val3.direction != -1 || val3.directionY != -1)) || (item5.FacingDirectionCondition == 7 && (val3.direction != 0 || val3.directionY != -1)) || (item5.FacingDirectionCondition == 8 && (val3.direction != 1 || val3.directionY != -1)) || (item5.FacingDirectionCondition == 9 && val3.direction != 1) || (item5.FacingDirectionCondition == 10 && val3.directionY != 1) || (item5.FacingDirectionCondition == 11 && val3.direction != -1) || (item5.FacingDirectionCondition == 12 && val3.directionY != -1) || item5.TriggerNumerator <= 0 || item5.TriggerDenominator <= 0 || (item5.TriggerNumerator < item5.TriggerDenominator && rd.Next(1, item5.TriggerDenominator + 1) > item5.TriggerNumerator) || Sundry.NPCKillRequirement(item5.KilledNPC) || !lNPC2.haveMarkers(item5.IndicatorConditions, val3) || Sundry.AIRequirement(item5.AiConditions, val3) || Sundry.MonsterRequirement(item5.MonsterCondition, val3) || Sundry.PlayerRequirement(item5.PlayerCondition, val3) || Sundry.ProjectileRequirement(item5.ProjectileCondition, val3))
                            {
                                continue;
                            }
                            foreach (var item6 in item5.IndicatorModifications)
                            {
                                lNPC2.setMarkers(item6.IndName, item6.IndStack, item6.Clear, item6.InjectionStackName, item6.InjectionStackRatio, item6.InjectionStackOperator, item6.RandomSmall, item6.RandomBig, ref rd, val3);
                            }
                            Sundry.SetMonsterMarkers(item5.MonsterIndicatorModifications, val3, ref rd);
                            if (item5.CanPassThroughWalls == -1)
                            {
                                val3.noTileCollide = false;
                                val3.netUpdate = true;
                            }
                            if (item5.CanPassThroughWalls == 1)
                            {
                                val3.noTileCollide = true;
                                val3.netUpdate = true;
                            }
                            if (item5.IgnoreGravity == -1)
                            {
                                val3.noGravity = false;
                                val3.netUpdate = true;
                            }
                            if (item5.IgnoreGravity == 1)
                            {
                                val3.noGravity = true;
                                val3.netUpdate = true;
                            }
                            if (item5.MonsterGodMode == -1)
                            {
                                val3.immortal = false;
                                val3.netUpdate = true;
                            }
                            if (item5.MonsterGodMode == 1)
                            {
                                val3.immortal = true;
                                val3.netUpdate = true;
                            }
                            if (item5.SwitchAi >= 0 && item5.SwitchAi != 27)
                            {
                                val3.aiStyle = item5.SwitchAi;
                                val3.netUpdate = true;
                            }
                            if (item5.ModifyDefense)
                            {
                                val3.defDefense = item5.MonsterDefense;
                                val3.defense = item5.MonsterDefense;
                                val3.netUpdate = true;
                            }
                            if (item5.AiValues.Count > 0)
                            {
                                for (var k = 0; k < val3.ai.Count(); k++)
                                {
                                    if (item5.AiValues.ContainsKey(k) && item5.AiValues.TryGetValue(k, out var value3))
                                    {
                                        val3.ai[k] = value3;
                                        val3.netUpdate = true;
                                    }
                                }
                            }
                            if (item5.IndicatorInjectAiValues.Count > 0)
                            {
                                for (var l = 0; l < val3.ai.Count(); l++)
                                {
                                    if (item5.IndicatorInjectAiValues.ContainsKey(l) && item5.IndicatorInjectAiValues.TryGetValue(l, out var value4))
                                    {
                                        var array = value4.Split('*');
                                        var result = 1f;
                                        if (array.Length == 2 && array[1] != "")
                                        {
                                            float.TryParse(array[1], out result);
                                        }
                                        val3.ai[l] = lNPC2.getMarkers(value4) * result;
                                        val3.netUpdate = true;
                                    }
                                }
                            }
                            foreach (var item7 in item5.PlaySounds)
                            {
                                if (item7.SoundID >= 0)
                                {
                                    NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(val3.Center, 1, item7.SoundID, item7.SoundSize, item7.HighPitch), -1, -1);
                                }
                            }
                            foreach (var item8 in item5.Commands)
                            {
                                Commands.HandleCommand(TSPlayer.Server, TShock.Config.Settings.CommandSilentSpecifier + lNPC2.ReplaceMarkers(item8));
                            }
                            if (item5.Broadcast != "")
                            {
                                TShock.Utils.Broadcast((item5.BroadcastHeadless ? "" : (val3.FullName + ": ")) + lNPC2.ReplaceMarkers(item5.Broadcast), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                            }
                            if (item5.PlayerRespawnTime >= -1)
                            {
                                lNPC2.RespawnSeconds = item5.PlayerRespawnTime;
                            }
                            if (item5.DefaultMaxSpawns >= -1)
                            {
                                lNPC2.DefaultMaxSpawns = item5.DefaultMaxSpawns;
                            }
                            if (item5.DefaultSpawnRate >= -1)
                            {
                                lNPC2.DefaultSpawnRate = item5.DefaultSpawnRate;
                            }
                            if (item5.BlockTeleporter != 0)
                            {
                                lNPC2.BlockTeleporter = item5.BlockTeleporter;
                            }
                            Sundry.HurtMonster(item5.StruckMonsters, val3);
                            Sundry.LaunchProjectile(item5.ProjectileGroups, val3, lNPC2);
                            Sundry.updataProjectile(item5.ProjUpdate, val3, lNPC2);
                            foreach (var item9 in item5.SummonedMonsters)
                            {
                                if (item9.Value >= 1 && item9.Key != 0)
                                {
                                    var num23 = Math.Min(item9.Value, 200);
                                    var nPCById2 = TShock.Utils.GetNPCById(item9.Key);
                                    if (nPCById2 != null && nPCById2.type != 113 && nPCById2.type != 0 && nPCById2.type < NPCID.Count)
                                    {
                                        TSPlayer.Server.SpawnNPC(nPCById2.type, nPCById2.FullName, num23, Terraria.Utils.ToTileCoordinates(val3.Center).X, Terraria.Utils.ToTileCoordinates(val3.Center).Y, 5, 5);
                                    }
                                }
                            }
                            if (item5.BuffRange > 0)
                            {
                                lNPC2.BuffR = item5.BuffRange;
                                lNPC2.RBuff = item5.SurroundingBuff;
                            }
                            if (item5.HealHealth > 0)
                            {
                                var val4 = val3;
                                val4.life += item5.HealHealth;
                                if (val3.life > val3.lifeMax)
                                {
                                    val3.life = val3.lifeMax;
                                }
                                if (val3.life < 1)
                                {
                                    val3.life = 1;
                                }
                                val3.HealEffect(item5.HealHealth, true);
                                val3.netUpdate = true;
                            }
                            if (item5.PercentageHeal > 0)
                            {
                                if (item5.PercentageHeal > 100)
                                {
                                    item5.PercentageHeal = 100;
                                }
                                var num24 = val3.lifeMax * item5.PercentageHeal / 100;
                                var val4 = val3;
                                val4.life += num24;
                                if (val3.life > val3.lifeMax)
                                {
                                    val3.life = val3.lifeMax;
                                }
                                if (val3.life < 1)
                                {
                                    val3.life = 1;
                                }
                                val3.HealEffect(num24, true);
                                val3.netUpdate = true;
                            }
                            if (item5.TransformMonster >= 0 && (item5.TransformMonster != 113 || item5.TransformMonster != 114))
                            {
                                val3.Transform(item5.TransformMonster);
                                val3.netUpdate = true;
                            }
                            if (item5.TriggerTimes != -1)
                            {
                                var TimeGroups = item5;
                                TimeGroups.TriggerTimes--;
                            }
                            if (item5.KnockbackRange > 0 && item5.KnockbackPower != 0)
                            {
                                num8 = item5.KnockbackRange;
                                num9 = item5.KnockbackPower;
                            }
                            if (item5.DamageRange > 0 && item5.DamageAmount != 0)
                            {
                                num10 = item5.DamageRange;
                                num11 = item5.DamageAmount;
                            }
                            if (item5.PullRange > 0)
                            {
                                num12 = item5.PullStart;
                                num13 = item5.PullRange;
                                num14 = item5.PullEnd;
                                num15 = item5.PullPointXOffset + (lNPC2.getMarkers(item5.IndicatorCountInjectPullPointXName) * item5.IndicatorCountInjectPullPointXFactor);
                                num16 = item5.PullPointYOffset + (lNPC2.getMarkers(item5.IndicatorCountInjectPullPointYName) * item5.IndicatorCountInjectPullPointYFactor);
                                flag2 = item5.InitialPullPointZero;
                            }
                            if (item5.ClearProjectileEndRange > 0)
                            {
                                num17 = item5.ClearProjectileStartRange;
                                num18 = item5.ClearProjectileEndRange;
                            }
                            if (item5.ReflectionRange > 0)
                            {
                                num19 = item5.ReflectionRange;
                            }
                            if (item5.DirectRetreat)
                            {
                                flag3 = true;
                            }
                            if (item5.JumpOutEvent)
                            {
                                break;
                            }
                            TimeEventLimits--;
                        }
                        lNPC2.LTime = lNPC2.TiemN;
                    }
                    if (lNPC2.LifeP != num20)
                    {
                        lNPC2.LifeP = num20;
                        if (lNPC2.LifeP != lNPC2.LLifeP)
                        {
                            var LifeEventLimits = lNPC2.Config.LifeEventLimit;
                            foreach (var item10 in lNPC2.PLife!)
                            {
                                if (LifeEventLimits <= 0)
                                {
                                    break;
                                }
                                if (item10.HealthRemainingRatio <= 0 || (item10.TriggerTimes <= 0 && item10.TriggerTimes != -1) || item10.HealthRemainingRatio < lNPC2.LifeP || item10.HealthRemainingRatio >= lNPC2.LLifeP)
                                {
                                    continue;
                                }
                                if (item10.KilledStack != 0)
                                {
                                    if (item10.KilledStack > 0)
                                    {
                                        if (lNKC < item10.KilledStack)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (lNKC >= Math.Abs(item10.KilledStack))
                                    {
                                        continue;
                                    }
                                }
                                if (item10.PlayerCount != 0)
                                {
                                    if (item10.PlayerCount > 0)
                                    {
                                        if (lNPC2.PlayerCount < item10.PlayerCount)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (lNPC2.PlayerCount >= Math.Abs(item10.PlayerCount))
                                    {
                                        continue;
                                    }
                                }
                                if (item10.TimeSpentCondition != 0)
                                {
                                    if (item10.TimeSpentCondition > 0)
                                    {
                                        if (lNPC2.TiemN < item10.TimeSpentCondition)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (lNPC2.TiemN >= Math.Abs(item10.TimeSpentCondition))
                                    {
                                        continue;
                                    }
                                }
                                if (item10.IDCondition != 0 && item10.IDCondition != val3.netID)
                                {
                                    continue;
                                }
                                if (item10.KillCondition != 0)
                                {
                                    if (item10.KillCondition > 0)
                                    {
                                        if (lNPC2.KillPlay < item10.KillCondition)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (lNPC2.KillPlay >= Math.Abs(item10.KillCondition))
                                    {
                                        continue;
                                    }
                                }
                                if (item10.StastServer != 0)
                                {
                                    if (item10.StastServer > 0)
                                    {
                                        if (lNPC2.OSTime < item10.StastServer)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (lNPC2.OSTime >= Math.Abs(item10.StastServer))
                                    {
                                        continue;
                                    }
                                }
                                if ((item10.DayAndNight == -1 && Main.dayTime) || (item10.DayAndNight == 1 && !Main.dayTime) || (item10.BloodMoon == -1 && Main.bloodMoon) || (item10.BloodMoon == 1 && !Main.bloodMoon) || (item10.HardMode == -1 && Main.hardMode) || (item10.HardMode == 1 && !Main.hardMode) || (item10.DownedGolemBoss == -1 && NPC.downedGolemBoss) || (item10.DownedGolemBoss == 1 && !NPC.downedGolemBoss) || (item10.DownedMoonlord == -1 && NPC.downedMoonlord) || (item10.DownedMoonlord == 1 && !NPC.downedMoonlord))
                                {
                                    continue;
                                }
                                if (item10.YAxisCondition != 0)
                                {
                                    if (item10.YAxisCondition > 0)
                                    {
                                        if (val3.Center.Y < item10.YAxisCondition << 4)
                                        {
                                            continue;
                                        }
                                    }
                                    else if (val3.Center.Y >= Math.Abs(item10.YAxisCondition << 4))
                                    {
                                        continue;
                                    }
                                }
                                if ((item10.FacingDirectionCondition == 1 && (val3.direction != 1 || val3.directionY != 0)) || (item10.FacingDirectionCondition == 2 && (val3.direction != 1 || val3.directionY != 1)) || (item10.FacingDirectionCondition == 3 && (val3.direction != 0 || val3.directionY != 1)) || (item10.FacingDirectionCondition == 4 && (val3.direction != -1 || val3.directionY != 1)) || (item10.FacingDirectionCondition == 5 && (val3.direction != -1 || val3.directionY != 0)) || (item10.FacingDirectionCondition == 6 && (val3.direction != -1 || val3.directionY != -1)) || (item10.FacingDirectionCondition == 7 && (val3.direction != 0 || val3.directionY != -1)) || (item10.FacingDirectionCondition == 8 && (val3.direction != 1 || val3.directionY != -1)) || (item10.FacingDirectionCondition == 9 && val3.direction != 1) || (item10.FacingDirectionCondition == 10 && val3.directionY != 1) || (item10.FacingDirectionCondition == 11 && val3.direction != -1) || (item10.FacingDirectionCondition == 12 && val3.directionY != -1) || item10.TriggerNumerator <= 0 || item10.TriggerDenominator <= 0 || (item10.TriggerNumerator < item10.TriggerDenominator && rd.Next(1, item10.TriggerDenominator + 1) > item10.TriggerNumerator) || Sundry.MonsterRequirement(item10.MonsterCondition, val3) || Sundry.PlayerRequirement(item10.PlayerCondition, val3))
                                {
                                    continue;
                                }
                                if (item10.CanPassThroughWalls == -1)
                                {
                                    val3.noTileCollide = false;
                                    val3.netUpdate = true;
                                }
                                if (item10.CanPassThroughWalls == 1)
                                {
                                    val3.noTileCollide = true;
                                    val3.netUpdate = true;
                                }
                                if (item10.IgnoreGravity == -1)
                                {
                                    val3.noGravity = false;
                                    val3.netUpdate = true;
                                }
                                if (item10.IgnoreGravity == 1)
                                {
                                    val3.noGravity = true;
                                    val3.netUpdate = true;
                                }
                                if (item10.MonsterGodMode == -1)
                                {
                                    val3.immortal = false;
                                    val3.netUpdate = true;
                                }
                                if (item10.MonsterGodMode == 1)
                                {
                                    val3.immortal = true;
                                    val3.netUpdate = true;
                                }
                                if (item10.SwitchAi >= 0 && item10.SwitchAi != 27)
                                {
                                    val3.aiStyle = item10.SwitchAi;
                                    val3.netUpdate = true;
                                }
                                if (item10.ModifyDefense)
                                {
                                    val3.defDefense = item10.MonsterDefense;
                                    val3.defense = item10.MonsterDefense;
                                    val3.netUpdate = true;
                                }
                                if (item10.Broadcast != "")
                                {
                                    TShock.Utils.Broadcast((item10.BroadcastHeadless ? "" : (val3.FullName + ": ")) + item10.Broadcast, Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                                }
                                if (item10.PlayerRespawnTime >= -1)
                                {
                                    lNPC2.RespawnSeconds = item10.PlayerRespawnTime;
                                }
                                Sundry.HurtMonster(item10.StruckMonsters, val3);
                                Sundry.LaunchProjectile(item10.ProjectileGroups, val3, lNPC2);
                                foreach (var item11 in item10.SummonedMonsters)
                                {
                                    if (item11.Value >= 1 && item11.Key != 0)
                                    {
                                        var num25 = Math.Min(item11.Value, 200);
                                        var nPCById3 = TShock.Utils.GetNPCById(item11.Key);
                                        if (nPCById3 != null && nPCById3.type != 113 && nPCById3.type != 0 && nPCById3.type < NPCID.Count)
                                        {
                                            TSPlayer.Server.SpawnNPC(nPCById3.type, nPCById3.FullName, num25, Terraria.Utils.ToTileCoordinates(val3.Center).X, Terraria.Utils.ToTileCoordinates(val3.Center).Y, 15, 15);
                                        }
                                    }
                                }
                                if (item10.BuffRange > 0)
                                {
                                    lNPC2.BuffR = item10.BuffRange;
                                    lNPC2.RBuff = item10.SurroundingBuff;
                                }
                                if (item10.HealHealth > 0)
                                {
                                    var val4 = val3;
                                    val4.life += item10.HealHealth;
                                    if (val3.life > val3.lifeMax)
                                    {
                                        val3.life = val3.lifeMax;
                                    }
                                    if (val3.life < 1)
                                    {
                                        val3.life = 1;
                                    }
                                    val3.HealEffect(item10.HealHealth, true);
                                    val3.netUpdate = true;
                                }
                                if (item10.PercentageHeal > 0)
                                {
                                    if (item10.PercentageHeal > 100)
                                    {
                                        item10.PercentageHeal = 100;
                                    }
                                    var val4 = val3;
                                    val4.life += (int) (val3.lifeMax * (item10.PercentageHeal / 100.0));
                                    if (val3.life > val3.lifeMax)
                                    {
                                        val3.life = val3.lifeMax;
                                    }
                                    if (val3.life < 1)
                                    {
                                        val3.life = 1;
                                    }
                                    val3.HealEffect(item10.HealHealth, true);
                                    val3.netUpdate = true;
                                }
                                if (item10.TriggerTimes != -1)
                                {
                                    var RatioGroups = item10;
                                    RatioGroups.TriggerTimes--;
                                }
                                if (item10.KnockbackRange > 0 && item10.KnockbackPower != 0)
                                {
                                    num8 = item10.KnockbackRange;
                                    num9 = item10.KnockbackPower;
                                }
                                if (item10.DamageRange > 0 && item10.DamageAmount != 0)
                                {
                                    num10 = item10.DamageRange;
                                    num11 = item10.DamageAmount;
                                }
                                if (item10.PullEnd > 0)
                                {
                                    num12 = item10.PullStart;
                                    num13 = item10.PullEnd;
                                    num14 = item10.PullEnd;
                                    num15 = item10.PullPointXOffset;
                                    num16 = item10.PullPointYOffset;
                                    flag2 = false;
                                }
                                if (item10.DirectRetreat)
                                {
                                    flag3 = true;
                                }
                                if (item10.JumpOutEvent)
                                {
                                    break;
                                }
                                LifeEventLimits--;
                            }
                            lNPC2.LLifeP = lNPC2.LifeP;
                        }
                    }
                    if (num19 > 0)
                    {
                        for (var m = 0; m < 1000; m++)
                        {
                            if (Main.projectile[m].active && Main.projectile[m].CanBeReflected() && val3.WithinRange(Main.projectile[m].Center, num19 << 4))
                            {
                                val3.ReflectProjectile(Main.projectile[m]);
                                NetMessage.SendData(27, -1, -1, null, m, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                    if (num18 > 0)
                    {
                        for (var n = 0; n < 1000; n++)
                        {
                            if (Main.projectile[n].active && val3.WithinRange(Main.projectile[n].Center, num18 << 4) && (num17 <= 0 || val3.WithinRange(Main.projectile[n].Center, num17 << 4)))
                            {
                                Main.projectile[n].active = false;
                                Main.projectile[n].type = 0;
                                TSPlayer.All.SendData((PacketTypes) 27, "", n, 0f, 0f, 0f, 0);
                            }
                        }
                    }
                    if (num14 > num12)
                    {
                        num14 = num12;
                    }
                    num14 *= 16;
                    var num26 = num15;
                    var num27 = num16;
                    if (!flag2)
                    {
                        num26 += val3.Center.X;
                        num27 += val3.Center.Y;
                    }
                    if (lNPC2.BuffR > 0 || (num8 > 0 && num9 != 0) || (num10 > 0 && num11 != 0) || num13 > 0)
                    {
                        var players = TShock.Players;
                        foreach (var val5 in players)
                        {
                            if (val5 == null)
                            {
                                continue;
                            }
                            if (Timeout(now))
                            {
                                ULock = false;
                                return;
                            }
                            if (!val5.Active || val5.Dead || val5.TPlayer.statLife < 1)
                            {
                                continue;
                            }
                            if (num13 > 0 && Sundry.WithinRange(val5.TPlayer.Center, (int) num26, (int) num27, num13 * 16))
                            {
                                if (num12 > 0)
                                {
                                    if (!Sundry.WithinRange(val5.TPlayer.Center, (int) num26, (int) num27, num12 * 16))
                                    {
                                        Sundry.PullTP(val5, num26, num27, num14);
                                    }
                                }
                                else
                                {
                                    Sundry.PullTP(val5, num26, num27, num14);
                                }
                            }
                            if (num10 > 0 && num11 != 0 && Sundry.WithinRange(val5.TPlayer.Center, val3.Center, num10 * 16))
                            {
                                if (num11 < 0)
                                {
                                    if (num11 > val5.TPlayer.statLifeMax2)
                                    {
                                        num11 = val5.TPlayer.statLifeMax2;
                                    }
                                    val5.Heal(Math.Abs(num11));
                                }
                                else if (num11 > val5.TPlayer.statLifeMax2 + val5.TPlayer.statDefense)
                                {
                                    val5.KillPlayer();
                                }
                                else
                                {
                                    val5.DamagePlayer(num11);
                                }
                            }
                            if (num8 > 0 && num9 != 0 && Sundry.WithinRange(val5.TPlayer.Center, val3.Center, num8 * 16))
                            {
                                Sundry.UserRepel(val5, val3.Center.X, val3.Center.Y, num9);
                            }
                            if (lNPC2.BuffR <= 0 || lNPC2.Time % 100 != 0 || !Sundry.WithinRange(val5.TPlayer.Center, val3.Center, lNPC2.BuffR * 16))
                            {
                                continue;
                            }
                            foreach (var item12 in lNPC2.RBuff!)
                            {
                                if ((item12.BuffStartRange <= 0 || !Sundry.WithinRange(val5.TPlayer.Center, val3.Center, item12.BuffStartRange * 16)) && (item12.BuffStopRange <= 0 || Sundry.WithinRange(val5.TPlayer.Center, val3.Center, item12.BuffStopRange * 16)))
                                {
                                    val5.SetBuff(item12.BuffID, 100, false);
                                    if (item12.TopTip != "")
                                    {
                                        var TopTips = item12.TopTip;
                                        var val6 = new Color(255, 255, 255);
                                        val5.SendData((PacketTypes) 119, TopTips, (int) val6.PackedValue, val5.X, val5.Y, 0f, 0);
                                    }
                                }
                            }
                        }
                    }
                    if (flag3)
                    {
                        var whoAmI2 = val3.whoAmI;
                        Main.npc[whoAmI2] = new NPC();
                        NetMessage.SendData(23, -1, -1, NetworkText.Empty, whoAmI2, 0f, 0f, 0f, 0, 0, 0);
                        if (!lNPC2.Config.DoNotAnnounceInfo)
                        {
                            TShock.Utils.Broadcast(GetString($"攻略失败: {val3.FullName} 已撤离."), Convert.ToByte(190), Convert.ToByte(150), Convert.ToByte(150));
                        }
                    }
                end_IL_022f:;
                }
            }
            var num28 = -1;
            var num29 = -1;
            var npc3 = Main.npc;
            foreach (var val7 in npc3)
            {
                if (val7 == null || !val7.active)
                {
                    continue;
                }
                lock (LNpcs!)
                {
                    var lNPC3 = LNpcs[val7.whoAmI];
                    if (lNPC3 != null && lNPC3.Config != null)
                    {
                        if (lNPC3.DefaultMaxSpawns > num28)
                        {
                            num28 = lNPC3.DefaultMaxSpawns;
                        }
                        if (lNPC3.DefaultSpawnRate > num29)
                        {
                            num29 = lNPC3.DefaultSpawnRate;
                        }
                    }
                }
            }
            if (num28 >= 0 && NPC.defaultMaxSpawns != num28)
            {
                NPC.defaultMaxSpawns = num28;
            }
            else if (num28 < 0)
            {
                NPC.defaultMaxSpawns = TShock.Config.Settings.DefaultMaximumSpawns;
            }
            if (num29 >= 0 && NPC.defaultSpawnRate != num29)
            {
                NPC.defaultSpawnRate = num29;
            }
            else if (num29 < 0)
            {
                NPC.defaultSpawnRate = TShock.Config.Settings.DefaultSpawnRate;
            }
            if (this.Config.DeathPerspective && (!this.Config.DeathPerspectiveInBOSS || flag))
            {
                if (this.TeamP <= this.Config.SmoothPerspective)
                {
                    this.TeamP++;
                }
                else
                {
                    this.TeamP = 0;
                    var players2 = TShock.Players;
                    foreach (var val8 in players2)
                    {
                        if (val8 == null)
                        {
                            continue;
                        }
                        if (Timeout(now))
                        {
                            ULock = false;
                            return;
                        }
                        if (!val8.Active || !val8.Dead || val8.TPlayer.statLife >= 1 || val8.Team == 0)
                        {
                            continue;
                        }
                        var num30 = -1;
                        var num31 = -1f;
                        for (var num32 = 0; num32 < 255; num32++)
                        {
                            if (Main.player[num32] != null && val8.Team == Main.player[num32].team && Main.player[num32].active && !Main.player[num32].dead)
                            {
                                var num33 = Math.Abs(Main.player[num32].position.X + (Main.player[num32].width / 2) - (val8.TPlayer.position.X + (val8.TPlayer.width / 2))) + Math.Abs(Main.player[num32].position.Y + (Main.player[num32].height / 2) - (val8.TPlayer.position.Y + (val8.TPlayer.height / 2)));
                                if (num31 == -1f || num33 < num31)
                                {
                                    num31 = num33;
                                    num30 = num32;
                                }
                            }
                        }
                        if (num30 != -1 && !Sundry.WithinRange(val8.TPlayer.Center, Main.player[num30].Center, this.Config.PerspectiveWaiting * 16))
                        {
                            val8.TPlayer.position = Main.player[num30].position;
                            NetMessage.SendData(13, -1, -1, NetworkText.Empty, val8.Index, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (this.Config.ErrorLogs)
            {
                TShock.Log.ConsoleError(GetString($"自定义怪物血量时钟:{ex}"));
            }
        }
        ULock = false;
    }
    #endregion

    #region 计时器超时方法
    public static bool Timeout(DateTime Start, bool warn = true, int ms = 500)
    {
        var flag = (DateTime.Now - Start).TotalMilliseconds >= ms;
        if (flag)
        {
            ULock = false;
        }
        if (warn && flag)
        {
            TShock.Log.Error(GetString("自定义怪物血量插件处理超时,已抛弃部分处理!"));
        }
        return flag;
    }
    #endregion

    #region 杀死玩家事件
    private void OnKillMe(object sender, KillMeEventArgs args)
    {
        if (args.Handled || args.Player == null || args.Pvp)
        {
            return;
        }
        var num = -1;
        var npc = Main.npc;
        foreach (var val in npc)
        {
            if (val == null || !val.active)
            {
                continue;
            }
            lock (LNpcs!)
            {
                var lNPC = LNpcs[val.whoAmI];
                if (lNPC != null && lNPC.Config != null)
                {
                    lNPC.KillPlay++;
                    if (lNPC.RespawnSeconds > num)
                    {
                        num = lNPC.RespawnSeconds;
                    }
                }
            }
        }
        if (TShock.Config.Settings.RespawnSeconds < 0 || TShock.Config.Settings.RespawnBossSeconds < 0)
        {
            return;
        }
        if (num >= 0)
        {
            if (this.LRespawnSeconds == -1 && this.LRespawnBossSeconds == -1)
            {
                this.LRespawnSeconds = TShock.Config.Settings.RespawnSeconds;
                this.LRespawnBossSeconds = TShock.Config.Settings.RespawnBossSeconds;
            }
            TShock.Config.Settings.RespawnSeconds = num;
            TShock.Config.Settings.RespawnBossSeconds = num;
        }
        else if (this.LRespawnSeconds >= 0 && this.LRespawnBossSeconds >= 0)
        {
            TShock.Config.Settings.RespawnSeconds = this.LRespawnSeconds;
            TShock.Config.Settings.RespawnBossSeconds = this.LRespawnBossSeconds;
            this.LRespawnSeconds = -1;
            this.LRespawnBossSeconds = -1;
        }
    }
    #endregion

    #region 清理弹幕事件
    private void OnProjectileKill(On.Terraria.Projectile.orig_Kill orig, Projectile self)
    {
        lock (LPrjs!)
        {
            if (LPrjs[self.identity] != null)
            {
                LPrjs[self.identity] = null!;
            }
        }
        orig.Invoke(self);
    }
    #endregion

    #region 获取指定ID对应的KC（击杀计数）值。
    public static long getLNKC(int id)
    {
        if (id == 0)
        {
            return 0L;
        }
        lock (LNkc!)
        {
            for (var i = 0; i < LNkc.Count; i++)
            {
                if (LNkc[i].ID == id)
                {
                    return LNkc[i].KC;
                }
            }
        }
        return 0L;
    }
    #endregion

    #region 增加指定ID的KC（击杀计数）值 并保存数据到磁盘。
    private void addLNKC(int id)
    {
        lock (LNkc!)
        {
            for (var i = 0; i < LNkc.Count; i++)
            {
                if (LNkc[i].ID == id)
                {
                    LNkc[i].KC++;
                    this.SD(notime: false);
                    return;
                }
            }
            LNkc.Add(new LNKC(id));
            this.SD(notime: false);
        }
    }
    #endregion

    #region 将当前的KC数据保存到磁盘，并更新最后保存时间戳。
    public void SD(bool notime)
    {
        if (!notime)
        {
            if ((DateTime.UtcNow - this.NPCKillDataTime).TotalMilliseconds < 15000.0)
            {
                return;
            }
            this.NPCKillDataTime = DateTime.UtcNow;
        }
        using var swriter = new StreamWriter(this.NPCKillPath);
        var text = this.OServerDataTime.ToString();
        for (var i = 0; i < LNkc!.Count; i++)
        {
            text += Environment.NewLine;
            text = text + LNkc[i].ID + "|" + LNkc[i].KC;
        }
        swriter.Write(text);
    }
    #endregion

    #region 从磁盘读取KC数据，并加载到内存中的LNkc列表。
    public void RD()
    {
        if (!File.Exists(this.NPCKillPath))
        {
            File.Create(this.NPCKillPath).Close();
        }
        using var reader = new StreamReader(this.NPCKillPath);
        var text = reader.ReadToEnd();
        var array = text.Split(Environment.NewLine.ToCharArray());
        if (array.Count() < 1 || !DateTime.TryParse(array[0], out var result))
        {
            return;
        }
        this.OServerDataTime = result;
        for (var i = 1; i < array.Count(); i++)
        {
            var array2 = array[i].Split('|');
            if (array2.Length == 2)
            {
                LNkc!.Add(new LNKC(int.Parse(array2[0]), long.Parse(array2[1])));
            }
        }
    }
    #endregion
}