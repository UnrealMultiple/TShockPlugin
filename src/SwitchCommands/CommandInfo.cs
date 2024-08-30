using Newtonsoft.Json;
using TShockAPI;
//using PlaceholderAPI;

namespace SwitchCommands
{
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
            X = 0;
            Y = 0;
        }

        public SwitchPos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "X: {0}, Y: {1}".SFormat(X, Y);
        }

        public override bool Equals(object obj)
        {
            var check = obj as SwitchPos;

            if (check == null) return false;

            return check.X == X && check.Y == Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
