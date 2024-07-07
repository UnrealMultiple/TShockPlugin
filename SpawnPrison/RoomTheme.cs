using Terraria.ID;
using static Plugin.Plugin;

namespace Plugin
{
    internal class RoomTheme
    {
        public ushort tile = 0;

        public ushort wall = 16;

        public TileInfo platform = new TileInfo(19, 0);

        public TileInfo chair = new TileInfo(15, 0);

        public TileInfo bench = new TileInfo(18, 0);

        public TileInfo torch = new TileInfo(4, 0);

        public void SetGlass()
        {
            tile = Config.TileID;
            wall = Config.WallID;
            platform.style = Config.PlatformStyle;
            chair.style = Config.ChairStyle;
            bench.style = Config.BenchStyle;
            torch.style = Config.TorchStyle;
        }

    }
}
