using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace CaiCustomEmojiCommand;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "CaiCustomEmojiCommand";

    [LocalizedPropertyName(CultureType.Chinese, "命令列表")]
    [LocalizedPropertyName(CultureType.English, "EmojiCommands")]
    public List<EmojiCommand> EmojiCommands { get; set; } = new();

    protected override void SetDefault()
    {
        this.EmojiCommands = new List<EmojiCommand>
        {
            new EmojiCommand(0, "/home")
        };
    }
}