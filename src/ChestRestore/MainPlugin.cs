using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChestRestore;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    private readonly Dictionary<int, bool> playersInEditMode = new Dictionary<int, bool>();

    public MainPlugin(Main game) : base(game) { }

    public override string Name => "ChestRestore";
    public override Version Version => new Version(1, 0, 4);
    public override string Author => "Cjx重构 | 肝帝熙恩简单修改";
    public override string Description => "无限宝箱插件";

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        Commands.ChatCommands.Add(new Command("chest.edit", this.ToggleEditMode, "chestedit", "ce", "修改箱子")
        {
            HelpText = "切换个人修改箱子名字和内容的模式"
        });
    }

    private void OnGetData(GetDataEventArgs args)
    {
        var tsplayer = TShock.Players[args.Msg.whoAmI];
        if (args.MsgID == PacketTypes.ChestItem)
        {
            if (!this.IsPlayerInEditMode(tsplayer))
            {
                args.Handled = true;
            }
        }

        if (args.MsgID == PacketTypes.ChestName)
        {
            if (!this.IsPlayerInEditMode(tsplayer) || !tsplayer.HasPermission("chest.name"))
            {
                args.Handled = true;
            }
        }
    }

    private bool IsPlayerInEditMode(TSPlayer player)
    {
        // 检查玩家是否在修改模式中
        return this.playersInEditMode.TryGetValue(player.Index, out var mode) && mode;
    }

    private void ToggleEditMode(CommandArgs args)
    {
        var player = args.Player;

        if (this.playersInEditMode.TryGetValue(player.Index, out var mode))
        {
            this.playersInEditMode[player.Index] = !mode;
            player.SendSuccessMessage(!mode ? "你已进入箱子修改模式。" : "你已退出箱子修改模式。");
        }
        else
        {
            this.playersInEditMode[player.Index] = true;
            player.SendSuccessMessage("你已进入箱子修改模式。");
        }
    }

    // 玩家离开时移除其修改模式
    private void OnLeave(LeaveEventArgs args)
    {
        this.playersInEditMode.Remove(args.Who);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.ToggleEditMode);
        }
        base.Dispose(disposing);
    }
}
