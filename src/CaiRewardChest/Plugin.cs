using On.OTAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CaiRewardChest;

[ApiVersion(2, 1)]
public class CaiRewardChest : TerrariaPlugin
{
    public CaiRewardChest(Main game)
        : base(game)
    {
    }

    public override string Author => "Cai";

    public override string Description => "奖励箱！！";

    public override string Name => "CaiRewardChest";

    public override Version Version => new(2024, 10, 1, 2);


    public override void Initialize()
    {
        Db.Init();
        GetDataHandlers.ChestOpen.Register(this.OnChestOpen);
        Hooks.Chest.InvokeQuickStack += this.ChestOnInvokeQuickStack;
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Init", this.InitChest, "初始化奖励箱", "rcinit"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Clear", this.ClearChest, "清空奖励箱", "rcclear"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Delete", this.DeleteChest, "删除奖励箱", "rcdelete", "rcdel"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Add", this.AddChest, "添加奖励箱", "rcadd"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Edit", this.EditChest, "编辑奖励箱", "rcedit"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.ChestOpen.UnRegister(this.OnChestOpen);
            Hooks.Chest.InvokeQuickStack -= this.ChestOnInvokeQuickStack;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InitChest ||
                                                x.CommandDelegate == this.ClearChest ||
                                                x.CommandDelegate == this.DeleteChest ||
                                                x.CommandDelegate == this.AddChest ||
                                                x.CommandDelegate == this.EditChest);
        }

        base.Dispose(disposing);
    }

    private bool ChestOnInvokeQuickStack(Hooks.Chest.orig_InvokeQuickStack orig, int playerid, Item item,
        int chestindex)
    {
        var chest = Db.GetChestById(chestindex);
        return chest == null;
    }

    private void EditChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要编辑的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Edit");
    }

    private void AddChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要添加的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Add");
    }

    private void DeleteChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要删除的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Del");
    }


    private void OnChestOpen(object? sender, GetDataHandlers.ChestOpenEventArgs e)
    {
        try
        {
            var chest = Db.GetChestByPos(e.X, e.Y);

            if (e.Player.ContainsData("WaitChestSetting"))
            {
                if (e.Player.GetData<string>("WaitChestSetting") == "Del")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest == null)
                    {
                        e.Player.SendWarningMessage(GetString("[i:48]这个箱子好像不是奖励箱捏~"));
                        e.Handled = true;
                        return;
                    }

                    Db.DelChest(chest.ChestId);
                    e.Player.SendSuccessMessage(GetString("[i:48]你删除了这个奖励箱~"));
                    e.Handled = true;
                    return;
                }

                if (e.Player.GetData<string>("WaitChestSetting") == "Add")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest != null)
                    {
                        e.Player.SendWarningMessage(GetString("[i:48]这个箱子好像是奖励箱捏~"));
                        e.Handled = true;
                        return;
                    }

                    Db.AddChest(Chest.FindChest(e.X, e.Y), e.X, e.Y);
                    e.Player.SendSuccessMessage(GetString("[i:48]你添加了一个奖励箱~"));
                    e.Handled = true;
                    return;
                }

                if (e.Player.GetData<string>("WaitChestSetting") == "Edit")
                {
                    e.Player.RemoveData("WaitChestSetting");
                    if (chest != null)
                    {
                        return;
                    }

                    e.Player.SendWarningMessage(GetString("[i:48]这个箱子好像不是奖励箱捏~"));
                    e.Handled = true;
                    return;
                }
            }


            if (chest == null)
            {
                return;
            }

            e.Handled = true;

            if (chest.HasOpenPlayer.Contains(e.Player.Account.ID))
            {
                e.Player.SendWarningMessage(
                    GetString($"[i:{WorldGen.GetChestItemDrop(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type)}]你已经领取过这个奖励箱啦!"));
                return;
            }

            Utils.GiveItem(chest, e);
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    private void ClearChest(CommandArgs args)
    {
        Db.ClearDb();
        args.Player.SendSuccessMessage(GetString("[i:48]奖励箱已全部清除~"));
    }

    private void InitChest(CommandArgs args)
    {
        Db.ClearDb();
        var count = 0;
        foreach (var chest in Main.chest)
        {
            if (chest != null && chest.item.Count(i=>i!=null && i.type != 0)>0)
            {
                Db.AddChest(Chest.FindChest(chest.x, chest.y), chest.x, chest.y);
                count++;
            }
        }

        args.Player.SendSuccessMessage(GetString($"[i:48]奖励箱初始化完成,共添加{count}个奖励箱~"));
    }

}