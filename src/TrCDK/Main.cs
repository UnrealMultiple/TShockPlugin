using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace TrCDK;

[ApiVersion(2, 1)]
public class TrCDK : TerrariaPlugin
{
    public override string Author => "Jonesn";
    public override string Description => GetString("CDK系统");
    public override string Name => "TrCDK";
    public override Version Version => new Version(1, 0, 0, 0);
    public TrCDK(Main game) : base(game) { }

    public override void Initialize()
    {
        Data.Init();
        // 添加所有CDK相关指令
        Commands.ChatCommands.Add(new Command("cdk.use", this.UseCDK, "cdk"));
        Commands.ChatCommands.Add(new Command("cdk.admin.loadall", this.LoadAllCDK, "cdkloadall"));
        Commands.ChatCommands.Add(new Command("cdk.admin.add", this.AddCDKCmd, "cdkadd"));
        Commands.ChatCommands.Add(new Command("cdk.admin.del", this.DelCDKCmd, "cdkdel"));
        Commands.ChatCommands.Add(new Command("cdk.admin.update", this.UpdateCDKCmd, "cdkupdate"));
        Commands.ChatCommands.Add(new Command("cdk.admin.give", this.GiveCDKCmd, "cdkgive"));
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            Data.Close();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.UseCDK ||
                                                c.CommandDelegate == this.LoadAllCDK ||
                                                c.CommandDelegate == this.AddCDKCmd ||
                                                c.CommandDelegate == this.DelCDKCmd ||
                                                c.CommandDelegate == this.UpdateCDKCmd ||
                                                c.CommandDelegate == this.GiveCDKCmd);
        }
        base.Dispose(Disposing);
    }

    // 添加CDK指令
    private void AddCDKCmd(CommandArgs args)
    {
        if (args.Parameters.Count < 6)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:用法: /cdkadd <CDK名称> <使用次数> <过期时间> <组限制> <玩家限制> <指令>]"));
            args.Player.SendInfoMessage(GetString("[c/ffd700:过期时间格式: yyyy-MM-ddThh:mm (例: 2025-12-31T23:59)]"));
            args.Player.SendInfoMessage(GetString("[c/ffd700:组限制和玩家限制可以用逗号分隔多个，不限制则填 none]"));
            return;
        }

        var cdkName = args.Parameters[0];
        if (!int.TryParse(args.Parameters[1], out var useTime))
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:使用次数必须是数字！]"));
            return;
        }

        var utilTime = args.Parameters[2];
        var groupLimit = args.Parameters[3] == "none" ? "" : args.Parameters[3];
        var playerLimit = args.Parameters[4] == "none" ? "" : args.Parameters[4];

        // 组合剩余参数作为指令
        var cmds = string.Join(" ", args.Parameters.Skip(5));

        try
        {
            var timeForInfo = new DateTimeFormatInfo
            {
                ShortDatePattern = "yyyy-MM-ddThh:mm"
            };
            var time = Convert.ToDateTime(utilTime, timeForInfo);

            if (Data.Insert(cdkName, useTime, Convert.ToDateTime(time.ToString()).Ticks, groupLimit, playerLimit, cmds))
            {
                args.Player.SendSuccessMessage(GetString($"[c/00FF00:【CDK管理】]\n[c/ffd700:成功添加CDK: {cdkName}]"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:添加CDK失败，可能已存在同名CDK]"));
            }
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage(GetString($"[c/FF0000:【CDK管理】]\n[c/ffd700:时间格式错误: {ex.Message}]"));
        }
    }

    // 删除CDK指令
    private void DelCDKCmd(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:用法: /cdkdel <CDK名称>]"));
            return;
        }

        var cdkName = args.Parameters[0];
        Data.DelCDK(cdkName);
        args.Player.SendSuccessMessage(GetString($"[c/00FF00:【CDK管理】]\n[c/ffd700:已删除CDK: {cdkName}]"));
    }

    // 更新CDK指令
    private void UpdateCDKCmd(CommandArgs args)
    {
        if (args.Parameters.Count < 7)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:用法: /cdkupdate <CDK名称> <使用次数> <过期时间> <组限制> <玩家限制> <已使用玩家> <指令>]"));
            args.Player.SendInfoMessage(GetString("[c/ffd700:过期时间格式: yyyy-MM-ddThh:mm]"));
            args.Player.SendInfoMessage(GetString("[c/ffd700:不需要修改的项目可以填 none]"));
            return;
        }

        var cdkName = args.Parameters[0];
        if (!int.TryParse(args.Parameters[1], out var useTime))
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:使用次数必须是数字！]"));
            return;
        }

        var utilTime = args.Parameters[2];
        var groupLimit = args.Parameters[3] == "none" ? "" : args.Parameters[3];
        var playerLimit = args.Parameters[4] == "none" ? "" : args.Parameters[4];
        var used = args.Parameters[5] == "none" ? "" : args.Parameters[5];
        var cmds = string.Join(" ", args.Parameters.Skip(6));

        try
        {
            var timeForInfo = new DateTimeFormatInfo
            {
                ShortDatePattern = "yyyy-MM-ddThh:mm"
            };
            var time = Convert.ToDateTime(utilTime, timeForInfo);

            Data.Update(cdkName, useTime, Convert.ToDateTime(time.ToString()).Ticks, groupLimit, playerLimit, used, cmds);
            args.Player.SendSuccessMessage(GetString($"[c/00FF00:【CDK管理】]\n[c/ffd700:成功更新CDK: {cdkName}]"));
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage(GetString($"[c/FF0000:【CDK管理】]\n[c/ffd700:时间格式错误: {ex.Message}]"));
        }
    }

    // 加载所有CDK指令
    private void LoadAllCDK(CommandArgs args)
    {
        var allcdk = Data.GetAllData();
        args.Player.SendInfoMessage(GetString("[c/00FF00:【CDK管理】]\n[c/ffd700:=== 所有CDK列表 ===]"));

        foreach (var cdk in allcdk)
        {
            var cdkObj = (CDK) cdk;
            var expireTime = new DateTime(cdkObj.Utiltime);
            args.Player.SendInfoMessage(GetString($"[c/ffd700:CDK: {cdkObj.Cdkname} | 剩余次数: {cdkObj.Usetime} | 过期时间: {expireTime:yyyy-MM-dd HH:mm}]"));
        }
    }

    // 给予CDK奖励指令
    private void GiveCDKCmd(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:用法: /cdkgive <玩家名> <指令列表>]"));
            args.Player.SendInfoMessage(GetString("[c/ffd700:指令列表用逗号分隔，例: give [plr] 1 1,heal [plr]]"));
            return;
        }

        var playerName = args.Parameters[0];
        var cmds = string.Join(" ", args.Parameters.Skip(1));

        var players = TShockAPI.TSPlayer.FindByNameOrID(playerName);
        if (players.Count == 0)
        {
            args.Player.SendErrorMessage(GetString($"[c/FF0000:【CDK管理】]\n[c/ffd700:找不到玩家: {playerName}]"));
            return;
        }

        if (players.Count > 1)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:找到多个匹配的玩家，请使用更精确的名称]"));
            return;
        }

        var targetPlayer = players[0];
        if (cmds != "")
        {
            var cmdArray = cmds.Split(',');
            foreach (var cmd in cmdArray)
            {
                var processedCmd = cmd.Replace("[plr]", targetPlayer.Name).Trim();
                targetPlayer.PermissionlessInvoke(processedCmd);
            }
            args.Player.SendSuccessMessage(GetString($"[c/00FF00:【CDK管理】]\n[c/ffd700:已给予玩家 {targetPlayer.Name} CDK奖励]"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:指令不能为空]"));
        }
    }

    // 使用CDK指令（玩家用）
    private void UseCDK(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:指令:/cdk CDK兑换码  -  兑换一个CDK礼包]"));
        }
        else
        {
            var cdk1 = Data.GetData(args.Parameters[0]);
            if (cdk1 == null)
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:CDK不存在或已失效]"));
                return;
            }

            if (cdk1.Playerlimit != "")
            {
                if (!cdk1.Playerlimit.Contains(args.Player.Name))
                {
                    args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你不在该CDK的领取名单中]"));
                    return;
                }
            }
            if (cdk1.Grouplimit != "")
            {
                if (!cdk1.Grouplimit.Contains(args.Player.Group.Name))
                {
                    args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你所在的组不能领取该CDK]"));
                    return;
                }
            }
            if (cdk1.Usetime < 1)
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你手慢了, 该CDK已经被领完]"));
                return;
            }
            var time2 = Convert.ToDateTime(DateTime.Now.ToString()).Ticks;
            var min = (cdk1.Utiltime - time2) / 10000000;
            if (min <= 0)
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你来晚了, 该CDK已经过期了]"));
                return;
            }
            if (cdk1.Used.Contains(args.Player.Name))
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:已经领取过了，不能太贪心哦]"));
                return;
            }
            var strcmd = cdk1.Cmds.Replace("[plr]", args.Player.Name);
            var cmds = strcmd.Split(',');
            foreach (var cmd in cmds)
            {
                args.Player.PermissionlessInvoke(cmd);
            }
            Data.Update(cdk1.Cdkname, cdk1.Usetime - 1, cdk1.Utiltime, cdk1.Grouplimit, cdk1.Playerlimit, (cdk1.Used == "") ? args.Player.Name : cdk1.Used + "," + args.Player.Name, cdk1.Cmds);
            args.Player.SendSuccessMessage(GetString("[c/00FF00:【CDK】]\n[c/ffd700:CDK兑换成功！]"));
        }
    }
}