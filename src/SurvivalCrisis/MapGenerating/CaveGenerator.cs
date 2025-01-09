using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.MapGenerating
{
	public partial class CaveGenerator: Generator
    {
        protected Random rand;
        protected ushort oreTypeEvil;
        protected ushort oreTypeH3;
        protected ushort aBrick;
        protected BiomeType biome;
        public CaveGenerator(TileSection? range = null) : base(range ?? SurvivalCrisis.Regions.Cave)
        {
            rand = new Random();
            oreTypeEvil = rand.Next(2) == 0 ? TileID.Demonite : TileID.Crimtane;
            oreTypeH3 = rand.Next(2) == 0 ? TileID.Titanium : TileID.Adamantite;
            aBrick = rand.Next(4) switch
            {
                0 => TileID.SolarBrick,
                1 => TileID.VortexBrick,
                2 => TileID.NebulaBrick,
                3 => TileID.StardustBrick
            };
            biome = BiomeType.Icy;
        }

		public override void Generate()
        {
            SurvivalCrisis.DebugMessage("生成洞穴...");
            GenerateCave(0.05);
            AddLifeCrystal(300);
            AddChest(ChestLevel.V1, 40);
            AddChest(ChestLevel.V2, 180);
            AddChest(ChestLevel.V3, 50);
            AddChest(ChestLevel.V7, 10);
            SurvivalCrisis.DebugMessage("洞穴生成成功");
        }


        protected ushort GetTileType(double f, BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.DeepCave:
                    if (f < 0.39)
                        return oreTypeH3;
                    if (f < 0.45)
                        return TileID.Stone;
                    if (f < 0.475)
                        return TileID.LunarBrick;
                    if (f < 0.49)
                        return aBrick;
                    break;
                case BiomeType.Icy:
                    if (f < 0.335)
                        return oreTypeEvil;
                    if (f < 0.37)
                        return TileID.SmoothSandstone;
                    if (f < 0.4)
                        return TileID.BreakableIce;
                    if (f < 0.43)
                        return TileID.IceBlock;
                    if (f < 0.49)
                        return TileID.SnowBlock;
                    break;
                case BiomeType.Meteorite:
                    if (f < 0.4)
                        return TileID.PumpkinBlock;
                    if (f < 0.45)
                        return TileID.Stone;
                    if (f < 0.475)
                        return TileID.TinPlating;
                    if (f < 0.49)
                        return TileID.MeteoriteBrick;
                    break;
                case BiomeType.Jungle:
                    if (f < 0.35)
                        return TileID.PumpkinBlock;
                    if (f < 0.45)
                        return TileID.Mudstone;
                    if (f < 0.475)
                        return TileID.Mud;
                    if (f < 0.49)
                        return TileID.JungleThorns;
                    break;
                case BiomeType.Building:
                    if (f < 0.43)
                        return TileID.Gold;
                    if (f < 0.47)
                        return TileID.Stone;
                    if (f < 0.49)
                        return TileID.DynastyWood;
                    break;
            }
            return TileID.HellstoneBrick;
        }

        protected ushort GetWallType(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Icy:
                    return WallID.SnowWallUnsafe;
                case BiomeType.Meteorite:
                    return WallID.CopperPipeWallpaper;
                case BiomeType.Jungle:
                    return WallID.Jungle;
                case BiomeType.Building:
                    return WallID.LivingWood;
            }
            return 0;
        }
        protected void FixTop(double delta)
        {
            PerlinNoise perlinNoise = new PerlinNoise();
            for (int i = Coverage.Left; i < Coverage.Right; i++)
            {
                double X = delta * i, Y = delta * Coverage.Top;
                int v = (int)(perlinNoise.Value(X, Y) * 12);
                for (int j = Coverage.Top - 1; j > Coverage.Top - v; j--)
                {
                    if (Main.tile[i, j].active())
                        break;
                    Main.tile[i, j].active(false);
                    Main.tile[i, j].wall = 0;
                }
            }
        }
        protected void GenerateCave(double delta)
        {
            var perlinNoise = new PerlinNoise();
            for (int i = 0; i < Coverage.Width; i++)
            {
                for (int j = 0; j < Coverage.Height; j++)
                {
                    var X = delta * i;
                    var Y = delta * j;
                    var v = perlinNoise.Value(X, Y);
                    var biome = this.biome;
                    var tileType = GetTileType(v, biome);
                    var wallType = GetWallType(biome);
                    if (tileType != TileID.HellstoneBrick)
                    {
                        Coverage.PlaceTileAt(i, j, tileType);
                        if (tileType == TileID.Crimtane || tileType == TileID.Demonite)
                        {
                            Coverage[i, j].color(PaintID.WhitePaint);
                        }
                    }
                    Coverage.PlaceWallAt(i, j, wallType);
                }
            }
            //FixTop(range, delta);
        }
        protected virtual void AddChest(ChestLevel chest, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x;
                int y;
                bool canPlaceChest;
                do
                {
                    x = rand.Next(0, Coverage.Width - 1);
                    y = rand.Next(1, Coverage.Height - 1);
                    canPlaceChest =
                        !Coverage[x, y - 1].active() && !Coverage[x + 1, y - 1].active() &&
                        !Coverage[x, y - 0].active() && !Coverage[x + 1, y - 0].active() &&
                        Coverage[x, y + 1].active() && Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceChest);
                Coverage[x + 0, y + 1].type = TileID.SnowBrick;
                Coverage[x + 1, y + 1].type = TileID.SnowBrick;
                Coverage[x + 0, y + 1].active(true);
                Coverage[x + 1, y + 1].active(true);
                Coverage[x + 0, y + 1].slope(0);
                Coverage[x + 1, y + 1].slope(0);
                chest.PlaceChest(x + Coverage.X, y + Coverage.Y);
            }
        }
        protected void AddLifeCrystal(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x;
                int y;
                bool canPlaceCrystal;
                do
                {
                    x = rand.Next(0, Coverage.Width - 1);
                    y = rand.Next(1, Coverage.Height - 1);
                    canPlaceCrystal =
                        !Coverage[x, y - 1].active() && !Coverage[x + 1, y - 1].active() &&
                        !Coverage[x, y - 0].active() && !Coverage[x + 1, y - 0].active() &&
                        Coverage[x, y + 1].active() && Coverage[x + 1, y + 1].active();
                }
                while (!canPlaceCrystal);
                Coverage[x + 0, y + 1].type = TileID.SnowBrick;
                Coverage[x + 1, y + 1].type = TileID.SnowBrick;
                Coverage[x + 0, y + 1].active(true);
                Coverage[x + 1, y + 1].active(true);
                Coverage[x + 0, y + 1].slope(0);
                Coverage[x + 1, y + 1].slope(0);
                WorldGen.AddLifeCrystal(x + Coverage.X, y + Coverage.Y);
            }
        }
    }
}
