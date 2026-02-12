using Economics.Shop.Model;
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

    public override string Description => GetString("经济商店!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 4);

    public Shop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config.Load();
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            Config.UnLoad();
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
                if (item.type == term.netID)
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