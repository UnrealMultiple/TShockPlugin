using Economics.Core.Attributes;
using Economics.Core.Command;
using Economics.Core.ConfigFiles;
using Economics.Core.EventArgs.PlayerEventArgs;
using Economics.Core.Extensions;
using Microsoft.Xna.Framework;
using Rests;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TerrariaApi.Server;
using TShockAPI;

namespace Economics.Core.Utils;

public partial class Helper
{
    private static readonly Regex Regex = ChatRegex();

    /// <summary>
    /// 生成渐变色消息
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetGradientText(string text)
    {
        var result = "";
        //匹配物品消息
        var matchs = Regex.Matches(text);
        var chat = matchs.Select(x => x.Groups).ToDictionary(x => x[1].Index, x => x);
        var info = Terraria.UI.Chat.ChatManager.ParseMessage(text, Color.White);
        var colors = Setting.Instance.GradientColor;
        var fullIndex = 1;
        var index = 0;
        foreach (var item in info)
        {
            for (var i = 0; i < item.Text.Length; i++)
            {
                fullIndex++;
                if (chat.TryGetValue(fullIndex - 1, out var group) && group != null)
                {
                    result += item.TextOriginal;
                    fullIndex += item.Text.Length + 1;
                    break;
                }
                else
                if (index >= colors.Count)
                {
                    result += item.Text[i];
                    index = 0;
                }
                else
                {
                    result += Setting.Instance.GradientColor[index].SFormat(item.Text[i]);
                }

                index++;
            }
        }
        return result;
    }

    public static void InitCommand()
    {
        foreach (var plugin in ServerApi.Plugins)
        {
            var types = plugin.Plugin.GetType().Assembly.GetExportedTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(BaseCommand)))
                {
                    var instance = Activator.CreateInstance(type);
                    if (instance is BaseCommand cmd)
                    {
                        Commands.ChatCommands.Add(new(cmd.Permissions, cmd.Invoke, cmd.Alias));
                    }
                }
            }
        }
    }


    public static void CountertopUpdate(PlayerCountertopArgs args)
    {
        StringBuilder sb = new();
        string down = new('\n', Setting.Instance.StatusTextShiftDown);
        string Left = new(' ', Setting.Instance.StatusTextShiftLeft);
        sb.AppendLine(down);
        args.Messages.OrderBy(x => x.Order).ForEach(x => sb.AppendLine(GetGradientText(x.Message) + Left));
        args.Player?.SendData(PacketTypes.Status, sb.ToString(), 0, 1);
    }

    [GeneratedRegex(@"\[(?<type>[^\]]+):(?<id>\d+)\]")]
    public static partial Regex ChatRegex();
}