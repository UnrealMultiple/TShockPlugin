using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Ezperm;

[ApiVersion(2, 1)]
public class Ezperm : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "大豆子,肝帝熙恩优化1449";
    public override string Description => GetString("一个指令帮助小白给初始服务器添加缺失的权限，还可以批量添删权限");
    public override Version Version => new Version(1, 2, 7);
    internal static Configuration Config = null!;
    public Ezperm(Main game) : base(game)
    {

    }
    private static void LoadConfig()
    {

        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);

    }
    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage("[Ezperm] 重新加载配置完毕。");
    }
    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        Commands.ChatCommands.Add(new Command("inperms.admin", this.Cmd, "inperms", "批量改权限"));
        LoadConfig();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.Cmd);
            GeneralHooks.ReloadEvent -= ReloadConfig;
        }
        base.Dispose(disposing);
    }

    private void Cmd(CommandArgs args)
    {
        foreach (var groupInfo in Config.Groups)
        {
            var groupName = groupInfo.Name;
            if (TShock.Groups.GetGroupByName(groupName) == null)
            {
                try
                {
                    TShock.Groups.AddGroup(groupName, null, "", Group.defaultChatColor);
                    args.Player.SendSuccessMessage(GetString($"组 {groupName} 不存在，已创建该组。") + "\r\n");
                }
                catch (GroupManagerException ex)
                {
                    args.Player.SendErrorMessage(ex.ToString());
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