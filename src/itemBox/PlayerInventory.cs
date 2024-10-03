using System;
using System.Collections.Generic;
using Terraria;

namespace itemBox;

public class PlayerInventory
{
	public List<Item> Inventory { get; set; } = new List<Item>();


	public void Clear()
	{
		Inventory.Clear();
	}

	public string GetString()
	{
		string text = "";
		foreach (Item item in Inventory)
		{
			text = text + item.netID + "-" + item.prefix + "-" + item.stack + ",";
		}
		return text;
	}

	public bool Load(string itemString)
	{
		try
		{
			string[] array = itemString.Split(",");
			foreach (string text in array)
			{
				string[] array2 = text.Split("-");
				Item item = new Item();
				item.netID = int.Parse(array2[0]);
				item.prefix = byte.Parse(array2[1]);
				item.stack = byte.Parse(array2[2]);
				Inventory.Add(item);
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			return false;
		}
		return true;
	}

	public static List<Item> Prase(string itemString)
	{
		List<Item> list = new List<Item>();
		try
		{
			if (string.IsNullOrEmpty(itemString))
			{
				return new List<Item>();
			}
			string[] array = itemString.Split(",");
			foreach (string text in array)
			{
				string[] array2 = text.Split("-");
				Item item = new Item();
				item.netID = int.Parse(array2[0]);
				item.prefix = byte.Parse(array2[1]);
				item.stack = int.Parse(array2[2]);
				list.Add(item);
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			return null;
		}
		return list;
	}

	public static string ToString(List<Item> items)
	{
		string text = "";
		foreach (Item item in items)
		{
			text = text + item.netID + "-" + item.prefix + "-" + item.stack + ",";
		}
		return text.Substring(0, text.Length - 1);
	}
}
