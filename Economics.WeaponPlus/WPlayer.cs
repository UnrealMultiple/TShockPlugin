using TShockAPI;

namespace Economics.WeaponPlus
{
    public class WPlayer
    {
        public TSPlayer me;

        public List<WItem> hasItems;

        private readonly int index = 255;

        private readonly string name = string.Empty;

        public bool isActive => me != null && me.Active && index == me.Index && me.TPlayer != null && index == me.TPlayer.whoAmI && me.TPlayer.active && name == me.Name && name == me.TPlayer.name;

        public WPlayer(TSPlayer? me)
        {
            if (me == null)
            {
                this.me = new TSPlayer(-1);
                index = 255;
                name = string.Empty;
            }
            else
            {
                this.me = me;
                index = me.Index;
                name = me.Name;
            }
            hasItems = new List<WItem>();
        }
    }
}
