using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PerPlayerLoot;

[ApiVersion(2, 1)]
public class PPLPlugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 6);

    public override string Author => "Codian, 肝帝熙恩汉化1449";

    public override string Description => GetString("玩家战利品单独箱子.");
    #endregion

    // 假宝箱数据库实例
    public static FakeChestDatabase fakeChestDb = new FakeChestDatabase();

    // 是否启用每个玩家单独的宝箱
    public static bool enablePpl = true;

    public PPLPlugin(Main game) : base(game) { }

    public override void Initialize()
    {
        fakeChestDb.Initialize();

        Commands.ChatCommands.Add(new Command("perplayerloot.toggle", this.ToggleCommand, "ppltoggle"));

        ServerApi.Hooks.WorldSave.Register(this, this.OnWorldSave);

        // 注册放置宝箱、打开宝箱和宝箱物品变更事件
        TShockAPI.GetDataHandlers.PlaceChest += this.OnChestPlace;
        TShockAPI.GetDataHandlers.ChestOpen += this.OnChestOpen;
        TShockAPI.GetDataHandlers.ChestItemChange += this.OnChestItemChange;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.ToggleCommand);

            ServerApi.Hooks.WorldSave.Deregister(this, this.OnWorldSave);

            TShockAPI.GetDataHandlers.PlaceChest -= this.OnChestPlace;
            TShockAPI.GetDataHandlers.ChestOpen -= this.OnChestOpen;
            TShockAPI.GetDataHandlers.ChestItemChange -= this.OnChestItemChange;
        }

        base.Dispose(disposing);
    }

    // 世界保存时保存假宝箱数据
    private void OnWorldSave(WorldSaveEventArgs args)
    {
        fakeChestDb.SaveFakeChests();
    }

    // 切换是否启用每个玩家单独的宝箱
    private void ToggleCommand(CommandArgs args)
    {
        enablePpl = !enablePpl;
        if (enablePpl)
        {
            args.Player.SendSuccessMessage(GetString("现在启用了每个玩家单独的宝箱!"));
        }
        else
        {
            args.Player.SendSuccessMessage(GetString("每个玩家单独的宝箱现在被禁用！您现在可以修改宝箱，它们将被视为普通宝箱."));
        }
    }

    // 处理宝箱物品变更事件
    private void OnChestItemChange(object? sender, GetDataHandlers.ChestItemEventArgs e)
    {
        if (!enablePpl)
        {
            return;
        }

        // 获取真实宝箱对象
        var realChest = Main.chest[e.ID];
        if (realChest == null)
        {
            return;
        }

        // 检查是否是猪猪银行或保险箱
        if (realChest.bankChest)
        {
            return;
        }

        // 检查是否是玩家放置的宝箱
        if (fakeChestDb.IsChestPlayerPlaced(realChest.x, realChest.y))
        {
            return;
        }

        // 构建物品
        var item = new Item();
        item.netDefaults(e.Type);
        item.stack = e.Stacks;
        item.prefix = e.Prefix;

        // 获取或创建每个玩家的假宝箱
        var fakeChest = fakeChestDb.GetOrCreateFakeChest(e.ID, e.Player.UUID);

        // 更新假宝箱中的物品
        fakeChest.item[e.Slot] = item;

        e.Handled = true;
    }

    // 构建伪造的宝箱物品包
    private byte[] ConstructSpoofedChestItemPacket(int chestId, int slot, Item item)
    {
        var memoryStream = new MemoryStream();
        var packetWriter = new OTAPI.PacketWriter(memoryStream);

        packetWriter.BaseStream.Position = 0L;
        var position = packetWriter.BaseStream.Position;

        packetWriter.BaseStream.Position += 2L;
        packetWriter.Write((byte) PacketTypes.ChestItem);

        packetWriter.Write((short) chestId);
        packetWriter.Write((byte) slot);

        var netId = (short) item.type;
        if (item.Name == null)
        {
            netId = 0;
        }

        packetWriter.Write((short) item.stack);
        packetWriter.Write(item.prefix);
        packetWriter.Write(netId);

        var positionAfter = (int) packetWriter.BaseStream.Position;

        packetWriter.BaseStream.Position = position;
        packetWriter.Write((ushort) positionAfter);
        packetWriter.BaseStream.Position = positionAfter;

        return memoryStream.ToArray();
    }

    // 处理宝箱打开事件
    private void OnChestOpen(object? sender, GetDataHandlers.ChestOpenEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (!enablePpl)
        {
            return;
        }

        // 获取宝箱 ID
        var chestId = Chest.FindChest(e.X, e.Y);
        if (chestId == -1)
        {
            return;
        }

        // 获取真实宝箱对象
        var realChest = Main.chest[chestId];

        // 确保宝箱存在
        if (realChest == null)
        {
            return;
        }

        // 检查是否是猪猪银行或保险箱
        if (realChest.bankChest)
        {
            return;
        }

        // 检查是否是玩家放置的宝箱
        if (fakeChestDb.IsChestPlayerPlaced(realChest.x, realChest.y))
        {
            return;
        }

        // 创建或获取每个玩家的假宝箱
        var fakeChest = fakeChestDb.GetOrCreateFakeChest(chestId, e.Player.UUID);

        // 发送消息给玩家
        e.Player.SendInfoMessage(GetString("这个箱子里的战利品是每个玩家单独的!"));

        // 伪造宝箱槽位
        for (var slot = 0; slot < fakeChest.maxItems; slot++)
        {
            // 获取假宝箱中的物品
            var item = fakeChest.item[slot];

            // 伪造客户端槽位
            var payload = this.ConstructSpoofedChestItemPacket(chestId, slot, item);
            e.Player.SendRawData(payload);
        }

        // 触发宝箱打开
        e.Player.SendData(PacketTypes.ChestOpen, "", chestId);

        // 设置服务器端的活动宝箱
        e.Player.ActiveChest = chestId;
        Main.player[e.Player.Index].chest = chestId;
        // 通知客户端更新客户端状态
        e.Player.SendData(PacketTypes.SyncPlayerChestIndex, null, e.Player.Index, chestId);

        // 标记事件已处理
        e.Handled = true;
    }

    // 处理放置宝箱事件
    private void OnChestPlace(object? sender, GetDataHandlers.PlaceChestEventArgs e)
    {
        if (!enablePpl)
        {
            return;
        }

        // 检查是否已经放置了玩家宝箱
        if (!fakeChestDb.IsChestPlayerPlaced(e.TileX, e.TileY - 1))
        {
            var chestId = Chest.FindChest(e.TileX, e.TileY - 1);
            if (chestId != -1)
            {
                // 清空现有宝箱物品
                Main.chest[chestId].item = new Item[Main.chest[chestId].maxItems];
            }
        }

        // 设置玩家放置的宝箱位置
        fakeChestDb.SetChestPlayerPlaced(e.TileX, e.TileY - 1); // 这个 -1 是为了调整坐标到正确的宝箱位置
    }
}