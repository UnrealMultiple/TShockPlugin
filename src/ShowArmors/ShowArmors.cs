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
    public override Version Version => new Version(1, 0, 5);

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
                str = GetString($"{target.Name} : 拿着 [i/p{target.SelectedItem.prefix}:{target.SelectedItem.netID}]{(ItemPrefix) target.SelectedItem.prefix}");
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
            if (armors[i] == null || armors[i].netID == 0)
            {
                continue;
            }
            else if (isArmor)
            {
                str += $"[i:{armors[i].netID}]";
                continue;
            }
            else if (isAccessories)
            {
                str += $"[i/p{armors[i].prefix}:{armors[i].netID}]" + $"{(ItemPrefix) armors[i].prefix}" + " ";
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
                TShock.Utils.Broadcast(GetString($"{target.Name}这家伙啥都没装备。只拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.netID}]{(ItemPrefix) target.SelectedItem.prefix}"), Microsoft.Xna.Framework.Color.Green);
            }
            else
            {
                TShock.Utils.Broadcast(str += GetString($" 拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.netID}]{(ItemPrefix) target.SelectedItem.prefix}"), Microsoft.Xna.Framework.Color.Green);
            }
        }
        else if (argsCount == 1)
        {
            if (nothingEquipped)
            {
                args.Player.SendSuccessMessage(GetString($"{target.Name}这家伙啥都没装备，只拿着 [i/p{target.SelectedItem.prefix}:{target.SelectedItem.netID}]{(ItemPrefix) target.SelectedItem.prefix}"));
            }
            else
            {
                args.Player.SendSuccessMessage(str += GetString($" 拿着: [i/p{target.SelectedItem.prefix}:{target.SelectedItem.netID}]{(ItemPrefix) target.SelectedItem.prefix}"));
            }
        }

    }

    public enum ItemPrefix
    {
        无附魔,
        大,
        巨大,
        危险,
        凶残,

        锋利,
        尖锐,
        微小,
        可怕,

        小,
        钝,
        倒霉,
        笨重,

        可耻,
        重,
        轻,
        精准,

        迅速,
        急速远程,
        恐怖,
        致命远程,

        可靠,
        可畏,
        无力,
        粗笨,

        强大,
        神秘,
        精巧,
        精湛,

        笨拙,
        无知,
        错乱,
        威猛,

        禁忌,
        天界,
        狂怒,
        锐利,
        高端,
        强力,
        碎裂,
        破损,
        粗劣,
        迅捷魔法,
        致命,
        灵活,
        灵巧,
        残暴,
        缓慢,
        迟钝,
        呆滞,
        惹恼,
        凶险,
        狂躁,
        致伤,
        强劲,
        粗鲁,
        虚弱,
        无情,
        暴怒,
        神级,
        恶魔,
        狂热,
        坚硬,
        守护,
        装甲,
        护佑,
        奥秘,
        精确,
        幸运,
        锯齿,
        尖刺,
        愤怒,
        险恶,
        轻快,
        快速,
        急速,
        迅捷,
        狂野,
        鲁莽,
        勇猛,
        暴力,
        传奇,
        虚幻,
        神话
    }

}