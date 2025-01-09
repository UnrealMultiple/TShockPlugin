namespace Plugin;

public class LPlayer
{
    public int Index { get; set; }

    public float x { get; set; }

    public float y { get; set; }

    public bool tp { get; set; }

    public LPlayer(int index)
    {
        this.Index = index;
        this.tp = false;
        this.x = 0f;
        this.y = 0f;
    }
}
