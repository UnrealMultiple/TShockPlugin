using Terraria;

namespace CaiRewardChest;

public class RewardChest
{
    public int ChestId;
    public List<int> HasOpenPlayer = new();
    public int X => Main.chest[this.ChestId].x;
    public int Y => Main.chest[this.ChestId].y;
    public Chest Chest => Main.chest[this.ChestId];
}