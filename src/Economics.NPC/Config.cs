using Economics.Core.ConfigFiles;
using Newtonsoft.Json;

namespace Economics.NPC;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "NPC.json";

    public class NpcAllocationRatioOption
    {
        [JsonProperty("转换率")]
        public float AllocationRatio { get; set; }

        [JsonProperty("进度条件")]
        public List<string> Progress { get; set; } = new();
    }

    [JsonProperty("开启提示")]
    public bool Prompt = true;

    [JsonProperty("提示内容")]
    public string PromptText = "你因击杀{0},获得额外奖励{1}{2}个";

    [JsonProperty("额外奖励列表")]
    public List<NpcOption> NPCS = new();

    [JsonProperty("转换率更改")]
    public Dictionary<int, NpcAllocationRatioOption> AllocationRatio = [];
}