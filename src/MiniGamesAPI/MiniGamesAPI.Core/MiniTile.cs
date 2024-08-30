using Terraria;
using TShockAPI;

namespace MiniGamesAPI;

public class MiniTile
{
    public int X { get; set; }

    public int Y { get; set; }

    public bool Active => this.Tile.active();

    public int Type => this.Tile.type;

    public ITile Tile { get; set; }

    public MiniTile(int x, int y, ITile tile)
    {
        this.X = x;
        this.Y = y;
        this.Tile = new Tile(tile);
    }

    public void Place()
    {
        Main.tile[this.X, this.Y] = new Tile(this.Tile);
    }

    public void Kill(bool fail = false, bool effectOnly = false, bool noitem = false)
    {
        WorldGen.KillTile(this.X, this.Y, fail, effectOnly, noitem);
    }

    public void Update()
    {
        TSPlayer.All.SendTileRect((short) this.X, (short) this.Y, 1, 1, 0);
    }
}