using Newtonsoft.Json;

namespace CriticalHit;
public class Config
{
    [JsonProperty("总开关")]
    public bool Enable = true;
    [JsonProperty("仅暴击显示")]
    public bool NoCritMessages = true;

    // 使用JsonConverter
    [JsonConverter(typeof(WeaponTypeDictionaryConverter))]
    [JsonProperty("消息分类")]
    public Dictionary<WeaponType, CritMessage> CritMessages { get; set; } = new Dictionary<WeaponType, CritMessage>();

    public void Write(string path)
    {
        using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
        Write(stream);
    }

    public void Write(Stream stream)
    {
        string value = JsonConvert.SerializeObject(this, (Formatting)1);
        using StreamWriter streamWriter = new StreamWriter(stream);
        streamWriter.Write(value);
    }

    public void Read(string path)
    {
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Read(stream);
            }
        }
    }

    public void Read(Stream stream)
    {
        using StreamReader streamReader = new StreamReader(stream);
        Config deserializedConfig = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        this.CopyFrom(deserializedConfig);
    }

    // 添加一个新的 CopyFrom 方法来复制属性值
    public void CopyFrom(Config sourceConfig)
    {
        this.Enable = sourceConfig.Enable;
        this.NoCritMessages = sourceConfig.NoCritMessages;
        this.CritMessages = sourceConfig.CritMessages;
    }
}

