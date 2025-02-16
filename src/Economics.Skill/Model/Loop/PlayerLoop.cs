using Economics.Skill.Internal;
using Economics.Skill.Model.Options;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Skill.Model.Loop;
public class PlayerLoop : BaseLoop
{

    [JsonProperty("传送")]
    public PlayerTpOption PlayerTpOption { get; set; } = new();

    [JsonProperty("无敌")]
    public PlayerGodOption PlayerGodOption { get; set; } = new();

    public void Spark(TSPlayer Player)
    {
        if (this.PlayerTpOption.Enable)
        {
            Player.Teleport(Player.X + (this.PlayerTpOption.X * 16 * (this.PlayerTpOption.Incline ? Player.TPlayer.direction : 1)), Player.Y + (this.PlayerTpOption.Y * 16));
        }

        if (this.PlayerGodOption.Enable)
        {
            SkillCD.GodPlayer(Player, this.PlayerGodOption.Time);
        }
    }
}
