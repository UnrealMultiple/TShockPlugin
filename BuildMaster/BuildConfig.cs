namespace MainPlugin
{
    public class BuildConfig
    {
        public bool UnlockAll { get; set; }

        public Dictionary<int, int> Range { get; set; }

        public List<int> BanItem { get; set; }

        public BuildConfig()
        {
            UnlockAll = true;
            Range = new Dictionary<int, int>();
            BanItem = new List<int>();
        }
    }
}
