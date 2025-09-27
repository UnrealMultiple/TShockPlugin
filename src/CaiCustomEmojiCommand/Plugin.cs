using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CaiCustomEmojiCommand;

[ApiVersion(2, 1)]
public class CaiCustomEmojiCommand : LazyPlugin
{

    public CaiCustomEmojiCommand(Main game) : base(game)
    {
    }

    public override string Author => "Cai";
    public override string Description => GetString("自定义Emoji表情执行命令");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(2025, 05, 18, 4);


    public override void Initialize()
    {
        GetDataHandlers.Emoji.Register(this.OnGetEmoji);
        base.Initialize();
    }


    private void OnGetEmoji(object? sender, GetDataHandlers.EmojiEventArgs e)
    {
        var emojiCommands = Config.Instance.EmojiCommands.Where(i => i.EmojiId == e.EmojiID);
        foreach (var i in emojiCommands)
        {
            i.ExecuteCommand(e.Player);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.Emoji.UnRegister(this.OnGetEmoji);
        }

        base.Dispose(disposing);
    }
}