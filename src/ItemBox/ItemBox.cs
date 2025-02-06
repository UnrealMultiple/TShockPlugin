using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ItemBox;

[ApiVersion(2, 1)]
public class Main : TerrariaPlugin
{
    public Main(Terraria.Main game)
        : base(game)
    {
    }

    public override string Author => "Cai & 2409";

    public override string Description => GetString("离线物品盒子系统.");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2024, 12, 18, 3);

    public override void Initialize()
    {
        DB.Connect();
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnLogin, 1);
        Commands.ChatCommands.Add(new Command(new List<string> { "ItemBox.claim" }, this.GetBoxItems, "cbox", "领盒子"));
        Commands.ChatCommands.Add(new Command(new List<string> { "ItemBox.give" }, this.GiveBoxItems, "gbox", "给盒子"));
        Commands.ChatCommands.Add(new Command(new List<string> { "ItemBox.remove" }, this.ResetBoxItems, "rbox", "重置盒子"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnLogin);
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method.DeclaringType?.Assembly == asm);
        }

        base.Dispose(disposing);
    }

    private void OnLogin(GreetPlayerEventArgs args)
    {
        var tSPlayer = TShock.Players[args.Who];
        List<Item> list;
        var text = "";
        list = Utils.GetItems(tSPlayer.Account.ID);
        if (list.Count == 0)
        {
            return;
        }

        foreach (var item in list)
        {
            text += $"[i/s{item.stack}:{item.netID}]";
        }

        tSPlayer.SendInfoMessage(GetString($"[i:4131]检测到你的邮箱里面有物品：{text}\n[i:4131]快输入/cbox领取吧 :)"));
    }

    private void ResetBoxItems(CommandArgs args)
    {
        DB.ClearDB();
        args.Player.SendInfoMessage(GetString("[i:4131]离线背包清理成功!"));
    }

    private void GiveBoxItems(CommandArgs args)
    {
        try
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendInfoMessage(GetString("[i:4131]/<gbox/给盒子> <用户名> <物品ID> <物品数量> <物品前缀> --- 给玩家背包发送物品"));
                return;
            }

            var userAccountId = TShock.UserAccounts.GetUserAccountID(args.Parameters[0]);
            if (userAccountId == -1)
            {
                args.Player.SendErrorMessage(GetString("[i:4131]发送邮箱失败, 玩家未注册!"));
                return;
            }

            List<Item> itemByIdOrName = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (itemByIdOrName.Count > 1)
            {
                args.Player.SendMultipleMatchError(itemByIdOrName);
                return;
            }

            if (itemByIdOrName.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("[i:4131]没有找到你需要的物品捏!"));
                return;
            }

            var netId = itemByIdOrName[0].netID;
            var num = int.Parse(args.Parameters[2]);
            if (num <= 0)
            {
                args.Player.SendErrorMessage(GetString("[i:4131]物品数量无效!"));
                return;
            }

            var maxStack = itemByIdOrName[0].maxStack;
            var num2 = num % maxStack;
            var num3 = num / maxStack;
            var num4 = int.Parse(args.Parameters[3]);
            var list = new List<Item>();
            for (var i = 1; i <= num3; i++)
            {
                var item = new Item();
                item.netID = netId;
                item.prefix = (byte) num4;
                item.stack = maxStack;
                list.Add(item);
            }

            if (num2 != 0)
            {
                var item = new Item();
                item.netID = netId;
                item.prefix = (byte) num4;
                item.stack = maxStack;
                item.stack = num2;
                list.Add(item);
            }

            Utils.GiveItem(userAccountId, list);
            if (maxStack == 1)
            {
                args.Player.SendInfoMessage(GetString($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{maxStack}:{netId}]×{num3},前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱"));
            }
            else if (num3 == 0 || num3 == 1)
            {
                args.Player.SendInfoMessage(GetString($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{(num2 != 0 ? num2 : maxStack)}:{netId}],前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱"));
            }
            else
            {
                args.Player.SendInfoMessage(GetString($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{maxStack}:{netId}]×{num3}+[i/p{args.Parameters[3]}/s{num2}:{args.Parameters[1]}],前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱"));
            }

            List<TSPlayer> list2 = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (list2.Count != 0)
            {
                var tSPlayer = list2[0];
                tSPlayer.SendMessage(GetString($"你收到了一份邮件,物品内容如下：[i/p{args.Parameters[3]}:{netId}]×{args.Parameters[2]}"), 0, byte.MaxValue, 0);
            }
        }
        catch
        {
            args.Player.SendErrorMessage(GetString("[i:4131]输入错误，正确输入：/<gbox/给盒子> <用户名> <物品ID> <物品数量> <物品前缀>---给玩家背包发送物品 "));
        }
    }

    private void GetBoxItems(CommandArgs args)
    {
        var text = "";
        List<Item> list;
        list = Utils.GetItems(args.Player.Account.ID);
        if (list.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("[i:4131]你邮箱没有盒子，领了个寂寞！"));
            return;
        }

        foreach (var item in list)
        {
            text += $"[i/s{item.stack}:{item.netID}]";
            args.Player.GiveItem(item.netID, item.stack, item.prefix);
        }

        DB.ClearPlayerInventory(args.Player.Account.ID);
        args.Player.SendInfoMessage(GetString($"[i:4131]成功领取邮箱物品：{text}"));
    }
}