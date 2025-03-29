using Newtonsoft.Json;
using TShockAPI;

namespace PacketsStop;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "数据包拦截.json");

    [JsonProperty("数据包名可查看", Order = 0)]
    public string README = "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs";

    [JsonProperty("插件指令与权限名", Order = 1)]
    public string README2 = "指令：pksp 权限：packetstop.use";

    [JsonProperty("功能开关", Order = 1)]
    public bool Enabled = false;

    [JsonProperty("拦截玩家名单",Order = 2)]
    public HashSet<string> PlayerList { get; set; } = new HashSet<string>();

    [JsonProperty("拦截数据包名", Order = 3)]
    public HashSet<string> Packets { get; set; } = new HashSet<string>();

    #region 预设参数方法
    public void SetDefault()
    {
        this.Packets = new HashSet<string>
        {
            "ConnectRequest",          // 连接请求 (1)
            "Disconnect",              // 断开连接 (2)
            "ContinueConnecting",      // 继续连接 (3)
            "PlayerInfo",              // 玩家信息 (4)
            "PlayerSlot",              // 玩家槽位 (5)
            "ContinueConnecting2",     // 继续连接2 (6)
            "WorldInfo",               // 世界信息 (7)
            "TileGetSection",          // 获取图格区块 (8)
            "Status",                  // 状态 (9)
            "TileSendSection",         // 发送图格区块 (10)
            "TileFrameSection",        // 图格框架区域 (11)
            "PlayerSpawn",             // 玩家生成 (12)
            "PlayerUpdate",            // 玩家更新 (13)
            "PlayerActive",            // 玩家活跃 (14)
            "PlayerHp",                // 玩家生命值 (16)
            "Tile",                    // 图格图块 (17)
            "TimeSet",                 // 时间设定 (18)
            "DoorUse",                 // 门的使用 (19)
            "TileSendSquare",          // 发送方格图格 (20)
            "ItemDrop",                // 物品掉落 (21)
            "ItemOwner",               // 物品归属 (22)
            "NpcUpdate",               // NPC更新 (23)
            "NpcItemStrike",           // NPC物品打击 (24)
            "ProjectileNew",           // 新弹幕 (27)
            "NpcStrike",               // NPC打击 (28)
            "ProjectileDestroy",       // 弹幕销毁 (29)
            "TogglePvp",               // 切换PVP模式 (30)
            "ChestGetContents",        // 获取宝箱内容 (31)
            "ChestItem",               // 宝箱物品 (32)
            "ChestOpen",               // 打开宝箱 (33)
            "PlaceChest",              // 放置宝箱 (34)
            "EffectHeal",              // 治疗效果 (35)
            "Zones",                   // 区域信息 (36)
            "PasswordRequired",        // 密码要求 (37)
            "PasswordSend",            // 发送密码 (38)
            "RemoveItemOwner",         // 移除物品归属 (39)
            "NpcTalk",                 // NPC对话 (40)
            "PlayerAnimation",         // 玩家动画 (41)
            "PlayerMana",              // 玩家魔法值 (42)
            "EffectMana",              // 魔法效果 (43)
            "PlayerTeam",              // 玩家队伍 (45)
            "SignRead",                // 阅读告示牌 (46)
            "SignNew",                 // 新建告示牌 (47)
            "LiquidSet",               // 液体设定 (48)
            "PlayerSpawnSelf",         // 玩家自我重生 (49)
            "PlayerBuff",              // 玩家增益效果 (50)
            "NpcSpecial",              // NPC特殊行为 (51)
            "ChestUnlock",             // 宝箱解锁 (52)
            "NpcAddBuff",              // NPC添加增益效果 (53)
            "NpcUpdateBuff",           // NPC更新增益效果 (54)
            "PlayerAddBuff",           // 玩家添加增益效果 (55)
            "UpdateNPCName",           // 更新NPC名称 (56)
            "UpdateGoodEvil",          // 更新善恶状态 (57)
            "PlayHarp",                // 演奏竖琴 (58)
            "HitSwitch",               // 触动开关 (59)
            "UpdateNPCHome",           // 更新NPC家园 (60)
            "SpawnBossorInvasion",     // 生成BOSS或入侵事件 (61)
            "PlayerDodge",             // 玩家闪避 (62)
            "PaintTile",               // 绘制图格图块 (63)
            "PaintWall",               // 绘制墙壁 (64)
            "Teleport",                // 传送 (65)
            "PlayerHealOther",         // 玩家治疗他人 (66)
            "Placeholder",             // 占位符 (67)
            "ClientUUID",              // 客户端UUID (68)
            "ChestName",               // 宝箱名称 (69)
            "CatchNPC",                // 捕获NPC (70)
            "ReleaseNPC",              // 释放NPC (71)
            "TravellingMerchantInventory", // 旅行商人的商品列表 (72)
            "TeleportationPotion",     // 传送药水 (73)
            "AnglerQuest",             // 渔翁任务 (74)
            "CompleteAnglerQuest",     // 完成渔翁任务 (75)
            "NumberOfAnglerQuestsCompleted", // 完成的渔翁任务数量 (76)
            "CreateTemporaryAnimation",// 创建临时动画 (77)
            "ReportInvasionProgress",  // 报告入侵进度 (78)
            "PlaceObject",             // 放置物体 (79)
            "SyncPlayerChestIndex",    // 同步玩家宝箱索引 (80)
            "CreateCombatText",        // 创建战斗文本 (81)
            "LoadNetModule",           // 加载网络模块 (82)
            "NpcKillCount",            // NPC击杀数 (83)
            "PlayerStealth",           // 玩家潜行状态 (84)
            "ForceItemIntoNearestChest", // 强制物品进入最近的宝箱 (85)
            "UpdateTileEntity",        // 更新图格实体 (86)
            "PlaceTileEntity",         // 放置图格实体 (87)
            "TweakItem",               // 调整物品 (88)
            "PlaceItemFrame",          // 放置物品框 (89)
            "UpdateItemDrop",          // 更新物品掉落 (90)
            "EmoteBubble",             // 表情泡泡 (91)
            "SyncExtraValue",          // 同步附加数值 (92)
            "SocialHandshake",         // 社交握手 (93)
            "KillPortal",              // 摧毁传送门 (95)
            "PlayerTeleportPortal",    // 玩家传送至传送门 (96)
            "NotifyPlayerNpcKilled",   // 通知玩家NPC被击杀 (97)
            "NotifyPlayerOfEvent",     // 通知玩家事件 (98)
            "UpdateMinionTarget",      // 更新随从目标 (99)
            "NpcTeleportPortal",       // NPC传送至传送门 (100)
            "UpdateShieldStrengths",   // 更新护盾强度 (101)
            "NebulaLevelUp",           // 星云等级提升 (102)
            "MoonLordCountdown",       // 月亮领主倒计时 (103)
            "NpcShopItem",             // NPC商店物品 (104)
            "GemLockToggle",           // 宝石锁切换 (105)
            "PoofOfSmoke",             // 一团烟雾特效 (106)
            "SmartTextMessage",        // 智能文本消息 (107)
            "WiredCannonShot",         // 有线大炮射击 (108)
            "MassWireOperation",       // 大规模电线操作 (109)
            "MassWireOperationPay",    // 大规模电线操作支付 (110)
            "ToggleParty",             // 切换派对 (111)
            "TreeGrowFX",              // 树木生长特效 (112)
            "CrystalInvasionStart",    // 水晶入侵开始 (113)
            "CrystalInvasionWipeAll",  // 水晶入侵全清 (114)
            "MinionAttackTargetUpdate",// 随从攻击目标更新 (115)
            "CrystalInvasionSendWaitTime", // 水晶入侵发送等待时间 (116)
            "PlayerHurtV2",            // 玩家受伤V2 (117)
            "PlayerDeathV2",           // 玩家死亡V2 (118)
            "CreateCombatTextExtended",// 创建扩展战斗文本 (119)
            "Emoji",                   // 表情符号 (120)
            "TileEntityDisplayDollItemSync", // 图格实体展示玩偶物品同步 (121)
            "RequestTileEntityInteraction", // 请求图格实体交互 (122)
            "WeaponsRackTryPlacing",   // 武器架尝试放置 (123)
            "TileEntityHatRackItemSync", // 图格实体帽子架物品同步 (124)
            "SyncTilePicking",         // 同步图格拾取 (125)
            "SyncRevengeMarker",       // 同步复仇标记 (126)
            "RemoveRevengeMarker",     // 移除复仇标记 (127)
            "LandGolfBallInCup",       // 高尔夫球进洞 (128)
            "FinishedConnectingToServer", // 完成连接到服务器 (129)
            "FishOutNPC",              // 捞出NPC (130)
            "TamperWithNPC",           // 篡改NPC (131)
            "PlayLegacySound",         // 播放遗留声音 (132)
            "FoodPlatterTryPlacing",   // 尝试放置食物盘 (133)
            "UpdatePlayerLuckFactors", // 更新玩家幸运因子 (134)
            "DeadPlayer",              // 玩家死亡 (135)
            "SyncCavernMonsterType",   // 同步洞穴怪物类型 (136)
            "RequestNPCBuffRemoval",   // 请求移除NPC增益效果 (137)
            "ClientSyncedInventory",   // 客户端同步库存 (138)
            "SetCountsAsHostForGameplay", // 设置作为游戏主机计数 (139)
            "SetMiscEventValues",      // 设置杂项事件值 (140)
            "RequestLucyPopup",        // 请求露西弹窗显示 (141)
            "SyncProjectileTrackers",  // 同步弹幕追踪器 (142)
            "CrystalInvasionRequestedToSkipWaitTime", // 水晶入侵请求跳过等待时间 (143)
            "RequestQuestEffect",      // 请求任务效果 (144)
            "SyncItemsWithShimmer",    // 与炫光同步物品 (145)
            "ShimmerActions",          // 炫光动作 (146)
            "SyncLoadout",             // 同步装备配置 (147)
            "SyncItemCannotBeTakenByEnemies" // 同步物品不可被敌人获取的状态 (148)
        };
    }
    #endregion

    #region 读取与创建配置文件方法
    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static Configuration Read()
    {
        if (!File.Exists(FilePath))
        {
            var NewConfig = new Configuration();
            NewConfig.SetDefault();
            new Configuration().Write();
            return NewConfig;
        }
        else
        {
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

}