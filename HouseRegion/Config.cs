using Newtonsoft.Json;

namespace HouseRegion
{
    public class Config
    {
        [JsonProperty("进出房屋提示")]
        public bool HouseRegion = true;

        [JsonProperty("房屋嘴大大小")]
        public int HouseMaxSize = 1000;

        [JsonProperty("房屋最小宽度")]
        public int MinWidth = 30;

        [JsonProperty("房屋最小高度")]
        public int MinHeight = 30;

        [JsonProperty("房屋最大数量")]
        public int HouseMaxNumber = 1;

        [JsonProperty("禁止锁房屋")]
        public bool LimitLockHouse = false;

        [JsonProperty("保护宝石锁")]
        public bool ProtectiveGemstoneLock = false;

        [JsonProperty("始终保护箱子")]
        public bool ProtectiveChest = true;

        [JsonProperty("冻结警告破坏者")]
        public bool WarningSpoiler = true;

        [JsonProperty("禁止分享所有者7")]
        public bool ProhibitSharingOwner = false;

        [JsonProperty("禁止分享使用者")]
        public bool ProhibitSharingUser = false;

        [JsonProperty("禁止所有者修改使用者")]
        public bool ProhibitOwnerModifyingUser = true;

        public static Config Read(string Path)//给定文件进行读
        {
            if (!File.Exists(Path)) return new Config();
            using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Read(fs);
        }
        public static Config Read(Stream stream)//给定流文件进行读取
        {
            using var sr = new StreamReader(stream);
            var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            return cf ?? new();
        }
        public void Write(string Path)//给定路径进行写
        {
            using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
            Write(fs);
        }
        public void Write(Stream stream)//给定流文件写
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using var sw = new StreamWriter(stream);
            sw.Write(str);
        }
    }
}