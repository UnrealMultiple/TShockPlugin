using Microsoft.Xna.Framework;
using System.IO.Streams;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.Net;

namespace HouseRegion;

public delegate bool GetDataHandlerDelegate(GetDataHandlerArgs args);//设置代理
public class GetDataHandlerArgs : EventArgs//要让这个数据在数据的基础上再多两个信息，用户和解析后的数据
{
    public TSPlayer Player { get; private set; }
    public MemoryStream Data { get; private set; }
    public Player TPlayer => this.Player.TPlayer;
    public GetDataHandlerArgs(TSPlayer player, MemoryStream data) { this.Player = player; this.Data = data; }
}
public static class GetDataHandlers
{
    static readonly string EditHouse = "house.edit";
    private static Dictionary<PacketTypes, GetDataHandlerDelegate> GetDataHandlerDelegates = null!;//创建词典
    public static void InitGetDataHandler()
    {
        GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
        {   {PacketTypes.Tile, HandleTile},//修改砖放或敲
		    {PacketTypes.DoorUse,HandleDoorUse},//使用门
            { PacketTypes.PlayerSlot, HandlePlayerSlot },
			//{PacketTypes.TileSendSquare, HandleSendTileSquareCentered},//地区改变
			{PacketTypes.ChestGetContents, HandleChestOpen },//打开箱子
			{PacketTypes.ChestItem, HandleChestItem },//更新箱子物品
			{PacketTypes.ChestOpen, HandleChestActive },//修改箱子
			{PacketTypes.PlaceChest, HandlePlaceChest },//放置箱子
			{PacketTypes.SignRead, HandleSignRead },//读标牌
			{PacketTypes.SignNew, HandleSign },//修改标牌
			{PacketTypes.LiquidSet, HandleLiquidSet},//放水//PacketTypes.ChestUnlock解锁箱子
			//{PacketTypes.HitSwitch,HandleHitSwitch},//点击开关
			{PacketTypes.PaintTile, HandlePaintTile},//油漆块
			{PacketTypes.PaintWall, HandlePaintWall},//油漆墙体
			//{PacketTypes.Teleport, HandleTeleport},//NPC或玩家传送处理//仿佛没有必要
			{PacketTypes.PlaceObject, HandlePlaceObject },//放置物体
			{PacketTypes.PlaceTileEntity, HandlePlaceTileEntity },//放置实体
			{PacketTypes.PlaceItemFrame, HandlePlaceItemFrame },//放置物品框
            {PacketTypes.WeaponsRackTryPlacing, HandleWeaponsRackTryPlacing },
            {PacketTypes.FoodPlatterTryPlacing, HandleFoodPlatterTryPlacing },
            {PacketTypes.RequestTileEntityInteraction, HandleRequestTileEntityInteraction },
            {PacketTypes.TileEntityHatRackItemSync, HandleTileEntityHatRackItemSync },
			//{ PacketTypes.SyncExtraValue, HandleSyncExtraValue },同步附加价值？
			{PacketTypes.GemLockToggle, HandleGemLockToggle },//宝石锁启动
			{PacketTypes.MassWireOperation, HandleMassWireOperation },//大规模电路
		};
    }

    private static bool HandleTileEntityHatRackItemSync(GetDataHandlerArgs args)
    {
        var id = args.Data.ReadInt32();
        var ply = args.Data.ReadByte();
        if (TileEntity.ByID.TryGetValue(id, out var tileEntity) && tileEntity is TEHatRack)
        {
            var house = Utils.InAreaHouse(tileEntity.Position.X, tileEntity.Position.Y);//直接读出放置房子
            if (house == null)
            {
                return false;
            }

            if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
            {
                return false;
            }

            if (Config.Instance.WarningSpoiler)
            {
                args.Player.Disable(GetString("无权修改房子保护的物品!"));
            }
            args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的物品。"));
            return true;
        }
        return false;
    }

    private static bool HandleFoodPlatterTryPlacing(GetDataHandlerArgs args)
    {
        var x = args.Data.ReadInt16();
        var y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        var te = (TEFoodPlatter) TileEntity.ByID[TEFoodPlatter.Find(x, y)];
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护的物品!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的物品。"));

        if (args.Player.SelectedItem.type > 0)
        {
            args.Player.SetData("PlaceSlot", (true, args.Player.TPlayer.selectedItem));
            NetMessage.SendData(86, -1, -1, NetworkText.Empty, te.ID);
        }
        return true;
    }

