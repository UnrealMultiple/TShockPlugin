using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BetterWhitelist;

[ApiVersion(2, 1)]
public class BetterWhitelist : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 6, 4);

    public override string Author => "豆沙，肝帝熙恩、Cai修改";

    public override string Description => GetString("通过检查玩家姓名的玩家白名单");
    public BetterWhitelist(Main game) : base(game)
    {
    }
    public override void Initialize()
    {
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        Commands.ChatCommands.Add(new Command("bwl.use", this.BetterWhitelistCommand, "bwl"));
    }

    private void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];

        if (BConfig.Instance.Enable && !BConfig.Instance.WhitePlayers.Contains(player.Name))
        {
            player.Disconnect(BConfig.Instance.NotInWhiteList);
        }
        else if (!BConfig.Instance.Enable)
        {
            TShock.Log.ConsoleInfo(GetString("[BetterWhitelist] 开关已被禁用，请检查配置文件!"));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
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
                                            GetString("/bwl false, 关闭插件\n"));
                break;

            case "list":
                foreach (var msg in BConfig.Instance.WhitePlayers)
                {
                    args.Player.SendInfoMessage(msg);
                }

                break;

            case "false":
                if (!BConfig.Instance.Enable)
                {
                    args.Player.SendErrorMessage(GetString("[BetterWhitelist] 插件已是关闭状态"));
                }
                else
                {
                    BConfig.Instance.Enable = false;
                    args.Player.SendSuccessMessage(GetString("[BetterWhitelist] 白名单禁用成功!"));
                    BConfig.Save();
                }

                break;

            case "true":
                if (BConfig.Instance.Enable)
                {
                    args.Player.SendErrorMessage(GetString("[BetterWhitelist] 白名单已是打开状态"));
                }
                else
                {
                    BConfig.Instance.Enable = true;
                    args.Player.SendSuccessMessage(GetString("[BetterWhitelist] 白名单启用成功!"));
                    foreach (var tsPlayer in TShock.Players.Where(p =>
                                 p != null && !BConfig.Instance.WhitePlayers.Contains(p.Name)))
                    {
                        tsPlayer.Disconnect(BConfig.Instance.NotInWhiteList);
                    }

                    BConfig.Save();
                }

                break;

            case "add":
                if (!BConfig.Instance.Enable)
                {
                    args.Player.SendErrorMessage(GetString("[BetterWhitelist] 开关已被禁用，请检查配置文件!"));
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
                if (!BConfig.Instance.Enable)
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