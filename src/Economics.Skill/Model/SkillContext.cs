using Economics.Skill.JSInterpreter;
using Economics.Skill.Model.Options;
using Economics.Skill.Model.Options.Projectile;
using Economics.Skill.Model.Options.Range;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Skill.Model;

public class SkillContext
{
    [JsonProperty("名称")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("喊话")]
    public string Broadcast { get; set; } = string.Empty;

    [JsonProperty("技能唯一")]
    public bool SkillUnique { get; set; }

    [JsonProperty("全服唯一")]
    public bool SkillUniqueAll { get; set; }

    [JsonProperty("技能价格")]
    public long Cost { get; set; }

    [JsonProperty("限制等级")]
    public List<string> LimitLevel { get; set; } = new();

    [JsonProperty("限制进度")]
    public List<string> LimitProgress { get; set; } = new();

    [JsonProperty("触发设置")]
    public SkillSparkOption SkillSpark { get; set; } = new();

    [JsonProperty("伤害敌怪")]
    public StrikeNpcOption StrikeNpc { get; set; } = new();

    [JsonProperty("敌怪BUFF")]
    public NpcBuffOption NpcBuff { get; set; } = new();

    [JsonProperty("范围命令")]
    public ExecCommandOption ExecCommand { get; set; } = new();

    [JsonProperty("治愈")]
    public HealPlayerHPOption HealPlayerHPOption { get; set; } = new();

    [JsonProperty("清理弹幕")]
    public ClearProjectileOption ClearProjectile { get; set; } = new();

    [JsonProperty("拉怪")]
    public PullNpcOption PullNpc { get; set; } = new();

    [JsonProperty("传送")]
    public PlayerTpOption PlayerTp { get; set; } = new();

    [JsonProperty("无敌")]
    public PlayerGodOption PlayerGod { get; set; } = new();

    [JsonProperty("范围Buff")]
    public BuffOption BuffOption { get; set; } = new();

    [JsonProperty("执行脚本")]
    public string? ExecuteScript
    {
        get { return JsScript?.FilePathOrUri; }
        set
        {
            JsScript = Set(value);
        }
    }

    [JsonProperty("弹幕")]
    public List<ProjectileOption> Projectiles { get; set; } = new();

    [JsonIgnore]
    public JsScript? JsScript { get; set; }

    public JsScript? Set(string? path)
    {
        var jistScript = new JsScript
        {
            FilePathOrUri = path
        };
        try
        {
            jistScript.Script = File.ReadAllText(Path.Combine(Interpreter.ScriptsDir, jistScript.FilePathOrUri));
        }
        catch (Exception ex)
        {
            TShock.Log.Error("无法加载{0}: {1}", path, ex.Message);
            return null;
        }
        ScriptContainer.PreprocessRequires(jistScript);
        return jistScript;
    }

    public void AddNpcBuff(TSPlayer Player)
    {
        if (NpcBuff.Enable)
            foreach (var npc in Player.TPlayer.position.FindRangeNPCs(NpcBuff.Range))
                foreach (var buff in NpcBuff.BuffOptions.Buffs)
                    npc.AddBuff(buff.BuffId, buff.Time);
    }

    public void AddPlayerBuff(TSPlayer Player)
    {
        foreach (var ply in Player.GetPlayerInRange(BuffOption.Range))
            foreach (var buff in BuffOption.Buffs)
                ply.SetBuff(buff.BuffId, buff.Time);
    }

    /// <summary>
    /// 通用技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public void EmitGeneralSkill(TSPlayer Player)
    {
        if (!string.IsNullOrEmpty(Broadcast))
            TShock.Utils.Broadcast(Broadcast, Color.Wheat);
        Player.StrikeNpc(StrikeNpc.Damage, StrikeNpc.Range * 16, Skill.Config.BanStrikeNpcs);
        Player.ExecRangeCommands(ExecCommand.Range * 16, ExecCommand.Commands);
        Player.HealAllLife(HealPlayerHPOption.Range * 16, HealPlayerHPOption.HP);
        Player.HealAllMana(HealPlayerHPOption.Range * 16, HealPlayerHPOption.MP);
        Player.ClearProj(ClearProjectile.Range * 16);
        Player.CollectNPC(PullNpc.Range, Skill.Config.BanPullNpcs, PullNpc.X * 16, PullNpc.Y * 16);
        if (PlayerTp.Enable)
            Player.Teleport(Player.X + PlayerTp.X * 16 * (PlayerTp.Incline ? Player.TPlayer.direction : 1), Player.Y + PlayerTp.Y * 16);
        if (PlayerGod.Enable)
            SkillCD.GodPlayer(Player, PlayerGod.Time);
        AddNpcBuff(Player);
        AddPlayerBuff(Player);
    }

}
