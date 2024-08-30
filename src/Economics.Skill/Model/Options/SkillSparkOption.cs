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

    public bool MeetHP(TSPlayer Player)
    {
        return (HpRatio ? (((float)Player.TPlayer.statLife / (float)Player.TPlayer.statLifeMax) * 100 <= HP) : Player.TPlayer.statLife <= HP) && !MoreHP;
    }

    public bool MeetMP(TSPlayer Player)
    {
        return (MpRatio ? (((float)Player.TPlayer.statMana / (float)Player.TPlayer.statManaMax) * 100 <= MP) : Player.TPlayer.statMana <= MP) && !MoreMP;
    }

    public bool HasItem(TSPlayer player)
    {
        foreach (var item in TermItem)
        {
            if (item.Inventory)
            {
                var inv = player.TPlayer.inventory.Where(x => x.netID == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                    return false;
            }
            if (item.Armory)
            {
                var inv = player.TPlayer.armor.Where(x => x.netID == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                    return false;
            }
            if (item.HeldItem)
            {
                if (player.SelectedItem.netID != item.netID)
                    return false;
            }
        }
        ConsumeItem(player);
        return true;
    }

    private void ConsumeItem(TSPlayer player)
    {
        foreach (var term in TermItem)
        {
            var stack = term.Stack;
            for (int j = 0; j < player.TPlayer.inventory.Length; j++)
            {
                var item = player.TPlayer.inventory[j];
                if (item.netID == term.netID && term.Consume)
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
