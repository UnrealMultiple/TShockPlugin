using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CaiCustomEmojiCommand;

[ApiVersion(2, 1)]
public class CaiCustomEmojiCommand : TerrariaPlugin
{
    private Config _config = null!;

    public CaiCustomEmojiCommand(Main game) : base(game)
    {
    }

    public override string Author => "Cai";
    public override string Description => "自定义Emoji表情执行命令";
    public override string Name => "CaiCustomEmojiCommand";
    public override Version Version => new(2024, 9, 8, 1);


    public override void Initialize()
    {
        this._config = Config.Read()!;
        GetDataHandlers.Emoji.Register(this.OnGetEmoji);
        GeneralHooks.ReloadEvent += this.GeneralHooksOnReloadEvent;
    }

    private void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        this._config = Config.Read()!;
        e.Player.SendSuccessMessage(GetString("[CustomEmojiCommand]配置文件已重载!"));
    }

    private void OnGetEmoji(object? sender, GetDataHandlers.EmojiEventArgs e)
    {
        var emojiCommands = this._config.EmojiCommands.Where(i => i.EmojiId == e.EmojiID);
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
            GeneralHooks.ReloadEvent -= this.GeneralHooksOnReloadEvent;
        }

        base.Dispose(disposing);
    }
}