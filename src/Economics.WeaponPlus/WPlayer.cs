using TShockAPI;

namespace Economics.WeaponPlus;

public class WPlayer
{
    public TSPlayer me;

    public List<WItem> hasItems;

    private readonly int index = 255;

    private readonly string name = string.Empty;

    public bool isActive => this.me != null && this.me.Active && this.index == this.me.Index && this.me.TPlayer != null && this.index == this.me.TPlayer.whoAmI && this.me.TPlayer.active && this.name == this.me.Name && this.name == this.me.TPlayer.name;

    public WPlayer(TSPlayer? me)
    {
        if (me == null)
        {
            this.me = new TSPlayer(-1);
            this.index = 255;
            this.name = string.Empty;
        }
        else
        {
            this.me = me;
            this.index = me.Index;
            this.name = me.Name;
        }
        this.hasItems = new List<WItem>();
    }
}