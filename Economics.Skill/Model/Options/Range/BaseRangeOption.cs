using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class BaseRangeOption
{
    [JsonProperty("范围")]
    public int Range { get; set; }
}
