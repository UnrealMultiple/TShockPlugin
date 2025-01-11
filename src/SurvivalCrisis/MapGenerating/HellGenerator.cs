using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
	public class HellGenerator : Generator
	{
        private Random rand;
		public HellGenerator() : base(SurvivalCrisis.Regions.Hell)
		{
            rand = new Random();
		}

		public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成地狱...");
            Coverage.Fill(TileID.Ash);
            AddChest(ChestLevel.V3, 30);
            AddChest(ChestLevel.V4, 40);
            AddChest(ChestLevel.V6, 50);
            SurvivalCrisis.DebugMessage("地狱生成成功");
        }

        private void AddChest(ChestLevel chest, int count)
        {
            const int brickWidth = 4;
            const int width = 14; // 箱子正方形宽度
            var placed = new List<Point>();
            placed.Add(new Point(30000, 30000));
            for (int i = 0; i < count; i++)
            {
                int x = 0;
                int y = 0;
                bool canPlaceChest;
                do
                {
                    x = rand.Next(0, Coverage.Width - width);
                    y = rand.Next(0, Coverage.Height - width);
                    var xMin = placed.Min(v => Math.Abs(x - v.X));
                    var yMin = placed.Min(v => Math.Abs(y - v.Y));
                    canPlaceChest = xMin >= width || yMin >= width;// && Coverage[x + 0, y + 1].active() && Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceChest);
                placed.Add(new Point(x, y));
                for (int j = x; j < x + width; j++)
                {
                    for (int k = y; k < y + width; k++)
                    {
                        Coverage[j, k].active(false);
                        Coverage[j, k].liquid = 255;
                        Coverage[j, k].liquidType(Tile.Liquid_Lava);
                    }
                }
                for (int j = x + (width - brickWidth * 2 - 2) / 2; j < x + width - (width - brickWidth * 2 - 2) / 2; j++)
                {
                    for (int k = y + (width - brickWidth * 2 - 2) / 2; k < y + width - (width - brickWidth * 2 - 2) / 2; k++)
                    {
                        Coverage[j, k].type = TileID.LihzahrdBrick;
                        Coverage[j, k].active(true);
                    }
                }
                for (int j = x + (width - 2) / 2; j < x + width - (width  - 2) / 2; j++)
                {
                    for (int k = y + (width  - 2) / 2; k < y + width - (width - 2) / 2; k++)
                    {
                        Coverage[j, k].active(false);
                        Coverage[j, k].liquid = 0;
                    }
                }
                chest.PlaceChest(x + (width - 2) / 2 + Coverage.X, y + (width - 2) / 2 + 1 + Coverage.Y);
                for (int j = x + (width - 2) / 2; j < x + width - (width - 2) / 2; j++)
                {
                    for (int k = y + (width - 2) / 2; k < y + width - (width - 2) / 2; k++)
                    {
                        Coverage[j, k].liquid = 255;
                    }
                }
            }
        }
    }
}
