namespace AutoStoreItems;

public class MyData
{
    //玩家数据表
    public List<ItemData> Items { get; set; } = new List<ItemData>();

    #region 数据结构
    public class ItemData
    {
        //玩家名字
        public string Name { get; set; }

        //自动识别模式
        public bool AutoMode { get; set; } = false;

        //装备识别模式
        public bool ArmorMode { get; set; } = false;

        //手持识别模式
        public bool HandMode { get; set; } = false;

        //监听储物空间位格开关
        public bool listen { get; set; } = false;

        //自动储存提示
        public bool Mess { get; set; } = true;

        //自动储存表
        public List<int> ItemType { get; set; }

        public ItemData(string name = "", bool enabled = true, bool auto = true, bool mess = true, List<int> item = null!)
        {
            this.Name = name ?? "";
            this.AutoMode = enabled;
            this.listen = auto;
            this.Mess = mess;
            this.ItemType = item;
        }
    }

    #endregion
}
