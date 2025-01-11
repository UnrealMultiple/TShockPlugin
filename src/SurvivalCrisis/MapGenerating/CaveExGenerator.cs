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
	public class CaveExGenerator : CaveGenerator
	{
		public CaveExGenerator() : base(SurvivalCrisis.Regions.CaveEx)
		{
			biome = BiomeType.DeepCave;
		}

		public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成深层洞穴...");
            GenerateCave(0.05);
            AddChest(ChestLevel.V2, 20);
            AddChest(ChestLevel.V3, 275);
            AddChest(ChestLevel.V4, 125);
            SurvivalCrisis.DebugMessage("深层洞穴生成成功");
        }

		protected override void AddChest(ChestLevel chest, int count)
		{
            const int width = 8; // 箱子正方形宽度
            var fillType = chest.Level % 4 + 1 == 4 ? TileID.RainbowBrick : TileID.LunarOre;
            var placed = new List<Point>();
            placed.Add(new Point(30000, 30000));
            for (int i = 0; i < count; i++)
            {
                int x;
                int y;
                bool canPlaceChest;
                do
                {
                    x = rand.Next(0, Coverage.Width - width);
                    y = rand.Next(0, Coverage.Height - width);
                    var xMin = placed.Min(v => Math.Abs(x - v.X));
                    var yMin = placed.Min(v => Math.Abs(y - v.Y));
                    canPlaceChest = xMin >= width && yMin >= width;
                }
                while (!canPlaceChest);
                for (int j = x; j < x + width; j++)
                {
                    for (int k = y; k < y + width; k++)
                    {
                        Coverage[j, k].type = fillType;
                        Coverage[j, k].active(true);
                        Coverage[j, k].slope(0);
                    }
                }
                chest.PlaceChest(x + (width - 2) / 2 + Coverage.X, y + (width - 2) / 2 + 1 + Coverage.Y);
            }
		}
	}
}
