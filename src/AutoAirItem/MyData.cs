namespace AutoAirItem;

public class MyData
{
    //玩家数据表
    public List<ItemData> Items { get; set; } = new List<ItemData>();

    #region 数据结构
    public class ItemData
    {
        //玩家名字
        public string Name { get; set; }

        //玩家自己的自动垃圾桶开关
        public bool Enabled { get; set; } = false;

        //监听自动垃圾桶位格开关
        public bool Auto { get; set; } = false;

        //自动垃圾桶的回收提示
        public bool Mess { get; set; } = true;

        //清理速度
        public long UpdateRate { get; set; } = 10;

        //自动垃圾桶表
        public List<string> ItemName { get; set; }

        public ItemData(string name = "", bool enabled = true, bool auto = true, bool mess = true, long up = 10, List<string> item = null!)
        {
            this.Name = name ?? "";
            this.Enabled = enabled;
            this.Auto = auto;
            this.Mess = mess;
            this.UpdateRate = up;
            this.ItemName = item ?? new List<string>();
        }
    }

    #endregion
}
