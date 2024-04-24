using Terraria.ID;
using Terraria;
using TShockAPI;

namespace MiniGamesAPI
{
    public class MiniTile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public bool Active => Tile.active();

        public int Type => Tile.type;

        public ITile Tile { get; set; }

        public MiniTile(int x, int y, ITile tile)
        {
            X = x;
            Y = y;
            Tile = (ITile)new Tile(tile);
        }

        public void Place()
        {
            Main.tile[X, Y] = (ITile)new Tile(Tile);
        }

        public void Kill(bool fail = false, bool effectOnly = false, bool noitem = false)
        {
            WorldGen.KillTile(X, Y, fail, effectOnly, noitem);
        }

        public void Update()
        {
            TSPlayer.All.SendTileRect((short)X, (short)Y, (byte)1, (byte)1, (TileChangeType)0);
        }
    }
}
