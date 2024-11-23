using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BanNpc;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "Patrikk,GK 改良";

    public override string Description => "禁止指定怪物的出没";

    public override string Name => "禁止怪物";

    public override Version Version => new(1, 0, 0, 3);

    private static Config Config { get; set; } = new();

    private static string PATH => Path.Combine(TShock.SavePath, "禁止怪物表.json");

    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => this.LoadConfig();
    }
    private void LoadConfig()
    {
        try
        {
            if (File.Exists(PATH))
            {
                Config = Config.Read(PATH);
            }
            else
            {
                TShock.Log.ConsoleError(GetString("未找到禁止怪物表，已为您创建！\n修改配置后输入/bm reload可以应用新的配置。"));
            }
            Config.Write(PATH);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString("禁止怪物表读取错误:") + ex.ToString());
        }
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        this.LoadConfig();
        Commands.ChatCommands.Add(new Command("bannpc.use", this.BanCommand, "bm"));
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
        ServerApi.Hooks.NpcTransform.Register(this, this.OnTransform);
        GeneralHooks.ReloadEvent += this._reloadHandler;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.BanCommand);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnSpawn);
            ServerApi.Hooks.NpcTransform.Deregister(this, this.OnTransform);
            GeneralHooks.ReloadEvent -= this._reloadHandler;
        }

        // Call the base class dispose method.
        base.Dispose(disposing);
    }

    private void BanCommand(CommandArgs args)
    {

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            if (Config.Npcs.Count < 1)
            {
                args.Player.SendInfoMessage(GetString("当前阻止表为空."));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("阻止怪物表: ") + string.Join(", ", Config.Npcs.Select(x => TShock.Utils.GetNPCById(x)?.FullName + "({0})".SFormat(x))));
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
                    if (Config.Npcs.Contains(npc.netID))
                    {
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 已在阻止列表中!"), npc.netID);
                        return;
                    }
                    Config.Npcs.Add(npc.netID);
                    Config.Write(PATH);
                    args.Player.SendSuccessMessage(GetString("已成功将NPC ID添加到阻止列表: {0}!"), npc.netID);
                    break;
                }
                case "delete":
                case "del":
                case "remove":
                {
                    if (!Config.Npcs.Contains(npc.netID))
                    {
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 不在筛选列表中!"), npc.netID);
                        return;
                    }
                    Config.Npcs.Remove(npc.netID);
                    Config.Write(PATH);
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

        if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
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

        if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
        {
            args.Handled = true;
            Main.npc[args.NpcId].active = false;
        }
    }
}