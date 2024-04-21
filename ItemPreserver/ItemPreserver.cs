using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ItemPreserver;

[ApiVersion(2, 1)]
public class ItemPreserver : TerrariaPlugin
{
    public override string Author => "肝帝熙恩 & 少司命";
    public override string Description => "指定物品不消耗";
    public override string Name => "ItemPreserver";
    public override Version Version => new Version(1, 0, 4);
    public static Configuration Config;

    public class Pitem
    { 
        public int Type { get; set; }

        public int Stack { get; set; }

        public Pitem(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }
    };

    public Dictionary<TSPlayer, Dictionary<int, Pitem>> ItemUse = new();

    public ItemPreserver(Main game) : base(game)
    {
        LoadConfig();
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        foreach (var ply in TShock.Players)
        {
            if (!ItemUse.TryGetValue(ply, out var slot) || slot == null)
            {
                ItemUse[ply] = new Dictionary<int, Pitem>();
            }
            if (ply != null && ply.Active)
            {
                for (int i = 0; i < ply.TPlayer.inventory.Length; i++)
                {
                    ItemUse[ply][i] = new(ply.TPlayer.inventory[i].netID, ply.TPlayer.inventory[i].stack);
                }
            }
        }
        args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(ItemPreserver).Name);
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        GetDataHandlers.PlayerSlot.Register(OnSlot);

    }

    private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        if (ItemUse.TryGetValue(e.Player, out Dictionary<int, Pitem>? itemUse) && itemUse != null)
        {
            if (itemUse.TryGetValue(e.Slot, out var slot) && slot != null)
            {
                if (e.Type == 0 || e.Stack == 0)
                {
                    if (e.Player.TPlayer.controlUseItem && e.Player.TPlayer.inventory[e.Slot].consumable && Config.NoConsumItem.Contains(slot.Type))
                    {
                        e.Player.TPlayer.inventory[e.Slot].netDefaults(slot.Type);
                        e.Player.TPlayer.inventory[e.Slot].stack = 1;
                        e.Player.SendData(PacketTypes.PlayerSlot, null, e.Player.Index, e.Slot);
                    }
                }
                else
                {
                    if (e.Player.TPlayer.controlUseItem && e.Player.TPlayer.inventory[e.Slot].consumable && Config.NoConsumItem.Contains(e.Type))
                    {
                        e.Player.TPlayer.inventory[e.Slot].stack = slot.Stack;
                        e.Player.SendData(PacketTypes.PlayerSlot, null, e.Player.Index, e.Slot);
                    }
                    else
                    {
                        ItemUse[e.Player][e.Slot].Stack = e.Stack;
                    }
                }
            }
            else
            {
                ItemUse[e.Player][e.Slot] = new(e.Type, e.Stack);
            }
        }
        else
        {
            ItemUse[e.Player] = new()
            {
                { e.Slot, new(e.Type, e.Stack) }
            };
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
        }
        base.Dispose(disposing);
    }

 

}
