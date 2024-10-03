using System;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace itemBox;

[ApiVersion(2, 1)]
public class Main : TerrariaPlugin
{
	public override string Author => "Cai & 2409";

	public override string Description => "An itemBox for item give to offline player.";

	public override string Name => "itemBox Extensions";

	public override Version Version => new Version(1, 0, 0, 0);

	public override void Initialize()
	{
		DB.Connect();
		ServerApi.Hooks.NetGreetPlayer.Register(this, OnLogin, 1);
		Commands.ChatCommands.Add(new Command(new List<string> { "itemBox.claim" }, Lbb, "cbox", "领盒子"));
		Commands.ChatCommands.Add(new Command(new List<string> { "itemBox.give" }, Gbb, "gbox", "给盒子"));
		Commands.ChatCommands.Add(new Command(new List<string> { "itemBox.remove" }, Rbb, "rbox", "清盒子"));
	}
    public Main(Terraria.Main game)
    : base(game)
	{

	}
    private void OnLogin(GreetPlayerEventArgs args)
	{
		TSPlayer tSPlayer = TShock.Players[args.Who];
		List<Item> list = new List<Item>();
		string text = "";
		list = Utils.GetItems(tSPlayer.Account.ID);
		if (list.Count == 0)
		{
			return;
		}
		foreach (Item item in list)
		{
			text += $"[i/s{item.stack}:{item.netID}]";
		}
		tSPlayer.SendInfoMessage("[i:4131]检测到你的邮箱里面有物品：" + text + "\n[i:4131]快输入/cbox领取吧 :)");
	}

	private void Rbb(CommandArgs args)
	{
		DB.ClearDB();
		args.Player.SendInfoMessage("[i:4131]离线背包清理成功!");
	}

	private void Gbb(CommandArgs args)
	{
		try
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage("[i:4131]/<gbox/给盒子> <用户名> <物品ID> <物品数量> <物品前缀>---给玩家背包发送物品");
				return;
			}
			int userAccountID = TShock.UserAccounts.GetUserAccountID(args.Parameters[0]);
			if (userAccountID == -1)
			{
				args.Player.SendErrorMessage("[i:4131]发送邮箱失败, 玩家未注册");
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
				args.Player.SendErrorMessage("[i:4131]没有找到你需要的物品呐!");
				return;
			}
			int netID = itemByIdOrName[0].netID;
			int num = int.Parse(args.Parameters[2]);
			if (num <= 0)
			{
				args.Player.SendErrorMessage("[i:4131]物品数量无效!");
				return;
			}
			int maxStack = itemByIdOrName[0].maxStack;
			int num2 = num % maxStack;
			int num3 = num / maxStack;
			int num4 = int.Parse(args.Parameters[3]);
			List<Item> list = new List<Item>();
			for (int i = 1; i <= num3; i++)
			{
				Item item = new Item();
				item.netID = netID;
				item.prefix = (byte)num4;
				item.stack = maxStack;
				list.Add(item);
			}
			if (num2 != 0)
			{
				Item item = new Item();
				item.netID = netID;
				item.prefix = (byte)num4;
				item.stack = maxStack;
				item.stack = num2;
				list.Add(item);
			}
			Utils.GiveItem(userAccountID, list);
			if (maxStack == 1)
			{
				args.Player.SendInfoMessage($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{maxStack}:{netID}]×{num3},前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱");
			}
			else if (num3 == 0 || num3 == 1)
			{
				args.Player.SendInfoMessage($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{((num2 != 0) ? num2 : maxStack)}:{netID}],前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱");
			}
			else
			{
				args.Player.SendInfoMessage($"[i:4131]成功把物品：[i/p{args.Parameters[3]}/s{maxStack}:{netID}]×{num3}+[i/p{args.Parameters[3]}/s{num2}:{args.Parameters[1]}],前缀：{args.Parameters[3]}，发送到[c/FFFFFF:{args.Parameters[0]}]的邮箱");
			}
			List<TSPlayer> list2 = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (list2.Count != 0)
			{
				TSPlayer tSPlayer = list2[0];
				tSPlayer.SendMessage($"你收到了一份邮件,物品内容如下：[i/p{args.Parameters[3]}:{netID}]×{args.Parameters[2]}", 0, byte.MaxValue, 0);
			}
		}
		catch
		{
			args.Player.SendErrorMessage("[i:4131]输入错误，正确输入：/<gbox/给盒子> <用户名> <物品ID> <物品数量> <物品前缀>---给玩家背包发送物品 ");
		}
	}

	private void Lbb(CommandArgs args)
	{
		string text = "";
		List<Item> list = new List<Item>();
		list = Utils.GetItems(args.Player.Account.ID);
		if (list.Count == 0)
		{
			args.Player.SendErrorMessage("[i:4131]你邮箱没有盒子，领了个寂寞！");
			return;
		}
		foreach (Item item in list)
		{
			text += $"[i/s{item.stack}:{item.netID}]";
			args.Player.GiveItem(item.netID, item.stack, item.prefix);
		}
		DB.ClearPlayerInventory(args.Player.Account.ID);
		args.Player.SendInfoMessage("[i:4131]成功领取邮箱物品：" + text);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
		base.Dispose(disposing);
	}
}
