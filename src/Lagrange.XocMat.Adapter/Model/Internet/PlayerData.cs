using ProtoBuf;

namespace MorMorAdapter.Model.Internet;

[ProtoContract]
public class PlayerData
{
    //在线状态
    [ProtoMember(1)] public bool OnlineStatu { get; set; }
    //玩家名字
    [ProtoMember(2)] public string Username { get; set; }
    //最大生命
    [ProtoMember(3)] public int statLifeMax { get; set; }
    //当前生命
    [ProtoMember(4)] public int statLife { get; set; }
    //最大法力 
    [ProtoMember(5)] public int statManaMax { get; set; }
    //当前法力
    [ProtoMember(6)] public int statMana { get; set; }
    //buff
    [ProtoMember(7)] public int[] buffType { get; set; }
    //buff时间
    [ProtoMember(8)] public int[] buffTime { get; set; }
    //背包
    [ProtoMember(9)] public Item[] inventory { get; set; }
    //宠物坐骑等
    [ProtoMember(10)] public Item[] miscEquip { get; set; }
    //宠物坐骑染料
    [ProtoMember(11)] public Item[] miscDye { get; set; }
    //套装
    [ProtoMember(12)] public Suits[] Loadout { get; set; }
    //垃圾桶
    [ProtoMember(13)] public Item[] trashItem { get; set; }
    //猪猪存钱罐
    [ProtoMember(14)] public Item[] Piggiy { get; set; }
    //保险箱
    [ProtoMember(15)] public Item[] safe { get; set; }
    //护卫熔炉
    [ProtoMember(16)] public Item[] Forge { get; set; }
    //虚空保险箱
    [ProtoMember(17)] public Item[] VoidVault { get; set; }
}
