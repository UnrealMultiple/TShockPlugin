using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;

namespace WorldModify
{
    public class MoonHelper
    {
        private static Dictionary<string, int> _moonPhases = new Dictionary<string, int>
    {
        { "满月", 1 },
        { "亏凸月", 2 },
        { "下弦月", 3 },
        { "残月", 4 },
        { "新月", 5 },
        { "娥眉月", 6 },
        { "上弦月", 7 },
        { "盈凸月", 8 }
    };

        private static Dictionary<string, int> _moonTypes = new Dictionary<string, int>
    {
        { "正常", 1 },
        { "火星样式", 2 },
        { "土星样式", 3 },
        { "秘银风格", 4 },
        { "明亮的偏蓝白色", 5 },
        { "绿色", 6 },
        { "糖果", 7 },
        { "金星样式", 8 },
        { "紫色的三重月亮", 9 }
    };

        public static string MoonPhaseDesc => _moonPhases.Keys.ElementAt(Main.moonPhase);

        public static string MoonTypeDesc => _moonTypes.Keys.ElementAt(Main.moonType);

        public static void ChangeMoonPhase(CommandArgs args)
        {
            if (args.Parameters.Count() == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                args.Player.SendInfoMessage("当前月相: {0}", _moonPhases.Keys.ElementAt(Main.moonPhase));
                args.Player.SendInfoMessage("用法：/moon <月相>");
                args.Player.SendInfoMessage("月相：{0} （可用数字 1~8 代替）", string.Join(", ", _moonPhases.Keys));
                return;
            }
            if (int.TryParse(args.Parameters[0], out var result))
            {
                if (result < 1 || result > 8)
                {
                    args.Player.SendErrorMessage("语法错误！用法：/moon <月相>");
                    args.Player.SendErrorMessage("月相：{0} （可用数字 1~8 代替）", string.Join(", ", _moonPhases.Keys));
                    return;
                }
            }
            else
            {
                if (!_moonPhases.ContainsKey(args.Parameters[0]))
                {
                    args.Player.SendErrorMessage("语法错误！用法：/moon <月相>");
                    args.Player.SendErrorMessage("月相：{0} （可用数字 1~8 代替）", string.Join(", ", _moonPhases.Keys));
                    return;
                }
                result = _moonPhases[args.Parameters[0]];
            }
            Main.dayTime = false;
            Main.moonPhase = result - 1;
            Main.time = 0.0;
            TSPlayer.All.SendData((PacketTypes)7);
            args.Player.SendSuccessMessage("月相已改为 {0}", _moonPhases.Keys.ElementAt(result - 1));
        }

        public static void ChangeMoonStyle(CommandArgs args)
        {
            if (args.Parameters.Count() == 0)
            {
                args.Player.SendInfoMessage("当前月亮样式: {0}", _moonTypes.Keys.ElementAt(Main.moonType));
                helpText();
                return;
            }
            if (args.Parameters[0].ToLowerInvariant() == "help")
            {
                helpText();
                return;
            }
            if (int.TryParse(args.Parameters[0], out var result))
            {
                if (result < 1 || result > 9)
                {
                    helpText();
                    return;
                }
            }
            else
            {
                if (!_moonTypes.ContainsKey(args.Parameters[0]))
                {
                    helpText();
                    return;
                }
                result = _moonTypes[args.Parameters[0]];
            }
            Main.dayTime = false;
            Main.moonType = result - 1;
            Main.time = 0.0;
            TSPlayer.All.SendData((PacketTypes)7);
            args.Player.SendSuccessMessage("月亮样式已改为 {0}", _moonTypes.Keys.ElementAt(result - 1));
            void helpText()
            {
                args.Player.SendInfoMessage("用法：/moonstyle <月亮样式>");
                args.Player.SendInfoMessage("月亮样式：{0} （可用数字 1~9 代替）", string.Join(", ", _moonTypes.Keys));
            }
        }

        public static void Clear()
        {
            _moonPhases.Clear();
            _moonTypes.Clear();
        }
    }
}
