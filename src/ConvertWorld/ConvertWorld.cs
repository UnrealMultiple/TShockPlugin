using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ConvertWorld;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "onusai 羽学";
    public override Version Version => new Version(1, 0, 5);
    public override string Description => GetString("击败指定怪物替换世界指定图格与所有箱子内物品");
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig(); ;
        GeneralHooks.ReloadEvent += LoadConfig;
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNPCKilled);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= LoadConfig;
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNPCKilled);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig(ReloadEventArgs? args = null)
    {
        Config = Configuration.Read();
        WriteName();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[替换世界物品]重新加载配置完毕。"));
    }
    #endregion

    #region 获取NPCID的中文名
    private static void WriteName()
    {
        foreach (var group in Config.BossList)
        {
            var Names = new HashSet<string>(group.Name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var id in group.ID)
            {
                string npcName;
                npcName = (string) Lang.GetNPCName(id);
                if (!Names.Contains(npcName))
                {
                    Names.Add(npcName);
                }
            }
            group.Name = string.Join(", ", Names);
        }
    }
    #endregion

    #region 击杀指定怪物组 开启转换的方法
    internal static Dictionary<int, HashSet<int>> BossDowned = new Dictionary<int, HashSet<int>>();
    private readonly object npcLock = new object();
    private void OnNPCKilled(NpcKilledEventArgs args)
    {
        if (args.npc == null || !Config.NpcKilled)
        {
            return;
        }

        var isNpc = Config.BossList.FirstOrDefault(n => n.ID.Contains(args.npc.netID));

        if (isNpc != null)
        {
            lock (this.npcLock)
            {
                var bossId = isNpc.ID.First();
                if (!BossDowned.ContainsKey(bossId))
                {
                    BossDowned[bossId] = new HashSet<int>();
                }

                if (Config.KillAll)
                {
                    BossDowned[bossId].Add(args.npc.netID);
                    if (BossDowned[bossId].Count == isNpc.ID.Length)
                    {
                        Convert();
                    }
                }
                else
                {
                    Convert();
                }
            }
        }
    }
    #endregion

    #region 替换方法
    public static void Convert()
    {
        if (!Config.NpcKilled)
        {
            return;
        }

        var TilesUpdate = 0;
        var ItemUpdate = 0;

        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                foreach (var ID in Config.BossList)
                {
                    if (ID.TileID.ContainsKey(Main.tile[x, y].type))
                    {
                        Main.tile[x, y].type = ID.TileID[Main.tile[x, y].type];
                        TilesUpdate++;
                    }
                }
            }
        }

        foreach (var chest in Main.chest)
        {
            if (chest == null)
            {
                continue;
            }

            foreach (var item in chest.item)
            {
                foreach (var ID in Config.BossList)
                {
                    if (ID.ItemID.ContainsKey(item.type))
                    {
                        var count = item.stack;
                        item.SetDefaults(ID.ItemID[item.type]);
                        item.stack = count;
                        ItemUpdate++;
                    }
                }
            }
        }

        if (TilesUpdate + ItemUpdate > 0)
        {
            UpdateWorld();
            TShock.Utils.Broadcast(GetString("已替换世界物品! \n") +
                GetString($"转换的图格数量：[c/BEE9FA:{TilesUpdate}] \n") +
                GetString($"转换的箱子物品数量：[c/FFC4C2:{ItemUpdate}]"), 255, 234, 115);
        }
        else
        {
            return;
        }
    }
    #endregion

    #region 更新世界方法
    public static void UpdateWorld()
    {
        if (!Config.NpcKilled)
        {
            return;
        }

        foreach (var sock in Netplay.Clients.Where(s => s.IsActive))
        {
            for (var i = Netplay.GetSectionX(0); i <= Netplay.GetSectionX(Main.maxTilesX); i++)
            {
                for (var j = Netplay.GetSectionY(0); j <= Netplay.GetSectionY(Main.maxTilesY); j++)
                {
                    sock.TileSections[i, j] = false;
                }
            }
        }
    }
    #endregion

}