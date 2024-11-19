using TShockAPI;
using TShockAPI.DB;

namespace SmartRegions;

public class SmartRegion
{
    private Region? _region = null;

    public string name = "";
    public string command = "";
    public double cooldown;
    public Region region
    {
        get
        {
            this._region ??= TShock.Regions.GetRegionByName(this.name);
            return this._region;
        }
    }
}