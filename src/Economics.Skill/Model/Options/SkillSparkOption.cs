using Economics.Skill.Converter;
using Economics.Skill.Enumerates;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TShockAPI;

namespace Economics.Skill.Model.Options;

public class SkillSparkOption
{
    [JsonProperty("触发模式")]
    [JsonConverter(typeof(SkillSparkConverter))]
    public List<SkillSparkType> SparkMethod { get; set; } = new();

    [JsonProperty("冷却")]
    public int CD { get; set; }

    [JsonProperty("血量")]
    public int HP { get; set; }

    [JsonProperty("血量比例计算")]
    public bool HpRatio { get; set; }

    [JsonProperty("大于血量")]
    public bool MoreHP { get; set; }

    [JsonProperty("蓝量")]
    public int MP { get; set; }

    [JsonProperty("蓝量比例计算")]
    public bool MpRatio { get; set; }

    [JsonProperty("大于蓝量")]
    public bool MoreMP { get; set; }

    [JsonProperty("物品条件")]
    public List<TermItem> TermItem { get; set; } = new();

    [JsonProperty("Buff条件")]
    public List<int> BuffsCondition { get; set; } = new();

    [JsonProperty("环境条件")]
    public List<string> EnvironmentCondition { get; set; } = new();

    [JsonProperty("技能条件")]
    public List<int> SkillCondition { get; set; } = new();

    public bool MeetHP(TSPlayer Player)
    {
        return (this.HpRatio ? (Player.TPlayer.statLife / (float) Player.TPlayer.statLifeMax * 100 <= this.HP) : Player.TPlayer.statLife <= this.HP) && !this.MoreHP;
    }

    public bool MeetMP(TSPlayer Player)
    {
        return (this.MpRatio ? (Player.TPlayer.statMana / (float) Player.TPlayer.statManaMax * 100 <= this.MP) : Player.TPlayer.statMana <= this.MP) && !this.MoreMP;
    }

    public bool HasItem(TSPlayer player)
    {
        foreach (var item in this.TermItem)
        {
            if (item.Inventory)
            {
                var inv = player.TPlayer.inventory.Where(x => x.type == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                {
                    return false;
                }
            }
            if (item.Armory)
            {
                var inv = player.TPlayer.armor.Take(10).Where(x => x.type == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                {
                    return false;
                }
            }
            if (item.Misc)
            {
                var inv = player.TPlayer.armor.Skip(10).Where(x => x.type == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                {
                    return false;
                }
            }
            if (item.HeldItem)
            {
                if (player.SelectedItem.type != item.netID)
                {
                    return false;
                }
            }
        }
        this.ConsumeItem(player);
        return true;
    }

    private void ConsumeItem(TSPlayer player)
    {
        foreach (var term in this.TermItem)
        {
            var stack = term.Stack;
            for (var j = 0; j < player.TPlayer.inventory.Length; j++)
            {
                var item = player.TPlayer.inventory[j];
                if (item.type == term.netID && term.Consume)
                {
                    if (item.stack >= stack)
                    {
                        item.stack -= stack;
                        TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", player.Index, j);
                    }
                    else
                    {
                        stack -= item.stack;
                    }
                }
            }
        }
    }
}