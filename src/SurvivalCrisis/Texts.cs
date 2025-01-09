using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalCrisis
{
	public static class Texts
	{
		public readonly static string SomeoneKilled = "有玩家被杀害了...";
		public readonly static string YouAreTraitor = "你的身份是: 背叛者";
		public readonly static string YouAreSurvivor = "你的身份是: 生存者";
		public readonly static string NotEnoughPlayers = "在线玩家数过少(至少4人)";
		public readonly static string GeneratingMap = "开始生成地图...";
		public readonly static string GeneratedMap = "地图生成完成";
		public readonly static string NotCompleteMap = "地图仍在生成, 请再等10s";
		public readonly static string SurvivorsWin = "胜利方是: 生存者";
		public readonly static string TraitorsWin = "胜利方是: 反叛者";
		public readonly static string NobodyWin = "没有胜利者";
		public readonly static string PvpOn = "PvP已开启，请当心你们中的背叛者";
		public readonly static string AvalancheComing = "雪崩即将来袭, 请尽快返回地面";
		public readonly static string AvalancheCome = "雪崩来袭了!";
		public readonly static string GlobalChatIsAvaiable = "全图广播已开启，现在可以任意交流了(需手持天气预报)";
		public readonly static string GlobalChatIsInterfered = "遭到不明信号干扰, 通讯系统与跃迁系统暂时瘫痪";
		public readonly static string InterferenceEnded = "信号干扰消失, 通讯系统与跃迁系统已恢复";
		public readonly static string TaskCompleted = "任务完成";
		public readonly static string TipsWhenStart = $@"{SurvivalCrisis.PeaceTime / 60}s后将开启pvp, 在此之前玩家可自行组队(一队至多2人)，决战开始后自动取消组队
决战前死于非玩家因素可复活
可通过猪猪来提交任务所需物品，将物品放入猪猪即可
敲击迷宫区内的神圣晶塔可以随机传送到另一个神圣晶塔处，利用这个快速通过迷宫吧
迷幻洞穴内的美杜莎有概率掉落[i:3781]，装备后攻击其他玩家时将自动消耗并向装备者揭示其身份
背叛者装备[i:4409]可看到其他玩家位置并自动进入背叛者频道
背叛者装备[i:854]后可通过猪猪购买背叛者商店物品
背叛者可通过使用[i:361]来干扰通讯和跃迁系统(即这段时间内死亡便直接出局)";

		public static class SpecialEvents
		{
			public readonly static string BunnyTime = "兔兔! 好多兔兔!";
			public readonly static Color BunnyTimeColor = new Color(255,255,255);

			public readonly static string NightMare = "你感到有点心神不宁...";
			public readonly static Color NightMareColor = new Color(221, 160, 221);

			public readonly static string LongLongDark = "你的眼前一片漆黑...";
			public readonly static Color LongLongDarkColor = new Color(128, 0, 128);

			public readonly static string Erosion = "你感觉周围的一切都在崩解...";
			public readonly static Color ErosionColor = new Color(148, 0, 211);

			public readonly static string PainOfRelief = "你的体力完全恢复了! 但代价是什么?";
			public readonly static Color PainOfReliefColor = new Color(152, 251, 152);

			public readonly static string BunnyRevenge = "兔兔! 好多兔兔......?";
			public readonly static Color BunnyRevengeColor = new Color(255, 228, 225);

			public readonly static string MagneticStorm = "侦测到强电磁干扰, 定位系统恢复中——";
			public readonly static Color MagneticStormColor = new Color(225, 255, 255);

			public readonly static string FatalBlizzard = "寒冬来临...尽快找点篝火取暖吧";
			public readonly static Color FatalBlizzardColor = new Color(70, 130, 180);

			public readonly static string BlessOfNature = "自然之力祝福着你!";
			public readonly static Color BlessOfNatureColor = new Color(60, 179, 113);

			public readonly static string Farce = "这将会是一场可悲的闹剧...";
			public readonly static Color FarceColor = new Color(188, 188, 255);
		}
	}
}
