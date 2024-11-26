namespace AutoAirItem;

public class MyData
{
    //玩家数据表
    public List<PlayerData> Items { get; set; } = new List<PlayerData>();

    #region 数据结构
    public class PlayerData
    {
        //玩家名字
        public string Name { get; set; }

        //玩家自己的自动垃圾桶开关
        public bool Enabled { get; set; } = false;

        //监听自动垃圾桶位格开关
        public bool Auto { get; set; } = false;

        //自动垃圾桶的回收提示
        public bool Mess { get; set; } = true;

        //自动垃圾桶表
        public List<int> ItemType { get; set; }

        //移除物品的字典
        public Dictionary<int, int> DelItem { get; set; } = new Dictionary<int, int>();

        public PlayerData(string name = "", bool enabled = true, bool auto = true, bool mess = true, List<int> item = null!, Dictionary<int, int> DelItem = null!)
        {
            this.Name = name ?? "";
            this.Enabled = enabled;
            this.Auto = auto;
            this.Mess = mess;
            this.ItemType = item;
            this.DelItem = DelItem;
        }
    }

    #endregion
}
