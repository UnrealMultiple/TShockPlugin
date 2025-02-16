using Economics.Skill.Model.Options;
using EconomicsAPI.Extensions;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Skill.Model.Loop;
public class RegionLoop : BaseLoop
{
    [JsonProperty("范围")]
    public int Range { get; set; }

    [JsonProperty("Buff列表")]
    public List<BuffOption> Buffs { get; set; } = new();

    [JsonProperty("清理弹幕")]
    public bool ClearProj { get; set; }

    [JsonProperty("命令")]
    public List<string> Commands { get; set; } = new();

    [JsonProperty("血量")]
    public int HP { get; set; }

    [JsonProperty("魔力")]
    public int MP { get; set; }

    [JsonProperty("NPC增益")]
    public List<BuffOption> NpcBuffs { get; set; } = new();

    [JsonProperty("拉怪")]
    public bool PullNpc { get; set; } = false;

    [JsonProperty("拉怪X轴调整")]
    public int PullNpcX { get; set; }

    [JsonProperty("拉怪Y轴调整")]
    public int PullNpcY { get; set; }

    [JsonProperty("伤害敌怪")]
    public int DamageNpc { get; set; }

    public void AddNpcBuff(TSPlayer Player)
    {
        if (this.NpcBuffs.Count > 0)
        {
            foreach (var npc in Player.TPlayer.position.FindRangeNPCs(this.Range))
            {
                foreach (var buff in this.NpcBuffs)
                {
                    npc.AddBuff(buff.BuffId, buff.Time);
                }
            }
        }
    }

    public void AddPlayerBuff(TSPlayer Player)
    {
        foreach (var ply in Player.GetPlayerInRange(this.Range))
        {
            foreach (var buff in this.Buffs)
            {
                ply.SetBuff(buff.BuffId, buff.Time);
            }
        }
    }

    /// <summary>
    /// 通用技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public void Spark(TSPlayer Player)
    {
        Player.StrikeNpc(this.DamageNpc, this.Range * 16, Skill.Config.BanStrikeNpcs);
        Player.ExecRangeCommands(this.Range * 16, this.Commands);
        Player.HealAllLife(this.Range * 16, this.HP);
        Player.HealAllMana(this.Range * 16, this.MP);
        Player.ClearProj(this.Range * 16);
        Player.CollectNPC(this.Range, Skill.Config.BanPullNpcs, this.PullNpcX * 16, this.PullNpcY * 16);
        this.AddNpcBuff(Player);
        this.AddPlayerBuff(Player);
    }
}
