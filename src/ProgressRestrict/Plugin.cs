using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ProgressRestrict;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命 & 恋恋魔改三合一";
    public override string Description => GetString("根据进度限制");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 5);

    public Config config = null!;

    private readonly bool[] RestrictedProjectiles = new bool[ProjectileID.Count];
    private readonly bool[] RestrictedItems = new bool[ItemID.Count];
    private readonly bool[] RestrictedBuffs = new bool[BuffID.Count];

    public string path = Path.Combine(TShock.SavePath, "超进度检测.json");

    public Plugin(Main game) : base(game)
    {
    }
    public override void Initialize()
    {
        this.config = Config.LoadConfig(this.path);
        GetDataHandlers.NewProjectile.Register(this.OnProjectile);
        GetDataHandlers.PlayerSlot.Register(this.OnSlot);
        GetDataHandlers.PlayerBuff.Register(this.OnBuff);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.UpdateRestricted2);
        GeneralHooks.ReloadEvent += this.Reload;
        DataSync.Plugin.OnProgressChanged += this.UpdateRestricted;
    }

    private void UpdateRestricted2(EventArgs args)
    {
        this.UpdateRestricted();
    }

    private void Reload(ReloadEventArgs e)
    {
        this.config = Config.LoadConfig(this.path);
        this.UpdateRestricted();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.NewProjectile.UnRegister(this.OnProjectile);
            GetDataHandlers.PlayerSlot.UnRegister(this.OnSlot);
            GetDataHandlers.PlayerBuff.UnRegister(this.OnBuff);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.UpdateRestricted2);
            GeneralHooks.ReloadEvent -= this.Reload;
            DataSync.Plugin.OnProgressChanged -= this.UpdateRestricted;
        }
        base.Dispose(disposing);
    }

    private void OnBuff(object? sender, GetDataHandlers.PlayerBuffEventArgs e)
    {
        if (e.Player.HasPermission("progress.buff.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }
        if (this.RestrictedBuffs[e.Type])
        {
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast(GetString($"玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Type)} ,已清除!"), Microsoft.Xna.Framework.Color.Red);
            }

            if (this.config.WriteLog)
            {
                TShock.Log.Info(GetString($"[ProgressBuff]: 玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Type)} ,已清除!"));
            }

            if (this.config.ClearBuff)
            {
                e.Handled = true;
                e.Player.SendData(PacketTypes.PlayerBuff, "", e.ID);
            }
        }
    }

    private void UpdateRestricted()
    {
        Array.Fill(this.RestrictedItems, false);
        Array.Fill(this.RestrictedProjectiles, false);
        Array.Fill(this.RestrictedBuffs, false);

        foreach (var f in this.config.Restrictions)
        {
            if (f.AllowRemoteUnlocked && DataSync.Plugin.SyncedProgress.TryGetValue(f.Progress, out var rv) && rv)
            {
                continue;
            }

            if (DataSync.Plugin.LocalProgress.TryGetValue(f.Progress, out var lv) && lv)
            {
                continue;
            }

            foreach (var i in f.RestrictedItems)
            {
                this.RestrictedItems[i] = true;
            }
            foreach (var i in f.RestrictedProjectiles)
            {
                this.RestrictedProjectiles[i] = true;
            }
            foreach (var i in f.RestrictedBuffs)
            {
                this.RestrictedBuffs[i] = true;
            }
        }
    }

    private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        if (e.Player.HasPermission("progress.item.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (this.RestrictedItems[e.Type])
        {
            if (this.config.PunishPlayer)
            {
                e.Player.SetBuff(156, 60 * this.config.PunishTime, false);
            }
            e.Player.SendErrorMessage(GetString($"检测到超进度物品{TShock.Utils.GetItemById(e.Type).Name}!"));
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast(GetString($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(e.Type).Name}!"), Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (this.config.WriteLog)
            {
                TShock.Log.Write(GetString($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{e.Slot}格检测到超进度物品 {TShock.Utils.GetItemById(e.Type).Name} x{e.Stack}"), System.Diagnostics.TraceLevel.Info);
            }
            if (this.config.ClearItem)
            {
                e.Stack = 0;
                TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, e.Slot);
            }
            if (this.config.KickPlayer)
            {
                e.Player.Kick(GetString("拥有超进度物品"));
            }
        }
    }


    private void OnProjectile(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.HasPermission("progress.projecttile.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (this.RestrictedProjectiles[e.Type])
        {
            if (this.config.PunishPlayer)
            {
                e.Player.SetBuff(156, 60 * this.config.PunishTime, false);
            }
            e.Player.SendErrorMessage(GetString($"检测到超进度弹幕{Lang.GetProjectileName(e.Type).Value}!"));
            if (this.config.Broadcast)
            {
                TShock.Utils.Broadcast(GetString($"检测到{e.Player.Name}使用超进度弹幕{Lang.GetProjectileName(e.Type).Value}!"), Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (this.config.WriteLog)
            {
                TShock.Log.Write(GetString($"[超进度弹幕限制] 玩家{e.Player.Name} 使用超进度弹幕 {Lang.GetProjectileName(e.Type).Value} ID =>{e.Type}"), System.Diagnostics.TraceLevel.Info);
            }
            if (this.config.ClearItem)
            {
                Main.projectile[e.Index].active = false;
                Main.projectile[e.Index].type = 0;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);
            }

            if (this.config.KickPlayer)
            {
                e.Player.Kick(GetString("使用超进度弹幕"));
            }
        }
    }
}