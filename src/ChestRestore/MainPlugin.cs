using MonoMod.RuntimeDetour;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChestRestore;

[ApiVersion(2, 1)]
public class MainPlugin(Main game) : TerrariaPlugin(game)
{
    private readonly Dictionary<int, bool> playersInEditMode = new Dictionary<int, bool>();

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
  
    public override Version Version => new Version(1, 2, 0);

    public override string Author => "Cjx重构 ，肝帝熙恩简单修改";
    public override string Description => GetString("无限宝箱插件");

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        Commands.ChatCommands.Add(new Command("chest.edit", this.ModifyChest, "mc"));
        Commands.ChatCommands.Add(new Command("chest.edit", this.ToggleEditMode, "chestedit", "ce", "修改箱子")
        {
            HelpText = GetString("切换个人修改箱子名字和内容的模式")
        });
    }

    private void ModifyChest(CommandArgs args)
    {
        for(var i = 0; i < Main.chest.Length; i++)
        {
            var chest = Main.chest[i];
            if (chest == null || chest.x < 0 || chest.y < 0)
            {
                continue;
            }

            for (var j = 0; j < chest.item.Length; j++)
            { 
                var item = chest.item[j];
                if (item.IsAir)
                {
                    continue;
                }

                item.stack = item.maxStack; // 设置物品堆叠数为最大值
                args.Player.SendData(PacketTypes.ChestItem, "", i, j, item.type, item.prefix);
            }
        }
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
        if (args.MsgID == PacketTypes.ChestOpen)
        {
            var nameOffset = args.Index + 6;
            var nameLength = args.Msg.readBuffer[nameOffset];

            // 检查是否正在尝试修改箱子名称
            if ((nameLength > 0 && nameLength <= 20) || nameLength == 255)
            {
                using var br = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
                _ = br.ReadInt16();
                _ = br.ReadInt16();
                _ = br.ReadInt16();
                var length = br.ReadByte();
                if (length <= 0)
                {
                    return;
                }
                var name = br.ReadString();
                if (Main.chest[tsplayer.TPlayer.chest].name == name || string.IsNullOrEmpty(name))
                {
                    return;
                }
                tsplayer.ActiveChest = -1;
                tsplayer.TPlayer.chest = -1;
                tsplayer.SendData(PacketTypes.ChestOpen, "", -1);
                tsplayer.SendErrorMessage(GetString("你没有权限修改箱子名称。"));
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
            player.SendSuccessMessage(!mode ? GetString("你已进入箱子修改模式。") : GetString("你已退出箱子修改模式。"));
        }
        else
        {
            this.playersInEditMode[player.Index] = true;
            player.SendSuccessMessage(GetString("你已进入箱子修改模式。"));
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
