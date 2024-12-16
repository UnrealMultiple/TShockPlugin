namespace SurfaceBlock;

public class MyData
{
    public Dictionary<string, PlayerData>? Dict { get; set; } = new Dictionary<string, PlayerData>();

    public class PlayerData
    {
        //销毁开关
        public bool Enabled { get; set; }
        //销毁时间
        public DateTime Time { get; set; }
        internal PlayerData(bool enabled = false, DateTime time = default)
        {
            this.Enabled = enabled;
            this.Time = time;
        }
    }
}
