using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ZHIPlayerManager
{
    [ApiVersion(2, 1)]
    public partial class ZHIPM : TerrariaPlugin
    {
        public override string Author => "z枳";

        public override string Description => "玩家管理，提供修改玩家的任何信息，允许玩家备份，可以回档等操作";

        public override string Name => "ZHIPlayerManager";

        public override Version Version => new Version(1, 0, 0, 5);

        #region 字段或属性
        /// <summary>
        /// 人物备份数据库
        /// </summary>
        public static ZplayerDB ZPDataBase { get; set; }
        /// <summary>
        /// 额外数据库
        /// </summary>
        public static ZplayerExtraDB ZPExtraDB { get; set; }
        /// <summary>
        /// 在线玩家的额外数据库的集合
        /// </summary>
        public static List<ExtraData> edPlayers { get; set; }
        /// <summary>
        /// 广播颜色
        /// </summary>
        public readonly static Color broadcastColor = new Color(0, 255, 213);
        /// <summary>
        /// 计时器，60 Timer = 1 秒
        /// </summary>
        public static long Timer
        {
            get;
            private set;
        }
        /// <summary>
        /// 清理数据的计时器
        /// </summary>
        public long cleartime = long.MaxValue;
        /// <summary>
        /// 记录当前时间
        /// </summary>
        public string now { get; set; }
        /// <summary>
        /// 记录需要冻结的玩家
        /// </summary>
        public static List<MessPlayer> frePlayers = new List<MessPlayer>();
        /// <summary>
        /// 需要记录的被击中的npc
        /// </summary>
        public static List<StrikeNPC> strikeNPC = new List<StrikeNPC>();

        public readonly string noplayer = "该玩家不存在，请重新输入";
        public readonly string manyplayer = "该玩家不唯一，请重新输入";
        public readonly string offlineplayer = "该玩家不在线，正在查询离线数据";

        public static ZhipmConfig config = new ZhipmConfig();

        /// <summary>
        /// 记录世界吞噬者的数据 <击中他的玩家的id, 那个玩家造成的伤害>
        /// </summary>
        public Dictionary<int, int> Eaterworld = new Dictionary<int, int>();
        /// <summary>
        /// 记录毁灭者的数据
        /// </summary>
        public Dictionary<int, int> Destroyer = new Dictionary<int, int>();
        /// <summary>
        /// 记录血肉墙的数据
        /// </summary>
        public Dictionary<int, int> FleshWall = new Dictionary<int, int>();

        #endregion

        public ZHIPM(Main game) : base(game) { }

        public override void Initialize()
        {
            if (!TShock.ServerSideCharacterConfig.Settings.Enabled)
            {
                Console.WriteLine("该插件需要开启SSC才能使用");
                return;
            }
            Timer = 0L;
            config = ZhipmConfig.LoadConfigFile();
            ZPDataBase = new ZplayerDB(TShock.DB);
            ZPExtraDB = new ZplayerExtraDB(TShock.DB);
            edPlayers = new List<ExtraData>();

            //用来对玩家进行额外数据库更新
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            //限制玩家名字类型
            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            //同步玩家的额外数据库
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            //用于统计击杀生物数，击杀别的等等
            ServerApi.Hooks.NpcStrike.Register(this, OnNpcStrike);
            //用于统计击杀生物数，击杀别的等等
            ServerApi.Hooks.NpcKilled.Register(this, OnNPCKilled);
            //记录死亡次数
            GetDataHandlers.KillMe.Register(OnPlayerKilled);
            //配置文件的更新
            GeneralHooks.ReloadEvent += OnReload;

            #region 指令
            Commands.ChatCommands.Add(new Command("zhipm.help", Help, "zhelp")
            {
                HelpText = "输入 /zhelp  来查看指令帮助"
            });
            Commands.ChatCommands.Add(new Command("zhipm.save", MySSCSave, "zsave")
            {
                HelpText = "输入 /zsave  来备份自己的人物存档"
            });
            Commands.ChatCommands.Add(new Command("zhipm.save", MySSCSaveAuto, "zsaveauto")
            {
                HelpText = "输入 /zsaveauto [minute]  来每隔 minute 分钟自动备份自己的人物存档，当 minute 为 0 时关闭该功能"
            });
            Commands.ChatCommands.Add(new Command("zhipm.save", ViewMySSCSave, "zvisa")
            {
                HelpText = "输入 /zvisa [num] 来查看自己的第几个人物备份"
            });
            Commands.ChatCommands.Add(new Command("zhipm.back", MySSCBack, "zback")
            {
                HelpText = "输入 /zback [name]  来读取该玩家的人物存档\n输入 /zback [name] [num]  来读取该玩家的第几个人物存档"
            });
            Commands.ChatCommands.Add(new Command("zhipm.clone", SSCClone, "zclone")
            {
                HelpText = "输入 /zclone [name1] [name2]  将玩家1的人物数据复制给玩家2\n输入 /zclone [name]  将该玩家的人物数据复制给自己"
            });
            Commands.ChatCommands.Add(new Command("zhipm.modify", SSCModify, "zmodify")
            {
                HelpText = "输入 /zmodify help  查看修改玩家数据的指令帮助"
            });
            Commands.ChatCommands.Add(new Command("zhipm.out", ZhiExportPlayer, "zout")
            {
                HelpText = "输入 /zout [name]  来导出该玩家的人物存档\n输入 /zout all  来导出所有人物的存档"
            });
            Commands.ChatCommands.Add(new Command("zhipm.sort", ZhiSortPlayer, "zsort")
            {
                HelpText = "输入 /zsort help  来查看排序系列指令帮助"
            });
            Commands.ChatCommands.Add(new Command("zhipm.hide", HideTips, "zhide")
            {
                HelpText = "输入 /zhide kill  来取消 kill + 1 的显示，再次使用启用显示\n输入 /zhide point  来取消 + 1 $ 的显示，再次使用启用显示"
            });

            Commands.ChatCommands.Add(new Command("zhipm.clear", Clear, "zclear")
            {
                HelpText = "输入 /zclear useless  来清理世界的掉落物品，非城镇或BossNPC，和无用射弹\n输入 /zclear buff [name]  来清理该玩家的所有Buff\n输入 /zclear buff all  来清理所有玩家所有Buff"
            });


            Commands.ChatCommands.Add(new Command("zhipm.freeze", ZFreeze, "zfre")
            {
                HelpText = "输入 /zfre [name]  来冻结该玩家"
            });
            Commands.ChatCommands.Add(new Command("zhipm.freeze", ZUnFreeze, "zunfre")
            {
                HelpText = "输入 /zunfre [name]  来解冻该玩家\n输入 /zunfre all  来解冻所有玩家"
            });


            Commands.ChatCommands.Add(new Command("zhipm.reset", ZResetPlayerDB, "zresetdb")
            {
                HelpText = "输入 /zresetdb [name]  来清理该玩家的备份数据\n输入 /zresetdb all  来清理所有玩家的备份数据"
            });
            Commands.ChatCommands.Add(new Command("zhipm.reset", ZResetPlayerEX, "zresetex")
            {
                HelpText = "输入 /zresetex [name]  来清理该玩家的额外数据\n输入 /zresetex all  来清理所有玩家的额外数据"
            });
            Commands.ChatCommands.Add(new Command("zhipm.reset", ZResetPlayer, "zreset")
            {
                HelpText = "输入 /zreset [name]  来清理该玩家的人物数据\n输入 /zreset all  来清理所有玩家的人物数据"
            });
            Commands.ChatCommands.Add(new Command("zhipm.reset", ZResetPlayerAll, "zresetallplayers")
            {
                HelpText = "输入 /zresetallplayers  来清理所有玩家的所有数据"
            });


            Commands.ChatCommands.Add(new Command("zhipm.vi", ViewInvent, "vi")
            {
                HelpText = "输入 /vi [name]  来查看该玩家的库存"
            });
            Commands.ChatCommands.Add(new Command("zhipm.vi", ViewInventDisorder, "vid")
            {
                HelpText = "输入 /vid [name]  来查看该玩家的库存，不分类"
            });
            Commands.ChatCommands.Add(new Command("zhipm.vs", ViewState, "vs")
            {
                HelpText = "输入 /vs [name]  来查看该玩家的状态"
            });


            Commands.ChatCommands.Add(new Command("zhipm.ban", SuperBan, "zban")
            {
                HelpText = "输入 /zban add [name] [reason]  来封禁无论是否在线的玩家，reason 可不填"
            });

            Commands.ChatCommands.Add(new Command("zhipm.fun", Function, "zbpos")
            {
                HelpText = "输入 /zbpos  来返回上次死亡地点"
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
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                ServerApi.Hooks.NpcStrike.Deregister(this, OnNpcStrike);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNPCKilled);
                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
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
                account = 0;
                name = "";
                uuid = "";
                IPs = "";
                pos = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
                clock = Timer;
            }

            public MessPlayer(int account, string name, string uuid, string IPs, Vector2 pos)
            {
                this.name = name;
                this.uuid = uuid;
                this.account = account;
                this.IPs = IPs;
                this.clock = Timer;
                if (pos == Vector2.Zero)
                    this.pos = new Vector2(0, 999999);
                else
                    this.pos = pos;
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
            public bool isBoss = false;
            /// <summary>
            /// 字典，用于记录 <击中他的玩家的id -> 该玩家造成的总伤害>
            /// </summary>
            public Dictionary<int, int> playerAndDamage = new Dictionary<int, int>();
            /// <summary>
            /// 受到的总伤害
            /// </summary>
            public int AllDamage = 0;
            /// <summary>
            /// 价值
            /// </summary>
            public float value = 0f;

            public StrikeNPC() { }

            public StrikeNPC(int index, int id, string name, bool isBoss, Dictionary<int, int> playerAndDamage, int allDamage, float value)
            {
                this.index = index;
                this.id = id;
                this.name = name;
                this.isBoss = isBoss;
                this.playerAndDamage = playerAndDamage;
                AllDamage = allDamage;
                this.value = value;
            }
        }
    }
}
