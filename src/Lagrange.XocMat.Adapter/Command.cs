using MorMorAdapter.Attributes;
using TShockAPI;

namespace MorMorAdapter;

public class Command
{
    //#region 清理弹幕(不清理玩家召唤物)
    //[CommandMatch("clearproj", Permission.ClearProj)]
    //public void ClearProj(CommandArgs args)
    //{
    //    int radius = 50;
    //    if (args.Parameters.Count == 1)
    //    {
    //        if (!int.TryParse(args.Parameters[0], out radius) || radius <= 0)
    //        {
    //            args.Player.SendErrorMessage("错误的范围!");
    //            return;
    //        }
    //    }
    //    int cleared = 0;
    //    for (int i = 0; i < Main.maxProjectiles; i++)
    //    {
    //        float dx = Main.projectile[i].position.X - args.Player.X;
    //        float dy = Main.projectile[i].position.Y - args.Player.Y;
    //        if (Main.projectile[i].active && !Main.projectile[i].sentry && !Main.projectile[i].minion && dx * dx + dy * dy <= radius * radius * 256)
    //        {
    //            Main.projectile[i].active = false;
    //            Main.projectile[i].type = 0;
    //            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", i);
    //            cleared++;
    //        }
    //    }
    //    TShock.Utils.Broadcast(string.Format("{0}清除了{1}范围内{2}个射弹!", args.Player.Name, radius, cleared), Color.Yellow);
    //}
    //#endregion

    //#region 花苞查找与移除
    //[CommandMatch("花苞", Permission.SearchWall)]
    //public void SearchWall(CommandArgs args)
    //{
    //    if (args.Parameters.Count == 1)
    //    {
    //        switch (args.Parameters[0].ToLower())
    //        {
    //            case "search":
    //                {
    //                    var n = 1;
    //                    int x = 0, y = 0, count = 0;
    //                    for (int i = 0; i < Main.tile.Width; i++)
    //                    {
    //                        for (int j = 0; j < Main.tile.Height; j++)
    //                        {
    //                            var tile = Main.tile[i, j];
    //                            var sync = Math.Abs(i - x) + Math.Abs(j - y) > 2;
    //                            if (tile != null && tile.type == 238 && sync)
    //                            {
    //                                x = i;
    //                                y = j;
    //                                count++;
    //                                if (!TShock.Warps.Warps.Any(s => s.Position.X == i && s.Position.Y == j))
    //                                {
    //                                    while (TShock.Warps.Warps.Any(x => x.Name == "花苞" + n))
    //                                    {
    //                                        n++;
    //                                    }
    //                                    TShock.Warps.Add(i, j, "花苞" + n);
    //                                }
    //                            }
    //                        }
    //                    }
    //                    args.Player.SendInfoMessage("已为你搜索到{0}个花苞，输入/warp list可以查看结果", count);
    //                    break;
    //                }
    //            case "clear":
    //                {
    //                    TShock.Warps.Warps.FindAll(x => x.Name.StartsWith("花苞")).ForEach(n =>
    //                    {
    //                        TShock.Warps.Remove(n.Name);
    //                    });
    //                    args.Player.SendSuccessMessage("已移除所以花苞传送点!");
    //                    break;
    //                }
    //        }
    //    }

    //}
    //#endregion

    //#region 自踢
    //[CommandMatch("自踢", Permission.KickSelf)]
    //public void KickSelf(CommandArgs args)
    //{
    //    args.Player.Disconnect("你自己踢出了自己!");
    //}
    //#endregion

    //#region 自杀
    //[CommandMatch("自杀", Permission.KillSelf)]
    //public void KillSelf(CommandArgs args)
    //{
    //    args.Player.KillPlayer();
    //}
    //#endregion

    //#region 幽灵模式
    //[CommandMatch("ghost", Permission.Ghost)]
    //public void Ghost(CommandArgs args)
    //{
    //    args.Player.TPlayer.ghost = !args.Player.TPlayer.ghost;
    //    args.Player.SendData(PacketTypes.PlayerInfo, "", args.Player.Index);
    //    args.Player.SendData(PacketTypes.PlayerUpdate, "", args.Player.Index);
    //}
    //#endregion

