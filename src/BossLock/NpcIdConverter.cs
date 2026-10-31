using Newtonsoft.Json;
using Terraria.ID;

namespace BossLock;

// 配置别名不依赖服务器的显示语言；写回时统一保存数字 ID。
public sealed class NpcIdConverter : JsonConverter<int>
{
    private static readonly Dictionary<string, int> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        ["史莱姆王"] = NPCID.KingSlime,
        ["克苏鲁之眼"] = NPCID.EyeofCthulhu,
        ["世界吞噬怪"] = NPCID.EaterofWorldsHead,
        ["世界吞噬者"] = NPCID.EaterofWorldsHead,
        ["世吞"] = NPCID.EaterofWorldsHead,
        ["克苏鲁之脑"] = NPCID.BrainofCthulhu,
        ["克脑"] = NPCID.BrainofCthulhu,
        ["蜂王"] = NPCID.QueenBee,
        ["骷髅王"] = NPCID.SkeletronHead,
        ["骷髅王手臂"] = NPCID.SkeletronHand,
        ["鹿角怪"] = NPCID.Deerclops,
        ["血肉墙"] = NPCID.WallofFlesh,
        ["肉山"] = NPCID.WallofFlesh,
        ["史莱姆皇后"] = NPCID.QueenSlimeBoss,
        ["双子魔眼"] = NPCID.Retinazer,
        ["激光眼"] = NPCID.Retinazer,
        ["魔焰眼"] = NPCID.Spazmatism,
        ["机械骷髅王"] = NPCID.SkeletronPrime,
        ["机械炮"] = NPCID.PrimeCannon,
        ["机械锯"] = NPCID.PrimeSaw,
        ["机械钳"] = NPCID.PrimeVice,
        ["机械激光"] = NPCID.PrimeLaser,
        ["毁灭者"] = NPCID.TheDestroyer,
        ["世纪之花"] = NPCID.Plantera,
        ["石巨人"] = NPCID.Golem,
        ["猪龙鱼公爵"] = NPCID.DukeFishron,
        ["光之女皇"] = NPCID.HallowBoss,
        ["拜月教邪教徒"] = NPCID.CultistBoss,
        ["拜月教徒"] = NPCID.CultistBoss,
        ["月亮领主"] = NPCID.MoonLordCore,
        ["月总"] = NPCID.MoonLordCore,
        ["南瓜王"] = NPCID.Pumpking,
        ["火星飞碟"] = NPCID.MartianSaucerCore,
        ["荷兰飞盗船"] = NPCID.PirateShip
    };

    public override int ReadJson(JsonReader reader, Type objectType, int existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer && reader.Value is long number &&
            number >= 0 && number < NPCID.Count)
        {
            return (int)number;
        }
        if (reader.TokenType == JsonToken.String)
        {
            var value = ((string)reader.Value!).Trim();
            if (int.TryParse(value, out var id) && id >= 0 && id < NPCID.Count) { return id; }
            if (Names.TryGetValue(value, out id)) { return id; }
        }
        throw new JsonSerializationException($"无效的 NpcId（{reader.Path}）：请使用数字 ID 或文档中支持的中文 Boss 名称。");
    }

    public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer) => writer.WriteValue(value);
}
