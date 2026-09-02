using Economics.Core.Command;
using Economics.Core.ConfigFiles;
using Terraria;
using Microsoft.Xna.Framework;
using System.Text;
using System.Text.RegularExpressions;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.Localization;
using Terraria.ID;

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


    public static void CountertopUpdate(TSPlayer player)
    {
        StringBuilder sb = new();
        string down = new('\n', Setting.Instance.StatusTextShiftDown);
        string left = new(' ', Setting.Instance.StatusTextShiftLeft);
        sb.AppendLine(down);
        Setting.Instance.StatusTextContent.ForEach(m => sb.AppendLine(PlaceholderManager.Resolve(m, player) + left));
        player?.SendData(PacketTypes.Status, sb.ToString(), 0, 0x01f);
    }

    public static string GetCurrentTime()
    {
        var num = Main.time / 3600.0;
        num += 4.5;
        if (!Main.dayTime)
        {
            num += 15.0;
        }
        num %= 24.0;
        return string.Format("{0}:{1:D2}", (int) Math.Floor(num), (int) Math.Floor(num % 1.0 * 60.0));
    }

    public static string GetAnglerQuestFishName()
    {
        var itemID = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        return (string) Lang.GetItemName(itemID);

    }
    public static int GetAnglerQuestFishId()
    {
        var itemID = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        return itemID;
    }

    private static readonly Regex fishMissionPlaceRegex = new(@"(?<=（抓捕位置：|\(Capturado no |\(Поймано в |\(można złapać w |\(Se trouve |\(Se encuentra en |\(Caught ).*?(?=）|\))");
    private static readonly Regex fishMissionPlaceExceptionalCasesRegex = new(@"(?<=（|\().*?(?=）|\))");

    public static string GetAnglerQuestFishingBiome()
    {
        var itemId = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        var questText = Language.GetTextValue($"AnglerQuestText.Quest_{ItemID.Search.GetName(itemId)}");
        return Language.ActiveCulture.Name switch
        {
            "en-US" or "fr-FR" or "es-ES" or "ru-RU" or "zh-Hans" or "pt-BR" or "pl-PL" =>
                fishMissionPlaceRegex.Match(questText).ToString(),
            _ =>
                fishMissionPlaceExceptionalCasesRegex.Match(questText).ToString()
        };
    }

    [GeneratedRegex(@"\[(?<type>[^\]]+):(?<id>\d+)\]")]
    public static partial Regex ChatRegex();
}