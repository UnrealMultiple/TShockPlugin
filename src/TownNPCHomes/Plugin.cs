using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace TownNPCHomes;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "棱镜 羽学优化";

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 5);

    private readonly ConcurrentDictionary<int, Vector2> npcHomePositions = new ConcurrentDictionary<int, Vector2>();

    public Plugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("tshock.world.movenpc", this.TeleportNpcToTheirHomesCmd, "npchome"));
        NPCHome += this.OnNpcHome;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.TeleportNpcToTheirHomesCmd);
            NPCHome -= this.OnNpcHome;
        }
        base.Dispose(disposing);
    }

    private void TeleportNpcToTheirHomesCmd(CommandArgs args)
    {
        var npcIdsToUpdate = new ConcurrentBag<int>();
        var npc = Main.npc;
        foreach (var val in npc)
        {
            if (val != null && !(!val.active || !val.townNPC) && !val.homeless)
            {
                var position = new Vector2(val.homeTileX * 16, val.homeTileY * 16);
                this.npcHomePositions.TryAdd(val.whoAmI, position);
                npcIdsToUpdate.Add(val.whoAmI);
            }
        }

        foreach (var id in npcIdsToUpdate)
        {
            if (this.npcHomePositions.TryGetValue(id, out var position))
            {
                this.TrySendNPCHomePosition(id, position);
            }
        }

        args.Player.SendSuccessMessage(GetString("已将所有城镇npc传送回家!"));
    }

    private void TrySendNPCHomePosition(int npcId, Vector2 position)
    {
        try
        {
            TSPlayer.All.SendData((PacketTypes) 23, "", npcId, position.X, position.Y, 0f, 0);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
            TShock.Log.ConsoleError(GetString($"发送NPC ({npcId}) 回家位置数据包时发生错误: {ex.Message}"));

        }
    }

    private void OnNpcHome(object? sender, NPCHomeChangeEventArgs args)
    {
        try
        {
            var val = Main.npc[args.ID];

            if (val == null)
            {
                args.Player.SendWarningMessage(GetString($"无法找到ID为{args.ID}的NPC，无法为其设置家的位置。"));
                return;
            }

            if (!val.homeless && !val.Bottom.Equals(new Vector2((float) args.X * 16, (float) args.Y * 16)))
            {
                val.Bottom = new Vector2((float) args.X * 16, (float) args.Y * 16);
                this.TrySendNPCHomePosition(args.ID, new Vector2((float) args.X * 16, (float) args.Y * 16));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
            args.Player.SendErrorMessage(GetString($"处理NPC回家事件时发生错误: {ex.Message}"));
        }
    }
}