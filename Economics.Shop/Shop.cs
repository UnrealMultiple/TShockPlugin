using System.Reflection;
using Economics.Shop.Model;
using EconomicsAPI.Configured;
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

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Shop.json");

    internal static Config Config { get; set; }
    public Shop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        GeneralHooks.ReloadEvent += (_) => Config = ConfigHelper.LoadConfig(PATH, Config);
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
