using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace SurvivalCrisis
{
	public static class Utils
	{
		public static string GenerateStr(this Random rand, int count)
		{
			string words = "qwertyuioplkjhgfdsazxcvbmQAZXSWEDCVFRTGBNHYUJMKIOLP1234567890";
			var chars = new char[count];
			for (int i = 0; i < count; i++)
			{
				chars[i] = words[rand.Next(words.Length)];
			}
			return new string(chars);
		}
		public static T Next<T>(this Random rand, IList<T> list)
		{
			return list[rand.Next(list.Count)];
		}
		public static T Next<T>(this Random rand, T[] array)
		{
			return array[rand.Next(array.Length)];
		}
		public static void Shuffle<T>(this Random rand, IList<T> list)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				int idx = rand.Next(list.Count - i - 1);
				var temp = list[idx];
				list[idx] = list[list.Count - i - 1];
				list[list.Count - i - 1] = temp;
			}
		}
		public static float NextFloat(this Random rand)
		{
			return (float)rand.NextDouble();
		}
		public static void SendCombatText(string msg, Color color, Point tilePos, int who = -1)
		{
			SendCombatText(msg, color, tilePos.X * 16, tilePos.Y * 16, who);
		}
		public static void SendCombatText(string msg, Color color, Vector2 pos, int who = -1)
		{
			SendCombatText(msg, color, pos.X, pos.Y, who);
		}
		public static void SendCombatText(string msg, Color color, float x, float y, int who = -1)
		{
			NetMessage.SendData(119, who, -1, NetworkText.FromLiteral(msg), (int)color.PackedValue, x, y, 0, 0, 0, 0);
		}
	}
}
