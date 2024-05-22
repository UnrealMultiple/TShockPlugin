using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model.Options;

public class Buff
{
    [JsonProperty("BuffID")]
    public int BuffId { get; set; }

    [JsonProperty("时长")]
    public int Time { get; set; }
}
