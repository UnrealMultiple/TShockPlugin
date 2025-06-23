using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using TShockAPI;


namespace CreateSpawn;

public class Building
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Tile[,]? Tiles { get; set; }
    public Point Origin { get; set; }

    public List<ChestItems>? ChestItems { get; set; }

    public List<Sign>? Signs { get; set; }

    public List<ItemFrames> ItemFrames { get; set; } = [];

    public List<WeaponRack> WeaponsRacks { get; set; } = [];

    public List<FPlatters> FoodPlatters { get; set; } = [];

    public List<DDolls> DisplayDolls { get; set; } = [];

    public List<HatRacks> HatRacks { get; set; } = [];

    public List<LogicSensors> LogicSensors { get; set; } = [];
}


public class ChestItems
{
    public Item? Item { get; set; }
    public Point Position { get; set; }
    public int Slot { get; set; }
}


public class ItemFrames
{
    public NetItem Item { get; set; }
    public Point Position { get; set; }
}

public class WeaponRack
{
    public NetItem Item { get; set; }
    public Point Position { get; set; }
}

public class FPlatters
{
    public NetItem Item { get; set; }
    public Point Position { get; set; }
}

public class DDolls
{
    public NetItem[] Items;
    public NetItem[] Dyes;
    public Point Position { get; set; }
}

public class HatRacks
{
    public NetItem[] Items { get; set; } = [];
    public NetItem[] Dyes { get; set; } = [];
    public Point Position { get; set; }
}

public class LogicSensors
{
    public Point Position { get; set; }
    public TELogicSensor.LogicCheckType type { get; set; }
}