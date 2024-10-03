using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Terraria;

namespace itemBox;

internal class Utils
{

	public static void GiveItem(int acID, int itemID, int prefix, int stack)
	{
		List<Item> list = new List<Item>();
		list = DB.GetUserInfo(acID);
		Item item = new Item();
		item.netID = itemID;
		item.prefix = (byte)prefix;
		item.stack = stack;
		list.Add(item);
		DB.UpdataInentory(acID, list);
	}

	public static void GiveItem(int acID, List<Item> items)
	{
		List<Item> list = new List<Item>();
		list = DB.GetUserInfo(acID);
		items = list.Concat(items).ToList();
		DB.UpdataInentory(acID, items);
	}

	public static List<Item> GetItems(int acID)
	{
		return DB.GetUserInfo(acID);
	}


}
