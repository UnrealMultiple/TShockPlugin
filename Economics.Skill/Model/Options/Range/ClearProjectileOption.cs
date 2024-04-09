using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class ClearProjectileOption : BaseRangeOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }
}
