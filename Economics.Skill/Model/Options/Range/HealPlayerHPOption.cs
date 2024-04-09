using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class HealPlayerHPOption : BaseRangeOption
{
    [JsonProperty("血量")]
    public int HP { get; set; }

    [JsonProperty("魔力")]
    public int MP { get; set; }
}
