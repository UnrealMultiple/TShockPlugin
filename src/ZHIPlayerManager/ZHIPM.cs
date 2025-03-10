using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
// ReSharper disable InconsistentNaming

namespace ZHIPlayerManager;

[ApiVersion(2, 1)]
public partial class ZHIPM : TerrariaPlugin
{
    public override string Author => "z枳";

    public override string Description => GetString("玩家管理，提供修改玩家的任何信息，允许玩家备份，可以回档等操作");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 2, 4);

    #region 字段或属性
    /// <summary>
    /// 人物备份数据库
    /// </summary>
    private static ZplayerDB ZPDataBase { get; set; } = null!;
    /// <summary>
    /// 额外数据库
    /// </summary>
    private static ZplayerExtraDB ZPExtraDB { get; set; } = null!;
    /// <summary>
    /// 在线玩家的额外数据库的集合
    /// </summary>
    private static List<ExtraData> edPlayers { get; set; } = new();
    /// <summary>
    /// 广播颜色
    /// </summary>
    private static readonly Color broadcastColor = new Color(0, 255, 214);
    /// <summary>
    /// 计时器，60 Timer = 1 秒
    /// </summary>
    private static long Timer
    {
        get;
        set;
    }
    /// <summary>
    /// 清理数据的计时器
    /// </summary>
    private long clearTimer = long.MaxValue;
    /// <summary>
    /// 记录当前时间
    /// </summary>
    private string now { get; set; } = "";
    /// <summary>
    /// 记录需要冻结的玩家
    /// </summary>
    private static readonly List<MessPlayer> frozenPlayers = new();
    /// <summary>
    /// 需要记录的被击中的npc
    /// </summary>
    private static readonly List<StrikeNPC> strikeNPC = new();

    private readonly string nonExistPlayerWarning = GetString("该玩家不存在，请重新输入");
    private readonly string MultipleMatchPlayerWarning = GetString("该玩家不唯一，请重新输入");
    private readonly string playerOfflineWarning = GetString("该玩家不在线，正在查询离线数据");

    private static ZhipmConfig config = new();

    /// <summary>
    /// 记录世界吞噬者的数据 击中他的玩家的id, 那个玩家造成的伤害
    /// </summary>
    private readonly Dictionary<int, int> Eaterworld = new();
    /// <summary>
    /// 记录毁灭者的数据
    /// </summary>
    private readonly Dictionary<int, int> Destroyer = new();
    /// <summary>
    /// 记录血肉墙的数据
    /// </summary>
    private readonly Dictionary<int, int> FleshWall = new();

    #endregion

    public ZHIPM(Main game) : base(game) { }

    public override void Initialize()
    {
        /*
        if (!TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            Console.WriteLine(GetString("该插件需要开启SSC才能使用"));
            return;
        }
        */
        Timer = 0L;
        config = ZhipmConfig.LoadConfigFile();
        ZPDataBase = new ZplayerDB(TShock.DB);
        ZPExtraDB = new ZplayerExtraDB(TShock.DB);
        edPlayers = new List<ExtraData>();

        //用来对玩家进行额外数据库更新
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        //限制玩家名字类型
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
        //同步玩家的额外数据库
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
        //用于统计击杀生物数，击杀别的等等
        ServerApi.Hooks.NpcStrike.Register(this, this.OnNpcStrike);
        //用于统计击杀生物数，击杀别的等等
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNPCKilled);
        //记录死亡次数
        GetDataHandlers.KillMe.Register(this.OnPlayerKilled);
        //配置文件的更新
        GeneralHooks.ReloadEvent += this.OnReload;

        #region 指令
        Commands.ChatCommands.Add(new Command("zhipm.help", this.Help, "zhelp")
        {
            HelpText = GetString("输入 /zhelp  来查看指令帮助")
        });
        Commands.ChatCommands.Add(new Command("zhipm.save", this.MySSCSave, "zsave")
        {
            HelpText = GetString("输入 /zsave  来备份自己的人物存档")
        });
        Commands.ChatCommands.Add(new Command("zhipm.save", this.MySSCSaveAuto, "zsaveauto")
        {
            HelpText = GetString("输入 /zsaveauto [minute]  来每隔 minute 分钟自动备份自己的人物存档，当 minute 为 0 时关闭该功能")
        });
        Commands.ChatCommands.Add(new Command("zhipm.save", this.ViewMySSCSave, "zvisa")
        {
            HelpText = GetString("输入 /zvisa [num] 来查看自己的第几个人物备份")
        });
        Commands.ChatCommands.Add(new Command("zhipm.back", this.MySSCBack, "zback")
        {
            HelpText = GetString("输入 /zback [name]  来读取该玩家的人物存档\n输入 /zback [name] [num]  来读取该玩家的第几个人物存档")
        });
        Commands.ChatCommands.Add(new Command("zhipm.clone", this.SSCClone, "zclone")
        {
            HelpText = GetString("输入 /zclone [name1] [name2]  将玩家1的人物数据复制给玩家2\n输入 /zclone [name]  将该玩家的人物数据复制给自己")
        });
        Commands.ChatCommands.Add(new Command("zhipm.modify", this.SSCModify, "zmodify")
        {
            HelpText = GetString("输入 /zmodify help  查看修改玩家数据的指令帮助")
        });
        Commands.ChatCommands.Add(new Command("zhipm.out", this.ZhiExportPlayer, "zout")
        {
            HelpText = GetString("输入 /zout [name]  来导出该玩家的人物存档\n输入 /zout all  来导出所有人物的存档")
        });
        Commands.ChatCommands.Add(new Command("zhipm.sort", this.ZhiSortPlayer, "zsort")
        {
            HelpText = GetString("输入 /zsort help  来查看排序系列指令帮助")
        });
        Commands.ChatCommands.Add(new Command("zhipm.hide", this.HideTips, "zhide")
        {
            HelpText = GetString("输入 /zhide kill  来取消 kill + 1 的显示，再次使用启用显示\n输入 /zhide point  来取消 + 1 $ 的显示，再次使用启用显示")
        });

        Commands.ChatCommands.Add(new Command("zhipm.clear", this.Clear, "zclear")
        {
            HelpText = GetString("输入 /zclear useless  来清理世界的掉落物品，非城镇或BossNPC，和无用射弹\n输入 /zclear buff [name]  来清理该玩家的所有Buff\n输入 /zclear buff all  来清理所有玩家所有Buff")
        });


        Commands.ChatCommands.Add(new Command("zhipm.freeze", this.ZFreeze, "zfre")
        {
            HelpText = GetString("输入 /zfre [name]  来冻结该玩家")
        });
        Commands.ChatCommands.Add(new Command("zhipm.freeze", this.ZUnFreeze, "zunfre")
        {
            HelpText = GetString("输入 /zunfre [name]  来解冻该玩家\n输入 /zunfre all  来解冻所有玩家")
        });


        Commands.ChatCommands.Add(new Command("zhipm.reset", this.ZResetPlayerDB, "zresetdb")
        {
            HelpText = GetString("输入 /zresetdb [name]  来清理该玩家的备份数据\n输入 /zresetdb all  来清理所有玩家的备份数据")
        });
        Commands.ChatCommands.Add(new Command("zhipm.reset", this.ZResetPlayerEX, "zresetex")
        {
            HelpText = GetString("输入 /zresetex [name]  来清理该玩家的额外数据\n输入 /zresetex all  来清理所有玩家的额外数据")
        });
        Commands.ChatCommands.Add(new Command("zhipm.reset", this.ZResetPlayer, "zreset")
        {
            HelpText = GetString("输入 /zreset [name]  来清理该玩家的人物数据\n输入 /zreset all  来清理所有玩家的人物数据")
        });
        Commands.ChatCommands.Add(new Command("zhipm.reset", this.ZResetPlayerAll, "zresetallplayers")
        {
            HelpText = GetString("输入 /zresetallplayers  来清理所有玩家的所有数据")
        });


        Commands.ChatCommands.Add(new Command("zhipm.vi", this.ViewInvent, "vi")
        {
            HelpText = GetString("输入 /vi [name]  来查看该玩家的库存")
        });
        Commands.ChatCommands.Add(new Command("zhipm.vi", this.ViewInventDisorder, "vid")
        {
            HelpText = GetString("输入 /vid [name]  来查看该玩家的库存，不分类")
        });
        Commands.ChatCommands.Add(new Command("zhipm.vs", this.ViewState, "vs")
        {
            HelpText = GetString("输入 /vs [name]  来查看该玩家的状态")
        });


        Commands.ChatCommands.Add(new Command("zhipm.ban", this.SuperBan, "zban")
        {
            HelpText = GetString("输入 /zban add [name] [reason]  来封禁无论是否在线的玩家，reason 可不填")
        });

        Commands.ChatCommands.Add(new Command("zhipm.fun", this.Function, "zbpos")
        {
            HelpText = GetString("输入 /zbpos  来返回上次死亡地点")
        });
        /*
        Commands.ChatCommands.Add(new Command("zhipm.find", FindItem, "zfind")
        {
            HelpText = "输入 /zfind [id]  来查找当前哪些玩家拥有此物品"
        });
        */
        #endregion
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnNpcStrike);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNPCKilled);
            GetDataHandlers.KillMe.UnRegister(this.OnPlayerKilled);
            GeneralHooks.ReloadEvent -= this.OnReload;
            Commands.ChatCommands.RemoveAll(
                x => x.CommandDelegate == this.Help
                     || x.CommandDelegate == this.MySSCSave
                     || x.CommandDelegate == this.MySSCSaveAuto
                     || x.CommandDelegate == this.ViewMySSCSave
                     || x.CommandDelegate == this.MySSCBack
                     || x.CommandDelegate == this.SSCClone
                     || x.CommandDelegate == this.SSCModify
                     || x.CommandDelegate == this.ZhiExportPlayer
                     || x.CommandDelegate == this.ZhiSortPlayer
                     || x.CommandDelegate == this.HideTips
                     || x.CommandDelegate == this.Clear
                     || x.CommandDelegate == this.ZFreeze
                     || x.CommandDelegate == this.ZUnFreeze
                     || x.CommandDelegate == this.ZResetPlayerDB
                     || x.CommandDelegate == this.ZResetPlayerEX
                     || x.CommandDelegate == this.ZResetPlayer
                     || x.CommandDelegate == this.ZResetPlayerAll
                     || x.CommandDelegate == this.ViewInvent
                     || x.CommandDelegate == this.ViewInventDisorder
                     || x.CommandDelegate == this.ViewState
                     || x.CommandDelegate == this.SuperBan
                     || x.CommandDelegate == this.Function
            );
            base.Dispose(disposing);
        }
    }

    /*
    private void MMHook_PatchVersion_GetData(On.Terraria.MessageBuffer.orig_GetData orig, MessageBuffer self, int start, int length, out int messageType)
    {
        try
        {
            if (self.readBuffer[start] == 5)
            {
                using BinaryReader data = new(new MemoryStream(self.readBuffer, start + 1, length - 1));
                var playerID = data.ReadByte();
                if (self.whoAmI != playerID)
                {
                    self.readBuffer[start] = byte.MaxValue;
                    orig(self, start, length, out messageType);
                    return;
                }

                var slot = data.ReadInt16();
                var stack = data.ReadInt16();
                var prefix = data.ReadByte();
                var type = data.ReadInt16();
                var existingItem = Terraria.Main.player[playerID].inventory[slot];

                if (existingItem == null)
                {
                    bool 合理 = 检测物品是否合理(type);
                    if (!合理)
                        self.readBuffer[start] = byte.MaxValue;

                    orig(self, start, length, out messageType);
                    return;
                }

                if (!existingItem.IsAir || (type != 0 && stack != 0))
                {
                    if (existingItem.netID != type || existingItem.stack != stack || existingItem.prefix != prefix)
                    {
                        bool 合理 = 检测物品是否合理(type);
                        if (!合理)
                            self.readBuffer[start] = byte.MaxValue;

                        orig(self, start, length, out messageType);
                        return;
                    }
                }

                self.readBuffer[start] = byte.MaxValue;
                orig(self, start, length, out messageType);
                return;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"5号包处理异常:{ex}");
            var plr = TShock.Players[self.whoAmI];
        }
        orig(self, start, length, out messageType);
    }

    private bool 检测物品是否合理(short type)
    {
        if (type == 违规id)
            return false;
        else
            return true;
    }


    */


    /// <summary>
    /// 用来记录被冻结玩家数据的类
    /// </summary>
    public class MessPlayer
    {
        public int account;
        public string name;
        public string uuid;
        public Vector2 pos;
        public long clock;
        /// <summary>
        /// 只接受来自 user 的knowIPs，不是单个ip
        /// </summary>
        public string IPs;

        public MessPlayer()
        {
            this.account = 0;
            this.name = "";
            this.uuid = "";
            this.IPs = "";
            this.pos = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            this.clock = Timer;
        }

        public MessPlayer(int account, string name, string uuid, string IPs, Vector2 pos)
        {
            this.name = name;
            this.uuid = uuid;
            this.account = account;
            this.IPs = IPs;
            this.clock = Timer;
            this.pos = pos == Vector2.Zero ? new Vector2(0, 999999) : pos;
        }
    }


    /// <summary>
    /// 用来记录被玩家击中的npc
    /// </summary>
    public class StrikeNPC
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int index;
        /// <summary>
        /// id
        /// </summary>
        public int id;
        /// <summary>
        /// 名字
        /// </summary>
        public string name = string.Empty;
        /// <summary>
        /// 是否为boss
        /// </summary>
        public bool isBoss;
        /// <summary>
        /// 字典，用于记录 击中他的玩家的id 该玩家造成的总伤害
        /// </summary>
        public Dictionary<int, int> playerAndDamage = new();
        /// <summary>
        /// 受到的总伤害
        /// </summary>
        public int AllDamage;
        /// <summary>
        /// 价值
        /// </summary>
        public float value;

        public StrikeNPC() { }

        public StrikeNPC(int index, int id, string name, bool isBoss, Dictionary<int, int> playerAndDamage, int allDamage, float value)
        {
            this.index = index;
            this.id = id;
            this.name = name;
            this.isBoss = isBoss;
            this.playerAndDamage = playerAndDamage;
            this.AllDamage = allDamage;
            this.value = value;
        }
    }
}