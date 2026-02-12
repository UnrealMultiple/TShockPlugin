using Microsoft.Xna.Framework;
using MiniGamesAPI;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TShockAPI;

namespace MainPlugin;

public class BuildPlayer : MiniPlayer, IComparable<BuildPlayer>
{
    public int AquiredMarks { get; set; }

    public MiniRegion? CurrentRegion { get; set; }

    public int GiveMarks { get; set; }

    public string SelectedTopic { get; set; }

    public bool Locked { get; set; }

    public bool Marked { get; set; }

    public BuildPlayer(int id, string name, TSPlayer player)
    {
        this.SelectedTopic = null!;

        this.ID = id;
        this.Name = name;
        this.Player = player;
        this.CurrentRoomID = 0;
        this.CurrentRegion = null;
        this.Locked = false;
        this.Marked = false;
        this.AquiredMarks = 0;
    }

    public void Join(BuildRoom room)
    {
        if (room.GetPlayerCount() >= room.MaxPlayer)
        {
            this.SendInfoMessage(GetString("此房间满人了"));
            return;
        }
        if (room.Status != 0)
        {
            this.SendInfoMessage(GetString("该房间状态无法加入游戏"));
            return;
        }
        if (ConfigUtils.GetRoomByID(this.CurrentRoomID) != null)
        {
            this.Leave();
        }
        this.Join(room.ID);
        room.Players.Add(this);
        room.Broadcast(GetString("玩家 ") + this.Name + GetString(" 加入了房间"), Color.MediumAquamarine);
        this.Teleport(room.WaitingPoint);
        if (!room.Loaded)
        {
            room.LoadRegion();
        }
    }

    public new void Leave()
    {
        var roomByID = ConfigUtils.GetRoomByID(this.CurrentRoomID);
        if (roomByID == null)
        {
            this.SendInfoMessage(GetString("房间不存在"));
            return;
        }
        if (roomByID.Status != 0)
        {
            this.SendInfoMessage(GetString("该房间状态无法离开游戏"));
            return;
        }
        roomByID.Players.Remove(this);
        roomByID.Broadcast(GetString("玩家 ") + this.Name + GetString(" 离开了房间"), Color.Crimson);
        this.Teleport(Main.spawnTileX, Main.spawnTileY);
        ((MiniPlayer) this).Leave();
    }

    public int CompareTo(BuildPlayer? other)
    {
        return this.AquiredMarks.CompareTo(other?.AquiredMarks ?? 0);
    }

    public void Creative()
    {
        if (ConfigUtils.config.UnlockAll)
        {
            for (var i = 1; i < ItemID.Count; i++)
            {
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(i, out var amountNeeded);
                Main.LocalPlayerCreativeTracker.ItemSacrifices.RegisterItemSacrifice(i, amountNeeded);
                NetManager.Instance.Broadcast(
                    NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(255, i, amountNeeded)
                );
            }
            return;
        }
        foreach (var key in ConfigUtils.config.Range.Keys)
        {
            for (var j = key; j < ConfigUtils.config.Range[key]; j++)
            {
                CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(j, out var amountNeeded);
                Main.LocalPlayerCreativeTracker.ItemSacrifices.RegisterItemSacrifice(j, amountNeeded);
                NetManager.Instance.Broadcast(
                    NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(255, j, amountNeeded)
                );
            }
        }
    }

    public void UnCreative()
    {
        for (var i = 1; i < ItemID.Count; i++)
        {
            Main.LocalPlayerCreativeTracker.ItemSacrifices.Reset();
            NetManager.Instance.Broadcast(
                NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(255, i, 0)
            );
        }
    }
}