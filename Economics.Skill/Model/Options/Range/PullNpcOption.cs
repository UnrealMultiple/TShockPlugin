using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class PullNpcOption : BaseRangeOption
{
    [JsonProperty("X轴调整")]
    public int X { get; set; }

    [JsonProperty("Y轴调整")]
    public int Y { get; set; }
}
