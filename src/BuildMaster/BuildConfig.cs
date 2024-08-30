namespace MainPlugin;

public class BuildConfig
{
    public bool UnlockAll { get; set; }

    public Dictionary<int, int> Range { get; set; }

    public List<int> BanItem { get; set; }

    public BuildConfig()
    {
        this.UnlockAll = true;
        this.Range = new Dictionary<int, int>();
        this.BanItem = new List<int>();
    }
}