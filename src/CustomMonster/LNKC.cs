namespace CustomMonster;

public class LNKC
{
    public int ID { get; set; }

    public long KC { get; set; }

    public LNKC(int id)
    {
        this.ID = id;
        this.KC = 1L;
    }

    public LNKC(int id, long kc)
    {
        this.ID = id;
        this.KC = kc;
    }
}
