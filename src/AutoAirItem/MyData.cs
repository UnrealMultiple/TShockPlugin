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

        public bool IsActive { get; set; }

        //玩家出入服务器的记录时间
        public DateTime LogTime { get; set; }

        //玩家自己的垃圾桶开关
        public bool Enabled { get; set; } = false;

        //监听垃圾桶位格开关
        public bool Auto { get; set; } = false;

        //垃圾桶的回收提示
        public bool Mess { get; set; } = true;

        //垃圾桶表
        public List<string> ItemName { get; set; }

        public ItemData(string name = "",bool Active = true, bool enabled = true, bool auto = true, bool mess = true, DateTime time = default, List<string> item = null!)
        {
            this.Name = name ?? "";
            this.IsActive = Active;
            this.Enabled = enabled;
            this.LogTime = time;
            this.Auto = auto;
            this.Mess = mess;
            this.ItemName = item ?? new List<string>();
        }
    }

    #endregion
}
