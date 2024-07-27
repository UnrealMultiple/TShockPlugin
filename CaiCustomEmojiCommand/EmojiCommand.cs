using Newtonsoft.Json;
using TShockAPI;

namespace CaiCustomEmojiCommand;

[Serializable]
public class EmojiCommand
{
    [JsonProperty("命令")] public string Command;

    [JsonProperty("表情ID")] public int EmojiId;

    public EmojiCommand(int emojiId, string command)
    {
        EmojiId = emojiId;
        Command = command;
    }

    public void ExecuteCommand(TSPlayer player)
    {
        Commands.HandleCommand(player, Command);
    }
}