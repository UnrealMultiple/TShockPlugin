using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class StrikeNpcOption : BaseRangeOption
{
    [JsonProperty("伤害")]
    public int Damage { get; set; }
}
