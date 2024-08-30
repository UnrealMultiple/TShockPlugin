using Economics.RPG.Extensions;
using Economics.Shop.Model;
using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using EconomicsAPI.Events;
using EconomicsAPI.Extensions;
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

    public override Version Version => new(1, 0, 0, 1);

    internal string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Shop.json");

    internal static Config Config { get; set; } = new();
    public Shop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
    }

    public void LoadConfig(ReloadEventArgs? args = null) => Config = ConfigHelper.LoadConfig(PATH, Config);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            GeneralHooks.ReloadEvent -= LoadConfig;
        }
        base.Dispose(disposing);
    }


    public static bool HasItem(TSPlayer player, List<ItemTerm> itemTerms)
    {
        foreach (ItemTerm term in itemTerms)
        {
            var count = 0;
            CheckBanksForItem(player, term.netID, ref count);
            if (count < term.Stack)
                return false;

        }
        ConsumeItem(player, itemTerms);
        return true;
    }

    private static void ConsumeItem(TSPlayer player, List<ItemTerm> terms)
    {
        foreach (var term in terms)
        {
            var stack = term.Stack;
            for (int j = 0; j < player.TPlayer.inventory.Length; j++)
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
        for (int j = 0; j < player.TPlayer.inventory.Length; j++)
        {
            if (player.TPlayer.inventory[j].type == itemId)// 检查猪猪储钱罐
            {
                itemCount += player.TPlayer.inventory[j].stack;
            }
        }
    }

  
}
