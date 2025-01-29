using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace PersonalPermission;

[ApiVersion(2, 1)]
public class PPMain : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 0, 5);
    public override string Author => "Megghy，肝帝熙恩更新1449";
    public override string Description => GetString("允许为玩家单独设置权限.");
    public PPMain(Main game) : base(game)
    {
    }
    public override void Initialize()
    {
        DB.TryCreateTable();
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin, int.MinValue);
        PlayerHooks.PlayerPermission += this.OnPermissionCheck;
        GeneralHooks.ReloadEvent += this.OnReload;
        Commands.ChatCommands.Add(new Command("personalpermission.admin", this.PPCommand, "pp"));
    }
    protected override void Dispose(bool disposing)
    {
        ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
        PlayerHooks.PlayerPermission -= this.OnPermissionCheck;
        GeneralHooks.ReloadEvent -= this.OnReload;
        Commands.ChatCommands.RemoveAll(p => p.CommandDelegate == this.PPCommand);
    }
    void OnJoin(JoinEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        var account = TShock.UserAccounts.GetUserAccountByName(plr.Name);
        plr.SetData("PersonalPermission", this.GetPPGroup(account == null ? new List<string>() : DB.GetPermissions(account.ID)));
    }
    void OnReload(ReloadEventArgs args)
    {
        TShock.Players.Where(p => p != null && p.Account != null).ForEach(p => p.SetData("PersonalPermission", this.GetPPGroup(DB.GetPermissions(p.Account.ID))));
        TShock.Log.ConsoleInfo(GetString("已重载玩家个人权限."));
    }
    Group GetPPGroup(List<string> list)
    {
        var group = new Group("PersonalPermission");
        list?.ForEach(p => group.AddPermission(p));
        return group;
    }
    public void OnPermissionCheck(PlayerPermissionEventArgs args)
    {
        try
        {
            var plr = args.Player;
            if (!plr.RealPlayer || plr.Group.TotalPermissions.Contains("*") || plr.Group.Name == "superadmin")
            {
                args.Result = PermissionHookResult.Granted;
                return;
            }
            var group = args.Player.GetData<Group>("PersonalPermission");
            group.Parent = args.Player.Group;
            args.Result = args.Player.GetData<Group>("PersonalPermission").HasPermission(args.Permission)
                ? PermissionHookResult.Granted
                : PermissionHookResult.Denied;
        }
        catch (Exception ex) { TShock.Log.ConsoleError(ex.Message); args.Result = PermissionHookResult.Denied; }
    }
    public void PPCommand(CommandArgs args)
    {
        var plr = args.Player;
        var cmd = args.Parameters;
        try
        {

            if (cmd.Count >= 1)
            {
                switch (cmd[0].ToLower())
                {
                    case "add":
                        if (cmd.Count >= 3)
                        {
                            var acclist = TShock.UserAccounts.GetUserAccountsByName(cmd[1]);
                            if (acclist.Count > 1)
                            {
                                plr.SendMultipleMatchError(acclist);
                            }
                            else if (acclist.Count == 0)
                            {
                                plr.SendErrorMessage(GetString($"未找到名称中包含 {cmd[1]} 的玩家."));
                            }
                            else
                            {
                                Add(acclist.First(), cmd[2]);
                            }
                        }
                        else
                        {
                            plr.SendInfoMessage(GetString("无效的命令. /pp add [C/66E5B5:玩家名] [C/66E5B5:权限]"));
                        }
                        break;
                    case "del":
                        if (cmd.Count >= 3)
                        {
                            var acclist = TShock.UserAccounts.GetUserAccountsByName(cmd[1]);
                            if (acclist.Count > 1)
                            {
                                plr.SendMultipleMatchError(acclist);
                            }
                            else if (acclist.Count == 0)
                            {
                                plr.SendErrorMessage(GetString($"未找到名称中包含 {cmd[1]} 的玩家."));
                            }
                            else
                            {
                                Del(acclist.First(), cmd[2]);
                            }
                        }
                        else
                        {
                            plr.SendInfoMessage(GetString($"无效的命令. /pp del [C/66E5B5:玩家名] [C/66E5B5:权限]"));
                        }
                        break;
                    case "list":
                        if (cmd.Count >= 2)
                        {
                            var acclist = TShock.UserAccounts.GetUserAccountsByName(cmd[1]);
                            if (acclist.Count > 1)
                            {
                                plr.SendMultipleMatchError(acclist);
                            }
                            else if (acclist.Count == 0)
                            {
                                plr.SendErrorMessage(GetString($"未找到名称中包含 {cmd[1]} 的玩家."));
                            }
                            else
                            {
                                var data = DB.GetPermissions(acclist[0].ID);
                                plr.SendInfoMessage(GetString($"玩家 {acclist[0].Name} 的所有权限: ({data.Count} 条)\n{string.Join(", ", data)}"));
                            }
                        }
                        else
                        {
                            plr.SendInfoMessage(GetString("无效的命令. /pp list [C/66E5B5:玩家名]"));
                        }
                        break;
                    case "search":
                        if (cmd.Count >= 2)
                        {
                            var list = new List<string>();
                            DB.GetAllPermissions().Where(data => data.Value.Contains(cmd[1])).ForEach(data => list.Add((TShock.UserAccounts.GetUserAccountByID(data.Key) ?? new UserAccount()).Name));
                            plr.SendInfoMessage(GetString($"拥有权限 {cmd[1]} 的所有玩家: ({list.Count} 位)\n{string.Join(", ", list)}"));
                        }
                        else
                        {
                            plr.SendInfoMessage(GetString("无效的命令. /pp search [C/66E5B5:权限名]"));
                        }
                        break;
                    default:
                        Help();
                        break;
                }
            }
            else
            {
                Help();
            }
        }
        catch (Exception ex) { TShock.Log.ConsoleError(ex.Message); }
        void Help()
        {
            plr.SendInfoMessage(GetString("可用命令:\n") +
                    GetString("/pp add [C/66E5B5:玩家名] [C/66E5B5:权限]\n") +
                    GetString("/pp del [C/66E5B5:玩家名] [C/66E5B5:权限]\n") +
                    GetString("/pp list [C/66E5B5:玩家名]\n") +
                    GetString("/pp search [C/66E5B5:权限名]\n"));
        }
        void Add(UserAccount account, string perm)
        {
            var data = DB.GetPermissions(account.ID);
            if (data != null && !data.Contains(perm))
            {
                data.Add(perm);
                if (DB.SetPermissions(account.ID, data))
                {
                    TShock.Players.FirstOrDefault(p => p?.Name == account.Name)?.GetData<Group>("PersonalPermission")?.AddPermission(perm);
                    plr.SendSuccessMessage(GetString($"已为玩家 {account.Name} 添加权限 {perm}."));
                }
                else
                {
                    plr.SendErrorMessage(GetString("添加权限失败."));
                }
            }
            else
            {
                plr.SendErrorMessage(GetString($"玩家 {account.Name} 已存在权限 {perm}."));
            }
        }
        void Del(UserAccount account, string perm)
        {
            var data = DB.GetPermissions(account.ID);
            if (data != null && data.Contains(perm))
            {
                data.Remove(perm);
                if (DB.SetPermissions(account.ID, data))
                {
                    TShock.Players.FirstOrDefault(p => p?.Name == account.Name)?.GetData<Group>("PersonalPermission")?.RemovePermission(perm);
                    plr.SendSuccessMessage(GetString($"已为玩家 {account.Name} 移除权限 {perm}."));
                }
                else
                {
                    plr.SendErrorMessage(GetString("移除权限失败."));
                }
            }
            else
            {
                plr.SendErrorMessage(GetString($"玩家 {account.Name} 未存在权限 {perm}."));
            }
        }
    }
}