using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using TShockAPI;
using static Terraria.GameContent.Creative.CreativePowers;

namespace MiniGamesAPI
{
    public class MiniPlayer
    {
        [JsonIgnore]
        public TSPlayer Player { get; set; }

        [JsonIgnore]
        public MiniCircle Circle { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public int Assistances { get; set; }

        public int ID { get; set; }

        public string Name { get; set; }

        public bool IsReady { get; set; }

        public bool Visiable { get; set; }

        public int CurrentRoomID { get; set; }

        public int SelectPackID { get; set; }

        [JsonIgnore]
        public MiniPack BackUp { get; set; }

        public global::MiniGamesAPI.Enum.PlayerStatus Status { get; set; }

        [JsonIgnore]
        public Vector2 Position => ((Entity)Player.TPlayer).position;

        public void Ready()
        {
            IsReady = !IsReady;
        }

        public MiniPlayer(int id, TSPlayer player)
        {
            ID = id;
            Name = player.Name;
            Player = player;
            BackUp = null;
            IsReady = false;
            Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
            Kills = 0;
            Deaths = 0;
            Assistances = 0;
            CurrentRoomID = 0;
            SelectPackID = 0;
        }

        public MiniPlayer()
        {
            Kills = 0;
            Deaths = 0;
            Assistances = 0;
            CurrentRoomID = 0;
            SelectPackID = 0;
            Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
            IsReady = false;
            HookManager.GameSecond = (HookManager.GameSecondD)Delegate.Combine(HookManager.GameSecond, new HookManager.GameSecondD(OnGameSecond));
        }

        private void OnGameSecond(GameSecondArgs args)
        {
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("玩家名：" + Name);
            stringBuilder.AppendLine($"击杀数：{Kills}");
            stringBuilder.AppendLine($"死亡数：{Deaths}");
            stringBuilder.AppendLine($"助攻数：{Assistances}");
            stringBuilder.AppendLine($"准备状态：{IsReady}");
            stringBuilder.AppendLine($"房间号：{CurrentRoomID}");
            stringBuilder.AppendLine($"当前状态：{Status}");
            return stringBuilder.ToString();
        }

        public virtual void Teleport(Point point)
        {
            int val = Math.Max(0, point.X);
            int val2 = Math.Max(0, point.Y);
            val = Math.Min(val, Main.maxTilesX - 1);
            val2 = Math.Min(val2, Main.maxTilesY - 1);
            Player.Teleport((float)(val * 16), (float)(val2 * 16 - 48), (byte)1);
        }

        public virtual void Teleport(int x, int y)
        {
            x = Math.Min(x, Main.maxTilesX - 1);
            y = Math.Min(y, Main.maxTilesY - 1);
            Player.Teleport((float)(x * 16), (float)(y * 16 - 48), (byte)1);
        }

        public virtual void SendInfoMessage(string msg)
        {

            Player.SendMessage(msg, Color.DarkTurquoise);
        }

        public virtual void SendSuccessMessage(string msg)
        {

            Player.SendMessage(msg, Color.MediumAquamarine);
        }

        public virtual void SendErrorMessage(string msg)
        {

            Player.SendMessage(msg, Color.Crimson);
        }

        public virtual void SendMessage(string msg, Color color)
        {

            Player.SendMessage(msg, color);
        }

        public virtual void SetBuff(int type, int time = 3600, bool bypass = false)
        {
            Player.SetBuff(type, time, bypass);
        }

        public virtual void SetPVP(bool value)
        {
            Player.TPlayer.hostile = value;
            Player.SendData((PacketTypes)30, "", Player.Index, 0f, 0f, 0f, 0);
            TSPlayer.All.SendData((PacketTypes)30, "", Player.Index, 0f, 0f, 0f, 0);
        }

        public virtual void SetTeam(int id)
        {
            if (Player != null)
            {
                Player.SetTeam(id);
            }
        }

        public virtual void RestorePlayerInv(MiniPack pack)
        {
            pack.RestoreCharacter(Player);
        }

        public virtual void DropItem(int itemID)
        {
            int num = Player.TPlayer.FindItem(itemID);
            Item val = Player.TPlayer.inventory[num];
            int num2 = Item.NewItem((IEntitySource)new EntitySource_DebugCommand(), ((Entity)Player.TPlayer).position, ((Entity)Player.TPlayer).width, ((Entity)Player.TPlayer).height, val.type, val.stack, true, (int)val.prefix, false, false);
            TSPlayer.All.SendData((PacketTypes)21, "", num2, 0f, 0f, 0f, 0);
            val.netID = 0;
            TSPlayer.All.SendData((PacketTypes)5, val.Name, Player.Index, (float)num, (float)(int)val.prefix, 0f, 0);
        }

        public virtual bool CheckContainItem(int netid)
        {
            return Player.TPlayer.HasItem(netid);
        }

        public float KDA()
        {
            float num = Kills;
            float num2 = Deaths;
            float num3 = Assistances;
            if (num == 0f)
            {
                num = 1f;
            }
            if (num2 == 0f)
            {
                num2 = 1f;
            }
            if (num3 == 0f)
            {
                num3 = 1f;
            }
            return (num + num3) / num2;
        }

        public Vector2 ToAnotherPlayer(MiniPlayer player)
        {
            return player.Position - Position;
        }

        public bool ClearRecord()
        {
            Kills = 0;
            Deaths = 0;
            Assistances = 0;
            return true;
        }

        public void SendBoardMsg(string info)
        {
            Player.SendData((PacketTypes)9, info, 0, 0f, 0f, 0f, 0);
        }

        public void Godmode(bool state)
        {
            Player.GodMode = state;
            ((APerPlayerTogglePower) CreativePowerManager.Instance.GetPower< CreativePowers.GodmodePower >()).SetEnabledState(Player.Index, Player.GodMode);
        }

        public int FindItem(int netid)
        {
            return Player.TPlayer.FindItem(netid);
        }

        public void RemoveItem(int netid)
        {
            int num = FindItem(netid);
            Player.TPlayer.inventory[num].netDefaults(0);
            Player.SendData((PacketTypes)5, "", Player.Index, (float)num, (float)(int)Player.TPlayer.inventory[num].prefix, 0f, 0);
        }

        public void Firework(int num)
        {
            int num2 = 0;
            switch (num)
            {
                case 1:
                    num2 = 167;
                    break;
                case 2:
                    num2 = 169;
                    break;
                case 3:
                    num2 = 168;
                    break;
                case 4:
                    num2 = 418;
                    break;
            }
            int num3 = Projectile.NewProjectile((IEntitySource)new EntitySource_DebugCommand(), ((Entity)Player.TPlayer).position.X, ((Entity)Player.TPlayer).position.Y - 64f, 0f, -8f, num2, 0, 0f, 255, 0f, 0f);
            TSPlayer.All.SendData((PacketTypes)27, "", num3, 0f, 0f, 0f, 0);
        }

        public void Join(int roomid)
        {
            CurrentRoomID = roomid;
            BackUp = new MiniPack(Name, 1);
            BackUp.CopyFromPlayer(Player);
        }

        public void Leave()
        {
            CurrentRoomID = 0;
            BackUp.RestoreCharacter(Player);
            BackUp = null;
            Player.SaveServerCharacter();
            SelectPackID = 0;
            IsReady = false;
            Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
            SendBoardMsg("");
            ClearRecord();
        }

        public void SetDifficulty(byte flag)
        {
            Player.TPlayer.difficulty = flag;
            TSPlayer.All.SendData((PacketTypes)4, "", Player.Index, 0f, 0f, 0f, 0);
        }
    }
}
