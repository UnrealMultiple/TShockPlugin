using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BanNpc;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "Patrikk,GK 改良";

    public override string Description => GetString("禁止指定怪物的出没");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 7);

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        this.addCommands.Add(new Command("bannpc.use", this.BanCommand, "bm"));
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
        ServerApi.Hooks.NpcTransform.Register(this, this.OnTransform);
        base.Initialize();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnSpawn);
            ServerApi.Hooks.NpcTransform.Deregister(this, this.OnTransform);
        }

        // Call the base class dispose method.
        base.Dispose(disposing);
    }

    private void BanCommand(CommandArgs args)
    {

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            if (Config.Instance.Npcs.Count < 1)
            {
                args.Player.SendInfoMessage(GetString("当前阻止表为空."));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("阻止怪物表: ") + string.Join(", ", Config.Instance.Npcs.Select(x => TShock.Utils.GetNPCById(x)?.FullName + "({0})".SFormat(x))));
            }

            return;
        }
        else if (args.Parameters.Count == 2)
        {
            NPC npc;
            var matchedNPCs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);
            if (matchedNPCs.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("无效NPC: {0} !"), args.Parameters[1]);
                return;
            }
            else if (matchedNPCs.Count > 1)
            {
                args.Player.SendMultipleMatchError(matchedNPCs.Select(i => i.FullName));
                return;
            }
            else
            {
                npc = matchedNPCs[0];
            }
            switch (args.Parameters[0].ToLower())
            {
                case "add":
                {
                    if (Config.Instance.Npcs.Contains(npc.netID))
                    {
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 已在阻止列表中!"), npc.netID);
                        return;
                    }
                    Config.Instance.Npcs.Add(npc.netID);
                    Config.Save();
                    args.Player.SendSuccessMessage(GetString("已成功将NPC ID添加到阻止列表: {0}!"), npc.netID);
                    break;
                }
                case "delete":
                case "del":
                case "remove":
                {
                    if (!Config.Instance.Npcs.Contains(npc.netID))
                    {
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 不在筛选列表中!"), npc.netID);
                        return;
                    }
                    Config.Instance.Npcs.Remove(npc.netID);
                    Config.Save();
                    args.Player.SendSuccessMessage(GetString("已成功从阻止列表中删除NPC ID: {0}!"), npc.netID);
                    break;
                }
                default:
                {
                    args.Player.SendErrorMessage(GetString("语法错误: /bm <add/del> [名称 或 ID]"));
                    break;
                }
            }
        }
        else
        {
            args.Player.SendInfoMessage("/bm");
            args.Player.SendInfoMessage("/bm list");
            args.Player.SendInfoMessage(GetString("/bm add [名称 或 ID]"));
            args.Player.SendInfoMessage(GetString("/bm del [名称 或 ID]"));
            return;
        }
    }
    private void OnTransform(NpcTransformationEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        if (Config.Instance.Npcs.Contains(Main.npc[args.NpcId].netID))
        {
            Main.npc[args.NpcId].active = false;
        }
    }
    private void OnSpawn(NpcSpawnEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        if (Config.Instance.Npcs.Contains(Main.npc[args.NpcId].netID))
        {
            args.Handled = true;
            Main.npc[args.NpcId].active = false;
        }
    }
}