    private static bool HandleRequestTileEntityInteraction(GetDataHandlerArgs args)
    {
        var id = args.Data.ReadInt32();
        var ply = args.Data.ReadByte();
        if (!TileEntity.IsOccupied(id, out var _) && TileEntity.ByID.TryGetValue(id, out var tileEntity))
        {
            var house = Utils.InAreaHouse(tileEntity.Position.X, tileEntity.Position.Y);//直接读出放置房子
            if (house == null)
            {
                return false;
            }

            if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
            {
                return false;
            }

            if (Config.Instance.WarningSpoiler)
            {
                args.Player.Disable(GetString("无权修改房子保护的物品!"));
            }
            args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的物品。"));
            return true;
        }
        return false;
    }

    private static bool HandlePlayerSlot(GetDataHandlerArgs args)
    {
        var plr = args.Data.ReadInt8();
        var slot = args.Data.ReadInt16();
        var plyData = args.Player.GetData<(bool, int)>("PlaceSlot");
        if (plyData.Item1 && plyData.Item2 == slot)
        {
            NetMessage.SendData(5, -1, -1, null, plr, slot);
            args.Player.RemoveData("PlaceSlot");
            return true;
        }
        return false;
    }

    private static bool HandleWeaponsRackTryPlacing(GetDataHandlerArgs args)
    {
        var x = args.Data.ReadInt16();
        var y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        var te = (TEWeaponsRack) TileEntity.ByID[TEWeaponsRack.Find(x, y)];
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护的物品!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的物品。"));
        if (args.Player.SelectedItem.type > 0)
        {
            args.Player.SetData("PlaceSlot", (true, args.Player.TPlayer.selectedItem));
            NetMessage.SendData(86, -1, -1, NetworkText.Empty, te.ID);
        }
        return true;
    }

