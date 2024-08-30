using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BetterWhitelist;

[ApiVersion(2, 1)]
public class BetterWhitelist : TerrariaPlugin
{
    private static readonly string BetterWhitelistDir = Path.Combine(TShock.SavePath, "BetterWhitelist");

    private static readonly string ConfigPath = Path.Combine(BetterWhitelistDir, "config.json");

    private static BConfig _config = null!;

    public BetterWhitelist(Main game) : base(game)
    {
        Order = -100;
    }

    public override string Name => "BetterWhitelist";

    public override Version Version => new(2, 5);

    public override string Author => "豆沙，肝帝熙恩、Cai修改";

    public override string Description => "通过检查玩家姓名的玩家白名单";

    public override void Initialize()
    {
        string path = Path.Combine(TShock.SavePath, "BetterWhitelist");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        Load();

        Commands.ChatCommands.Add(new Command("bwl.use", BetterWhitelistCommand, "bwl"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == BetterWhitelistCommand);
        base.Dispose(disposing);
    }

    private void BetterWhitelistCommand(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage("用法: 输入 /bwl help 显示帮助信息.");
            return;
        }

        string command = args.Parameters[0];

        switch (command.ToLowerInvariant())
        {
            case "help":
                args.Player.SendInfoMessage("-------[BetterWhitelist]-------");
                args.Player.SendInfoMessage("/bwl help, 显示帮助信息\n" +
                                            "/bwl add {name}, 添加玩家名到白名单中\n" +
                                            "/bwl del {name}, 将玩家移出白名单\n" +
                                            "/bwl list, 显示白名单上的全部玩家\n" +
                                            "/bwl true, 启用插件\n" +
                                            "/bwl false, 关闭插件\n" +
                                            "/bwl reload, 重载插件");
                break;

            case "list":
                foreach (string msg in _config.WhitePlayers) args.Player.SendInfoMessage(msg);
                break;

            case "false":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage("禁用失败! 插件已是关闭状态");
                }
                else
                {
                    _config.Disabled = true;
                    args.Player.SendSuccessMessage("禁用成功!");
                    File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                }

                break;

            case "true":
                if (!_config.Disabled)
                {
                    args.Player.SendErrorMessage("启用失败! 插件已是打开状态");
                }
                else
                {
                    _config.Disabled = false;
                    args.Player.SendSuccessMessage("启用成功!");
                    foreach (TSPlayer tsPlayer in TShock.Players.Where(p =>
                                 p != null && !_config.WhitePlayers.Contains(p.Name)))
                        tsPlayer.Disconnect(_config.NotInWhiteList);
                    File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                }

                break;

            case "add":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage("插件开关已被禁用，请检查配置文件!");
                }
                else
                {
                    if (args.Parameters.Count == 1)
                    {
                        args.Player.SendSuccessMessage("用法错误!正确用法: /bwl add <玩家名>");
                        return;
                    }

                    string playerNameToAdd = args.Parameters[1];

                    if (playerNameToAdd != null && !_config.WhitePlayers.Contains(playerNameToAdd))
                    {
                        _config.WhitePlayers.Add(playerNameToAdd);
                        args.Player.SendSuccessMessage("添加成功!");
                        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                    }
                    else
                    {
                        args.Player.SendSuccessMessage("添加失败! 该玩家已经在白名单中");
                    }
                }

                break;

            case "reload":
                Load();
                args.Player.SendSuccessMessage("重载成功!");
                break;

            case "del":
                if (_config.Disabled)
                {
                    args.Player.SendErrorMessage("插件开关已被禁用，请检查配置文件!");
                }
                else
                {
                    if (args.Parameters.Count == 1)
                    {
                        args.Player.SendSuccessMessage("用法错误!正确用法: /bwl del <玩家名>");
                        return;
                    }

                    string playerNameToDelete = args.Parameters[1];
                    if (playerNameToDelete != null && _config.WhitePlayers.Contains(playerNameToDelete))
                    {
                        _config.WhitePlayers.Remove(playerNameToDelete);
                        args.Player.SendSuccessMessage("删除成功!");
                        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                        TShock.Players
                            .Where(p => p != null && p.Name == playerNameToDelete)
                            .ForEach(p => p.Disconnect(_config.NotInWhiteList));
                    }
                }

                break;
        }
    }


    private void Load()
    {
        _config = BConfig.Load(ConfigPath);
        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
    }
}