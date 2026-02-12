using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static SurfaceBlock.Tool;

namespace SurfaceBlock;

[ApiVersion(2, 1)]
public class SurfaceBlock : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学 Cai 西江小子 熙恩";
    public override string Description => GetString("禁止特定弹幕在地表产生");
    public override Version Version => new Version(2, 0, 0, 3);
    #endregion

    #region 注册与卸载钩子
    public SurfaceBlock(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.TileEdit += this.OnTileEdit!;
        GetDataHandlers.NewProjectile += this.ProjectNew!;
        GetDataHandlers.PlayerUpdate += this.OnPlayerUpdate!;
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.TileEdit -= this.OnTileEdit!;
            GetDataHandlers.NewProjectile -= this.ProjectNew!;
            GetDataHandlers.PlayerUpdate -= this.OnPlayerUpdate!;
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 玩家进服自动建数据
    internal static MyData Mydata = new();
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        if (!Configuration.Instance.Enabled || plr == null)
        {
            return;
        }

        // 如果玩家不在数据表中，则创建新数据
        if (!Mydata.Dict!.ContainsKey(plr.Name))
        {
            Mydata.Dict.Add(plr.Name, new MyData.PlayerData());
        }
    }
    #endregion

    #region 弹幕更新时触发消除方法（计算世界大小）
    public void ProjectNew(object sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        var plr = e.Player;
        if (!Configuration.Instance.Enabled || plr == null ||
            Configuration.Instance.ClearTable == null ||
            plr.HasPermission("SurfaceBlock") ||
            plr.HasPermission("免检地表弹幕"))
        {
            return;
        }

        if (Configuration.Instance.ClearTable.Contains(e.Type))
        {
            if (!Main.remixWorld) //正常种子
            {
                if (e.Position.Y < Main.worldSurface * 16)
                {
                    Remover(e);
                }
            }
            else //颠倒
            {
                if (GetWorldSize() == 3)
                {
                    RemixWorld(e, 54.5);
                }

                if (GetWorldSize() == 2)
                {
                    RemixWorld(e, 48.5);
                }

                if (GetWorldSize() == 1)
                {
                    RemixWorld(e, 40.0);
                }
            }
        }
    }
    #endregion

    #region 移除弹幕方法（并开启标识提供给其他方法作为参考使用）
    public static void Remover(GetDataHandlers.NewProjectileEventArgs e)
    {
        var now = DateTime.UtcNow;
        if (Mydata.Dict != null && Mydata.Dict.TryGetValue(e.Player.Name, out var data))
        {
            var name = (string) Terraria.Lang.GetProjectileName(e.Type);
            if (name.StartsWith("ProjectileName."))
            {
                name = GetString("未知");
            }

            Main.projectile[e.Index].type = 0;
            Main.projectile[e.Index].frame = 0;
            Main.projectile[e.Index].timeLeft = -1;
            Main.projectile[e.Index].active = false;

            if (Configuration.Instance.Mess)
            {
                TShock.Utils.Broadcast(GetString($"玩家 [c/4EA4F2:{e.Player.Name}] 使用禁地表弹幕 [c/15EDDB:{name}] 已清除!"), 240, 255, 150);
            }

            TSPlayer.All.RemoveProjectile(e.Index, e.Owner);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", e.Index);

            //开启销毁标识 记录当前销毁弹幕时间给PlayerUpdate TileEdit ItemDorp 3个方法用
            data.Enabled = true;
            data.Time = now;
        }
    }
    #endregion

    #region 玩家移动触发销毁方法(并用于关闭销毁标识)
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (plr == null || !plr.IsLoggedIn || !plr.Active || !Configuration.Instance.Enabled ||
           plr.HasPermission("SurfaceBlock") || Configuration.Instance.ClearTable == null ||
           plr.HasPermission("免检地表弹幕"))
        {
            return;
        }

        if (Mydata.Dict != null && Mydata.Dict.TryGetValue(plr.Name, out var data))
        {
            if (data.Enabled)
            {
                for (var i = 0; i < Main.projectile.Length; i++)
                {
                    var proj = Main.projectile[i];

                    if (Configuration.Instance.ClearTable.Contains(proj.type))
                    {
                        if (Configuration.Instance.ItemDorp)
                        {
                            ItemDorp(plr);
                        }

                        proj.type = 0;
                        proj.frame = 0;
                        proj.timeLeft = -1;
                        proj.active = false;
                        TSPlayer.All.RemoveProjectile(i, proj.owner);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", i);
                    }
                }

                //只有在超过了Config指定秒数再关闭，避免频繁发射弹幕不断开关影响update性能
                if ((DateTime.UtcNow - data.Time).TotalSeconds >= Configuration.Instance.Seconds)
                {
                    data.Enabled = false;
                }
            }
        }
    }
    #endregion

    #region 手持物品掉落方法
    private static void ItemDorp(TSPlayer plr)
    {
        var item = TShock.Utils.GetItemById(plr.SelectedItem.type);
        var stack = plr.SelectedItem.stack;
        var MyItem = Item.NewItem(null, (int) plr.X, (int) plr.Y, item.width, item.height, item.type, stack);
        if (MyItem >= 0 && MyItem < Main.item.Length)
        {
            var newItem = Main.item[MyItem];
            newItem.wet = Collision.WetCollision(newItem.position, item.width, item.height);
            newItem.playerIndexTheItemIsReservedFor = plr.Index;
            if (plr.TPlayer.selectedItem >= 0 && plr.TPlayer.selectedItem < plr.TPlayer.inventory.Length)
            {
                plr.TPlayer.inventory[plr.TPlayer.selectedItem].SetDefaults(0);
                NetMessage.SendData(5, -1, -1, null, plr.Index, plr.TPlayer.selectedItem);
            }

            plr.SendData(PacketTypes.PlayerSlot, null, MyItem);
            plr.SendData(PacketTypes.UpdateItemDrop, null, MyItem);
            plr.SendData(PacketTypes.ItemOwner, null, MyItem);
            plr.SendData(PacketTypes.TweakItem, null, MyItem, 255f, 63f);
        }
    }
    #endregion

    #region 恢复被破坏的图格方法
    private static readonly Dictionary<(int, int), Tile> Orig = new Dictionary<(int, int), Tile>();
    private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
    {
        var plr = args.Player;
        if (plr == null || !plr.IsLoggedIn || !plr.Active ||
            !Configuration.Instance.Enabled || !Configuration.Instance.KillTile)
        {
            return;
        }

        if (Mydata.Dict != null && Mydata.Dict.TryGetValue(plr.Name, out var data))
        {
            if (data.Enabled)
            {
                //获取图格回滚大小
                GetRollbackSize(args.X, args.Y, out var width, out var length, out var offY);
                var x = (short) (args.X - width);
                var y = (short) (args.Y + offY);
                var w = (byte) (width * 2);
                var h = (byte) (length + 1);

                TSPlayer.All.SendTileRect(x, y, w, h);
                args.Handled = true;
            }
        }
    }
    #endregion
}