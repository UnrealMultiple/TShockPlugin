using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class FinalEvent : SpecialEvent
	{
		private int BossComingTime;
		private SurvivalCrisis Game;
		private int bossLife;
		private int bossLifeMax;
		private bool enableVote;
		public FinalEvent()
		{
			
		}
		public override void Reset()
		{
			base.Reset();
			Game = SurvivalCrisis.Instance;
			BossComingTime = SurvivalCrisis.BossComingTime;
			StartDelay = 0;
			TimeLeft = SurvivalCrisis.BossComingTime + SurvivalCrisis.FPTime + SurvivalCrisis.NightTime;
		}
		public override void Start()
		{
			base.Start();
		}
		public override void Update()
		{
			if (Game.GameTime > BossComingTime + SurvivalCrisis.FPTime)
			{
				if (Game.FinalBoss.active && Game.FinalBoss.type == Game.FinalBossType)
				{
					bossLife = Game.FinalBoss.life;
					bossLifeMax = Game.FinalBoss.lifeMax;
				}
				else
				{
					var player = Game.Participants.First(p => p.TSPlayer != null && p.Identity != PlayerIdentity.Watcher);
					foreach(var npc in Main.npc)
					{
						if (npc.active)
						{
							if (npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeVice|| npc.type == NPCID.PrimeLaser|| npc.type == NPCID.PrimeCannon)
							{
								npc.life = 0;
								npc.checkDead();
							}
						}
					}
					SurvivalCrisis.GameOperations.SpawnFinalBoss();
					Game.FinalBoss.life = bossLife;
					Game.FinalBoss.lifeMax = bossLifeMax;
				}
			}
			if (Game.GameTime % 60 == 0)
			{
				string msg;
				if (Game.GameTime <= BossComingTime + SurvivalCrisis.FPTime)
				{
					msg = $"距离决战还有{(BossComingTime + SurvivalCrisis.FPTime - Game.GameTime) / 60}s";
				}
				else
				{
					msg = $"战斗还剩{(SurvivalCrisis.TotalTime + SurvivalCrisis.FPTime - Game.GameTime) / 60}s";
					if (Game.FinalBoss.active && Game.FinalBoss.target >= 0)
					{
						var target = Game.Players[Game.FinalBoss.target];
						if (target?.Identity == PlayerIdentity.Watcher)
						{
							Game.FinalBoss.target = -1;
						}
					}
					foreach (var npc in Main.npc)
					{
						if (npc.active)
						{
							if (npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeVice)
							{
								npc.life = 0;
								npc.checkDead();
							}
						}
					}
				}
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Game.Players[i]?.TSPlayer != null)
					{
						Game.Players[i].AddStatusMessage(msg);
					}
				}
			}
			if (Game.GameTime + SurvivalCrisis.FBPTime == BossComingTime)
			{
				SurvivalCrisis.GameOperations.TeleportPlayers(SurvivalCrisis.Regions.Hall.Center);
				SurvivalCrisis.GameOperations.TogglePvp(false);

				var traitorCount = Game.Participants.Count(p => p?.Identity == PlayerIdentity.Traitor);
				var survivorCount = Game.Participants.Count(p => p?.Identity == PlayerIdentity.Survivor);
				if (traitorCount < (traitorCount + survivorCount) / 2)
				{
					foreach (var player in Game.Participants)
					{
						if (player.IsValid() && player.Identity != PlayerIdentity.Watcher)
						{
							player.HideName();
							player.Team = 1;
							player.CanVote = true;
							player.Piggybank[player.Piggybank.Count / 2].SetDefaults(ItemID.WormholePotion);
							for (int i = 0; i < player.Inventory.Count; i++)
							{
								if (player.Inventory[i].Type == ItemID.WeatherRadio)
								{
									player.Inventory[i].SetDefaults(ItemID.WormholePotion);
									break;
								}
							}
						}
					}
					enableVote = true;
					Game.BCToAll("请在2min内打开大地图点击其他人的头像以投票(票数最多者将出出局)", Color.CornflowerBlue);
					Game.BCToAll("虫洞药水已经添加到你的猪猪中", Color.CornflowerBlue);
				}
			}
			else if (Game.GameTime == BossComingTime - SurvivalCrisis.FBPTime / 3)
			{
				if (enableVote)
				{
					foreach (var player in Game.Participants)
					{
						if (player.IsValid() && player.Identity != PlayerIdentity.Watcher)
						{
							player.CanVote = false;
						}
					}
					var players = Game.Participants.Where(p => p?.TSPlayer != null && p.Identity != PlayerIdentity.Watcher);
					players = players.OrderBy(p => -p.VotedCount);
					foreach (var player in players)
					{
						Game.BCToAll($"{player.Index}号得票数: {player.VotedCount}票", Color.Indigo);
					}
					var votedCount = players.First().VotedCount;
					if (votedCount != 0)
					{
						var ps = players.Where(plr => votedCount == plr.VotedCount);
						if (ps.Count() == 1)
						{
							var p = ps.First();
							p.Identity = PlayerIdentity.Watcher;
							var identity = p.Party switch
							{
								PlayerIdentity.Survivor => "生存者",
								PlayerIdentity.Traitor => "背叛者"
							};
							Game.BCToAll($"{p.Index}号被驱逐", Color.CornflowerBlue);
							Game.BCToAll($"{p.Index}号的身份是: {identity}", Color.CornflowerBlue);
						}
						else if (ps.Count() == 2)
						{
							foreach (var p in ps)
							{
								p.TSPlayer.SetBuff(BuffID.WitheredArmor, 360000);
								p.TSPlayer.SetBuff(BuffID.WitheredWeapon, 360000);
							}
							Game.BCToAll($"{ps.ElementAt(0)}号、{ps.ElementAt(1)}号平票, 同时接受惩罚: 枯萎", Color.CornflowerBlue);
						}
						else
						{
							Game.BCToAll("平票人数过多, 投票无效", Color.CornflowerBlue);
						}
					}
				}
			}
			else if (Game.GameTime == BossComingTime + SurvivalCrisis.FPTime)
			{
				TShockAPI.TSPlayer.Server.SetTime(false, 5);
				SurvivalCrisis.GameOperations.SpawnFinalBoss();
				SurvivalCrisis.GameOperations.TogglePvp(true);
				bossLife = Game.FinalBoss.life;
				bossLifeMax = Game.FinalBoss.lifeMax;
			}
		}
		public override void End(in bool gameEnd)
		{
			if (!gameEnd)
			{
				SurvivalCrisis.GameOperations.GameEnd(PlayerIdentity.Traitor);
			}
			base.End(gameEnd);
		}
	}
}
