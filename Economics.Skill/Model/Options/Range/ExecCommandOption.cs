using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options.Range;

public class ExecCommandOption : BaseRangeOption
{
    [JsonProperty("命令")]
    public List<string> Commands { get; set; } = new();
}
