using Terraria;

namespace CaiRewardChest;

public class RewardChest
{
    public int ChestId;
    public List<int> HasOpenPlayer = new();
    public int X => Main.chest[ChestId].x;
    public int Y => Main.chest[ChestId].y;
    public Chest Chest => Main.chest[ChestId];
}