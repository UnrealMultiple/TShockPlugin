using Newtonsoft.Json;

namespace CustomMonster;

public class AudioGroup
{
    [JsonProperty(PropertyName = "声音ID")]
    public int SoundID = -1;

    [JsonProperty(PropertyName = "声音规模")]
    public float SoundSize = -1f;

    [JsonProperty(PropertyName = "高音补偿")]
    public float HighPitch = -1f;
}
