using TShockAPI;

namespace MiniGamesAPI;

public class MiniItem
{
    public int Slot { get; set; }

    public byte Prefix { get; set; }

    public int NetID { get; set; }

    public int Stack { get; set; }

    public MiniItem(int slot, byte prefix, int netid, int stack)
    {
        this.Slot = slot;
        this.Prefix = prefix;
        this.NetID = netid;
        this.Stack = stack;
    }

    public NetItem ToNetItem()
    {
        return new NetItem(this.NetID, this.Stack, this.Prefix);
    }
}