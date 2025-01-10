using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalCrisis
{
	public class DataDetail
	{
		public int DamageCaused
		{
			get;
			set;
		}
		public int KillingCount
		{
			get;
			set;
		}
		public int KilledCount
		{
			get;
			set;
		}
		public int GameCounts
		{
			get;
			set;
		}
		public int WinCounts
		{
			get;
			set;
		}
		/// <summary>
		/// 最长生存帧数(1/60s)
		/// </summary>
		public int MaxSurvivalFrames
		{
			get;
			set;
		}
		public int TotalScore
		{
			get;
			set;
		}
	}
}
