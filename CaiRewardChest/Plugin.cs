using System.Diagnostics;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Utils = TShockAPI.Utils;

namespace CaiRewardChest;

[ApiVersion(2, 1)]
public class CaiRewardChest : TerrariaPlugin
{
    public override string Author => "Cai";

    public override string Description => "奖励箱！！";

    public override string Name => "CaiRewardChest";

    public override Version Version => new(1, 0, 0, 0);

    public CaiRewardChest(Main game)
        : base(game)
    {
    }


    public override void Initialize()
    {
        Db.Init();
        GetDataHandlers.ChestOpen.Register(OnChestOpen);
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Admin", InitChest, "初始化奖励箱"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Admin", ClearChest, "清空奖励箱"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Admin", DeleteChest, "删除奖励箱"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Admin", AddChest, "添加奖励箱"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Admin", EditChest, "编辑奖励箱"));
    }

    private void EditChest(CommandArgs args)
    {
        args.Player.SendInfoMessage("[i:48]请打开需要编辑的奖励箱~");
        args.Player.SetData("WaitChestSetting", "Edit");
    }

    private void AddChest(CommandArgs args)
    {
        args.Player.SendInfoMessage("[i:48]请打开需要添加的奖励箱~");
        args.Player.SetData("WaitChestSetting", "Add");
    }

    private void DeleteChest(CommandArgs args)
    {
        args.Player.SendInfoMessage("[i:48]请打开需要删除的奖励箱~");
        args.Player.SetData("WaitChestSetting", "Del");
    }


    private void OnChestOpen(object? sender, GetDataHandlers.ChestOpenEventArgs e)
    {
        try
        {
            RewardChest? chest = Db.GetChestByPos(e.X, e.Y);

            if (e.Player.ContainsData("WaitChestSetting"))
            {
                if (e.Player.GetData<string>("WaitChestSetting") == "Del")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest == null)
                    {
                        e.Player.SendWarningMessage("[i:48]这个箱子好像不是奖励箱捏~");
                        e.Handled = true;
                        return;
                    }

                    Db.DelChest(chest.ChestId);
                    e.Player.SendSuccessMessage("[i:48]你删除了这个奖励箱~");
                    e.Handled = true;
                    return;
                }

                if (e.Player.GetData<string>("WaitChestSetting") == "Add")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest != null)
                    {
                        e.Player.SendWarningMessage("[i:48]这个箱子好像是奖励箱捏~");
                        e.Handled = true;
                        return;
                    }

                    Db.AddChest(Chest.FindChest(e.X, e.Y), e.X, e.Y);
                    e.Player.SendSuccessMessage("[i:48]你添加了一个奖励箱~");
                    e.Handled = true;
                    return;
                }

                if (e.Player.GetData<string>("WaitChestSetting") == "Edit")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest != null) return;
                    e.Player.SendWarningMessage("[i:48]这个箱子好像不是奖励箱捏~");
                    e.Handled = true;
                    return;
                }
            }


            if (chest == null) return;
            e.Handled = true;

            if (chest.HasOpenPlayer.Contains(e.Player.Account.ID))
            {
                e.Player.SendWarningMessage(
                    $"[i:{WorldGen.GetChestItemDrop(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type)}]你已经领取过这个奖励箱啦!");
                return;
            }

            GiveItem(chest, e);
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static int InventorySlotAvailableCount(TSPlayer plr)
    {
        int num = 0;
        if (plr.RealPlayer)
            for (int index = 0; index < 50; ++index)
                if (plr.TPlayer.inventory[index] == null || !plr.TPlayer.inventory[index].active ||
                    plr.TPlayer.inventory[index].Name == "")
                    ++num;
        return num;
    }

    public void GiveItem(RewardChest chest, GetDataHandlers.ChestOpenEventArgs args)
    {
        int chestType = WorldGen.GetChestItemDrop(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type);
        if (InventorySlotAvailableCount(args.Player) >=
            chest.Chest.item.Count(i => i != null && i.netID != 0 && i.stack != 0) + 1)
        {
            foreach (Item? i in chest.Chest.item) args.Player.GiveItem(i.netID, i.stack, i.prefix);
            List<string> itemsReceived = chest.Chest.item
                .Where(i => i != null && i.netID != 0 && i.stack != 0)
                .Select(i => TShock.Utils.ItemTag(i)).ToList();


            itemsReceived.Add(TShock.Utils.ItemTag(new Item()
            {
                netID = chestType,
                stack = 1
            }));
            args.Player.GiveItem(chestType, 1, 0);
            args.Player.SendSuccessMessage($"[i:{chestType}]你打开了一个奖励箱: " +
                                           $"" + string.Join(", ", itemsReceived));
            chest.HasOpenPlayer.Add(args.Player.Account.ID);
            Db.UpdateChest(chest);
        }
        else
        {
            args.Player.SendWarningMessage($"[i:{chestType}]你的背包格子不够哦," +
                                           $"还需要清空{chest.Chest.item.Count(i => i != null && i.netID != 0 && i.stack != 0) + 1 - InventorySlotAvailableCount(args.Player)}个格子!");
        }
    }

    private void ClearChest(CommandArgs args)
    {
        Db.ClearDb();
        args.Player.SendSuccessMessage("[i:48]奖励箱已全部清除~");
    }

    private void InitChest(CommandArgs args)
    {
        Db.ClearDb();
        int count = 0;
        foreach (Chest? chest in Main.chest)
            if (chest != null)
            {
                Db.AddChest(Chest.FindChest(chest.x, chest.y), chest.x, chest.y);
                count++;
            }

        args.Player.SendSuccessMessage($"[i:48]奖励箱初始化完成,共添加{count}个奖励箱~");
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing) GetDataHandlers.ChestOpen.UnRegister(OnChestOpen);

        base.Dispose(disposing);
    }
}