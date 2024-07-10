using Terraria.ID;
using static SpawnInfra.Plugin;

namespace SpawnInfra
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
            foreach (var item in Config.Prison)
            {
                tile = item.TileID;
                wall = item.WallID;
                platform.style = item.PlatformStyle;
                chair.style = item.ChairStyle;
                bench.style = item.BenchStyle;
                torch.style = item.TorchStyle;
            }
        }

    }
}
