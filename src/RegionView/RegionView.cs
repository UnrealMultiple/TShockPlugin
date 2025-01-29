using Microsoft.Xna.Framework;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace RegionView;

[ApiVersion(2, 1)]
public class RegionView : TerrariaPlugin
{
    public const int NearRange = 100;

    public List<RegionPlayer> Players { get; } = new();

    public static Color[] TextColors { get; } = new[]
    {
        new Color(244,  93,  93),
        new Color(244, 169,  93),
        new Color(244, 244,  93),
        new Color(169, 244,  93),
        new Color( 93, 244,  93),
        new Color( 93, 244, 169),
        new Color( 93, 244, 244),
        new Color( 93, 169, 244),
        new Color( 93,  93, 244),
        new Color(169,  93, 244),
        new Color(244,  93, 244),
        new Color(244,  93, 169)
    };

    public override string Author => "TBC开发者团队,肝帝熙恩汉化";

    public override string Description => GetString("为地区添加区域边界视图。");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 3);

    private readonly System.Timers.Timer _refreshTimer = new(5000);

    public RegionView(Main game)
        : base(game)
    {
        this.Order = 1;
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("regionvision.regionview", this.CommandView, "regionview", "rv")
        {
            AllowServer = false,
            HelpText = GetString("显示指定区域的边界")
        });

        Commands.ChatCommands.Add(new Command("regionvision.regionview", this.CommandClear, "regionclear", "rc")
        {
            AllowServer = false,
            HelpDesc = new string[]
            {
                GetString("用法: /rc"),
                GetString("从您的视图中移除所有区域显示")
            }
        });

        Commands.ChatCommands.Add(new Command("regionvision.regionviewnear", this.CommandViewNearby, "regionviewnear", "rvn")
        {
            AllowServer = false,
            HelpText = GetString("开启或关闭自动显示您附近的区域")
        });

        GetDataHandlers.TileEdit += HandlerList<GetDataHandlers.TileEditEventArgs>.Create(this.OnTileEdit!, HandlerPriority.High, false);

        ServerApi.Hooks.ServerJoin.Register(this, this.OnPlayerJoin);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnPlayerLeave);

        PlayerHooks.PlayerCommand += this.OnPlayerCommand;
        RegionHooks.RegionCreated += this.RegionCreated;
        RegionHooks.RegionDeleted += this.RegionDeleted;

        this._refreshTimer.AutoReset = false;

        this._refreshTimer.Elapsed += (x, _) => this.RefreshRegions();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.CommandView || c.CommandDelegate == this.CommandClear || c.CommandDelegate == this.CommandViewNearby);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnPlayerJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnPlayerLeave);
            PlayerHooks.PlayerCommand -= this.OnPlayerCommand;
            RegionHooks.RegionCreated -= this.RegionCreated;
            RegionHooks.RegionDeleted -= this.RegionDeleted;
            this._refreshTimer.Elapsed -= (x, _) => this.RefreshRegions();
        }
        base.Dispose(disposing);
    }

    private void RegionDeleted(RegionHooks.RegionDeletedEventArgs args)
    {
        if (args.Region.WorldID != Main.worldID.ToString())
        {
            return;
        }

        // If any players were viewing this region, clear its border.
        lock (this.Players)
        {
            foreach (var player in this.Players)
            {
                for (var i = 0; i < player.Regions.Count; i++)
                {
                    var region = player.Regions[i];
                    if (region.Name.Equals(args.Region.Name))
                    {
                        player.TSPlayer.SendMessage(GetString($"区域显示 {region.Name} 已被删除。"), TextColors[region.Color - 13]);
                        region.Refresh(player.TSPlayer);
                        player.Regions.RemoveAt(i);

                        foreach (var region2 in player.Regions)
                        {
                            region2.SetFakeTiles();
                        }

                        foreach (var region2 in player.Regions)
                        {
                            region2.Refresh(player.TSPlayer);
                        }

                        foreach (var region2 in player.Regions.Reverse<Region>())
                        {
                            region2.UnsetFakeTiles();
                        }

                        break;
                    }
                }
            }
        }
    }

    private void RegionCreated(RegionHooks.RegionCreatedEventArgs args)
    {
        this._refreshTimer.Stop();
        this.RefreshRegions();
    }

    public RegionPlayer? FindPlayer(int index)
    {
        return this.Players.FirstOrDefault(p => p.Index == index);
    }

    private void CommandView(CommandArgs args)
    {
        TShockAPI.DB.Region? tRegion = null;
        var matches = new List<TShockAPI.DB.Region>();

        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("用法: /regionview <区域名称>"));
            return;
        }

        // Find the specified region.
        for (var pass = 1; pass <= 3 && tRegion == null && matches.Count == 0; pass++)
        {
            foreach (var _tRegion in TShock.Regions.Regions)
            {
                switch (pass)
                {
                    case 1:  // Pass 1: exact match
                        if (_tRegion.Name == args.Parameters[0])
                        {
                            tRegion = _tRegion;
                            break;
                        }
                        else if (_tRegion.Name.Equals(args.Parameters[0], StringComparison.OrdinalIgnoreCase))
                        {
                            matches.Add(_tRegion);
                        }

                        break;
                    case 2:  // Pass 2: case-sensitive partial match
                        if (_tRegion.Name.StartsWith(args.Parameters[0]))
                        {
                            matches.Add(_tRegion);
                        }

                        break;
                    case 3:  // Pass 3: case-insensitive partial match
                        if (_tRegion.Name.StartsWith(args.Parameters[0], StringComparison.OrdinalIgnoreCase))
                        {
                            matches.Add(_tRegion);
                        }

                        break;
                }
                if (tRegion != null)
                {
                    break;
                }
            }
        }

        if (tRegion == null)
        {
            if (matches.Count == 1)
            {
                tRegion = matches[0];
            }
            else if (matches.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("没有找到这样的区域。"));
                return;
            }
            else if (matches.Count > 5)
            {
                args.Player.SendErrorMessage(GetString("找到了多个匹配的区域：{0} 等{1}个更多。请更具体一些。"), string.Join(", ", matches.Take(5).Select(r => r.Name)), matches.Count - 5);
                return;
            }
            else if (matches.Count > 1)
            {
                args.Player.SendErrorMessage(GetString("找到了多个匹配的区域：{0}。请更具体一些。"), string.Join(", ", matches.Select(r => r.Name)));
                return;
            }
        }

        if (tRegion!.Area.Width < 0 || tRegion.Area.Height < 0)
        {
            args.Player.SendErrorMessage(GetString("区域 {0} 不包含任何图块。 (找到的尺寸: {1} × {2})\n使用 [c/FF8080:/region resize] 来修复它。"), tRegion.Name, tRegion.Area.Width, tRegion.Area.Height);
            return;
        }

        lock (this.Players)
        {
            var player = this.FindPlayer(args.Player.Index);

            if (player == null)
            {
                return;
            }

            // Register this region.
            var region = player.Regions.FirstOrDefault(r => r.Name == tRegion.Name);

            if (region == null)
            {
                region = new Region(tRegion.Name, tRegion.Area);
            }
            else
            {
                player.Regions.Remove(region);
            }

            foreach (var _region in player.Regions)
            {
                _region.SetFakeTiles();
            }

            if (region.ShowArea != region.Area)
            {
                region.Refresh(player.TSPlayer);
            }

            player.Regions.Add(region);

            region.CalculateArea(args.Player);
            region.SetFakeTiles();
            region.Refresh(player.TSPlayer);

            foreach (var _region in player.Regions.Reverse<Region>())
            {
                _region.UnsetFakeTiles();
            }

            var message = GetString($"您现在正在查看 {region.Name} 区域。");
            // 如果区域很大，显示区域的大小。
            if (tRegion.Area.Width >= Region.MaximumSize || tRegion.Area.Height >= Region.MaximumSize)
            {
                int num;
                if (tRegion.Area.Bottom < args.Player.TileY)
                {
                    num = args.Player.TileY - tRegion.Area.Bottom;
                    message += GetString($" 边界位于您上方 {num} 个图块");
                }
                else if (tRegion.Area.Top > args.Player.TileY)
                {
                    message += GetString($" 边界位于您下方 {tRegion.Area.Top - args.Player.TileY} 个图块");
                }
                else
                {
                    message += GetString($" 边界位于您上方 {args.Player.TileY - tRegion.Area.Top} 个图块，下方 {tRegion.Area.Bottom - args.Player.TileY} 个图块");
                }
                if (tRegion.Area.Right < args.Player.TileX)
                {
                    message += GetString($"，位于您右侧 {args.Player.TileX - tRegion.Area.Right} 个图块。");
                }
                else if (tRegion.Area.Left > args.Player.TileX)
                {
                    message += GetString($"，位于您左侧 {tRegion.Area.Left - args.Player.TileX} 个图块。");
                }
                else
                {
                    message += GetString($"，位于您右侧 {args.Player.TileX - tRegion.Area.Left} 个图块，左侧 {tRegion.Area.Right - args.Player.TileX} 个图块。");
                }
            }
            args.Player.SendMessage(message, TextColors[region.Color - 13]);

            this._refreshTimer.Interval = 7000;
            this._refreshTimer.Enabled = true;
        }
    }

    private void CommandClear(CommandArgs args)
    {
        lock (this.Players)
        {
            var player = this.FindPlayer(args.Player.Index);
            if (player == null)
            {
                return;
            }

            player.IsViewingNearby = false;
            ClearRegions(player);
        }
    }

    private void CommandViewNearby(CommandArgs args)
    {
        lock (this.Players)
        {
            var player = this.FindPlayer(args.Player.Index);

            if (player == null)
            {
                return;
            }

            if (player.IsViewingNearby)
            {
                player.IsViewingNearby = false;
                args.Player.SendInfoMessage(GetString("您不再查看您附近的区域。"));
            }
            else
            {
                player.IsViewingNearby = true;
                args.Player.SendInfoMessage(GetString("您现在正在查看您附近的区域。"));

                this._refreshTimer.Interval = 1500;
                this._refreshTimer.Enabled = true;
            }
        }
    }

    public static void ClearRegions(RegionPlayer player)
    {
        foreach (var region in player.Regions)
        {
            region.Refresh(player.TSPlayer);
        }

        player.Regions.Clear();
    }

    private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs e)
    {
        if (e.Action is > GetDataHandlers.EditAction.KillTileNoItem or GetDataHandlers.EditAction.KillWall)
        {
            return;
        }

        if (e.Action == GetDataHandlers.EditAction.PlaceTile && e.EditData == Terraria.ID.TileID.MagicalIceBlock)
        {
            return;
        }

        lock (this.Players)
        {
            var player = this.FindPlayer(e.Player.Index);
            if (player == null)
            {
                return;
            }

            if (player.Regions.Count == 0)
            {
                return;
            }

            // Stop the edit if a phantom tile is the only thing making it possible.
            foreach (var region in player.Regions)
            {
                // Clear the region borders if they break one of the phantom ice blocks.
                if ((e.Action == GetDataHandlers.EditAction.KillTile || e.Action == GetDataHandlers.EditAction.KillTileNoItem) && (Main.tile[e.X, e.Y] == null || !Main.tile[e.X, e.Y].active()) &&
                    e.X >= region.ShowArea.Left - 1 && e.X <= region.ShowArea.Right + 1 && e.Y >= region.ShowArea.Top - 1 && e.Y <= region.ShowArea.Bottom + 1 &&
                    !(e.X >= region.ShowArea.Left + 2 && e.X <= region.ShowArea.Right - 2 && e.Y >= region.ShowArea.Top + 2 && e.Y <= region.ShowArea.Bottom - 2))
                {
                    e.Handled = true;
                    //clearRegions(player);
                    break;
                }
                if ((e.Action == GetDataHandlers.EditAction.PlaceTile || e.Action == GetDataHandlers.EditAction.PlaceWall) && !TileValidityCheck(region, e.X, e.Y, e.Action))
                {
                    e.Handled = true;
                    player.TSPlayer.SendData(PacketTypes.TileSendSquare, "", 1, e.X, e.Y, 0, 0);
                    if (e.Action == GetDataHandlers.EditAction.PlaceTile)
                    {
                        GiveTile(player, e);
                    }

                    if (e.Action == GetDataHandlers.EditAction.PlaceWall)
                    {
                        GiveWall(player, e);
                    }

                    break;
                }
            }

            if (e.Handled)
            {
                ClearRegions(player);
            }
        }
    }

    private void OnPlayerJoin(JoinEventArgs e)
    {
        lock (this.Players)
        {
            this.Players.Add(new(e.Who));
        }
    }

    private void OnPlayerLeave(LeaveEventArgs e)
    {
        lock (this.Players)
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                if (this.Players[i].Index == e.Who)
                {
                    this.Players.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void OnPlayerCommand(PlayerCommandEventArgs e)
    {
        if (e.Parameters.Count >= 2 && e.CommandName.ToLower() == "region" && new[] { "delete", "resize", "expand" }.Contains(e.Parameters[0].ToLower()))
        {
            if (Commands.ChatCommands.Any(c => c.HasAlias("region") && c.CanRun(e.Player)))
            {
                this._refreshTimer.Interval = 1500;
            }
        }
    }

    private void Tick(object sender, ElapsedEventArgs e)
    {
        this.RefreshRegions();
    }

    private void RefreshRegions()
    {
        var anyRegions = false;

        // Check for regions that have changed.
        lock (this.Players)
        {
            foreach (var player in this.Players)
            {
                var refreshFlag = false;

                for (var i = 0; i < player.Regions.Count; i++)
                {
                    var region = player.Regions[i];
                    var tRegion = TShock.Regions.GetRegionByName(region.Name);

                    if (tRegion == null)
                    {
                        // The region was removed.
                        refreshFlag = true;
                        region.Refresh(player.TSPlayer);
                        player.Regions.RemoveAt(i--);
                    }
                    else
                    {
                        var newArea = tRegion.Area;
                        if (!region.Command && (!player.IsViewingNearby || !IsPlayerNearby(player.TSPlayer, region.Area)))
                        {
                            // The player is no longer near the region.
                            refreshFlag = true;
                            region.Refresh(player.TSPlayer);
                            player.Regions.RemoveAt(i--);
                        }
                        else
                        if (newArea != region.Area)
                        {
                            // The region was resized.
                            if (newArea.Width < 0 || newArea.Height < 0)
                            {
                                refreshFlag = true;
                                region.Refresh(player.TSPlayer);
                                player.Regions.RemoveAt(i--);
                            }
                            else
                            {
                                anyRegions = true;
                                refreshFlag = true;
                                region.Refresh(player.TSPlayer);
                                region.Area = newArea;
                                region.CalculateArea(player.TSPlayer);
                            }
                        }
                        else
                        {
                            anyRegions = true;
                        }
                    }
                }

                if (player.IsViewingNearby)
                {
                    anyRegions = true;

                    // Search for nearby regions
                    foreach (var tRegion in TShock.Regions.Regions)
                    {
                        if (tRegion.WorldID == Main.worldID.ToString() && tRegion.Area.Width >= 0 && tRegion.Area.Height >= 0)
                        {
                            if (IsPlayerNearby(player.TSPlayer, tRegion.Area))
                            {
                                if (!player.Regions.Any(r => r.Name == tRegion.Name))
                                {
                                    refreshFlag = true;
                                    var region = new Region(tRegion.Name, tRegion.Area, false);
                                    region.CalculateArea(player.TSPlayer);
                                    player.Regions.Add(region);
                                    player.TSPlayer.SendMessage(GetString($"你正在看区域 {region.Name}."), TextColors[region.Color - 13]);
                                }
                            }
                        }
                    }
                }

                if (refreshFlag)
                {
                    foreach (var region in player.Regions)
                    {
                        region.SetFakeTiles();
                    }

                    foreach (var region in player.Regions)
                    {
                        region.Refresh(player.TSPlayer);
                    }

                    foreach (var region in player.Regions.Reverse<Region>())
                    {
                        region.UnsetFakeTiles();
                    }
                }
            }
        }

        if (anyRegions)
        {
            this._refreshTimer.Interval = 7000;
            this._refreshTimer.Enabled = true;
        }
    }

    public static bool IsPlayerNearby(TSPlayer tPlayer, Rectangle area)
    {
        var playerX = (int) (tPlayer.X / 16);
        var playerY = (int) (tPlayer.Y / 16);

        return playerX >= area.Left - NearRange &&
                playerX <= area.Right + NearRange &&
                playerY >= area.Top - NearRange &&
                playerY <= area.Bottom + NearRange;
    }

    public static bool TileValidityCheck(Region region, int x, int y, GetDataHandlers.EditAction editType)
    {
        // Check if there's a wall or another tile next to this tile.
        if (editType == GetDataHandlers.EditAction.PlaceWall)
        {
            if (Main.tile[x, y] != null && Main.tile[x, y].active())
            {
                return true;
            }

            if (Main.tile[x - 1, y] != null && ((Main.tile[x - 1, y].active() && !Main.tileNoAttach[Main.tile[x - 1, y].type]) || Main.tile[x - 1, y].wall > 0))
            {
                return true;
            }

            if (Main.tile[x + 1, y] != null && ((Main.tile[x + 1, y].active() && !Main.tileNoAttach[Main.tile[x + 1, y].type]) || Main.tile[x + 1, y].wall > 0))
            {
                return true;
            }

            if (Main.tile[x, y - 1] != null && ((Main.tile[x, y - 1].active() && !Main.tileNoAttach[Main.tile[x, y - 1].type]) || Main.tile[x, y - 1].wall > 0))
            {
                return true;
            }

            if (Main.tile[x, y + 1] != null && ((Main.tile[x, y + 1].active() && !Main.tileNoAttach[Main.tile[x, y + 1].type]) || Main.tile[x, y + 1].wall > 0))
            {
                return true;
            }
        }
        else
        {
            if (Main.tile[x, y] != null && Main.tile[x, y].wall > 0)
            {
                return true;
            }

            if (Main.tile[x - 1, y] != null && Main.tile[x - 1, y].wall > 0)
            {
                return true;
            }

            if (Main.tile[x + 1, y] != null && Main.tile[x + 1, y].wall > 0)
            {
                return true;
            }

            if (Main.tile[x, y - 1] != null && Main.tile[x, y - 1].wall > 0)
            {
                return true;
            }

            if (Main.tile[x, y + 1] != null && Main.tile[x, y + 1].wall > 0)
            {
                return true;
            }

            if (Main.tile[x - 1, y] != null && Main.tile[x - 1, y].active() && !Main.tileNoAttach[Main.tile[x - 1, y].type])
            {
                return true;
            }

            if (Main.tile[x + 1, y] != null && Main.tile[x + 1, y].active() && !Main.tileNoAttach[Main.tile[x + 1, y].type])
            {
                return true;
            }

            if (Main.tile[x, y - 1] != null && Main.tile[x, y - 1].active() && !Main.tileNoAttach[Main.tile[x, y - 1].type])
            {
                return true;
            }

            if (Main.tile[x, y - 1] != null && Main.tile[x, y + 1].active() && !Main.tileNoAttach[Main.tile[x, y + 1].type])
            {
                return true;
            }
        }

        // Check if this tile is next to a region boundary.
        return x < region.ShowArea.Left - 1 || x > region.ShowArea.Right + 1 || y < region.ShowArea.Top - 1 || y > region.ShowArea.Bottom + 1 ||
            (x >= region.ShowArea.Left + 2 && x <= region.ShowArea.Right - 2 && y >= region.ShowArea.Top + 2 && y <= region.ShowArea.Bottom - 2);
    }

    public static void GiveTile(RegionPlayer player, GetDataHandlers.TileEditEventArgs e)
    {
        var item = new Item();
        var found = false;

        for (var i = 1; i <= Terraria.ID.ItemID.Count; i++)
        {
            item.SetDefaults(i, true);
            if (item.createTile == e.EditData && item.placeStyle == e.Style)
            {
                if (item.tileWand != -1)
                {
                    item.SetDefaults(item.tileWand, true);
                }

                found = true;
                break;
            }
        }

        if (found)
        {
            GiveItem(player, item);
        }
    }

    public static void GiveWall(RegionPlayer player, GetDataHandlers.TileEditEventArgs e)
    {
        var item = new Item(); var found = false;
        for (var i = 1; i <= Terraria.ID.ItemID.Count; i++)
        {
            item.SetDefaults(i, true);
            if (item.createWall == e.EditData)
            {
                found = true;
                break;
            }
        }
        if (found)
        {
            item.stack = 1;
            GiveItem(player, item);
        }
    }

    public static void GiveItem(RegionPlayer player, Item item)
    {
        player.TSPlayer.GiveItem(item.type, 1);
    }
}