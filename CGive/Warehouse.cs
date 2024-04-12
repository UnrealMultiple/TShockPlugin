namespace CGive;

internal class Warehouse
{
    public int stack { get; set; }

    public int netID { get; set; }

    public int prefix { get; set; }

    public Warehouse(int stack, int netID, int prefix = 0)
    {
        this.stack = stack;
        this.netID = netID;
        this.prefix = prefix;
    }
}
