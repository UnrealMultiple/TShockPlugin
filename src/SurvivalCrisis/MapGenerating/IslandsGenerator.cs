using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using Microsoft.Xna.Framework;

namespace SurvivalCrisis.MapGenerating
{
	public partial class IslandsGenerator : Generator
	{
		private const int IslandCount = 12;
		private const int VanillaIslandCount = 2;
		private Random rand;

		public IslandsGenerator() : base(SurvivalCrisis.Regions.Islands)
		{
			rand = new Random();
		}

		public override void Generate()
		{
			SurvivalCrisis.DebugMessage("生成空岛...");

			bool checkProbability(double α)
			{
				return rand.NextDouble() < 0.3 - (α - 0.45) * (α - 0.45);
			}
			var savedIslands = LoadIslands();

			var islands = new List<TileSection>(savedIslands.Count);

			int t = 0;
			while (t < IslandCount)
			{
				var x = rand.Next(Coverage.Width) + Coverage.X;
				var y = rand.Next(Coverage.Height) + Coverage.Y;
				var island = rand.Next(savedIslands);
				var block = new TileSection(x, y, island.Width, island.Height);
				if (Coverage.InRange(block) && checkProbability((double)(y - Coverage.Y) / Coverage.Height))
				{
					island.AffixTo(block);
					islands.Add(block);
					t++;
				}
			}
			t = 0;
			while (t < VanillaIslandCount)
			{
				var x = rand.Next(Coverage.Width / 3) + Coverage.Width / 3 + Coverage.X;
				var y = rand.Next(Coverage.Height / 2) + Coverage.Height / 4 + Coverage.Y;
				var block = new TileSection(x, y, 120, 60);
				if (Coverage.InRange(block) && checkProbability((double)y / Coverage.Height))
				{
					WorldGen.CloudLake(x, y);
					t++;
				}
			}

			foreach (var island in islands)
			{
				ChestLevel.V5.Generate(island);
			}
			SurvivalCrisis.DebugMessage("空岛生成成功");
		}

		/// <summary>
		/// 空岛
		/// </summary>
		/// <returns></returns>
		private List<TileBlockData> LoadIslands()
		{
			var files = new DirectoryInfo(SurvivalCrisis.IslandsPath).GetFiles("SkyIsland*V5.sec");
			var islands = new List<TileBlockData>(files.Length);
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					islands.Add(TileBlockData.FromFile(files[i].FullName));
				}
				catch
				{
					var msg = $"无效文件: {files[i].FullName}";
					TShock.Log.ConsoleError(msg);
				}
			}
			return islands;
		}
	}
}
