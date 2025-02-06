using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using TShockAPI;

namespace MiniGamesAPI;

public class MiniPlayer
{
    [JsonIgnore]
    public TSPlayer Player { get; set; }

    [JsonIgnore]
    public MiniCircle? Circle { get; set; }

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
    public Vector2 Position => this.Player.TPlayer.position;

    public void Ready()
    {
        this.IsReady = !this.IsReady;
    }

    public MiniPlayer(int id, TSPlayer player)
    {
        this.ID = id;
        this.Name = player.Name;
        this.Player = player;
        this.BackUp = null!;
        this.IsReady = false;
        this.Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
        this.Kills = 0;
        this.Deaths = 0;
        this.Assistances = 0;
        this.CurrentRoomID = 0;
        this.SelectPackID = 0;
    }

    public MiniPlayer()
    {
        this.Name = null!;
        this.Player = null!;
        this.BackUp = null!;

        this.Kills = 0;
        this.Deaths = 0;
        this.Assistances = 0;
        this.CurrentRoomID = 0;
        this.SelectPackID = 0;
        this.Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
        this.IsReady = false;
        HookManager.GameSecond = (HookManager.GameSecondD) Delegate.Combine(HookManager.GameSecond, new HookManager.GameSecondD(this.OnGameSecond));
    }

    private void OnGameSecond(GameSecondArgs args)
    {
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(GetString($"玩家名：{this.Name}"));
        stringBuilder.AppendLine(GetString($"击杀数：{this.Kills}"));
        stringBuilder.AppendLine(GetString($"死亡数：{this.Deaths}"));
        stringBuilder.AppendLine(GetString($"助攻数：{this.Assistances}"));
        stringBuilder.AppendLine(GetString($"准备状态：{this.IsReady}"));
        stringBuilder.AppendLine(GetString($"房间号：{this.CurrentRoomID}"));
        stringBuilder.AppendLine(GetString($"当前状态：{this.Status}"));
        return stringBuilder.ToString();
    }

    public virtual void Teleport(Point point)
    {
        var val = Math.Max(0, point.X);
        var val2 = Math.Max(0, point.Y);
        val = Math.Min(val, Main.maxTilesX - 1);
        val2 = Math.Min(val2, Main.maxTilesY - 1);
        this.Player.Teleport(val * 16, (val2 * 16) - 48, 1);
    }

    public virtual void Teleport(int x, int y)
    {
        x = Math.Min(x, Main.maxTilesX - 1);
        y = Math.Min(y, Main.maxTilesY - 1);
        this.Player.Teleport(x * 16, (y * 16) - 48, 1);
    }

    public virtual void SendInfoMessage(string msg)
    {

        this.Player.SendMessage(msg, Color.DarkTurquoise);
    }

    public virtual void SendSuccessMessage(string msg)
    {

        this.Player.SendMessage(msg, Color.MediumAquamarine);
    }

    public virtual void SendErrorMessage(string msg)
    {

        this.Player.SendMessage(msg, Color.Crimson);
    }

    public virtual void SendMessage(string msg, Color color)
    {

        this.Player.SendMessage(msg, color);
    }

    public virtual void SetBuff(int type, int time = 3600, bool bypass = false)
    {
        this.Player.SetBuff(type, time, bypass);
    }

    public virtual void SetPVP(bool value)
    {
        this.Player.TPlayer.hostile = value;
        this.Player.SendData((PacketTypes) 30, "", this.Player.Index, 0f, 0f, 0f, 0);
        TSPlayer.All.SendData((PacketTypes) 30, "", this.Player.Index, 0f, 0f, 0f, 0);
    }

    public virtual void SetTeam(int id)
    {
        this.Player?.SetTeam(id);
    }

    public virtual void RestorePlayerInv(MiniPack pack)
    {
        pack.RestoreCharacter(this.Player);
    }

    public virtual void DropItem(int itemID)
    {
        var num = this.Player.TPlayer.FindItem(itemID);
        var val = this.Player.TPlayer.inventory[num];
        var num2 = Item.NewItem(new EntitySource_DebugCommand(), this.Player.TPlayer.position, this.Player.TPlayer.width, this.Player.TPlayer.height, val.type, val.stack, true, val.prefix, false, false);
        TSPlayer.All.SendData((PacketTypes) 21, "", num2, 0f, 0f, 0f, 0);
        val.netID = 0;
        TSPlayer.All.SendData((PacketTypes) 5, val.Name, this.Player.Index, num, val.prefix, 0f, 0);
    }

    public virtual bool CheckContainItem(int netid)
    {
        return this.Player.TPlayer.HasItem(netid);
    }

    public float KDA()
    {
        float num = this.Kills;
        float num2 = this.Deaths;
        float num3 = this.Assistances;
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
        return player.Position - this.Position;
    }

    public bool ClearRecord()
    {
        this.Kills = 0;
        this.Deaths = 0;
        this.Assistances = 0;
        return true;
    }

    public void SendBoardMsg(string info)
    {
        this.Player.SendData((PacketTypes) 9, info, 0, 0f, 0f, 0f, 0);
    }

    public void Godmode(bool state)
    {
        this.Player.GodMode = state;
        CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(this.Player.Index, this.Player.GodMode);
    }

    public int FindItem(int netid)
    {
        return this.Player.TPlayer.FindItem(netid);
    }

    public void RemoveItem(int netid)
    {
        var num = this.FindItem(netid);
        this.Player.TPlayer.inventory[num].netDefaults(0);
        this.Player.SendData((PacketTypes) 5, "", this.Player.Index, num, this.Player.TPlayer.inventory[num].prefix, 0f, 0);
    }

    public void Firework(int num)
    {
        var num2 = 0;
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
        var num3 = Projectile.NewProjectile(new EntitySource_DebugCommand(), this.Player.TPlayer.position.X, this.Player.TPlayer.position.Y - 64f, 0f, -8f, num2, 0, 0f, 255, 0f, 0f);
        TSPlayer.All.SendData((PacketTypes) 27, "", num3, 0f, 0f, 0f, 0);
    }

    public void Join(int roomid)
    {
        this.CurrentRoomID = roomid;
        this.BackUp = new MiniPack(this.Name, 1);
        this.BackUp.CopyFromPlayer(this.Player);
    }

    public void Leave()
    {
        this.CurrentRoomID = 0;
        this.BackUp.RestoreCharacter(this.Player);
        this.BackUp = null!;
        this.Player.SaveServerCharacter();
        this.SelectPackID = 0;
        this.IsReady = false;
        this.Status = global::MiniGamesAPI.Enum.PlayerStatus.Waiting;
        this.SendBoardMsg("");
        this.ClearRecord();
    }

    public void SetDifficulty(byte flag)
    {
        this.Player.TPlayer.difficulty = flag;
        TSPlayer.All.SendData((PacketTypes) 4, "", this.Player.Index, 0f, 0f, 0f, 0);
    }
}