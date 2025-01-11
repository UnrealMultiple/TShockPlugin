using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.MapGenerating
{
	public abstract class Generator
	{
		public TileSection Coverage
		{
			get;
		}
		protected Generator(TileSection coverage)
		{
			Coverage = coverage;
		}

		public abstract void Generate();
	}
}
