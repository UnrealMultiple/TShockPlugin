using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionView
{
    public class RegionPlayer
    {
		public int Index { get; }

		public TSPlayer TSPlayer 
		{ 
			get
				=> TShock.Players[Index];
		}

		public List<Region> Regions { get; } = new();

		public bool IsViewingNearby { get; set; }

		public RegionPlayer(int index) 
			=> Index = index;
	}
}
