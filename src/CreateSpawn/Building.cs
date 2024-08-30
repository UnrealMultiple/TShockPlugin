namespace CreateSpawn;

public class Building
{
    public byte bTileHeader { get; set; }

    public byte bTileHeader2 { get; set; }

    public byte bTileHeader3 { get; set; }

    public short frameX { get; set; }

    public short frameY { get; set; }

    public byte liquid { get; set; }

    public ushort sTileHeader { get; set; }

    public ushort type { get; set; }

    public ushort wall { get; set; }
}
