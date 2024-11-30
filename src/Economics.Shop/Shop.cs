using Economics.Shop.Model;
using EconomicsAPI.Configured;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Shop;

[ApiVersion(2, 1)]
public class Shop : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new(2, 0, 0, 0);

    internal string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Shop.json");

    internal static Config Config { get; set; } = new();
    public Shop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        GeneralHooks.ReloadEvent += this.LoadConfig;
    }

    public void LoadConfig(ReloadEventArgs? args = null)
    {
        Config = ConfigHelper.LoadConfig(this.PATH, Config);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }


    public static bool HasItem(TSPlayer player, List<ItemTerm> itemTerms)
    {
        foreach (var term in itemTerms)
        {
            var count = 0;
            CheckBanksForItem(player, term.netID, ref count);
            if (count < term.Stack)
            {
                return false;
            }
        }
        ConsumeItem(player, itemTerms);
        return true;
    }

    private static void ConsumeItem(TSPlayer player, List<ItemTerm> terms)
    {
        foreach (var term in terms)
        {
            var stack = term.Stack;
            for (var j = 0; j < player.TPlayer.inventory.Length; j++)
            {
                var item = player.TPlayer.inventory[j];
                if (item.netID == term.netID)
                {
                    if (item.stack >= stack)
                    {
                        item.stack -= stack;
                        TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", player.Index, j);
                    }
                    else
                    {
                        stack -= item.stack;
                    }
                }
            }
        }
    }

    private static void CheckBanksForItem(TSPlayer player, int itemId, ref int itemCount)
    {
        for (var j = 0; j < player.TPlayer.inventory.Length; j++)
        {
            if (player.TPlayer.inventory[j].type == itemId)// 检查猪猪储钱罐
            {
                itemCount += player.TPlayer.inventory[j].stack;
            }
        }
    }


}