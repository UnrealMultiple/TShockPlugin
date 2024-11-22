using Newtonsoft.Json;
using TShockAPI;

namespace DonotFuck;

public class Configuration
{
    #region 实例变量
    [JsonProperty("每页行数")]
    public int PageSize = 30;

    [JsonProperty("记录日志")]
    public bool Log = true;

    [JsonProperty("脏话表")]
    public HashSet<string> DirtyWords { get; set; } = new HashSet<string>(); 
    #endregion

    #region 预设参数方法
    public void Ints()
    {
        this.DirtyWords = new HashSet<string>{ "6", "六" };
    }
    #endregion

    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "禁止脏话", "禁止脏话.json");

    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static Configuration Read()
    {
        if (!File.Exists(FilePath))
        {
            var NewConfig = new Configuration();
            NewConfig.Ints();
            new Configuration().Write();
            return NewConfig;
        }
        else
        {
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

    #region 增删改方法
    internal bool Exempt(string text)
    {
        return this.DirtyWords.Contains(text);
    }

    public bool Add(string text)
    {
        if (this.Exempt(text))
        {
            return false;
        }
        this.DirtyWords.Add(text);
        this.Write();
        return true;
    }

    public bool Del(string text)
    {
        if (this.Exempt(text))
        {
            this.DirtyWords.Remove(text);
            this.Write();
            return true;
        }
        return false;
    }
    #endregion
}