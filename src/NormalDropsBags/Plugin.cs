using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace NormalDropsBags;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    // 同时请参阅TShock的插件开发骨架示例：TShock文档-Hello World

    public override string Author => "Quinci 羽学适配";

    public override string Description => GetString("让原本在普通难度下不掉落宝物袋的Boss开始掉落此类稀有战利品。");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 3, 5);

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        //从terria服务器api注册钩子（tshock的服务器补丁版本，打开Terraria API/OTAPI）
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKill);
        ServerApi.Hooks.NpcLootDrop.Register(this, this.OnDropLoot);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //从它们各自的钩子集合中移除钩子以进行垃圾收集（IDisposable）。目前不太有用，因为这是在服务器关闭时使用的
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Deregister(this, this.OnDropLoot);
        }
        base.Dispose(disposing);
    }

    private void OnNpcKill(NpcKilledEventArgs eventArgs)
    {

        // 检查是否为Boss或Betsy，且当前游戏模式为普通模式；同时确认目标不是疯狂教徒（在任何情况下它都不应掉落宝物袋，这是无法获得的）
        if ((eventArgs.npc.boss || eventArgs.npc.netID == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.npc.netID != Terraria.ID.NPCID.CultistBoss)
        {
            switch (eventArgs.npc.netID) // 检查NPC类型。EOL（可能是“End of Level”的缩写，意为阶段结束）和史莱姆女皇拥有特殊的掉落机制，所以我打算替换它。
            {
                case Terraria.ID.NPCID.KingSlime:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.KingSlimeBossBag, 1);
                    return;
                case Terraria.ID.NPCID.EyeofCthulhu:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.EyeOfCthulhuBossBag, 1);
                    return;
                case Terraria.ID.NPCID.EaterofWorldsHead:
                case Terraria.ID.NPCID.EaterofWorldsBody:
                case Terraria.ID.NPCID.EaterofWorldsTail:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.EaterOfWorldsBossBag, 1);
                    return;
                case Terraria.ID.NPCID.BrainofCthulhu:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.BrainOfCthulhuBossBag, 1);
                    return;
                case Terraria.ID.NPCID.QueenBee:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.QueenBeeBossBag, 1);
                    return;
                case Terraria.ID.NPCID.SkeletronHead:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.SkeletronBossBag, 1);
                    return;
                case Terraria.ID.NPCID.Deerclops:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.DeerclopsBossBag, 1);
                    return;
                case Terraria.ID.NPCID.WallofFlesh:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.WallOfFleshBossBag, 1);
                    return;
                case Terraria.ID.NPCID.DukeFishron:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.FishronBossBag, 1);
                    return;
                case Terraria.ID.NPCID.QueenSlimeBoss:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.QueenSlimeBossBag, 1);
                    return;
                case Terraria.ID.NPCID.Retinazer:
                case Terraria.ID.NPCID.Spazmatism:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.TwinsBossBag, 1);
                    return;
                case Terraria.ID.NPCID.TheDestroyer:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.DestroyerBossBag, 1);
                    return;
                case Terraria.ID.NPCID.SkeletronPrime:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.SkeletronPrimeBossBag, 1);
                    return;
                case Terraria.ID.NPCID.Plantera:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.PlanteraBossBag, 1);
                    return;
                case Terraria.ID.NPCID.Golem:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.GolemBossBag, 1);
                    return;
                case Terraria.ID.NPCID.MoonLordCore:
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.MoonLordBossBag, 1);
                    return;
                case Terraria.ID.NPCID.HallowBoss: //eol 636
                    if (eventArgs.npc.AI_120_HallowBoss_IsGenuinelyEnraged()) // 检查当前是否处于白天末期（eol: end of the day），如果是的话，则丢弃Terraprisma
                    {
                        Terraria.Item.NewItem(new EntitySource_DebugCommand(), eventArgs.npc.Center, Terraria.ID.ItemID.EmpressBlade, 1);//5005 TerraPrisma
                    } // DropItemInstanced() 方法将通知每个客户端存在一个物品，但在服务器端不会占用一个活跃的物品槽位，因此该物品不会被覆盖，这样每个客户端都可以收集这个物品。
                    Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(eventArgs.npc, Terraria.ID.ItemID.FairyQueenBossBag, 1); //4782 eol Boss宝藏袋
                    return;
            }

        }
    }
    private void OnDropLoot(NpcLootDropEventArgs eventArgs)
    {
        // 与上述相同的条件
        if ((Terraria.Main.npc[eventArgs.NpcArrayIndex].boss || eventArgs.NpcId == Terraria.ID.NPCID.DD2Betsy) && Terraria.Main.GameMode == 0 && eventArgs.NpcId != Terraria.ID.NPCID.CultistBoss)
        {
            eventArgs.Handled = true; // 阻止游戏处理该事件。此举可防止NPC通过NPCLootOld()方法掉落战利品。金币、心形、法力之星以及治疗药水使用不同的方法，因此它们仍能保持正常行为。由于我们将用宝物袋替代战利品，所以取消了战利品掉落。
        }
    }
}