using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Net;
using Lagrange.XocMat.Adapter.Protocol.Action;
using Lagrange.XocMat.Adapter.Protocol.Action.Receive;
using Lagrange.XocMat.Adapter.Protocol.Action.Response;
using Lagrange.XocMat.Adapter.Protocol.Internet;
using Terraria;
using Terraria.IO;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace Lagrange.XocMat.Adapter;

public class ActionHandler
{
    public delegate void ActionHandlerInvoker(BaseAction action);

    private static readonly Dictionary<ActionType, ActionHandlerInvoker> _action = new()
    {
        { ActionType.DeadRank , DeadRankHandler },
        { ActionType.OnlineRank , OnlineRankHandler },
        { ActionType.WorldMap , WorldMapHandler },
        { ActionType.GameProgress , GameProgressHandler },
        { ActionType.UpLoadWorld , UploadWorldHandler },
        { ActionType.Inventory , InventoryHandler },
        { ActionType.ResetServer , RestServerHandler },
        { ActionType.ServerOnline , ServerOnlineHandler },
        { ActionType.RegisterAccount , RegisterAccountHandler },
        { ActionType.PluginMsg , PluginMsgHandler },
        { ActionType.PrivateMsg , PrivateMsgHandler },
        { ActionType.Command , CommandHandler },
        { ActionType.ReStartServer , ReStartServerHandler },
        { ActionType.ServerStatus , ServerStatusHandler },
        { ActionType.ResetPassword , ResetPasswordHandler },
        { ActionType.ConnectStatus, ConnectStatusHandler },
        { ActionType.Account, AccountHandler },
        { ActionType.PlayerStrikeBoss, StrikeBossHandler },
        { ActionType.ExportPlayer, ExportPlayerHandler }
    };

