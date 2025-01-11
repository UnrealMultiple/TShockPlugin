using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using TShockAPI;

namespace CaiCustomEmojiCommand;

[Serializable]
public class EmojiCommand
{
    [LocalizedPropertyName(CultureType.Chinese, "命令")]
    [LocalizedPropertyName(CultureType.English, "Command")]
    public string Command { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "表情ID")]
    [LocalizedPropertyName(CultureType.English, "EmojiId")]
    public int EmojiId { get; set; }

    public EmojiCommand(int emojiId, string command)
    {
        this.EmojiId = emojiId;
        this.Command = command;
    }

    public void ExecuteCommand(TSPlayer player)
    {
        Commands.HandleCommand(player, this.Command);
    }
}