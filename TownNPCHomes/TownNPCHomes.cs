using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public override string Author => "棱镜 羽学优化";

    public override string Name => "TownNPCHomes";

    public override Version Version => new Version(1, 1, 0);

    private ConcurrentDictionary<int, Vector2> npcHomePositions = new ConcurrentDictionary<int, Vector2>();

    public MainPlugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("tshock.world.movenpc", new CommandDelegate(TeleportNpcToTheirHomesCmd), new string[1] { "npchome" }));
        NPCHome += OnNpcHome;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NPCHome -= OnNpcHome;
        }
        base.Dispose(disposing);
    }

    private void TeleportNpcToTheirHomesCmd(CommandArgs args)
    {
        ConcurrentBag<int> npcIdsToUpdate = new ConcurrentBag<int>();
        NPC[] npc = Main.npc;
        foreach (NPC val in npc)
        {
            if (val != null && !(!val.active || !val.townNPC) && !val.homeless)
            {
                Vector2 position = new Vector2((float)(val.homeTileX * 16), (float)(val.homeTileY * 16));
                npcHomePositions.TryAdd(val.whoAmI, position);
                npcIdsToUpdate.Add(val.whoAmI);
            }
        }

        foreach (int id in npcIdsToUpdate)
        {
            Vector2 position;
            if (npcHomePositions.TryGetValue(id, out position))
            {
                TrySendNPCHomePosition(id, position);
            }
        }

        args.Player.SendSuccessMessage("已将所有城镇npc传送回家!");
    }

    private void TrySendNPCHomePosition(int npcId, Vector2 position)
    {
        try
        {
            TSPlayer.All.SendData((PacketTypes)23, "", npcId, position.X, position.Y, 0f, 0);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
            Console.WriteLine($"发送NPC ({npcId}) 回家位置数据包时发生错误: {ex.Message}");

        }
    }

    private void OnNpcHome(object sender, NPCHomeChangeEventArgs args)
    {
        try
        {
            NPC val = Main.npc[args.ID];

            if (val == null)
            {
                args.Player.SendSuccessMessage($"无法找到ID为{args.ID}的NPC，无法为其设置家的位置。");
                return;
            }

            if (!val.homeless && !val.Bottom.Equals(new Vector2((float)args.X * 16, (float)args.Y * 16)))
            {
                val.Bottom = new Vector2((float)args.X * 16, (float)args.Y * 16);
                TrySendNPCHomePosition(args.ID, new Vector2((float)args.X * 16, (float)args.Y * 16));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
            args.Player.SendSuccessMessage($"处理NPC回家事件时发生错误: {ex.Message}");
        }
    }
}
