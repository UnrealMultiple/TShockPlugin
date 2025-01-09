﻿using On.OTAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CaiRewardChest;

[ApiVersion(2, 1)]
public class CaiRewardChest : TerrariaPlugin
{
    public static List<int> RewardChestId = new(); //用于防止快速堆叠爆SQL

    public CaiRewardChest(Main game)
        : base(game)
    {
    }

    public override string Author => "Cai";

    public override string Description => GetString("奖励箱！！");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2024, 12, 18, 2);


    public override void Initialize()
    {
        RewardChestId = RewardChest.GetAllChestId();
        GetDataHandlers.ChestOpen.Register(OnChestOpen);
        Hooks.Chest.InvokeQuickStack += ChestOnInvokeQuickStack;
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Init", InitChest, "初始化奖励箱", "rcinit"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Clear", ClearChest, "清空奖励箱", "rcclear"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Delete", DeleteChest, "删除奖励箱", "rcdelete", "rcdel"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Add", AddChest, "添加奖励箱", "rcadd"));
        Commands.ChatCommands.Add(new Command("CaiRewardChest.Edit", EditChest, "编辑奖励箱", "rcedit"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.ChestOpen.UnRegister(OnChestOpen);
            Hooks.Chest.InvokeQuickStack -= ChestOnInvokeQuickStack;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == InitChest ||
                                                 x.CommandDelegate == ClearChest ||
                                                 x.CommandDelegate == DeleteChest ||
                                                 x.CommandDelegate == AddChest ||
                                                 x.CommandDelegate == EditChest);
        }

        base.Dispose(disposing);
    }

    private static bool ChestOnInvokeQuickStack(Hooks.Chest.orig_InvokeQuickStack orig, int playerid, Item item,
        int chestindex)
    {
        return !RewardChestId.Contains(chestindex);
    }

    private static void OnChestOpen(object? sender, GetDataHandlers.ChestOpenEventArgs e)
    {
        if (e.Player.ContainsData("WaitChestSetting"))
        {
            var chest = RewardChest.GetChestByPos(e.X, e.Y);
            if (e.Player.GetData<string>("WaitChestSetting") == "Add")
            {
                e.Player.RemoveData("WaitChestSetting");
                if (chest != null)
                {
                    e.Player.SendWarningMessage(GetString("[i:48]这个箱子好像是奖励箱捏~"));
                    e.Handled = true;
                    return;
                }

                RewardChest.AddChest(Chest.FindChest(e.X, e.Y), e.X, e.Y);
                e.Player.SendSuccessMessage(GetString("[i:48]你添加了一个奖励箱~"));
                e.Handled = true;
                return;
            }


            if (e.Player.GetData<string>("WaitChestSetting") == "Del")
            {
                e.Player.RemoveData("WaitChestSetting");
                if (chest == null)
                {
                    e.Player.SendWarningMessage(GetString("[i:48]这个箱子好像不是奖励箱捏~"));
                    e.Handled = true;
                    return;
                }

                RewardChest.DelChest(chest.ChestId);
                e.Player.SendSuccessMessage(GetString("[i:48]你删除了这个奖励箱~"));
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

        if (!RewardChestId.Contains(Chest.FindChest(e.X, e.Y)))
        {
            return;
        }

        var chest2 = RewardChest.GetChestByPos(e.X, e.Y)!;
        e.Handled = true;

        if (chest2.HasOpenPlayer.Contains(e.Player.Account.ID))
        {
            e.Player.SendWarningMessage(
                GetString($"[i:{WorldGen.GetChestItemDrop(chest2.X, chest2.Y, Main.tile[chest2.X, chest2.Y].type)}]你已经领取过这个奖励箱啦!"));
            return;
        }

        Utils.GiveItem(chest2, e);
    }

    private static void EditChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要编辑的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Edit");
    }

    private static void AddChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要添加的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Add");
    }

    private static void DeleteChest(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("[i:48]请打开需要删除的奖励箱~"));
        args.Player.SetData("WaitChestSetting", "Del");
    }


    private static void ClearChest(CommandArgs args)
    {
        RewardChest.ClearDb();
        args.Player.SendSuccessMessage(GetString("[i:48]奖励箱已全部清除~"));
    }

    private static void InitChest(CommandArgs args)
    {
        RewardChest.ClearDb();
        var count = 0;
        foreach (var chest in Main.chest)
        {
            if (chest != null && chest.item.Count(i => i != null && i.type != 0) > 0)
            {
                RewardChest.AddChest(Chest.FindChest(chest.x, chest.y), chest.x, chest.y);
                count++;
            }
        }

        args.Player.SendSuccessMessage(GetString($"[i:48]奖励箱初始化完成,共添加{count}个奖励箱~"));
    }
}