    public static bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
    {
        if (GetDataHandlerDelegates.TryGetValue(type, out var handler))
        {
            try { return handler(new GetDataHandlerArgs(player, data)); }
            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误调用事件时出错:") + ex.ToString()); }
        }
        return false;
    }
    private static bool HandleTile(GetDataHandlerArgs args)//17修改砖放或敲
    {
        int action = args.Data.ReadInt8();//类型
        int x = args.Data.ReadInt16();
        int y = args.Data.ReadInt16();

        var house = Utils.InAreaHouse(x, y);//直接读出敲的房子
        if (HousingPlugin.LPlayers[args.Player.Index]!.Look)//敲击砖块确认房屋名
        {
            if (house == null)
            {
                args.Player.SendMessage(GetString("敲击处不属于任何房子。"), Color.Yellow);
            }
            else
            {
                var AuthorNames = "";
                try { AuthorNames = TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(house.Author)).Name; }
                catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误超标错误:") + ex.ToString()); }
                args.Player.SendMessage(GetString($"敲击处为 {AuthorNames} 的房子: {house.Name} 状态: {(!house.Locked || Config.Instance.LimitLockHouse ? GetString("未上锁") : GetString("已上锁"))}"), Color.Yellow);
            }
            args.Player.SendTileSquareCentered(x, y);
            HousingPlugin.LPlayers[args.Player.Index]!.Look = false;
            return true;
        }
        if (args.Player.AwaitingTempPoint > 0)//设置点位
        {
            args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = x;
            args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = y;
            if (args.Player.AwaitingTempPoint == 1)
            {
                args.Player.SendMessage(GetString("保护区左上角已设置!"), Color.Yellow);
            }

            if (args.Player.AwaitingTempPoint == 2)
            {
                args.Player.SendMessage(GetString("保护区右下角已设置!"), Color.Yellow);
            }

            args.Player.SendTileSquareCentered(x, y);
            args.Player.AwaitingTempPoint = 0;
            return true;
        }
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力损坏被房子保护的地区。"));
        args.Player.SendTileSquareCentered(x, y);
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandleDoorUse(GetDataHandlerArgs args)//19使用门
    {
        args.Data.ReadInt8();//跳过对于我们的目的没影响的类型
        int x = args.Data.ReadInt16();
        int y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if (!house.Locked || Config.Instance.LimitLockHouse)
        {
            return false;//没锁，那随便开
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改门!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的地区的门。"));
        args.Player.SendTileSquareCentered(x, y);
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandleSendTileSquareCentered(GetDataHandlerArgs args)//20地区改变，比如腐化血腥花草什么的
    {
        //return false;//此处虽然可以阻止地形改变但是由于资源耗费巨大不建议使用
        var size = args.Data.ReadInt16();
        int tileX = args.Data.ReadInt16();
        int tileY = args.Data.ReadInt16();
        //Console.WriteLine("地区改变{0} X{1} Y{2}" ,size, tileX, tileY);
        if (size > 1)
        {
            return false;//腐化血腥的生成好像都是1
        }
        
        var house = Utils.InAreaHouse(tileX, tileY);//确定是否保护区
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        args.Player.SendTileSquareCentered(tileX, tileY);
        return true;
    }
    private static bool HandleChestOpen(GetDataHandlerArgs args)//31打开箱子
    {
        int x = args.Data.ReadInt16();
        int y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if ((!house.Locked || Config.Instance.LimitLockHouse) && !Config.Instance.ProtectiveChest)
        {
            return false;//没锁,且不保护箱子，那随便开
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权打开箱子!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力打开被房子保护的地区的箱子。"));
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandleChestItem(GetDataHandlerArgs args)//32更新箱子
    {
        var id = args.Data.ReadInt16();
        var x = Main.chest[id].x;
        var y = Main.chest[id].y;
        var house = Utils.InAreaHouse(x, y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if ((!house.Locked || Config.Instance.LimitLockHouse) && !Config.Instance.ProtectiveChest)
        {
            return false;//没锁,且不保护箱子，那随便开
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权更新箱子!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力更新被房子保护的地区的箱子。"));
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandleChestActive(GetDataHandlerArgs args)//33修改箱子
    {
        args.Data.ReadInt16();//跳过编号
        int x = args.Data.ReadInt16();
        int y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if ((!house.Locked || Config.Instance.LimitLockHouse) && !Config.Instance.ProtectiveChest)
        {
            return false;//没锁,且不保护箱子，那随便开
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改箱子!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的地区的箱子。"));
        args.Player.SendData(PacketTypes.ChestOpen, "", -1);
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandlePlaceChest(GetDataHandlerArgs args)//34放置箱子
    {
        args.Data.ReadByte();
        int tileX = args.Data.ReadInt16();
        int tileY = args.Data.ReadInt16();
        var rect = new Rectangle(tileX, tileY, 3, 3);//创造个同样大小的范围虽然箱子是2*2但是梳妆台却是3因此就大不就小
        for (var i = 0; i < HousingPlugin.Houses.Count; i++)
        {
            var house = HousingPlugin.Houses[i]; if (house == null)
            {
                continue;
            }

            if (house.HouseArea.Intersects(rect) && !(args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house)))
            {
                if (Config.Instance.WarningSpoiler)
                {
                    args.Player.Disable(GetString("无权放置家具!"));
                }

                args.Player.SendErrorMessage(GetString("你没有权力放置被房子保护的地区的家具。"));
                args.Player.SendTileSquareCentered(tileX, tileY, 3);
                return true;//假表示允许修改//真表示禁止修改
            }
        }
        return false;
    }
    private static bool HandleSignRead(GetDataHandlerArgs args)//46读标牌，应该不影响读吧？
    {
        return false;
    }
    private static bool HandleSign(GetDataHandlerArgs args)//46写标牌
    {
        var id = args.Data.ReadInt16();
        var x = args.Data.ReadInt16();
        var y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改标牌!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的地区的标牌。"));
        args.Player.SendData(PacketTypes.SignNew, "", id);
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandleLiquidSet(GetDataHandlerArgs args)//48放水损毁
    {
        int tileX = args.Data.ReadInt16();
        int tileY = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(tileX, tileY);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权放水!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力在被房子保护的地区放水。"));
        args.Player.SendTileSquareCentered(tileX, tileY);
        return true;//假表示允许修改//真表示禁止修改
    }
    //private static bool HandleHitSwitch(GetDataHandlerArgs args)//59点击开关，暂时无意义
    //{
    //    return false;
    //    int tileX = args.Data.ReadInt16();
    //    int tileY = args.Data.ReadInt16();
    //    //Console.WriteLine("点开关X{0} Y{1}}" , tileX, tileY);
    //}
    private static bool HandlePaintTile(GetDataHandlerArgs args)//63油漆块
    {
        var X = args.Data.ReadInt16();
        var Y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(X, Y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权油漆砖!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力在被房子保护的地区油漆砖。"));
        args.Player.SendData(PacketTypes.PaintTile, "", X, Y, Main.tile[X, Y].color());
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandlePaintWall(GetDataHandlerArgs args)//64油漆墙
    {
        var X = args.Data.ReadInt16();
        var Y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(X, Y);//直接读出房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权油漆墙!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力在被房子保护的地区油漆墙。"));
        args.Player.SendData(PacketTypes.PaintWall, "", X, Y, Main.tile[X, Y].wallColor());
        return true;//假表示允许修改//真表示禁止修改
    }
    //private static bool HandleTeleport(GetDataHandlerArgs args)//65处理玩家传送,暂时无意义
    //{
    //    return false;
    //    //if (HousingPlugin.LConfig.禁止传送进房子)
    //    var Flags = args.Data.ReadInt8();
    //    var ID = args.Data.ReadInt16();
    //    var X = args.Data.ReadSingle();
    //    var Y = args.Data.ReadSingle();
    //}
    private static bool HandlePlaceObject(GetDataHandlerArgs args)//79放置物块
    {
        int x = args.Data.ReadInt16();
        int y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的地区。"));
        args.Player.SendTileSquareCentered(x, y);
        return true;//假表示允许修改//真表示禁止修改
    }
    private static bool HandlePlaceTileEntity(GetDataHandlerArgs args)//87放置实体
    {
        var x = args.Data.ReadInt16();
        var y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的地区。"));
        args.Player.SendTileSquareCentered(x, y);
        return true;//假表示允许修改//真表示禁止修改
    }

    private static bool HandlePlaceItemFrame(GetDataHandlerArgs args)//89放置物品框
    {
        var x = args.Data.ReadInt16();
        var y = args.Data.ReadInt16();
        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        var te = (TEItemFrame) TileEntity.ByID[TEItemFrame.Find(x, y)];
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权修改房子保护的物品!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力修改被房子保护的物品。"));

        if (args.Player.SelectedItem.type > 0)
        {
            args.Player.SetData("PlaceSlot", (true, args.Player.TPlayer.selectedItem));
            NetMessage.SendData(86, -1, -1, NetworkText.Empty, te.ID);
        }
        return true;
    }
    private static bool HandleGemLockToggle(GetDataHandlerArgs args)//105宝石锁
    {
        var x = (int) args.Data.ReadInt16();
        var y = (int) args.Data.ReadInt16();
        if (!Config.Instance.ProtectiveGemstoneLock)
        {
            return false;
        }

        var house = Utils.InAreaHouse(x, y);//直接读出放置房子
        if (house == null)
        {
            return false;
        }

        if (args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
        {
            return false;
        }

        if (Config.Instance.WarningSpoiler)
        {
            args.Player.Disable(GetString("无权触发房子保护的宝石锁!"));
        }

        args.Player.SendErrorMessage(GetString("你没有权力触发被房子保护的宝石锁。"));
        return true;
    }
    private static bool HandleMassWireOperation(GetDataHandlerArgs args)//109规模电路
    {
        int x1 = args.Data.ReadInt16();
        int y1 = args.Data.ReadInt16();
        int x2 = args.Data.ReadInt16();
        int y2 = args.Data.ReadInt16();
        var A = new Rectangle(Math.Min(x1, x2), args.TPlayer.direction != 1 ? y1 : y2, Math.Abs(x2 - x1) + 1, 1);
        var B = new Rectangle(args.TPlayer.direction != 1 ? x2 : x1, Math.Min(y1, y2), 1, Math.Abs(y2 - y1) + 1);
        for (var i = 0; i < HousingPlugin.Houses.Count; i++)
        {
            var house = HousingPlugin.Houses[i]; if (house == null)
            {
                continue;
            }

            if (house.HouseArea.Intersects(A) || house.HouseArea.Intersects(B))
            {
                if (!(args.Player.Group.HasPermission(EditHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}