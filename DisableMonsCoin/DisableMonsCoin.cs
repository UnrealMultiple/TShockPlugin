using Terraria;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class DisableMonsCoin : TerrariaPlugin
{
    // 插件的名称。
    public override string Name => "禁止怪物掉钱";
    // 插件的当前版本。
    public override Version Version => new Version(1, 0, 0);
    // 插件的作者。
    public override string Author => "羽学";
    // 插件用途的简短一行描述。
    public override string Description => "清理怪物身边的钱币";

    public NPC RealNPC { get; set; }
    private int radius;

    // 插件的构造函数
    // 在这里设置插件的顺序（可选）和任何其他构造函数逻辑
    public DisableMonsCoin(Main game) : base(game)
    {
    }

    // 执行插件初始化逻辑。
    // 在这里添加你的钩子、配置文件读写等。
    public override void Initialize()
    {
        //On.Terraria.NPC.NPCLoot_DropMoney += OnDropMoney; //西江提供的方法
        ServerApi.Hooks.NpcKilled.Register(this, NPC_Killed);
    }

    /*
    //西江提供的清理掉钱方法
    private void OnDropMoney(On.Terraria.NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
    {
        for (int i = 0; i < Terraria.Main.maxItems; i++)
        {
            var item = Terraria.Main.item[i];
            float dX = item.position.X - RealNPC.position.X;
            float dY = item.position.Y - RealNPC.position.Y;
            if (item.active == true && (item.netID == 71 || item.netID == 72 || item.netID == 73 || item.netID == 74) && dX * dX + dY * dY <= radius * radius * 256f)
            {
                Terraria.Main.item[i].active = false;
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", i);
            }
        }
    }
    */

    // 当NPC被杀死时触发此方法
    private void NPC_Killed(NpcKilledEventArgs args)
    {
        RealNPC = args.npc; // 初始化RealNPC引用
        ClearCoins(10); // 示例：默认清理半径为10格
    }


    // 清理怪物周围钱币的方法
    public void ClearCoins(int radius)
    {
        for (int i = 0; i < Terraria.Main.maxItems; i++)
        {
            var item = Terraria.Main.item[i];
            float dX = item.position.X - RealNPC.position.X;
            float dY = item.position.Y - RealNPC.position.Y;
            if (item.active == true && (item.netID == 71 || item.netID == 72 || item.netID == 73 || item.netID == 74) && dX * dX + dY * dY <= radius * radius * 256f)
            {
                Terraria.Main.item[i].active = false;
                TSPlayer.All.SendData(PacketTypes.ItemDrop, "", i);
            }
        }
    }

    // 执行插件清理逻辑
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //On.Terraria.NPC.NPCLoot_DropMoney += OnDropMoney;
            ServerApi.Hooks.NpcKilled.Register(this, NPC_Killed);
        }
        base.Dispose(disposing);
    }
}