    private static void ExportPlayerHandler(BaseAction action)
    {
        if (action is not ExportPlayerArgs data) return;
        if (data.Names == null || !data.Names.Any())
        {
            data.Names = TShock.UserAccounts.GetUserAccounts().Select(x => x.Name).ToList();
        }

        var res = new ExportPlayer()
        {
            Status = true,
            Echo = data.Echo,
            Message = "导出成功"
        };
        var playerfiles = new List<PlayerFile>();
        foreach (var name in data.Names)
        {
            var playerfile = new PlayerFile()
            {
                Name = name
            };
            var account = TShock.UserAccounts.GetUserAccountByName(name);
            if (account == null)
            {
                playerfile.Active = false;
            }
            else
            {
                var playerData = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), account.ID);
                var player = Utils.CreateAPlayer(name, playerData);
                try
                {
                    playerfile.Buffer = Utils.ExportPlayer(player);
                }
                catch
                {
                    continue;
                }
            }
            playerfiles.Add(playerfile);
        }
        res.PlayerFiles = playerfiles;
        ResponseAction(res);
    }

    private static void StrikeBossHandler(BaseAction action)
    {
        var res = new PlayerStrikeBoss()
        {
            Echo = action.Echo,
            Status = true,
            Message = "查询成功",
            Damages = Plugin.DamageBoss.Values.ToList(),
        };

        ResponseAction(res);
    }

    private static void AccountHandler(BaseAction action)
    {
        if (action is not QueryAccountArgs data) return;
        var res = new QueryAccount()
        {
            Status = true,
            Echo = data.Echo,
            Message = "查询成功"
        };
        if (string.IsNullOrEmpty(data.Target))
        {
            TShock.UserAccounts.GetUserAccounts().ForEach(account =>
            {
                res.Accounts.Add(new Account()
                {
                    Group = account.Group,
                    Name = account.Name,
                    IP = account.KnownIps,
                    UUID = account.UUID,
                    ID = account.ID,
                    Password = account.Password,
                    RegisterTime = account.Registered,
                    LastLoginTime = account.LastAccessed
                });
            });
        }
        else
        {
            var target = TShock.UserAccounts.GetUserAccountByName(data.Target);
            if (target != null)
            {
                res.Accounts.Add(new Account()
                {
                    Group = target.Group,
                    Name = target.Name,
                    IP = target.KnownIps,
                    UUID = target.UUID,
                    ID = target.ID,
                    Password = target.Password,
                    RegisterTime = target.Registered,
                    LastLoginTime = target.LastAccessed
                });
            }
            else
            {
                res.Message = "目标用户不存在!";
                res.Status = false;
            }

        }
        ResponseAction(res);
    }

    private static void ConnectStatusHandler(BaseAction action)
    {
        if (action is not SocketConnectStatusArgs data) return;
        switch (data.Status)
        {
            case SocketConnentType.Success:
                TShock.Log.ConsoleInfo($"[Lagrange.XocMat.Adapter] 与({data.ServerName})服务器验证成功，成功连接到XocMat机器人...");
                break;
            case SocketConnentType.VerifyError:
                TShock.Log.ConsoleError($"[Lagrange.XocMat.Adapter] 与({data.ServerName})服务器的通信令牌验证失败...");
                break;
            case SocketConnentType.ServerNull:
                TShock.Log.ConsoleError($"[Lagrange.XocMat.Adapter] 无法在XocMat机器人上找到({data.ServerName})服务器...");
                break;
            default:
                TShock.Log.ConsoleError("[Lagrange.XocMat.Adapter] 因未知错误无验证通信令牌...");
                break;
        }
        ResponseAction(new BaseActionResponse()
        {
            Status = true
        });
    }

    private static void ResetPasswordHandler(BaseAction action)
    {
        if (action is not PlayerPasswordResetArgs data) return;
        var msg = "更改成功";
        var status = true;
        var account = new UserAccount()
        {
            Name = data.Name
        };
        try
        {
            TShock.UserAccounts.SetUserAccountPassword(account, data.Password);
        }
        catch (UserAccountNotExistException)
        {
            msg = "所更改的玩家账户不存在!";
            status = false;
        }
        catch (UserAccountManagerException)
        {
            msg = $"尝试更改{data.Name} 的密码失败，原因不明,请查看服务器控制台了解详情。";
            status = false;
        }
        catch (ArgumentOutOfRangeException)
        {
            msg = $"密码必须不少于{TShock.Config.Settings.MinimumPasswordLength}个字符";
            status = false;
        }
        var res = new BaseActionResponse()
        {
            Status = status,
            Message = msg,
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    public static void Adapter(BaseAction action)
    {
        if (_action.TryGetValue(action.ActionType, out var Handler))
        {
            Handler(action);
        }
    }

    private static void ResponseAction<T>(T obj) where T : BaseAction
    {
        obj.MessageType = PostMessageType.Action;
        Plugin.Services.SendMessage(Utils.SerializeObj(obj));
    }

    private static void ServerStatusHandler(BaseAction action)
    {
        var res = new ServerStatus()
        {
            Status = true,
            Message = "获取成功",
            Echo = action.Echo,
            WorldName = Main.worldName,
            WorldID = Main.worldID,
            WorldMode = Main.GameMode,
            WorldSeed = Main.ActiveWorldFileData.SeedText,
            Plugins = ServerApi.Plugins.Select(x => new PluginInfo()
            {
                Name = x.Plugin.Name,
                Author = x.Plugin.Author,
                Description = x.Plugin.Description,
            }).ToList(),
            WorldHeight = Main.maxTilesY,
            WorldWidth = Main.maxTilesX,
            TShockPath = Environment.CurrentDirectory,
            RunTime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime
        };
        ResponseAction(res);
    }

    private static void ReStartServerHandler(BaseAction action)
    {
        if (action is not ReStartServerArgs data) return;
        var res = new BaseActionResponse()
        {
            Status = true,
            Message = "正在进行重启",
            Echo = action.Echo
        };
        ResponseAction(res);
        Utils.ReStarServer(data.StartArgs, true);
    }

    private static void CommandHandler(BaseAction action)
    {
        if (action is not ServerCommandArgs data) return;
        var player = new OneBotPlayer("XocMatBot");
        Commands.HandleCommand(player, data.Text);
        var res = new ServerCommand(player.CommandOutput)
        {
            Status = true,
            Message = "执行成功",
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    private static void PrivateMsgHandler(BaseAction action)
    {
        if (action is not PrivatMsgArgs data) return;
        TShock.Players.FirstOrDefault(x => x != null && x.Name == data.Name && x.Active)
            ?.SendMessage(data.Text, data.Color[0], data.Color[1], data.Color[2]);
        var res = new BaseActionResponse()
        {
            Status = true,
            Message = "发送成功",
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    private static void PluginMsgHandler(BaseAction action)
    {
        if (action is not BroadcastArgs data) return;
        TShock.Utils.Broadcast(data.Text, data.Color[0], data.Color[1], data.Color[2]);
        var res = new BaseActionResponse()
        {
            Status = true,
            Message = "发送成功",
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    private static void RegisterAccountHandler(BaseAction action)
    {
        var res = new BaseActionResponse()
        {
            Echo = action.Echo
        };
        if (action is not RegisterAccountArgs data) return;
        try
        {
            var account = new UserAccount()
            {
                Name = data.Name,
                Group = data.Group
            };
            account.CreateBCryptHash(data.Password);
            TShock.UserAccounts.AddUserAccount(account);
            res.Status = true;
            res.Message = "注册成功";
        }
        catch (Exception ex)
        {
            res.Status = false;
            res.Message = ex.Message;
        }
        ResponseAction(res);
    }

    private static void ServerOnlineHandler(BaseAction action)
    {
        var players = TShock.Players.Where(x => x != null && x.Active).Select(x => new PlayerInfo(x)).ToList();
        var res = new ServerOnline(players)
        {
            Status = true,
            Message = "查询成功",
            Echo = action.Echo
        };
        ResponseAction(res);
    }

    private static void RestServerHandler(BaseAction action)
    {
        if (action is not ResetServerArgs data) return;
        var res = new BaseActionResponse()
        {
            Status = true,
            Message = "正在重置",
            Echo = data.Echo
        };
        ResponseAction(res);
        Utils.RestServer(data);
    }

    private static void InventoryHandler(BaseAction action)
    {
        if (action is not QueryPlayerInventoryArgs data) return; ;
        var inventory = Utils.BInvSee(data.Name);
        var res = new PlayerInventory(inventory)
        {
            Status = inventory != null,
            Message = "",
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    private static void UploadWorldHandler(BaseAction action)
    {
        WorldFile.SaveWorld();
        var buffer = File.ReadAllBytes(Main.worldPathName);
        var res = new UpLoadWorldFile()
        {
            Status = true,
            Message = "成功",
            Echo = action.Echo,
            WorldBuffer = buffer,
            WorldName = Main.worldName
        };
        ResponseAction(res);
    }

    private static void GameProgressHandler(BaseAction action)
    {
        var res = new GameProgress(Utils.GetGameProgress())
        {
            Status = true,
            Message = "进度查询成功",
            Echo = action.Echo
        };
        ResponseAction(res);
    }

    private static void WorldMapHandler(BaseAction action)
    {
        if (action is not MapImageArgs data) return;
        var buffer = Utils.CreateMapBytes(data.ImageType);
        var res = new MapImage(buffer)
        {
            Status = true,
            Message = "地图生成成功",
            Echo = data.Echo
        };
        ResponseAction(res);
    }

    private static void OnlineRankHandler(BaseAction action)
    {
        var res = new PlayerOnlineRank(Plugin.Onlines)
        {
            Status = true,
            Message = "在线排行查询成功",
            Echo = action.Echo
        };
        ResponseAction(res);
    }

    private static void DeadRankHandler(BaseAction action)
    {
        var res = new DeadRank(Plugin.Deaths)
        {
            Status = true,
            Message = "死亡排行查询成功",
            Echo = action.Echo
        };
        ResponseAction(res);
    }
}
