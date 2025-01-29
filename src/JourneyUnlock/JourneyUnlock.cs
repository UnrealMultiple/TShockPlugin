using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;

namespace JourneyUnlock;

[ApiVersion(2, 1)]
public class JourneyUnlock : TerrariaPlugin
{
    public override string Author => "Maxthegreat99，肝帝熙恩汉化";

    public override string Description => GetString("允许您为旅途中的角色解锁所有或特定物品");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 1, 5);

    public JourneyUnlock(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("journeyunlock.unlock", this.unlockCommand, "journeyunlock", "junlock", "i解锁")
        { AllowServer = false });

        Commands.ChatCommands.Add(new Command("journeyunlock.unlockfor", this.unlockForCommand, "unlockfor", "unlockf", "g解锁玩家"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.unlockCommand || x.CommandDelegate == this.unlockForCommand);
        }
        base.Dispose(disposing);
    }

    private void unlock(Player tplayer, TSPlayer sender, string parameter, bool isSelf)
    {

        //Case: unlock every item
        if (parameter == "*")
        {
            for (var i = 0; i < ItemID.Count; i++)
            {
                tplayer.creativeTracker.ItemSacrifices.RegisterItemSacrifice(i, 999);
                var _response = NetCreativeUnlocksModule.SerializeItemSacrifice(i, 999);
                NetManager.Instance.SendToClient(_response, tplayer.whoAmI);
            }
            if (!isSelf)
            {
                TSPlayer.FindByNameOrID(tplayer.name)[0].SendInfoMessage(GetString("[旅程解锁] {0} 已为您解锁所有物品！"), sender.Name);
                sender.SendSuccessMessage(GetString("[旅程解锁] 成功为 {0} 解锁所有物品！"), tplayer.name);
            }
            else
            {
                sender.SendSuccessMessage(GetString("[旅程解锁] 成功解锁所有物品！"));
            }

            return;
        }

        //Case: unlock a specific item via id
        if (int.TryParse(parameter, out var itemid) && TShock.Utils.GetItemById(itemid) != null)
        {
            tplayer.creativeTracker.ItemSacrifices.RegisterItemSacrifice(itemid, 999);
            var _response = NetCreativeUnlocksModule.SerializeItemSacrifice(itemid, 999);
            NetManager.Instance.SendToClient(_response, tplayer.whoAmI);

            if (!isSelf)
            {
                TSPlayer.FindByNameOrID(tplayer.name)[0].SendInfoMessage(GetString("[旅程解锁] {0} 为您解锁了物品 [i:{1}]！"), sender.Name, itemid);
                sender.SendSuccessMessage(GetString("[旅程解锁] 成功为 {1} 解锁了物品 [i:{0}]！"), itemid, tplayer.name);
            }
            else
            {
                sender.SendSuccessMessage(GetString("[旅程解锁] 成功解锁物品 [i:{0}]！"), itemid);
            }

            return;
        }
        else if (TShock.Utils.GetItemById(itemid) == null || itemid < 1 || itemid > ItemID.Count)
        {
            sender.SendErrorMessage(GetString("[旅程解锁] 物品ID无效！"));
            return;
        }

        //Case: unlock a specific item via name
        var itemname = parameter;

        if (TShock.Utils.GetItemByName(itemname).Count == 0)
        {
            sender.SendErrorMessage(GetString("[旅程解锁] 未找到对应物品！"));
            return;
        }

        if (TShock.Utils.GetItemByName(itemname).Count > 1)
        {
            sender.SendErrorMessage(GetString("[旅程解锁] 找到了多个同名物品！"));
            return;
        }

        itemid = TShock.Utils.GetItemByName(itemname)[0].netID;

        tplayer.creativeTracker.ItemSacrifices.RegisterItemSacrifice(itemid, 999);
        var response = NetCreativeUnlocksModule.SerializeItemSacrifice(itemid, 999);
        NetManager.Instance.SendToClient(response, tplayer.whoAmI);

        if (!isSelf)
        {
            TSPlayer.FindByNameOrID(tplayer.name)[0].SendInfoMessage(GetString("[旅程解锁] {0} 为您解锁了物品 [i:{1}]！"), sender.Name, itemid);
            sender.SendSuccessMessage(GetString("[旅程解锁] 成功为 {1} 解锁了物品 [i:{0}]！"), itemid, tplayer.name);
        }
        else
        {
            sender.SendSuccessMessage(GetString("[旅程解锁] 成功解锁物品 [i:{0}]！"), itemid);
        }
    }

    private void unlockCommand(CommandArgs args)
    {
        // 使用不当
        if (args.Parameters.Count != 1)
        {
            args.Player.SendInfoMessage(GetString("[旅程解锁] /i解锁 {物品名/ID} - 解锁指定物品。"));
            args.Player.SendInfoMessage(GetString("[旅程解锁] /i解锁 * - 解锁所有物品。"));
            return;
        }

        this.unlock(args.TPlayer, args.Player, args.Parameters[0], true);
    }

    private void unlockForCommand(CommandArgs args)
    {
        // 使用不当
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("[旅程解锁] /g解锁 {玩家名} {物品名/ID} - 为指定玩家解锁物品。"));
            args.Player.SendInfoMessage(GetString("[旅程解锁] /g解锁 {玩家名} * - 为指定玩家解锁所有物品。"));
            return;
        }

        if (TSPlayer.FindByNameOrID(args.Parameters[0]).Count == 0)
        {
            args.Player.SendErrorMessage(GetString("[旅程解锁] 未找到玩家！"));
            return;
        }
        if (TSPlayer.FindByNameOrID(args.Parameters[0]).Count > 1)
        {
            args.Player.SendErrorMessage(GetString("[旅程解锁] 找到多个同名玩家！"));
            return;
        }

        var player = TSPlayer.FindByNameOrID(args.Parameters[0])[0];

        this.unlock(player.TPlayer, args.Player, args.Parameters[1], false);
    }
}