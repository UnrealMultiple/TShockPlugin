using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace Plugin;

[ApiVersion(2, 1)]
public class ShowArmors : TerrariaPlugin
{
    public override string Author => "Ak，肝帝熙恩更新";

    public override string Description => GetString("展示装备栏");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 6);

    public ShowArmors(Main game) : base(game)
    {
    }

    public override void Initialize()
    {

        Commands.ChatCommands.Add(new Command("ShowArmors", this.ShowMySlots, "装备", "show", "zb")
        {
            HelpText = GetString("发送自己的装备配置到聊天框。别名：show 和 zb ")
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 清理托管资源
            Commands.ChatCommands.RemoveAll(cmd => cmd.CommandDelegate == this.ShowMySlots);
        }
        base.Dispose(disposing);
    }

    private void ShowMySlots(CommandArgs args)
    {
        TSPlayer? target = null;
        Item[]? armors = null;
        var str = "";
        const int MAX_SLOTS_NUMBER = 10;
        var argsCount = args.Parameters.Count;
        var nothingEquipped = false;

        if (argsCount != 0 && argsCount != 1)
        {
            args.Player.SendErrorMessage(GetString("语法错误！正确语法 [c/55D284:/装备] [c/55D284:<玩家>]"));
        }
        else if (argsCount == 0)
        {
            target = args.Player;
            armors = target.TPlayer.armor;
            str = $"{target.Name}" + " : ";
        }
        else if (argsCount == 1)
        {
            var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (players.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("不存在该玩家!"));
            }
            else if (players.Count > 1)
            {
                args.Player.SendMultipleMatchError(players.Select(p => p.Name));
            }
            else
            {
                target = players[0];
                armors = target.TPlayer.armor;
                str = GetString($"{target.Name} : 拿着 [i/p{target.SelectedItem.prefix}:{target.SelectedItem.type}]{Lang.prefix[target.SelectedItem.prefix].Value}");
            }
        }
        if (target is null || armors is null)
        {
            return;
        }
        for (var i = 0; i < MAX_SLOTS_NUMBER; i++)
        {
            var isArmor = i < 3;
            var isAccessories = i < MAX_SLOTS_NUMBER;
            if (armors[i] == null || armors[i].type == 0)
            {
                continue;
            }
            else if (isArmor)
            {
                str += $"[i:{armors[i].type}]";
                continue;
            }
            else if (isAccessories)
            {
                str += $"[i/p{armors[i].prefix}:{armors[i].type}]" + $"{Lang.prefix[armors[i].prefix].Value}" + " ";
            }
            else
            {
                continue;
            }
        }

        nothingEquipped = str == ($"{target.Name}" + " : ");
        if (argsCount == 0)
        {
            if (nothingEquipped)
            {
                TShock.Utils.Broadcast(GetString($"{target.Name}这家伙啥都没装备。只拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.type}]{Lang.prefix[target.SelectedItem.prefix].Value}"), Microsoft.Xna.Framework.Color.Green);
            }
            else
            {
                TShock.Utils.Broadcast(str += GetString($" 拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.type}]{Lang.prefix[target.SelectedItem.prefix].Value}"), Microsoft.Xna.Framework.Color.Green);
            }
        }
        else if (argsCount == 1)
        {
            if (nothingEquipped)
            {
                args.Player.SendSuccessMessage(GetString($"{target.Name}这家伙啥都没装备，只拿着 [i/p{target.SelectedItem.prefix}:{target.SelectedItem.type}]{Lang.prefix[target.SelectedItem.prefix].Value}"));
            }
            else
            {
                args.Player.SendSuccessMessage(str += GetString($" 拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.type}]{Lang.prefix[target.SelectedItem.prefix].Value}"));
            }
        }

    }
    

}