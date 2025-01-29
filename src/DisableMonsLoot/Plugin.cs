using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static DisableMonsLoot.Commands;
using static DisableMonsLoot.Configuration;


namespace DisableMonsLoot;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 3, 4);
    public override string Author => "羽学";
    public override string Description => GetString("清理怪物身边掉落物");
    #endregion

    #region 注册释放钩子
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => LoadConfig();
    }
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.NpcKilled.Register(this, this.KillItem);
        TShockAPI.Commands.ChatCommands.Add(new Command("killitem.admin", kdm, "kdm", "禁掉落"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.NpcKilled.Deregister(this, this.KillItem);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == kdm);
        }

        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig()
    {
        Config = Read();
        WriteName();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[禁怪物掉落]重新加载配置完毕。"));
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



    #region 击杀指定怪物组 关闭清理的方法
    internal static Dictionary<int, HashSet<int>> BossDowned = new Dictionary<int, HashSet<int>>();
    private readonly object npcLock = new object();
    public NPC RealNPC { get; set; } = null!;
    private void KillItem(NpcKilledEventArgs args)
    {
        lock (this.npcLock)
        {
            if (args.npc == null || !Config.Enabled)
            {
                return;
            }

            foreach (var npc in Config.BossList)
            {
                if (npc.ID.Contains(args.npc.netID))
                {
                    if (!Config.KillAll)
                    {
                        if (!BossDowned.ContainsKey(npc.ID.First()))
                        {
                            BossDowned[npc.ID.First()] = new HashSet<int>();
                            npc.Enabled = false;
                            Config.Write();
                        }
                    }

                    else
                    {
                        BossDowned[npc.ID.First()].Add(args.npc.netID);
                        if (BossDowned[npc.ID.First()].Count == npc.ID.Length)
                        {
                            npc.Enabled = false;
                            Config.Write();
                        }
                    }
                    break;
                }

                if (npc.Enabled) //控制是否清理
                {
                    this.RealNPC = args.npc;
                    this.ClearItems(Config.radius, npc.ItemID);
                }

            }
        }
    }
    #endregion



    #region 清理物品方法
    private void ClearItems(int radius, int[] ItemIDs)
    {
        if (!Config.Enabled)
        {
            return;
        }

        for (var i = 0; i < Terraria.Main.maxItems; i++)
        {
            var item = Terraria.Main.item[i];
            var dx = item.position.X - this.RealNPC.position.X;
            var dy = item.position.Y - this.RealNPC.position.Y;
            var Distance = (dx * dx) + (dy * dy);

            if (item.active && Distance <= radius * radius * 256f)
            {
                if (ItemIDs.Contains(item.netID))
                {
                    Terraria.Main.item[i].active = false;
                    TSPlayer.All.SendData(PacketTypes.ItemDrop, "", i);
                }
            }
        }
    }
    #endregion

}