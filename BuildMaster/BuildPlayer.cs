using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TShockAPI;
using MiniGamesAPI;

namespace MainPlugin
{
    public class BuildPlayer : MiniPlayer, IComparable<BuildPlayer>
    {
        public int AquiredMarks { get; set; }

        public MiniRegion CurrentRegion { get; set; }

        public int GiveMarks { get; set; }

        public string SelectedTopic { get; set; }

        public bool Locked { get; set; }

        public bool Marked { get; set; }

        public BuildPlayer(int id, string name, TSPlayer player)
        {
            ((MiniPlayer)this).ID = id;
            ((MiniPlayer)this).Name = name;
            ((MiniPlayer)this).Player = player;
            ((MiniPlayer)this).CurrentRoomID = 0;
            CurrentRegion = null;
            Locked = false;
            Marked = false;
            AquiredMarks = 0;
        }

        public void Join(BuildRoom room)
        {
            if (room.GetPlayerCount() >= ((MiniRoom)room).MaxPlayer)
            {
                ((MiniPlayer)this).SendInfoMessage("此房间满人了");
                return;
            }
            if ((int)((MiniRoom)room).Status != 0)
            {
                ((MiniPlayer)this).SendInfoMessage("该房间状态无法加入游戏");
                return;
            }
            if (ConfigUtils.GetRoomByID(((MiniPlayer)this).CurrentRoomID) != null)
            {
                Leave();
            }
            ((MiniPlayer)this).Join(((MiniRoom)room).ID);
            room.Players.Add(this);
            room.Broadcast("玩家 " + ((MiniPlayer)this).Name + " 加入了房间", Color.MediumAquamarine);
            ((MiniPlayer)this).Teleport(room.WaitingPoint);
            if (!room.Loaded)
            {
                room.LoadRegion();
            }
        }

        public void Leave()
        {
            BuildRoom roomByID = ConfigUtils.GetRoomByID(((MiniPlayer)this).CurrentRoomID);
            if (roomByID == null)
            {
                ((MiniPlayer)this).SendInfoMessage("房间不存在");
                return;
            }
            if ((int)((MiniRoom)roomByID).Status != 0)
            {
                ((MiniPlayer)this).SendInfoMessage("该房间状态无法离开游戏");
                return;
            }
            roomByID.Players.Remove(this);
            roomByID.Broadcast("玩家 " + ((MiniPlayer)this).Name + " 离开了房间", Color.Crimson);
            ((MiniPlayer)this).Teleport(Main.spawnTileX, Main.spawnTileY);
            ((MiniPlayer)this).Leave();
        }

        public int CompareTo(BuildPlayer other)
        {
            return other.AquiredMarks.CompareTo(AquiredMarks);
        }

        public void Creative()
        {
            if (ConfigUtils.config.UnlockAll)
            {
                for (int i = 1; i <= 5042; i++)
                {
                    NetPacket val = NetCreativeUnlocksModule.SerializeItemSacrifice(i, 999);
                    NetManager.Instance.SendToClient(val, ((MiniPlayer)this).Player.Index);
                }
                return;
            }
            foreach (int key in ConfigUtils.config.Range.Keys)
            {
                for (int j = key; j < ConfigUtils.config.Range[key]; j++)
                {
                    NetPacket val2 = NetCreativeUnlocksModule.SerializeItemSacrifice(j, 999);
                    NetManager.Instance.SendToClient(val2, ((MiniPlayer)this).Player.Index);
                }
            }
        }

        public void UnCreative()
        {
            for (int i = 1; i <= 5042; i++)
            {
                NetPacket val = NetCreativeUnlocksModule.SerializeItemSacrifice(i, 0);
                NetManager.Instance.SendToClient(val, ((MiniPlayer)this).Player.Index);
            }
        }
    }
}
