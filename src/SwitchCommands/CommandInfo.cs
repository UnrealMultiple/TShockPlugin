using Newtonsoft.Json;
using TShockAPI;
//using PlaceholderAPI;

namespace SwitchCommands;

public class CommandInfo
{
    [JsonProperty("指令")]
    public List<string> commandList = new List<string>();
    [JsonProperty("冷却时间")]
    public float cooldown = 0;
    [JsonProperty("忽略权限")]
    public bool ignorePerms = false;
    [JsonProperty("开关说明")]
    public string show = "";
}

public class SwitchPos
{
    public int X = 0;
    public int Y = 0;

    public SwitchPos()
    {
        this.X = 0;
        this.Y = 0;
    }

    public SwitchPos(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        return "X: {0}, Y: {1}".SFormat(this.X, this.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is SwitchPos check && check.X == this.X && check.Y == this.Y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}