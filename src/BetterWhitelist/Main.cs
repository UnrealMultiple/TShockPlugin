using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BetterWhitelist;

[ApiVersion(2, 1)]
public class BetterWhitelist : LazyPlugin
{
    public BetterWhitelist(Main game) : base(game)
    {
    }

    public override string Name => "BetterWhitelist";

    public override Version Version => new(2, 6);

    public override string Author => "豆沙，肝帝熙恩、Cai修改";

    public override string Description => "通过检查玩家姓名的玩家白名单";

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("bwl.use", this.BetterWhitelistCommand, "bwl"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.BetterWhitelistCommand);
        }

        base.Dispose(disposing);
    }

    private void BetterWhitelistCommand(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("用法: 输入 /bwl help 显示帮助信息."));
            return;
        }

        var command = args.Parameters[0];

        switch (command.ToLowerInvariant())
        {
            case "help":
                args.Player.SendInfoMessage(GetString("-------[BetterWhitelist]-------"));
                args.Player.SendInfoMessage(GetString("/bwl help, 显示帮助信息\n") +
                                            GetString("/bwl add {name}, 添加玩家名到白名单中\n") +
                                            GetString("/bwl del {name}, 将玩家移出白名单\n") +
                                            GetString("/bwl list, 显示白名单上的全部玩家\n") +
                                            GetString("/bwl true, 启用插件\n") +
                                            GetString("/bwl false, 关闭插件\n") +
                                            GetString("/bwl reload, 重载插件"));
                break;

            case "list":
                foreach (var msg in BConfig.Instance.WhitePlayers)
                {
                    args.Player.SendInfoMessage(msg);
                }

                break;

            case "false":
                if (BConfig.Instance.Disabled)
                {
                    args.Player.SendErrorMessage(GetString("禁用失败! 插件已是关闭状态"));
                }
                else
                {
                    BConfig.Instance.Disabled = true;
                    args.Player.SendSuccessMessage(GetString("禁用成功!"));
                    BConfig.Save();
                }

                break;

            case "true":
                if (!BConfig.Instance.Disabled)
                {
                    args.Player.SendErrorMessage(GetString("启用失败! 插件已是打开状态"));
                }
                else
                {
                    BConfig.Instance.Disabled = false;
                    args.Player.SendSuccessMessage(GetString("启用成功!"));
                    foreach (var tsPlayer in TShock.Players.Where(p =>
                                 p != null && !BConfig.Instance.WhitePlayers.Contains(p.Name)))
                    {
                        tsPlayer.Disconnect(BConfig.Instance.NotInWhiteList);
                    }

                    BConfig.Save(); 
                }

                break;

            case "add":
                if (BConfig.Instance.Disabled)
                {
                    args.Player.SendErrorMessage(GetString("插件开关已被禁用，请检查配置文件!"));
                }
                else
                {
                    if (args.Parameters.Count == 1)
                    {
                        args.Player.SendSuccessMessage(GetString("用法错误!正确用法: /bwl add <玩家名>"));
                        return;
                    }

                    var playerNameToAdd = args.Parameters[1];

                    if (playerNameToAdd != null && !BConfig.Instance.WhitePlayers.Contains(playerNameToAdd))
                    {
                        BConfig.Instance.WhitePlayers.Add(playerNameToAdd);
                        args.Player.SendSuccessMessage(GetString("添加成功!"));
                        BConfig.Save();
                    }
                    else
                    {
                        args.Player.SendSuccessMessage(GetString("添加失败! 该玩家已经在白名单中"));
                    }
                }

                break;
            case "del":
                if (BConfig.Instance.Disabled)
                {
                    args.Player.SendErrorMessage(GetString("插件开关已被禁用，请检查配置文件!"));
                }
                else
                {
                    if (args.Parameters.Count == 1)
                    {
                        args.Player.SendSuccessMessage(GetString("用法错误!正确用法: /bwl del <玩家名>"));
                        return;
                    }

                    var playerNameToDelete = args.Parameters[1];
                    if (playerNameToDelete != null && BConfig.Instance.WhitePlayers.Contains(playerNameToDelete))
                    {
                        BConfig.Instance.WhitePlayers.Remove(playerNameToDelete);
                        args.Player.SendSuccessMessage(GetString("删除成功!"));
                        BConfig.Save();
                        TShock.Players
                            .Where(p => p != null && p.Name == playerNameToDelete)
                            .ForEach(p => p.Disconnect(BConfig.Instance.NotInWhiteList));
                    }
                }

                break;
        }
    }
}