namespace RewardSection;

public class LPlayer
{
    public int Index { get; set; }
    public bool tip { get; set; }

    public LPlayer(int index)
    {
        this.Index = index;
        this.tip = false;
    }
}