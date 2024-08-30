namespace CreateSpawn;

internal class Map
{
    private static readonly string PATH = Path.Combine(TShockAPI.TShock.SavePath, "Create.map");
    public static void SaveMap(List<Building> building)
    {
        using FileStream fs = new(PATH, FileMode.OpenOrCreate);
        using BinaryWriter writer = new(fs);
        writer.Write("by 少司命");
        writer.Write(building.Count);
        foreach (var data in building)
        {
            writer.Write(data.bTileHeader);
            writer.Write(data.bTileHeader2);
            writer.Write(data.bTileHeader3);
            writer.Write(data.frameX);
            writer.Write(data.frameY);
            writer.Write(data.liquid);
            writer.Write(data.sTileHeader);
            writer.Write(data.type);
            writer.Write(data.wall);
        }
    }

    public static List<Building> LoadMap()
    {
        using FileStream fs = new(PATH, FileMode.OpenOrCreate);
        using BinaryReader reader = new(fs);
        reader.ReadString();
        var count = reader.ReadInt32();
        var building = new List<Building>();
        for (int i = 0; i < count; i++)
        {
            building.Add(new()
            {
                bTileHeader = reader.ReadByte(),
                bTileHeader2 = reader.ReadByte(),
                bTileHeader3 = reader.ReadByte(),
                frameX = reader.ReadInt16(),
                frameY = reader.ReadInt16(),
                liquid = reader.ReadByte(),
                sTileHeader = reader.ReadUInt16(),
                type = reader.ReadUInt16(),
                wall = reader.ReadUInt16(),
            });
        }
        return building;
    }
}
