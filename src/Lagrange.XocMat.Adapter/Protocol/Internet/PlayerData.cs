using ProtoBuf;

namespace Lagrange.XocMat.Adapter.Protocol.Internet;

[ProtoContract]
public class PlayerData
{
    //在线状态
    [ProtoMember(1)] public bool OnlineStatu { get; set; }
    //玩家名字
    [ProtoMember(2)] public string Username { get; set; } = "";
    //最大生命
    [ProtoMember(3)] public int statLifeMax { get; set; }
    //当前生命
    [ProtoMember(4)] public int statLife { get; set; }
    //最大法力 
    [ProtoMember(5)] public int statManaMax { get; set; }
    //当前法力
    [ProtoMember(6)] public int statMana { get; set; }
    //buff
    [ProtoMember(7)] public int[] buffType { get; set; } = Array.Empty<int>();
    //buff时间
    [ProtoMember(8)] public int[] buffTime { get; set; } = Array.Empty<int>();
    //背包
    [ProtoMember(9)] public Item[] inventory { get; set; } = Array.Empty<Item>();
    //宠物坐骑等
    [ProtoMember(10)] public Item[] miscEquip { get; set; } = Array.Empty<Item>();
    //宠物坐骑染料
    [ProtoMember(11)] public Item[] miscDye { get; set; } = Array.Empty<Item>();
    //套装
    [ProtoMember(12)] public Suits[] Loadout { get; set; } = Array.Empty<Suits>();
    //垃圾桶
    [ProtoMember(13)] public Item[] trashItem { get; set; } = Array.Empty<Item>();
    //猪猪存钱罐 
    [ProtoMember(14)] public Item[] Piggiy { get; set; } = Array.Empty<Item>();
    //保险箱
    [ProtoMember(15)] public Item[] safe { get; set; } = Array.Empty<Item>();
    //护卫熔炉
    [ProtoMember(16)] public Item[] Forge { get; set; } = Array.Empty<Item>();
    //虚空保险箱
    [ProtoMember(17)] public Item[] VoidVault { get; set; } = Array.Empty<Item>();
}
