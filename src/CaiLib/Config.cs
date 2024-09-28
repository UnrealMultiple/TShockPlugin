using Newtonsoft.Json;
using TShockAPI;

namespace CaiLib;

public class CaiConfig<TSettings> where TSettings : new()
{
    /// <summary>
    /// 设置Config对象
    /// </summary>
    /// <param name="name">配置文件的名字</param>
    /// <param name="settings">配置文件的类模板</param>
    public CaiConfig(string name, TSettings settings)
    {
        this.FilePath = Path.Combine(TShock.SavePath, name);
        this.Settings = settings;
    }


    [JsonIgnore]
    public string FilePath = Path.Combine(TShock.SavePath, "CaiLib.json");

    public virtual TSettings Settings { get; set; } = new TSettings();

    /// <summary>
    /// 写入Config
    /// </summary>
    public void Write()
    {
        using (var fs = new FileStream(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(str);
            }
        }
    }
    /// <summary>
    /// 创造并读取Config
    /// </summary>
    public CaiConfig<TSettings> Read()
    {
        if (!File.Exists(this.FilePath))
        {
            return this;
        }

        using (var fs = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var sr = new StreamReader(fs))
            {
                var cf = JsonConvert.DeserializeObject<CaiConfig<TSettings>>(sr.ReadToEnd());
                return cf;
            }
        }
    }
}