    //#region 设置旅途模式难度
    //[CommandMatch("旅途难度", Permission.SetJourneyDifficult)]
    //public void JourneyDiff(CommandArgs args)
    //{
    //    if (args.Parameters.Count == 1)
    //    {
    //        if (!Main._currentGameModeInfo.IsJourneyMode)
    //        {
    //            args.Player.SendErrorMessage("必须在旅途模式下才能设置难度！");
    //            return;
    //        }
    //        if (Utils.SetJourneyDiff(args.Parameters[0]))
    //            args.Player.SendSuccessMessage("难度成功设置为 {0}!", args.Parameters[0]);
    //    }
    //    else
    //    {
    //        args.Player.SendErrorMessage("正确语法: /旅途难度 <难度模式>");
    //        args.Player.SendErrorMessage("有效的难度模式: master，journey，normal，expert");
    //    }
    //}
    //#endregion

    #region 设置进度
    [CommandMatch("设置进度", Permission.SetProgress)]
    public void SetProgress(CommandArgs args)
    {
        if (args.Parameters.Count == 2)
        {
            bool enable;
            switch (args.Parameters[1])
            {
                case "开启":
                case "开":
                case "true":
                    enable = true;
                    break;
                case "关闭":
                case "关":
                case "false":
                    enable = false;
                    break;
                default:
                    args.Player.SendErrorMessage("请输入一个正确的状态!");
                    return;
            }
            if (Utils.SetGameProgress(args.Parameters[0], enable))
            {
                args.Player.SendSuccessMessage($"成功设置进度`{args.Parameters[0]}`状态为{enable}");
            }
            else
            {
                args.Player.SendErrorMessage($"进度{args.Parameters[0]}不存在!");
            }
        }
        else
        {
            args.Player.SendErrorMessage("语法错误，正确语法:\n!设置进度 [进度名] [true|开|开启|关|关闭|false]");
        }
    }
    #endregion

    //#region 在线排行
    //[CommandMatch("在线排行", Permission.OnlineRank)]
    //public void OnlineRank(CommandArgs args)
    //{
    //    void Show(List<string> line)
    //    {
    //        if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out int pageNumber))
    //            return;

    //        PaginationTools.SendPage(
    //            args.Player,
    //            pageNumber,
    //            line,
    //            new PaginationTools.Settings
    //            {
    //                MaxLinesPerPage = 6,
    //                NothingToDisplayString = "没有在线数据!",
    //                HeaderFormat = "在线排行 ({0}/{1})：",
    //                FooterFormat = "输入 {0}在线排行 {{0}} 查看更多".SFormat(Commands.Specifier)
    //            }
    //        );
    //    }
    //    var list = Plugin.Onlines.OrderByDescending(x => x.Value).Select(x => $"[{x.Key}] => 在线{x.Value / 60}分钟");
    //    Show(list.ToList());
    //}
    //#endregion

    //#region 死亡排行
    //[CommandMatch("死亡排行", Permission.DeathRank)]
    //public void DeathRank(CommandArgs args)
    //{
    //    void Show(List<string> line)
    //    {
    //        if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out int pageNumber))
    //            return;

    //        PaginationTools.SendPage(
    //            args.Player,
    //            pageNumber,
    //            line,
    //            new PaginationTools.Settings
    //            {
    //                MaxLinesPerPage = 6,
    //                NothingToDisplayString = "没有死亡数据!",
    //                HeaderFormat = "死亡排行 ({0}/{1})：",
    //                FooterFormat = "输入 {0}死亡排行 {{0}} 查看更多".SFormat(Commands.Specifier)
    //            }
    //        );
    //    }
    //    var list = Plugin.Deaths.OrderByDescending(x => x.Value).Select(x => $"[{x.Key}] => 死亡{x.Value}次");
    //    Show(list.ToList());
    //}
    //#endregion

    //#region 生成地图
    //[CommandMatch("生成地图", Permission.DeathRank)]
    //public void GenerateMap(CommandArgs args)
    //{
    //    var bytes = Utils.CreateMapBytes();
    //    File.WriteAllBytes("test.png", bytes);
    //}
    //#endregion
}
