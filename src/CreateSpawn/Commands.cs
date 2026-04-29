using LazyAPI.Attributes;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace CreateSpawn;

[Command("cb")]
[Permissions("create.build.copy")]
public class Commands
{
    [Alias("spawn")]
    public static void SpawnBuild(CommandArgs args, string name)
    {
        var ply = args.Player;
        if (TileHelper.NeedWaitTask(ply))
        {
            return;
        }

        var clip = Map.LoadClip(name);
        if (clip == null)
        {
            ply.SendErrorMessage(GetString("剪贴板为空或未找到指定建筑!"));
            return;
        }

        var startX = 0;
        var startY = 0;

        if (ply.RealPlayer)
        {
            startX = ply.TileX - (clip.Width / 2);
            startY = ply.TileY - clip.Height;
        }
        else
        {
            startX = Terraria.Main.spawnTileX - Config.Instance.CentreX + Config.Instance.AdjustX;
            startY = Terraria.Main.spawnTileY - Config.Instance.CountY + Config.Instance.AdjustY;
        }

        Utils.SpawnBuilding(ply, startX, startY, clip);
    }

    [Alias("set")]
    [RealPlayer]
    public static void CopySet(CommandArgs args, int type)
    {
        switch (type)
        {
            case 1:
                args.Player.AwaitingTempPoint = 1;
                args.Player.SendInfoMessage(GetString("请选择复制区域的左上角"));
                break;
            case 2:
                args.Player.AwaitingTempPoint = 2;
                args.Player.SendInfoMessage(GetString("请选择复制区域的右下角"));
                break;
            default:
                args.Player.SendInfoMessage(GetString($"正确指令：/cb set <1/2> --选择复制的区域"));
                break;
        }
    }

    [Alias("list")]
    public static void BuildList(CommandArgs args)
    {
        var clipNames = Map.GetAllClipNames();

        if (clipNames.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("当前[c/64E0DA:没有可用]的复制建筑。"));
            return;
        }

        args.Player.SendInfoMessage(GetString($"\n当前可用的复制建筑 [c/64E0DA:{clipNames.Count}个]:"));
        for (var i = 0; i < clipNames.Count; i++)
        {
            var msg = $"[c/D0AFEB:{i + 1}.] [c/FFFFFF:{clipNames[i]}]";
            args.Player.SendMessage(msg, Color.AntiqueWhite);
        }

        args.Player.SendInfoMessage(GetString($"可使用指定粘贴指令:[c/D0AFEB:/cb spawn 名字]"));
    }

    [Alias("save")]
    [RealPlayer]
    public static void CopyBuilding(CommandArgs args, string name)
    {
        var plr = args.Player;
        if (plr.TempPoints[0].X == 0 || plr.TempPoints[1].X == 0)
        {
            plr.SendInfoMessage(GetString("您还没有选择区域！"));
            plr.SendMessage(GetString("使用方法: /cb set 1 选择左上角"), Color.AntiqueWhite);
            plr.SendMessage(GetString("使用方法: /cb set 2 选择右下角"), Color.AntiqueWhite);
            return;
        }

        var clip = Utils.CopyBuilding(
            plr.TempPoints[0].X, plr.TempPoints[0].Y,
            plr.TempPoints[1].X, plr.TempPoints[1].Y);
        Map.SaveClip(name, clip);
        plr.SendSuccessMessage(GetString($"已复制区域 ({clip.Width}x{clip.Height})"));
    }

    [Alias("back")]
    [RealPlayer]
    public static void BackBuilding(CommandArgs args)
    {
        Utils.Restore(args.Player);
    }

    [Alias("zip")]
    public static void BackupAndDeleteAllDataFiles(CommandArgs args)
    {
        Map.BackupAndDeleteAllDataFiles();
        args.Player.SendSuccessMessage(GetString("已备份并清空所有建筑数据文件。"));
    }

    [Alias("auto")]
    [Flexible]
    public static void AutoSpawn(CommandArgs args, string action)
    {
        var name = args.Parameters.Count > 2 ? string.Join(" ", args.Parameters.Skip(2)) : null;
        action = action.ToLowerInvariant();
        var list = Config.Instance.AutoSpawnBuilds;
        switch (action)
        {
            case "add":
                if (string.IsNullOrWhiteSpace(name))
                {
                    args.Player.SendErrorMessage(GetString("用法: /cb auto add <建筑名>"));
                    return;
                }

                if (list.Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    args.Player.SendInfoMessage(GetString($"建筑 {name} 已在自动生成列表中。"));
                    return;
                }

                if (Map.LoadClip(name) == null)
                {
                    args.Player.SendErrorMessage(GetString($"未找到建筑: {name}"));
                    return;
                }

                list.Add(name);
                Config.Save();
                args.Player.SendSuccessMessage(GetString($"已添加自动生成建筑: {name}"));
                break;

            case "remove":
                if (string.IsNullOrWhiteSpace(name))
                {
                    args.Player.SendErrorMessage(GetString("用法: /cb auto remove <建筑名>"));
                    return;
                }

                var removed = list.RemoveAll(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                {
                    args.Player.SendInfoMessage(GetString($"自动生成列表中不存在: {name}"));
                    return;
                }

                Config.Save();
                args.Player.SendSuccessMessage(GetString($"已移除自动生成建筑: {name}"));
                break;

            case "list":
                if (list.Count == 0)
                {
                    args.Player.SendInfoMessage(GetString("自动生成建筑列表为空。"));
                    return;
                }

                args.Player.SendInfoMessage(GetString($"自动生成建筑列表 ({list.Count})："));
                foreach (var build in list)
                {
                    args.Player.SendInfoMessage($"- {build}");
                }

                break;

            case "clear":
                list.Clear();
                Config.Save();
                args.Player.SendSuccessMessage(GetString("已清空自动生成建筑列表。"));
                break;

            default:
                args.Player.SendInfoMessage(GetString("用法: /cb auto <add/remove/list/clear> [建筑名]"));
                break;
        }
    }

    [Alias("help")]
    public static void HelpCmd(CommandArgs args)
    {
        var random = new Random();
        var color = RandomColors(random);

        args.Player.SendInfoMessage(GetString($"复制建筑指令菜单"));
        args.Player.SendMessage(GetString($"/cb set 1 ——敲击或放置一个方块到左上角"), color);
        args.Player.SendMessage(GetString($"/cb set 2 ——敲击或放置一个方块到右下角"), color);
        args.Player.SendMessage(GetString($"/cb save 名字 ——添加建筑"), color);
        args.Player.SendMessage(GetString($"/cb spawn 名字 ——生成建筑"), color);
        args.Player.SendMessage(GetString($"/cb back ——还原图格"), color);
        args.Player.SendMessage(GetString($"/cb list —-文件列表"), color);
        args.Player.SendMessage(GetString($"/cb zip ——清空建筑并备份为zip"), color);
        args.Player.SendMessage(GetString($"/cb auto add 名字 ——加入新世界自动生成列表"), color);
        args.Player.SendMessage(GetString($"/cb auto remove 名字 ——移除自动生成列表"), color);
        args.Player.SendMessage(GetString($"/cb auto list ——查看自动生成列表"), color);
        args.Player.SendMessage(GetString($"/cb auto clear ——清空自动生成列表"), color);
    }

    private static Color RandomColors(Random random)
    {
        var r = random.Next(150, 200);
        var g = random.Next(170, 200);
        var b = random.Next(170, 200);
        var color = new Color(r, g, b);
        return color;
    }
}
