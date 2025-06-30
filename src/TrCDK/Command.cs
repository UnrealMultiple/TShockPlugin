using LazyAPI.Attributes;
using LazyAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TrCDK;

[Command("cdk")]
[Permissions("cdk.use")]
public class Command
{
    [Alias("give")]
    [Permissions("cdk.give")]
    public static void Give(CommandArgs args, TSPlayer? player, string cmd)
    {
        if(player == null)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:玩家不存在]"));
            return;
        }
        var cmds = cmd.Split(',');
        if (cmds.Length > 0)
        {
            args.Player.ExecCommand(cmds);
            args.Player.SendSuccessMessage(GetString($"[c/00FF00:【CDK管理】]\n[c/ffd700:已给予玩家 {player.Name} CDK奖励]"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK管理】]\n[c/ffd700:指令不能为空]"));
        }
    }

    [Alias("update")]
    [Permissions("cdk.update")]
    public static void Update(CommandArgs args, string cdkname, int usetime, long utiltime, string grouplimit = "", string playerlimit = "", string cmds = "")
    {
        var cdkData = CDK.GetData(cdkname);
        if (cdkData == null)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:CDK不存在]"));
            return;
        }
        CDK.Update(cdkname, usetime, utiltime, grouplimit, playerlimit, cdkData.Used, cmds);
        args.Player.SendSuccessMessage(GetString("[c/00FF00:【CDK】]\n[c/ffd700:CDK更新成功]"));
    }

    [Alias("del")]
    [Permissions("cdk.del")]
    public static void Delete(CommandArgs args, string cdkname)
    {
        if (CDK.GetData(cdkname) == null)
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:CDK不存在]"));
            return;
        }
        CDK.DelCDK(cdkname);
        args.Player.SendSuccessMessage(GetString("[c/00FF00:【CDK】]\n[c/ffd700:CDK删除成功]"));
    }

    [Alias("add")]
    [Permissions("cdk.add")]
    public static void Add(CommandArgs args, string cdkname, int usetime, long utiltime, string grouplimit = "", string playerlimit = "", string cmds = "")
    {
        if (CDK.Insert(cdkname, usetime, utiltime, grouplimit, playerlimit, cmds))
        {
            args.Player.SendSuccessMessage(GetString("[c/00FF00:【CDK】]\n[c/ffd700:CDK添加成功]"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:CDK添加失败]"));
        }
    }

    [Alias("loadall")]
    [Permissions("cdk.loadall")]
    public static void LoadAll(CommandArgs args)
    {
        var cdkList = CDK.GetAllData();
        if (cdkList.Length == 0)
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:没有可用的CDK]"));
            return;
        }
        var sb = new StringBuilder();
        sb.AppendLine(GetString("[c/00FF00:【CDK】]\n[c/ffd700:当前可用的CDK列表]"));
        foreach (var cdk in cdkList)
        {
            sb.AppendLine(GetString($"[c/ffd700:{cdk.Name}] [c/00FF00:剩余使用次数: {cdk.Usetime}] [c/ffd700:过期时间: {new DateTime(cdk.Utiltime).ToLocalTime()}]"));
        }
        args.Player.SendInfoMessage(sb.ToString().Trim());
    }

    [Main]
    [RealPlayer]
    public static void Use(CommandArgs args, string cdk)
    {
        var cdkData = CDK.GetData(cdk);
        if (cdkData == null)
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:CDK不存在或已失效]"));
            return;
        }

        if (cdkData.Playerlimit != "")
        {
            if (!cdkData.Playerlimit.Contains(args.Player.Name))
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你不在该CDK的领取名单中]"));
                return;
            }
        }
        if (cdkData.Grouplimit != "")
        {
            if (!cdkData.Grouplimit.Contains(args.Player.Group.Name))
            {
                args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你所在的组不能领取该CDK]"));
                return;
            }
        }
        if (cdkData.Usetime < 1)
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你手慢了, 该CDK已经被领完]"));
            return;
        }
        var time2 = Convert.ToDateTime(DateTime.Now.ToString()).Ticks;
        var min = (cdkData.Utiltime - time2) / 10000000;
        if (min <= 0)
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:你来晚了, 该CDK已经过期了]"));
            return;
        }
        if (cdkData.Used.Contains(args.Player.Name))
        {
            args.Player.SendInfoMessage(GetString("[c/FF0000:【CDK】]\n[c/ffd700:已经领取过了，不能太贪心哦]"));
            return;
        }
        var strcmd = cdkData.Cmds.Replace("[plr]", args.Player.Name);
        var cmds = strcmd.Split(',');
        args.Player.ExecCommand(cmds);
        CDK.Update(cdkData.Name, cdkData.Usetime - 1, cdkData.Utiltime, cdkData.Grouplimit, cdkData.Playerlimit, (cdkData.Used == "") ? args.Player.Name : cdkData.Used + "," + args.Player.Name, cdkData.Cmds);
        args.Player.SendSuccessMessage(GetString("[c/00FF00:【CDK】]\n[c/ffd700:CDK兑换成功！]"));
    }
}
