using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ItemPreserver;

[ApiVersion(2, 1)]
public class ItemPreserver : TerrariaPlugin
{
    public override string Author => "肝帝熙恩 & 少司命";
    public override string Description => GetString("指定物品不消耗");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 10);
    public static Configuration Config = new();

    public class Pitem
    {
        public int Type { get; set; }

        public int Stack { get; set; }

        public Pitem(int type, int stack)
        {
            this.Type = type;
            this.Stack = stack;
        }
    };

    public Dictionary<TSPlayer, Dictionary<int, Pitem>> ItemUse = new();

    public ItemPreserver(Main game) : base(game)
    {
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
            if (ply != null && ply.Active)
            {
                if (!this.ItemUse.TryGetValue(ply, out var slot) || slot == null)
                {
                    this.ItemUse[ply] = new Dictionary<int, Pitem>();
                }
                for (var i = 0; i < ply.TPlayer.inventory.Length; i++)
                {
                    this.ItemUse[ply][i] = new(ply.TPlayer.inventory[i].netID, ply.TPlayer.inventory[i].stack);
                }
            }
        }
        args.Player?.SendSuccessMessage(GetString($"[{nameof(ItemPreserver)}] 重新加载配置完毕。"));
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += this.ReloadConfig;
        GetDataHandlers.PlayerSlot.Register(this.OnSlot);
        LoadConfig();
    }

    private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        if (this.ItemUse.TryGetValue(e.Player, out var itemUse) && itemUse != null)
        {
            if (itemUse.TryGetValue(e.Slot, out var slot) && slot != null)
            {
                if (e.Type == 0 || e.Stack == 0)
                {
                    if (e.Player.TPlayer.controlUseItem
                        && e.Player.TPlayer.inventory[e.Slot].consumable
                        && Config.NoConsumItem.TryGetValue(slot.Type, out var num)
                        && (slot.Stack >= num || num <= 0))
                    {
                        e.Player.TPlayer.inventory[e.Slot].netDefaults(slot.Type);
                        e.Player.TPlayer.inventory[e.Slot].stack = 1;
                        e.Player.SendData(PacketTypes.PlayerSlot, null, e.Player.Index, e.Slot);
                    }
                }
                else
                {
                    if (e.Player.TPlayer.controlUseItem
                        && e.Player.TPlayer.inventory[e.Slot].consumable
                        && Config.NoConsumItem.TryGetValue(e.Type, out var num)
                        && (slot.Stack >= num || num <= 0))
                    {
                        e.Player.TPlayer.inventory[e.Slot].stack = slot.Stack;
                        e.Player.SendData(PacketTypes.PlayerSlot, null, e.Player.Index, e.Slot);
                    }
                    else
                    {
                        this.ItemUse[e.Player][e.Slot].Stack = e.Stack;
                    }
                }
            }
            else
            {
                this.ItemUse[e.Player][e.Slot] = new(e.Type, e.Stack);
            }
        }
        else
        {
            this.ItemUse[e.Player] = new()
            {
                { e.Slot, new(e.Type, e.Stack) }
            };
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.ReloadConfig;
            GetDataHandlers.PlayerSlot.UnRegister(this.OnSlot);
        }
        base.Dispose(disposing);
    }



}