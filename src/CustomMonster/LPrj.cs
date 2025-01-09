namespace CustomMonster;

public class LPrj
{
    public int Index { get; set; }

    public int Type { get; set; }

    public int UseI { get; set; }

    public string Notes { get; set; }

    public LPrj(int index, int useIndex, int type, string notes)
    {
        this.Index = index;
        this.UseI = useIndex;
        this.Type = type;
        this.Notes = notes;
    }
}

