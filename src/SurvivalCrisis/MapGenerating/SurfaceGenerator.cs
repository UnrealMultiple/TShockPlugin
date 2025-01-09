using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis.MapGenerating
{
	public class SurfaceGenerator : Generator
	{
		private Random rand;
		private IslandsGenerator.TileBlockData hall;
		private IslandsGenerator.TileBlockData npcHouse;
		private int[] snowMinX;
		private int[] snowMaxX;
		private int snowTop;
		private int snowBottom;
		private int snowOriginLeft;
		private int snowOriginRight;
		private int waterLine;
		private int lavaLine;
		private int maxTunnels;
		private int numTunnels;
		private int[] tunnelX;
		public SurfaceGenerator() : base(SurvivalCrisis.Regions.Surface)
		{
			rand = new Random();
			snowMinX = new int[Coverage.Height];
			snowMaxX = new int[Coverage.Height];
			snowTop = 0;
			snowBottom = 0;
			snowOriginLeft = 0;
			snowOriginRight = Coverage.Width;
			lavaLine = Coverage.Height * 3 / 4;
			waterLine = 0;
			maxTunnels = 10;
			tunnelX = new int[maxTunnels];
			numTunnels = 0;
		}

		public override void Generate()
		{
			SurvivalCrisis.DebugMessage("生成地表...");
			try
			{
				npcHouse = IslandsGenerator.TileBlockData.FromFile(Path.Combine(SurvivalCrisis.IslandsPath, "NPCHouse.sec"));
				hall = IslandsGenerator.TileBlockData.FromFile(Path.Combine(SurvivalCrisis.IslandsPath, "Hall.sec"));
				GenerateSnowySurface();
				GenerateHall();
				AddTrees();
			}
			catch(Exception e)
			{
				TSPlayer.All.SendErrorMessage(e.ToString());
				TSPlayer.Server.SendErrorMessage(e.ToString());
			}
			SurvivalCrisis.DebugMessage("地表生成成功");
		}

		private void AddTrees()
		{
			int count = 0;
			int maxCount = 8000;
			var settings = new WorldGen.GrowTreeSettings
			{
				TreeHeightMax = 50,
				TreeHeightMin = 10,
				GroundTest = x => true,
				WallTest = x => true,
				SaplingTileType = TileID.Saplings,
				TreeTileType = TileID.Trees
			};
			for (int x = 0; x < Coverage.Width - 1; x++)
			{
				for (int y = 20; y <= 100; y++)
				{
					bool canPlantTree =
							Coverage[x, y].wall == 0 && Coverage[x + 1, y].wall == 0 &&
							!Coverage[x, y].active() && !Coverage[x + 1, y].active() &&
							Coverage[x, y].liquid == 0 && Coverage[x + 1, y].liquid == 0 &&
							Coverage[x, y + 1].active() && Coverage[x + 1, y + 1].active() &&
							Coverage[x, y + 1].type == TileID.SnowBlock && Coverage[x + 1, y + 1].type == TileID.SnowBlock;

					if (canPlantTree && rand.NextDouble() < 0.15)
					{
						WorldGen.GrowTreeWithSettings(Coverage.X + x, Coverage.Y + y + 1, settings);
						count++;
					}
					if (count > maxCount)
					{
						return;
					}
				}
			}
		}

		private void Tunnels()
		{
			for (int i = 0; i < (int)(Coverage.Width * 0.0015); i++)
			{
				if (numTunnels >= maxTunnels - 1)
				{
					break;
				}
				int[] array = new int[10];
				int[] array2 = new int[10];
				int num861 = rand.Next(450, Coverage.Width - 450);
				while ((double)num861 > (double)Coverage.Width * 0.4 && (double)num861 < (double)Coverage.Width * 0.6)
				{
					num861 = rand.Next(450, Coverage.Width - 450);
				}
				int num862 = 0;
				bool flag58;
				do
				{
					flag58 = false;
					for (int idx = 0; idx < 10; idx++)
					{
						for (num861 %= Coverage.Width; !Coverage[num861, num862].active(); num862++)
						{
						}
						if (Coverage[num861, num862].type == 53)
						{
							flag58 = true;
						}
						array[idx] = num861;
						array2[idx] = num862 - rand.Next(11, 16);
						num861 += rand.Next(5, 11);
					}
				}
				while (flag58);
				tunnelX[numTunnels] = array[5];
				numTunnels++;
				for (int num864 = 0; num864 < 10; num864++)
				{
					TileRunner(array[num864], array2[num864], rand.Next(5, 8), rand.Next(6, 9), 0, addTile: true, -2f, -0.3f);
					TileRunner(array[num864], array2[num864], rand.Next(5, 8), rand.Next(6, 9), 0, addTile: true, 2f, -0.3f);
				}
			}
		}

		private void GenerateHall()
		{
			var npcs = new[] { NPCID.Nurse, NPCID.Guide, NPCID.GoblinTinkerer, NPCID.ArmsDealer, NPCID.Merchant };
			var point = Coverage.Center;
			point.Y = 20;
			while (!Coverage[point].active() && Coverage[point].wall == WallID.None)
			{
				point.Y++;
			}
			point.Y -= 5;
			SurvivalCrisis.Regions.Hall = Coverage.SubSection(point.X - hall.Width / 2, point.Y - hall.Height, hall.Width + npcs.Length * (2 + npcHouse.Width), hall.Height);
			SurvivalCrisis.Regions.Hall.TurnToAir();
			hall.AffixTo(SurvivalCrisis.Regions.Hall);

			var raw = point;

			point.X += hall.Width / 2 + 2;
			point.Y -= hall.Height / 2 + npcHouse.Height / 2;


			for (int i = 0; i < npcs.Length; i++)
			{
				var section = Coverage.SubSection(point.X, point.Y, npcHouse.Width, npcHouse.Height);
				npcHouse.AffixTo(section);
				var idx = NPC.NewNPC(new EntitySource_DebugCommand(), (point.X + npcHouse.Width / 2 + Coverage.X) * 16, (point.Y + 2 + Coverage.Y) * 16, npcs[i]);
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", idx);
				point.X += 2 + npcHouse.Width;
			}

			raw.Y -= hall.Height;
		}

		private void GenerateSnowySurface()
		{
			// Tunnels();
			//for (int i = 0; i < 10; i++)
			//{
			//	var x = rand.Next(Coverage.Width);
			//	var y = rand.Next(35, 60);
			//	TileRunner(x, y, rand.Next(5, 10), rand.Next(50, 200), TileID.SnowBlock, true, rand.Next(250, 400) / 100f, killTile: false);
			//}
			//for (int i = 0; i < 60; i++)
			//{
			//	var x = rand.Next(Coverage.Width);
			//	var y = rand.Next(45, 70);
			//	TileRunner(x, y, rand.Next(5, 8), rand.Next(20, 80), TileID.SnowBlock, true, rand.Next(250, 400) / 100f, killTile: false);
			//}
			{
				float k = 85;
				float kAcceleration = 0;
				int accelT = 0;
				float kVelocity = 0;
				double m = 0;
				for (int x = 0; x < Coverage.Width; x++)
				{
					if (accelT > 0)
					{
						kVelocity += kAcceleration;
						accelT--;
					}
					else
					{
						kVelocity *= 0.75f;
						if (Math.Abs(kVelocity) < 0.2f)
						{
							m += 0.200;
							if (rand.NextDouble() < 0.25 + m)
							{
								var a = rand.Next(-10, 10) / 10f;
								a = a * a * a * 16;
								accelT = rand.Next(15, 45);
								kAcceleration = a / accelT;
								m = 0;
							}
						}
						else
						{
							m = 0;
						}
					}
					while (k + kVelocity < 70 || k + kVelocity >= 90)
					{
						kVelocity *= -0.25f;
						kAcceleration *= Math.Sign(kVelocity);
					}
					k += kVelocity;
					for (int y = (int)k; y < 90; y++)
					{
						Coverage.PlaceTileAt(new Point(x, y), TileID.SnowBlock);
					}
				}
			}
			for (int k = 90; k < Coverage.Height; k++)
			{
				for (int j = 0; j < Coverage.Width; j++)
				{
					Coverage.PlaceTileAt(new Point(j, k), TileID.SnowBlock);
				}
			}
			for (int i = 0; i < 10; i++)
			{
				var x = rand.Next(Coverage.Width);
				var y = rand.Next(50, Coverage.Height);
				TileRunner(x, y, rand.Next(5, 15), rand.Next(400, 1300), TileID.SnowBlock, true, speedY: rand.Next(250, 400) / 100f, killTile: true);
			}
			for (int i = 0; i < 5; i++)
			{
				var x = rand.Next(Coverage.Width);
				var y = rand.Next(50, Coverage.Height);
				TileRunner(x, y, rand.Next(15, 20), rand.Next(750, 1300), TileID.SnowBlock, true, speedY: rand.Next(350, 450) / 100f, killTile: true);
			}
			for (int i = 0; i < 60; i++)
			{
				var x = rand.Next(Coverage.Width);
				var y = rand.Next(90, Coverage.Height);
				TileRunner(x, y, rand.Next(15, 40), rand.Next(10, 30), TileID.SnowBlock, true, speedY: rand.Next(250, 400) / 100f, killTile: true);
			}
			for (int i = 0; i < 100; i++)
			{
				var x = rand.Next(Coverage.Width);
				var y = rand.Next(105, Coverage.Height);
				TileRunner(x, y, rand.Next(10, 20), rand.Next(5, 40), TileID.SnowBlock, true, speedY: rand.Next(250, 400) / 100f, killTile: true);
				if (rand.NextDouble() < 0.3 + 0.6 * (y - 100) / (Coverage.Height - 100))
				{
					ChestLevel chest = ChestLevel.V1;
					if (Coverage.Height - y < 80)
					{
						var a = rand.NextDouble();
						if (a > 0.95)
						{
							chest = ChestLevel.V3;
						}
						else if (a > 0.7)
						{
							chest = ChestLevel.V2;
						}
					}
					else
					{
						if (rand.NextDouble() > 0.9)
						{
							chest = ChestLevel.V2;
						}
					}
					x += 5;
					y += 3;
					Coverage.PlaceTileAt(new Point(x + 0, y + 1), TileID.SnowBlock);
					Coverage.PlaceTileAt(new Point(x + 1, y + 1), TileID.SnowBlock);
					chest.PlaceChest(x + Coverage.X, y + Coverage.Y);
				}
			}
			for (int i = 0; i < Coverage.Width; i++)
			{
				//for (int j = 0; j < 2; j++)
				//{
				//	if (!Coverage[i, j].active())
				//	{
				//		Coverage[i, j].liquid = 1;
				//		Coverage[i, j].liquidType(Tile.Liquid_Water);
				//	}
				//}
				if (!Coverage[i, Coverage.Height - 2].active())
				{
					Coverage[i, Coverage.Height - 1].active(false);
				}
			}
			for (int k = 90; k < Coverage.Height; k++)
			{
				for (int j = 0; j < Coverage.Width; j++)
				{
					if (!Coverage[j, k].active() || Coverage[j, k].type == TileID.Heart || Coverage[j, k].type == TileID.Containers)
					{
						Coverage.PlaceWallAt(new Point(j, k), WallID.SnowWallUnsafe);
					}
				}
			}
			{
				int x = Coverage.Width * 1 / 10;
				int mountCount = rand.Next(3, 6);
				for (int i = 0; i < mountCount; i++)
				{
					List<int> heights = new List<int>();
					for (; x < Coverage.Width * 9 / 10; x++)
					{
						for (int y = 60; y < 90; y++)
						{
							if (Coverage[x, y].active())
							{
								if (heights.Count == 0)
								{
									if (rand.NextDouble() < 0.5f)
									{
										heights.Add(y);
									}
								}
								else
								{
									heights.Add(y);
									break;
								}
							}
						}
						if (heights.Count > 50)
						{
							var average = heights.Average();
							var σ2 = heights.Average(value => (value - average) * (value - average));

							if (σ2 < 0.07)
							{
								SurvivalCrisis.DebugMessage($"({x - 49 + Coverage.X}, {heights[0] + Coverage.Y})");
								const float q = 0.4f;
								var width = MakeMount(x - 49, heights[0], (rand.NextFloat() * 1f + 2.0f) * q * q, 5 * q);
								x += width;
								break;
							}
							heights.Clear();
						}
					}
				}
			}
			{

				int t = 0;
				int maxLakeCount = rand.Next(3, 6);
				int x = 0;
				int y = 0;
				int size = rand.Next(400, 900);
				int failAttemptCount = 0;
				while (t < maxLakeCount)
				{
					x = rand.Next(x, Coverage.Width);
					y = 0;
					while (!Coverage[x, y].active() && y < Coverage.Height)
					{
						y++;
					}
					if (y == Coverage.Height)
					{
						continue;
					}
					if (TryGenerateLake(x, y, size))
					{
						size = rand.Next(400, 900);
						failAttemptCount = 0;
						t++;
					}
					else
					{
						failAttemptCount++;
						if (failAttemptCount >= 20)
						{
							failAttemptCount = 0;
							t++;
						}
					}
				}
				// Commands.HandleCommand(TSPlayer.Server, "/settle");
			}
			for (int k = 90; k < Coverage.Height; k++)
			{
				for (int j = 0; j < Coverage.Width; j++)
				{
					if (!Coverage[j, k].active() && Coverage[j, k].wall == WallID.None)
					{
						Coverage[j, k].wall = WallID.SnowWallUnsafe;
					}
				}
			}
		}

		/// <summary>
		/// (x, y)为山脚(左)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="slope">只是用来大致衡量有多陡罢了</param>
		private int MakeMount(int x, int y, float slope, float initialVel = 3.85f)
		{
			int highest = 10000000;
			x += Coverage.X;
			y += Coverage.Y;
			int i = x;
			float j = y;
			float jVelocity = -initialVel;
			while (j <= y)
			{
				j += jVelocity * (0.6f + rand.NextFloat() * 0.8f);
				jVelocity += slope * (rand.NextFloat() * 0.05f + 0.0283f);
				for (int k = (int)Math.Ceiling(j); !Main.tile[i, k].active() && Main.tile[i, k].wall == 0; k++)
				{
					Main.tile[i, k].active(true);
					if (i > x)
					{
						Main.tile[i - 1, k].slope(0);
					}
					Main.tile[i, k].slope(0);
					Main.tile[i, k].type = TileID.SnowBlock;
				}
				if (jVelocity < -0.002)
				{
					Main.tile[i, (int)Math.Ceiling(j)].slope(Tile.Type_SlopeDownRight);
				}
				else if (jVelocity > 0.002f)
				{
					// Main.tile[i, (int)Math.Ceiling(j)].slope(Tile.Type_SlopeUpLeft);
				}
				if (j < highest)
				{
					highest = (int)Math.Ceiling(j);
				}
				i++;
			}
			var m = (i + x) / 2 - Coverage.X;
			var a = rand.Next(x + (i - x) / 3, i - (i - x) / 3) - Coverage.X;
			var b = rand.Next(highest + (y - highest) * 4 / 5, highest + (y - highest) * 6 / 7) - Coverage.Y;
			var dir = Math.Sign(a - m);
			void ClearCircle(Point point, float radius)
			{
				if (point.X - radius < 0 || point.X + radius >= Coverage.Width)
				{
					TSPlayer.All.SendErrorMessage($"point: ({point.X}, {point.Y}) radius: {radius} {Coverage.Width}");
					return;
				}
				for (int i = 0; i <= radius; i++)
				{
					for (int j = 0; j <= radius; j++)
					{
						if (i * i + j * j <= radius * radius)
						{
							Coverage[point.X + i, point.Y + j].active(false);
							Coverage[point.X + i, point.Y + j].wall = WallID.SnowWallUnsafe;
							Coverage[point.X - i, point.Y + j].active(false);
							Coverage[point.X - i, point.Y + j].wall = WallID.SnowWallUnsafe;
							Coverage[point.X + i, point.Y - j].active(false);
							Coverage[point.X + i, point.Y - j].wall = WallID.SnowWallUnsafe;
							Coverage[point.X - i, point.Y - j].active(false);
							Coverage[point.X - i, point.Y - j].wall = WallID.SnowWallUnsafe;
						}
					}
				}
				bool removeWall(int x, int y)
				{
					(int X, int Y) p = (x, y);
					bool u = Coverage[p.X, p.Y - 1].wall == WallID.None && !Coverage[p.X, p.Y - 1].active();
					bool d = Coverage[p.X, p.Y + 1].wall == WallID.None && !Coverage[p.X, p.Y + 1].active();
					return
						!Coverage[p.X, p.Y].active() && (u || d);
				}
				for (int i = (int)Math.Ceiling(radius); i >= 0 ; i--)
				{
					for (int j = (int)Math.Ceiling(radius); j >=0; j--)
					{
						if (i * i + j * j <= radius * radius)
						{
							if (removeWall(point.X + i, point.Y + j))
							{
								Coverage[point.X + i, point.Y + j].wall = WallID.None;
							}
							if (removeWall(point.X - i, point.Y + j))
							{
								Coverage[point.X - i, point.Y + j].wall = WallID.None;
							}
							if (removeWall(point.X + i, point.Y - j))
							{
								Coverage[point.X + i, point.Y - j].wall = WallID.None;
							}
							if (removeWall(point.X - i, point.Y - j))
							{
								Coverage[point.X - i, point.Y - j].wall = WallID.None;
							}
						}
					}
				}
			}
			double EI(Point point)
			{
				int radius = 4;
				if (point.X - radius < 0 || point.X + radius >= Coverage.Width)
				{
					TSPlayer.All.SendErrorMessage($"point: ({point.X}, {point.Y}) radius: {radius} {Coverage.Width}");
					return 1;
				}
				int c = 0;
				for (int i = 0; i < radius; i++)
				{
					for (int j = 0; j < radius; j++)
					{
						if (i * i + j * j <= radius * radius)
						{
							if (!Coverage[point.X + i, point.Y + j].active())
							{
								c++;
							}
							if (!Coverage[point.X - i, point.Y + j].active())
							{
								c++;
							}
							if (!Coverage[point.X + i, point.Y - j].active())
							{
								c++;
							}
							if (!Coverage[point.X - i, point.Y - j].active())
							{
								c++;
							}
						}
					}
				}
				return c / Math.PI * radius * radius;
			}
			float aU = a;
			float bU = b;
			while (EI(new Point((int)(aU + dir * 2 * 5.5f), (int)bU)) < 0.66)
			{
				var current = new Point((int)aU, (int)bU);
				ClearCircle(current, rand.NextFloat() * 1.5f + 4.25f);
				aU += (0.75f + rand.NextFloat() * 1.5f) * dir;
				if (rand.NextDouble() < 0.3)
				{
					bU += 1;
				}
			}
			for (int z = 0; z < 2; z++)
			{
				aU += dir * 5.5f;
				var current = new Point((int)aU, (int)bU);
				ClearCircle(current, rand.NextFloat() * 1.5f + 4.55f);
				aU += (0.75f + rand.NextFloat() * 1.5f) * dir;
				if (rand.NextDouble() < 0.3)
				{
					bU += 1;
				}
			}
			float aD = a;
			float bD = b;
			while (bD < 155)
			{
				var current = new Point((int)aD, (int)bD);
				ClearCircle(current, rand.NextFloat() * 1.5f + 4.25f);
				aD += (0.55f + rand.NextFloat() * 1.5f) * dir;
				bD += rand.NextFloat() * 3f + 3f;
			}
			TileRunner((int)aD, (int)bD, rand.Next(9, 16), rand.Next(200, 800), TileID.SnowBlock, true, speedY: rand.Next(350, 450) / 100f, killTile: true);
			// TileRunner(a, b, rand.Next(15, 20), rand.Next(10, 20), TileID.SnowBlock, true, speedX: rand.Next(150, 200) / 100f, killTile: true);

			return i - x;
		}
		private bool TryGenerateLake(int x, int y,int size)
		{
			bool check(Point p)
			{
				if (p.X < 3 || p.Y < 2 || p.X + 3 >= Coverage.Width || p.Y + 2 >= Coverage.Height)
				{
					return false;
				}
				return
					Coverage[p.X - 1, p.Y + 0].active() &&
					Coverage[p.X - 2, p.Y + 0].active() &&
					Coverage[p.X + 1, p.Y + 0].active() &&
					Coverage[p.X + 2, p.Y + 0].active() &&
					Coverage[p.X + 0, p.Y + 1].active() &&
					Coverage[p.X + 0, p.Y + 2].active();
			}

			var current = new Point(x, y);
			var queue = new Queue<Point>(size);
			var tags = new Dictionary<Point, bool>();
			tags.Add(current, true);
			queue.Enqueue(current);
			while (queue.Count > 0 && tags.Count < size)
			{
				current = queue.Dequeue();
				var pNearby = new List<Point>()
				{
					new Point(current.X, current.Y + 1),
					new Point(current.X - 1, current.Y),
					new Point(current.X + 1, current.Y),
				};
				for (int j = 0; j < 4; j++)
				{
					for (int i = 0; i < 4; i++)
					{
						if (rand.NextDouble() < 0.65 - i * i / 66.0 - j * 0.3)
						{
							pNearby.Add(new Point(current.X - i, current.Y + j));
						}
						if (rand.NextDouble() < 0.65 - i * i / 66.0 - j * 0.3)
						{
							pNearby.Add(new Point(current.X - i, current.Y + j));
						}
						if (rand.NextDouble() < 0.4 - j * 0.09 + size / 1600.0)
						{
							continue;
						}
						break;
					}
				}
				for (int i = 0; i < pNearby.Count; i++)
				{
					if (check(pNearby[i]) && !tags.ContainsKey(pNearby[i]))
					{
						tags.Add(pNearby[i], true);
						queue.Enqueue(pNearby[i]);
					}
				}
			}
			if (tags.Count >= size)
			{
				SurvivalCrisis.DebugMessage($"lake: ({x + Coverage.X}, {y + Coverage.Y}) size: {size}");
				foreach (var p in tags.Keys)
				{
					Coverage[p.X, p.Y].active(false);
					Coverage[p.X, p.Y].liquid = 255;
					Coverage[p.X, p.Y].liquidType(Tile.Liquid_Water);
				}
				foreach (var p in tags.Keys)
				{
					if (!Coverage[p.X, p.Y].active() && Coverage[p.X, p.Y].liquid == 255)
					{
						if (p.Y > 2 && Coverage[p.X, p.Y - 1].active() && !Coverage[p.X, p.Y - 2].active())
						{
							Coverage[p.X, p.Y].active(false);
						}
					}
				}
				return true;
			}
			return false;
		}
		private void TileRunnerWeak(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool killTile = false, bool noYChange = false, bool overRide = true, int ignoreTileType = -1)
		{
			bool mudWall = false;
			int beachDistance = int.MaxValue;

			double num = strength;
			float num2 = steps;
			Vector2 vector = default(Vector2);
			vector.X = i;
			vector.Y = j;
			Vector2 vector2 = default(Vector2);
			vector2.X = (float)rand.Next(-10, 11) * 0.1f;
			vector2.Y = (float)rand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				vector2.X = speedX;
				vector2.Y = speedY;
			}
			bool flag = type == 368;
			bool flag2 = type == 367;
			bool lava = false;
			while (num > 0.0 && num2 > 0f)
			{
				if (vector.Y < 0f && num2 > 0f && type == 59)
				{
					num2 = 0f;
				}
				num = strength * (double)(num2 / (float)steps);
				num2 -= 1f;
				int num3 = (int)((double)vector.X - num * 0.5);
				int num4 = (int)((double)vector.X + num * 0.5);
				int num5 = (int)((double)vector.Y - num * 0.5);
				int num6 = (int)((double)vector.Y + num * 0.5);
				if (num3 < 1)
				{
					num3 = 1;
				}
				if (num4 > Coverage.Width - 1)
				{
					num4 = Coverage.Width - 1;
				}
				if (num5 < 1)
				{
					num5 = 1;
				}
				if (num6 > Coverage.Height - 1)
				{
					num6 = Coverage.Height - 1;
				}
				for (int k = num3; k < num4; k++)
				{
					if (k < beachDistance + 50 || k >= Coverage.Width - beachDistance - 50)
					{
						lava = false;
					}
					for (int l = num5; l < num6; l++)
					{
						if ((false && l < Coverage.Height - 300 && type == 57) || (ignoreTileType >= 0 && Coverage[k, l].active() && Coverage[k, l].type == ignoreTileType) || !((double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.5 * (1.0 + (double)rand.Next(-10, 11) * 0.015)))
						{
							continue;
						}
						if (mudWall && (double)l > Main.worldSurface && Coverage[k, l - 1].wall != 2 && l < Coverage.Height - 210 - rand.Next(3) && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.45 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							if (l > lavaLine - rand.Next(0, 4) - 50)
							{
								if (Coverage[k, l - 1].wall != 64 && Coverage[k, l + 1].wall != 64 && Coverage[k - 1, l].wall != 64 && Coverage[k + 1, l].wall != 64)
								{
									PlaceWall(k, l, 15, mute: true);
								}
							}
							else if (Coverage[k, l - 1].wall != 15 && Coverage[k, l + 1].wall != 15 && Coverage[k - 1, l].wall != 15 && Coverage[k + 1, l].wall != 15)
							{
								PlaceWall(k, l, 64, mute: true);
							}
						}
						if (type < 0)
						{
							if (Coverage[k, l].type == 53)
							{
								continue;
							}
							if (type == -2 && Coverage[k, l].active() && (l < waterLine || l > lavaLine))
							{
								Coverage[k, l].liquid = byte.MaxValue;
								Coverage[k, l].lava(lava);
								if (l > lavaLine)
								{
									Coverage[k, l].lava(lava: true);
								}
							}
							Coverage[k, l].active(active: false);
							continue;
						}
						if (flag && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							PlaceWall(k, l, 180, mute: true);
						}
						if (flag2 && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							PlaceWall(k, l, 178, mute: true);
						}
						if (overRide || !Coverage[k, l].active())
						{
							var tile = Coverage[k, l];
							bool flag3 = false;
							flag3 = Main.tileStone[type] && tile.type != 1;
							if (!TileID.Sets.CanBeClearedDuringGeneration[tile.type])
							{
								flag3 = true;
							}
							switch (tile.type)
							{
								case 53:
									if (type == 59 && /*UndergroundDesertLocation.Contains(k, l)*/false)
									{
										flag3 = true;
									}
									if (type == 40)
									{
										flag3 = true;
									}
									if ((double)l < Main.worldSurface && type != 59)
									{
										flag3 = true;
									}
									break;
								case 45:
								case 147:
								case 189:
								case 190:
								case 196:
								case 460:
									flag3 = true;
									break;
								case 396:
								case 397:
									flag3 = !TileID.Sets.Ore[type];
									break;
								case 1:
									if (type == 59 && (double)l < Main.worldSurface + (double)rand.Next(-50, 50))
									{
										flag3 = true;
									}
									break;
								case 367:
								case 368:
									if (type == 59)
									{
										flag3 = true;
									}
									break;
							}
							if (!flag3)
							{
								tile.type = (ushort)type;
							}
						}
						if (addTile)
						{
							Coverage[k, l].active(active: true);
							Coverage[k, l].liquid = 0;
							Coverage[k, l].lava(lava: false);
							if (killTile)
							{
								Coverage[k, l].active(false);
							}
						}
						if (noYChange && (double)l < Main.worldSurface && type != 59)
						{
							Coverage[k, l].wall = 2;
						}
						if (type == 59 && l > waterLine && Coverage[k, l].liquid > 0)
						{
							Coverage[k, l].lava(lava: false);
							Coverage[k, l].liquid = 0;
						}
					}
				}
				vector += vector2;
				if ((!false || rand.Next(3) != 0) && num > 50.0)
				{
					vector += vector2;
					num2 -= 1f;
					vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
					vector2.X += (float)rand.Next(-10, 11) * 0.05f;
					if (num > 100.0)
					{
						vector += vector2;
						num2 -= 1f;
						vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
						vector2.X += (float)rand.Next(-10, 11) * 0.05f;
						if (num > 150.0)
						{
							vector += vector2;
							num2 -= 1f;
							vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
							vector2.X += (float)rand.Next(-10, 11) * 0.05f;
							if (num > 200.0)
							{
								vector += vector2;
								num2 -= 1f;
								vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
								vector2.X += (float)rand.Next(-10, 11) * 0.05f;
								if (num > 250.0)
								{
									vector += vector2;
									num2 -= 1f;
									vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
									vector2.X += (float)rand.Next(-10, 11) * 0.05f;
									if (num > 300.0)
									{
										vector += vector2;
										num2 -= 1f;
										vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
										vector2.X += (float)rand.Next(-10, 11) * 0.05f;
										if (num > 400.0)
										{
											vector += vector2;
											num2 -= 1f;
											vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
											vector2.X += (float)rand.Next(-10, 11) * 0.05f;
											if (num > 500.0)
											{
												vector += vector2;
												num2 -= 1f;
												vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
												vector2.X += (float)rand.Next(-10, 11) * 0.05f;
												if (num > 600.0)
												{
													vector += vector2;
													num2 -= 1f;
													vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
													vector2.X += (float)rand.Next(-10, 11) * 0.05f;
													if (num > 700.0)
													{
														vector += vector2;
														num2 -= 1f;
														vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
														vector2.X += (float)rand.Next(-10, 11) * 0.05f;
														if (num > 800.0)
														{
															vector += vector2;
															num2 -= 1f;
															vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
															vector2.X += (float)rand.Next(-10, 11) * 0.05f;
															if (num > 900.0)
															{
																vector += vector2;
																num2 -= 1f;
																vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
																vector2.X += (float)rand.Next(-10, 11) * 0.05f;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				vector2.X += (float)rand.Next(-10, 11) * 0.05f;
				if (false)
				{
					vector2.X += (float)rand.Next(-10, 11) * 0.25f;
				}
				if (vector2.X > 1f)
				{
					vector2.X = 1f;
				}
				if (vector2.X < -1f)
				{
					vector2.X = -1f;
				}
				if (!noYChange)
				{
					vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
					if (vector2.Y > 1f)
					{
						vector2.Y = 1f;
					}
					if (vector2.Y < -1f)
					{
						vector2.Y = -1f;
					}
				}
				else if (type != 59 && num < 3.0)
				{
					if (vector2.Y > 1f)
					{
						vector2.Y = 1f;
					}
					if (vector2.Y < -1f)
					{
						vector2.Y = -1f;
					}
				}
				if (type == 59 && !noYChange)
				{
					if ((double)vector2.Y > 0.5)
					{
						vector2.Y = 0.5f;
					}
					if ((double)vector2.Y < -0.5)
					{
						vector2.Y = -0.5f;
					}
					if ((double)vector.Y < Main.rockLayer + 100.0)
					{
						vector2.Y = 1f;
					}
					if (vector.Y > (float)(Coverage.Height - 300))
					{
						vector2.Y = -1f;
					}
				}
			}
		}
		private void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool killTile = false, bool noYChange = false, bool overRide = true, int ignoreTileType = -1)
		{
			bool mudWall = false;
			int beachDistance = int.MaxValue;

			double num = strength;
			float num2 = steps;
			Vector2 vector = default(Vector2);
			vector.X = i;
			vector.Y = j;
			Vector2 vector2 = default(Vector2);
			vector2.X = (float)rand.Next(-10, 11) * 0.1f;
			vector2.Y = (float)rand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				vector2.X = speedX;
				vector2.Y = speedY;
			}
			bool flag = type == 368;
			bool flag2 = type == 367;
			bool lava = false;
			while (num > 0.0 && num2 > 0f)
			{
				if (vector.Y < 0f && num2 > 0f && type == 59)
				{
					num2 = 0f;
				}
				num = strength;// * (double)(num2 / (float)steps);
				num2 -= 1f;
				int num3 = (int)((double)vector.X - num * 0.5);
				int num4 = (int)((double)vector.X + num * 0.5);
				int num5 = (int)((double)vector.Y - num * 0.5);
				int num6 = (int)((double)vector.Y + num * 0.5);
				if (num3 < 1)
				{
					num3 = 1;
				}
				if (num4 > Coverage.Width - 1)
				{
					num4 = Coverage.Width - 1;
				}
				if (num5 < 1)
				{
					num5 = 1;
				}
				if (num6 > Coverage.Height - 1)
				{
					num6 = Coverage.Height - 1;
				}
				for (int k = num3; k < num4; k++)
				{
					if (k < beachDistance + 50 || k >= Coverage.Width - beachDistance - 50)
					{
						lava = false;
					}
					for (int l = num5; l < num6; l++)
					{
						if ((false && l < Coverage.Height - 300 && type == 57) || (ignoreTileType >= 0 && Coverage[k, l].active() && Coverage[k, l].type == ignoreTileType) || !((double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.5 * (1.0 + (double)rand.Next(-10, 11) * 0.015)))
						{
							continue;
						}
						if (mudWall && (double)l > Main.worldSurface && Coverage[k, l - 1].wall != 2 && l < Coverage.Height - 210 - rand.Next(3) && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.45 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							if (l > lavaLine - rand.Next(0, 4) - 50)
							{
								if (Coverage[k, l - 1].wall != 64 && Coverage[k, l + 1].wall != 64 && Coverage[k - 1, l].wall != 64 && Coverage[k + 1, l].wall != 64)
								{
									PlaceWall(k, l, 15, mute: true);
								}
							}
							else if (Coverage[k, l - 1].wall != 15 && Coverage[k, l + 1].wall != 15 && Coverage[k - 1, l].wall != 15 && Coverage[k + 1, l].wall != 15)
							{
								PlaceWall(k, l, 64, mute: true);
							}
						}
						if (type < 0)
						{
							if (Coverage[k, l].type == 53)
							{
								continue;
							}
							if (type == -2 && Coverage[k, l].active() && (l < waterLine || l > lavaLine))
							{
								Coverage[k, l].liquid = byte.MaxValue;
								Coverage[k, l].lava(lava);
								if (l > lavaLine)
								{
									Coverage[k, l].lava(lava: true);
								}
							}
							Coverage[k, l].active(active: false);
							continue;
						}
						if (flag && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							PlaceWall(k, l, 180, mute: true);
						}
						if (flag2 && (double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < strength * 0.3 * (1.0 + (double)rand.Next(-10, 11) * 0.01))
						{
							PlaceWall(k, l, 178, mute: true);
						}
						if (overRide || !Coverage[k, l].active())
						{
							var tile = Coverage[k, l];
							bool flag3 = false;
							flag3 = Main.tileStone[type] && tile.type != 1;
							if (!TileID.Sets.CanBeClearedDuringGeneration[tile.type])
							{
								flag3 = true;
							}
							switch (tile.type)
							{
								case 53:
									if (type == 59 && /*UndergroundDesertLocation.Contains(k, l)*/false)
									{
										flag3 = true;
									}
									if (type == 40)
									{
										flag3 = true;
									}
									if ((double)l < Main.worldSurface && type != 59)
									{
										flag3 = true;
									}
									break;
								case 45:
								case 147:
								case 189:
								case 190:
								case 196:
								case 460:
									flag3 = true;
									break;
								case 396:
								case 397:
									flag3 = !TileID.Sets.Ore[type];
									break;
								case 1:
									if (type == 59 && (double)l < Main.worldSurface + (double)rand.Next(-50, 50))
									{
										flag3 = true;
									}
									break;
								case 367:
								case 368:
									if (type == 59)
									{
										flag3 = true;
									}
									break;
							}
							if (!flag3)
							{
								tile.type = (ushort)type;
							}
						}
						if (addTile)
						{
							Coverage[k, l].active(active: true);
							Coverage[k, l].liquid = 0;
							Coverage[k, l].lava(lava: false);
							if (killTile)
							{
								Coverage[k, l].active(false);
							}
						}
						if (noYChange && (double)l < Main.worldSurface && type != 59)
						{
							Coverage[k, l].wall = 2;
						}
						if (type == 59 && l > waterLine && Coverage[k, l].liquid > 0)
						{
							Coverage[k, l].lava(lava: false);
							Coverage[k, l].liquid = 0;
						}
					}
				}
				vector += vector2;
				if ((!false || rand.Next(3) != 0) && num > 50.0)
				{
					vector += vector2;
					num2 -= 1f;
					vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
					vector2.X += (float)rand.Next(-10, 11) * 0.05f;
					if (num > 100.0)
					{
						vector += vector2;
						num2 -= 1f;
						vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
						vector2.X += (float)rand.Next(-10, 11) * 0.05f;
						if (num > 150.0)
						{
							vector += vector2;
							num2 -= 1f;
							vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
							vector2.X += (float)rand.Next(-10, 11) * 0.05f;
							if (num > 200.0)
							{
								vector += vector2;
								num2 -= 1f;
								vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
								vector2.X += (float)rand.Next(-10, 11) * 0.05f;
								if (num > 250.0)
								{
									vector += vector2;
									num2 -= 1f;
									vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
									vector2.X += (float)rand.Next(-10, 11) * 0.05f;
									if (num > 300.0)
									{
										vector += vector2;
										num2 -= 1f;
										vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
										vector2.X += (float)rand.Next(-10, 11) * 0.05f;
										if (num > 400.0)
										{
											vector += vector2;
											num2 -= 1f;
											vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
											vector2.X += (float)rand.Next(-10, 11) * 0.05f;
											if (num > 500.0)
											{
												vector += vector2;
												num2 -= 1f;
												vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
												vector2.X += (float)rand.Next(-10, 11) * 0.05f;
												if (num > 600.0)
												{
													vector += vector2;
													num2 -= 1f;
													vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
													vector2.X += (float)rand.Next(-10, 11) * 0.05f;
													if (num > 700.0)
													{
														vector += vector2;
														num2 -= 1f;
														vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
														vector2.X += (float)rand.Next(-10, 11) * 0.05f;
														if (num > 800.0)
														{
															vector += vector2;
															num2 -= 1f;
															vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
															vector2.X += (float)rand.Next(-10, 11) * 0.05f;
															if (num > 900.0)
															{
																vector += vector2;
																num2 -= 1f;
																vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
																vector2.X += (float)rand.Next(-10, 11) * 0.05f;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				vector2.X += (float)rand.Next(-10, 11) * 0.05f;
				if (false)
				{
					vector2.X += (float)rand.Next(-10, 11) * 0.25f;
				}
				if (vector2.X > 1f)
				{
					vector2.X = 1f;
				}
				if (vector2.X < -1f)
				{
					vector2.X = -1f;
				}
				if (!noYChange)
				{
					vector2.Y += (float)rand.Next(-10, 11) * 0.05f;
					if (vector2.Y > 1f)
					{
						vector2.Y = 1f;
					}
					if (vector2.Y < -1f)
					{
						vector2.Y = -1f;
					}
				}
				else if (type != 59 && num < 3.0)
				{
					if (vector2.Y > 1f)
					{
						vector2.Y = 1f;
					}
					if (vector2.Y < -1f)
					{
						vector2.Y = -1f;
					}
				}
				if (type == 59 && !noYChange)
				{
					if ((double)vector2.Y > 0.5)
					{
						vector2.Y = 0.5f;
					}
					if ((double)vector2.Y < -0.5)
					{
						vector2.Y = -0.5f;
					}
					if ((double)vector.Y < Main.rockLayer + 100.0)
					{
						vector2.Y = 1f;
					}
					if (vector.Y > (float)(Coverage.Height - 300))
					{
						vector2.Y = -1f;
					}
				}
			}
		}
	
		private void PlaceWall(int i, int j, ushort type, bool mute)
		{
			Coverage.PlaceWallAt(new Point(i, j), type);
		}
	}
}
