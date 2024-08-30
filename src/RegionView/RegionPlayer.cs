using TShockAPI;

namespace RegionView;

public class RegionPlayer
{
    public int Index { get; }

    public TSPlayer TSPlayer => TShock.Players[this.Index];

    public List<Region> Regions { get; } = new();

    public bool IsViewingNearby { get; set; }

    public RegionPlayer(int index)
    {
        this.Index = index;
    }
}