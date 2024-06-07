
using EconomicsAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics.Skill.Model;

public class TermItem : Item
{
    [JsonProperty("背包物品")]
    public bool Inventory { get; set; } = false;

    [JsonProperty("装备饰品")]
    public bool Armory { get; set; } = false;

    [JsonProperty("手持物品")]
    public bool HeldItem { get; set; } = false;
}
