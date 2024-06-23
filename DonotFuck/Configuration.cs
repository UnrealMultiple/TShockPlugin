using Newtonsoft.Json;
using TShockAPI;

namespace DonotFuck
{
    public class Configuration
    {
        [JsonProperty("是否封禁")]
        public bool Ban = true; //布尔型，只有开或关

        [JsonProperty("封禁时长")] //用来在生成配置文件后对变量汉化
        public int ProhibitionTime = 10; //在DonotFuck类里，如果需要定义它，可以写成：Conifg.ProhibitionTime

        [JsonProperty("检查次数")]
        public int InspectedQuantity = 5; //整数型，只有数字

        [JsonProperty("脏话表")] //提供一个哈希集合用于存储敏感词或脏话词汇。
        public HashSet<string> DirtyWords { get; set; } = new HashSet<string>();

        //存放配置文件的位置
        public static readonly string FilePath = Path.Combine(TShock.SavePath,"禁止脏话.json");

        // 添加构造函数，用Read方法引用进去
        public Configuration()
        {
            DirtyWords = new HashSet<string>
            {
                "操",
                "妈的",
                "傻逼",
                "煞笔",
                "你妈",
                "草你",
            };
        }

        // 将当前对象序列化为 JSON 格式，并以格式化的、可读的方式写入指定路径的文件中。
        public void Write(string path)
        {
            // 使用FileStream打开或创建指定路径的文件，设置模式为Create（如果文件不存在则创建，存在则覆盖），访问模式为Write，共享模式为Write，允许其他进程同时进行写入操作。
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                // 使用JsonConvert将当前对象序列化为JSON字符串，并设置输出格式为缩进格式，提高可读性。
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);

                // 创建一个StreamWriter，将其与FileStream关联，以便将文本数据写入文件流。
                using (var sw = new StreamWriter(fs))
                {
                    // 使用StreamWriter将序列化后的JSON字符串写入到文件中。
                    sw.Write(str);
                }
            }
        }


        // 从指定路径读取配置文件。如果文件不存在，则创建一个默认配置文件并返回该默认配置。
        public static Configuration Read(string path)
        {
            // 检查指定路径的文件是否存在。
            if (!File.Exists(path))
            {
                // 如果配置文件不存在，则创建一个新的默认配置实例。
                var defaultConfig = new Configuration(); // 使用默认构造函数初始化配置。

                // 将这个默认配置写入到指定路径，确保有配置文件可返回。
                defaultConfig.Write(path);

                // 返回新创建的默认配置。
                return defaultConfig;
            }

            // 文件存在时，使用FileStream以只读和文件共享读取模式打开文件。
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs)) // 使用StreamReader来读取文件内容。
            {
                // 读取文件的全部内容，然后使用JsonConvert反序列化为Configuration对象。
                var jsonContent = sr.ReadToEnd();
                var cf = JsonConvert.DeserializeObject<Configuration>(jsonContent);

                // 返回反序列化得到的Configuration对象。
                return cf;
            }
        }
    }
}