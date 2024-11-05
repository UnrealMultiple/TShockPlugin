namespace Plugin;

public class LPlayer
{
    public int Index { get; set; }

    public float x { get; set; }

    public float y { get; set; }

    public bool tp { get; set; }

    public LPlayer(int index)
    {
        Index = index;
        tp = false;
        x = 0f;
        y = 0f;
    }
}
