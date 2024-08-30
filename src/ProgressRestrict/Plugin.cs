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
    public override string Description => "根据进度限制";
    public override string Name => "超进度限制";
    public override Version Version => new(1, 0, 0, 1);

    public Config config;

    private readonly bool[] RestrictedProjectiles = new bool[ProjectileID.Count];
    private readonly bool[] RestrictedItems = new bool[ItemID.Count];
    private readonly bool[] RestrictedBuffs = new bool[BuffID.Count];

    public string path = Path.Combine(TShock.SavePath, "超进度检测.json");

    public Plugin(Main game) : base(game)
    {
        config = Config.LoadConfig(path);
    }
    public override void Initialize()
    {
        config = Config.LoadConfig(path);
        GetDataHandlers.NewProjectile.Register(OnProjectile);
        GetDataHandlers.PlayerSlot.Register(OnSlot);
        GetDataHandlers.PlayerBuff.Register(OnBuff);
        ServerApi.Hooks.GamePostInitialize.Register(this, UpdateRestricted2);
        GeneralHooks.ReloadEvent += Reload;
        DataSync.Plugin.OnProgressChanged += UpdateRestricted;
    }

    private void UpdateRestricted2(EventArgs args)
    {
        UpdateRestricted();
    }

    private void Reload(ReloadEventArgs e)
    {
        config = Config.LoadConfig(path);
        UpdateRestricted();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.NewProjectile.UnRegister(OnProjectile);
            GetDataHandlers.PlayerSlot.UnRegister(OnSlot);
            GetDataHandlers.PlayerBuff.UnRegister(OnBuff);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, UpdateRestricted2);
            GeneralHooks.ReloadEvent -= Reload;
            DataSync.Plugin.OnProgressChanged -= UpdateRestricted;
        }
        base.Dispose(disposing);
    }

    private void OnBuff(object? sender, GetDataHandlers.PlayerBuffEventArgs e)
    {
        if (e.Player.HasPermission("progress.buff.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }
        if (RestrictedBuffs[e.Type])
        {
            if (config.Broadcast)
            {
                TShock.Utils.Broadcast($"玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Type)} ,已清除!", Microsoft.Xna.Framework.Color.Red);
            }

            if (config.WriteLog)
            {
                TShock.Log.Info($"[ProgressBuff]: 玩家 {e.Player.Name} 拥有超进度buff {TShock.Utils.GetBuffName(e.Type)} ,已清除!");
            }

            if (config.ClearBuff)
            {
                e.Handled = true;
                e.Player.SendData(PacketTypes.PlayerBuff, "", e.ID);
            }
        }
    }

    private void UpdateRestricted()
    {
        Array.Fill(RestrictedItems, false);
        Array.Fill(RestrictedProjectiles, false);
        Array.Fill(RestrictedBuffs, false);

        foreach (var f in config.Restrictions)
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
                RestrictedItems[i] = true;
            }
            foreach (var i in f.RestrictedProjectiles)
            {
                RestrictedProjectiles[i] = true;
            }
            foreach (var i in f.RestrictedBuffs)
            {
                RestrictedBuffs[i] = true;
            }
        }
    }

    private void OnSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        if (e.Player.HasPermission("progress.item.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (RestrictedItems[e.Type])
        {
            if (config.PunishPlayer)
            {
                e.Player.SetBuff(156, 60 * config.PunishTime, false);
            }
            e.Player.SendErrorMessage($"检测到超进度物品{TShock.Utils.GetItemById(e.Type).Name}!");
            if (config.Broadcast)
            {
                TShock.Utils.Broadcast($"检测到{e.Player.Name}拥有超进度物品{TShock.Utils.GetItemById(e.Type).Name}!", Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (config.WriteLog)
            {
                TShock.Log.Write($"[超进度物品限制] 玩家{e.Player.Name} 在背包第{e.Slot}格检测到超进度物品 {TShock.Utils.GetItemById(e.Type).Name} x{e.Stack}", System.Diagnostics.TraceLevel.Info);
            }
            if (config.ClearItem)
            {
                e.Stack = 0;
                TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", e.Player.Index, e.Slot);
            }
            if (config.KickPlayer)
            {
                e.Player.Kick("拥有超进度物品");
            }
        }
    }


    private void OnProjectile(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.HasPermission("progress.projecttile.white") || e.Handled || !e.Player.IsLoggedIn)
        {
            return;
        }

        if (RestrictedProjectiles[e.Type])
        {
            if (config.PunishPlayer)
            {
                e.Player.SetBuff(156, 60 * config.PunishTime, false);
            }
            e.Player.SendErrorMessage($"检测到超进度弹幕{Lang.GetProjectileName(e.Type).Value}!");
            if (config.Broadcast)
            {
                TShock.Utils.Broadcast($"检测到{e.Player.Name}使用超进度弹幕{Lang.GetProjectileName(e.Type).Value}!", Microsoft.Xna.Framework.Color.DarkRed);
            }
            if (config.WriteLog)
            {
                TShock.Log.Write($"[超进度弹幕限制] 玩家{e.Player.Name} 使用超进度弹幕 {Lang.GetProjectileName(e.Type).Value} ID =>{e.Type}", System.Diagnostics.TraceLevel.Info);
            }
            if (config.ClearItem)
            {
                Main.projectile[e.Index].active = false;
                Main.projectile[e.Index].type = 0;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);
            }

            if (config.KickPlayer)
            {
                e.Player.Kick("使用超进度弹幕");
            }
        }
    }
}