using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Ezperm;

[ApiVersion(2, 1)]
public class Ezperm : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "大豆子,肝帝熙恩优化1449";
    public override string Description => GetString("一个指令帮助小白给初始服务器添加缺失的权限，还可以批量添删权限");
    public override Version Version => new Version(1, 2, 9);
    public Ezperm(Main game) : base(game)
    {

    }
    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("inperms.admin", this.Cmd, "inperms", "批量改权限"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.Cmd);
        }
        base.Dispose(disposing);
    }

    private void Cmd(CommandArgs args)
    {
        foreach (var groupInfo in Configuration.Instance.Groups)
        {
            var groupName = groupInfo.Name;

            // 检查组是否存在，如果不存在则创建
            if (TShock.Groups.GetGroupByName(groupName) == null)
            {
                try
                {
                    // 如果父组为空字符串，则不设置父组
                    var parentGroup = string.IsNullOrWhiteSpace(groupInfo.Parent) ? null : groupInfo.Parent;
                    TShock.Groups.AddGroup(groupName, parentGroup, "", Group.defaultChatColor);
                    args.Player.SendSuccessMessage(GetString($"组 {groupName} 不存在，已创建该组。") + "\r\n");
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.ToString());
                }
            }
            else
            {
                // 如果组已存在，更新父组
                if (!string.IsNullOrWhiteSpace(groupInfo.Parent))
                {
                    try
                    {
                        var group = TShock.Groups.GetGroupByName(groupName);
                        var newParentGroup = groupInfo.Parent;

                        // 如果父组为 "null"，则将父组设置为空
                        if (groupInfo.Parent.Equals("null", StringComparison.OrdinalIgnoreCase))
                        {
                            newParentGroup = null;
                        }

                        TShock.Groups.UpdateGroup(groupName, newParentGroup, group.Permissions, group.ChatColor, group.Suffix, group.Prefix);
                        args.Player.SendSuccessMessage(GetString($"成功将组 {groupName} 的父组设置为 {newParentGroup ?? "空"}。") + "\r\n");
                    }
                    catch (GroupManagerException ex)
                    {
                        args.Player.SendErrorMessage(ex.ToString());
                    }
                }
            }

            // 添加权限
            if (groupInfo.AddPermissions.Count > 0)
            {
                try
                {
                    var response = TShock.Groups.AddPermissions(groupName, groupInfo.AddPermissions);
                    if (response.Length > 0)
                    {
                        args.Player.SendSuccessMessage(GetString($"成功为组 {groupName} 添加权限: {string.Join(", ", groupInfo.AddPermissions)}") + "\r\n");
                    }
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.ToString());
                }
            }

            // 删除权限
            if (groupInfo.DelPermissions.Count > 0)
            {
                try
                {
                    var response = TShock.Groups.DeletePermissions(groupName, groupInfo.DelPermissions);
                    if (response.Length > 0)
                    {
                        args.Player.SendSuccessMessage(GetString($"成功为组 {groupName} 删除权限: {string.Join(", ", groupInfo.DelPermissions)}") + "\r\n\r\n");
                    }
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.Message);
                }
            }
        }
    }
}