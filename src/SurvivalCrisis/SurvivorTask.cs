using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis
{
	using Effects;
	public class SurvivorTask
	{
		public class ItemInfo
		{
			public int ID { get; }
			public double Score { get; }
			public int RequiredCount
			{
				get;
				private set;
			}
			public int CurrentCount
			{
				get;
				private set;
			}

			public int CountStillNeed
			{
				get => RequiredCount - CurrentCount;
			}
			public int[] Contributions { get; }

			public ItemInfo(int id, int count, double score)
			{
				ID = id;
				Score = score;
				RequiredCount = count;
				CurrentCount = 0;
				Contributions = new int[40];
			}


			public void Contribute(GamePlayer player, int count)
			{
				CurrentCount += count;
				Contributions[player.Index] += count;
			}
			public string GetDetailContributions()
			{
				var sb = new StringBuilder(20);
				sb.Append(ToItemIcon());
				sb.Append(": ");
				foreach (var player in SurvivalCrisis.Instance.Participants)
				{
					sb.Append($"{player.Index}号({Contributions[player.Index]})   ");
				}
				return sb.ToString();
			}
			public string ToItemIcon()
			{
				return $"[i/s{RequiredCount}:{ID}]";
			}
			public override string ToString()
			{
				if (CountStillNeed == 0)
				{
					return string.Empty;
				}
				return $"[i/s{CountStillNeed}:{ID}]";
			}
			public string ToString2()
			{
				return $"[i:{ID}]({CurrentCount}/{RequiredCount})";
			}
		}
		/// <summary>
		/// 中二就完事了
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// 完整描述
		/// </summary>
		public string Text { get; }
		/// <summary>
		/// 中二就完事了
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// 描述奖励
		/// </summary>
		public string BonusDescription { get; }
		/// <summary>
		/// 任务所需物品
		/// </summary>
		public ItemInfo[] Items { get; }
		public int ID { get; }

		public SurvivorTask(int id)
		{
			ID = id;
			switch(id)
			{
				case 1:
					{
						Name = "[c/4444df:通讯系统维修]";
						Description = @"你和你的同伴们发现先前用于联络救援的设备受到了不明损害.
好在, 修理起来并不困难, 只需要稍微费点功夫......";
						BonusDescription = "[c/fefe11:开启全服通讯，手持天气收音机时发言将全服可见]";
						Items = new[]
						{
							new ItemInfo(ItemID.FallenStar, 8, 2),
							new ItemInfo(ItemID.LifeCrystal, 8, 4),
							new ItemInfo(ItemID.IronOre, 60, 0.35),
							new ItemInfo(ItemID.GoldOre, 60, 0.4)
						};
						Text = BuildText();
					}
					break;
			}
		}

		private string BuildText()
		{
			var sb = new StringBuilder(30);
			sb.Append("生存者任务 -- ").AppendLine(Name);
			sb.AppendLine(Description);
			sb.Append("  所需物品: ");
			foreach (var item in Items)
			{
				sb.Append(item.ToItemIcon());
			}
			sb.AppendLine();
			sb.AppendLine("奖励: ");
			sb.Append("     ").Append(BonusDescription);
			return sb.ToString();
		}

		public string CurrentProcess()
		{
			var str = string.Join("", (object[])Items);
			return "还需:" + str;
		}

		public void CheckPiggy(GamePlayer player)
		{
			bool anyContribution = false;
			var piggy = player.Piggybank;
			//var sb = new StringBuilder(50 * 6);
			//for (int i = 0; i < piggy.Count; i++)
			//{
			//	if (i % 10 == 0)
			//	{
			//		sb.AppendLine();
			//	}
			//	sb.Append(piggy[i].IsAir ? "□" : $"[i/s{piggy[i].Stack}:{piggy[i].Type}]");
			//}
			//SurvivalCrisis.DebugMessage(sb.ToString());
			for (int i = 0; i < piggy.Count; i++)
			{
				for (int j = 0; j < Items.Length; j++)
				{
					if (Items[j].CountStillNeed == 0)
					{
						continue;
					}
					if (piggy[i].Type == Items[j].ID)
					{
						int c;
						if (piggy[i].Stack > Items[j].CountStillNeed)
						{
							c = Items[j].CountStillNeed;
						}
						else
						{
							c = piggy[i].Stack;
						}
						Items[j].Contribute(player, c);
						piggy[i].Stack -= c;
						SurvivalCrisis.Instance.BCToAll($"{player.Index}号为任务贡献了[i/s{c}:{Items[j].ID}]", Color.Yellow);
						player.AddPerformanceScore(c * Items[j].Score, "提交任务物品");
						anyContribution = true;
					}
				}
			}
			if (anyContribution)
			{
				if (Items.Sum(item => item.CountStillNeed) > 0)
				{
					// SurvivalCrisis.Instance.BCToAll(CurrentProcess(), Color.LawnGreen);
				}
				else
				{
					CompleteTask();
				}
			}
		}

		public void CompleteTask()
		{
			SurvivalCrisis.Instance.BCToAll(Texts.TaskCompleted, Color.LightGoldenrodYellow);
			switch(ID)
			{
				case 1:
					{
						foreach (var player in SurvivalCrisis.Instance.Participants)
						{
							if (player.TSPlayer != null)
							{
								player.AddPerformanceScore(20, "任务完成");
							}
						}
						SurvivalCrisis.GameOperations.AddEffect(new GlobalChat());
						SurvivalCrisis.Instance.BCToAll(Texts.GlobalChatIsAvaiable, Color.ForestGreen);
					}
					break;
			}
			SurvivalCrisis.GameOperations.ToNextTask();
		}
	}
}
