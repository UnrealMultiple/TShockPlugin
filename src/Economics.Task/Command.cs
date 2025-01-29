using EconomicsAPI.Attributes;
using EconomicsAPI.Extensions;
using TShockAPI;

namespace Economics.Task;

[RegisterSeries]
public class Command
{
    [CommandMap("task", Permission.TaskUse)]
    private void CTask(CommandArgs args)
    {
        void ShowTask(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                args.Player,
                pageNumber,
                line,
                new PaginationTools.Settings
                {
                    MaxLinesPerPage = Plugin.TaskConfig.PageCount,
                    NothingToDisplayString = GetString("当前没有任务"),
                    HeaderFormat = GetString("任务列表 ({0}/{1})："),
                    FooterFormat = GetString("输入 {0}task list {{0}} 查看更多").SFormat(Commands.Specifier)
                }
            );
        }
        if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var line = Plugin.TaskConfig.Tasks.Select(x => $"{x.TaskID.Color(TShockAPI.Utils.PinkHighlight)}.{x.TaskName.Color(TShockAPI.Utils.CyanHighlight)}").ToList();
            ShowTask(line);
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "info")
        {
            if (int.TryParse(args.Parameters[1], out var index))
            {
                var task = Plugin.TaskConfig.GetTask(index);
                if (task == null)
                {
                    args.Player.SendErrorMessage(GetString("不存在此任务!"));
                }
                else
                {
                    args.Player.SendMessage(GetString($"{task.TaskName}介绍: {task.Description}"), Microsoft.Xna.Framework.Color.Wheat);
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("错误的技能序号!"));
            }
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "pick")
        {
            if (UserTaskData.HasTask(args.Player.Name))
            {
                args.Player.SendErrorMessage(GetString("您还有一个任务正在进行，不能接多个任务!"));
            }
            else
            {
                if (int.TryParse(args.Parameters[1], out var index))
                {
                    var task = Plugin.TaskConfig.GetTask(index);
                    if (task != null)
                    {
                        if (Plugin.TaskFinishManager.HasFinishTask(index, args.Player.Name))
                        {
                            args.Player.SendErrorMessage(GetString($"此任务你已经完成过了!"));
                            return;
                        }
                        if (!Plugin.InOfFinishTask(args.Player, task.FinishTask))
                        {
                            args.Player.SendErrorMessage(GetString($"必须完成任务 {string.Join(",", task.FinishTask)} 才能接此任务"));
                            return;
                        }
                        if (!RPG.RPG.InLevel(args.Player.Name, task.LimitLevel))
                        {
                            args.Player.SendErrorMessage(GetString($"只有在{string.Join(", ", task.LimitLevel)} 以及符等级才能接取此任务"));
                            return;
                        }
                        if (!args.Player.InProgress(task.LimitProgress))
                        {
                            args.Player.SendErrorMessage(GetString($"需要满足进度{string.Join(", ", task.LimitLevel)}才能接取此任务"));
                            return;
                        }

                        UserTaskData.Add(args.Player.Name, index);
                        Plugin.TaskFinishManager.Add(index, args.Player.Name, TaskStatus.Ongoing);
                        args.Player.SendSuccessMessage(GetString("任务接取成功!"));
                        args.Player.SendSuccessMessage(GetString($"任务名称:{task.TaskName}"));
                        args.Player.SendSuccessMessage(GetString($"任务介绍:{task.Description}"));
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("任务不存在!"));
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("输入了一个错误的序号!"));
                }
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0] == "prog")
        {
            if (UserTaskData.HasTask(args.Player.Name))
            {
                var progress = UserTaskData.GetTaskProgress(args.Player);
                progress.ForEach(x => args.Player.SendInfoMessage(x));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("你没有接取一个任务!"));
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "del")
        {
            if (UserTaskData.HasTask(args.Player.Name))
            {
                UserTaskData.Remove(args.Player.Name);
                args.Player.SendSuccessMessage(GetString("你已放弃一个任务!!"));
            }
            else
            {
                args.Player.SendSuccessMessage(GetString("你还没有开始接任务!!"));
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "pr")
        {
            var task = UserTaskData.GetUserTask(args.Player.Name);
            if (task != null)
            {
                if (UserTaskData.DectTaskFinish(args.Player))
                {
                    Plugin.TaskFinishManager.Update(task.TaskID, args.Player.Name, TaskStatus.Success);
                    UserTaskData.FinishTask(args.Player);
                    args.Player.SendSuccessMessage(task.FinishTaskFormat, args.Player.Name);
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("你当前的任务还没有完成!"));
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("你还没有接一个任务!"));
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset")
        {
            if (!args.Player.HasPermission(Permission.TaskAdmin))
            {
                args.Player.SendErrorMessage(GetString("你没有使用该指令的权限!"));
                return;
            }
            Plugin.TaskFinishManager.RemoveAll();
            UserTaskData.Clear();
            Plugin.KillNPCManager.RemoveAll();
            Plugin.TallkManager.RemoveAll();
            args.Player.SendSuccessMessage(GetString("已清空完成任务!"));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("/task list 查看任务列表"));
            args.Player.SendInfoMessage(GetString("/task info <序号> 查看任务详情"));
            args.Player.SendInfoMessage(GetString("/task pick <序号> 接取一个任务"));
            args.Player.SendInfoMessage(GetString("/task prog 查看任务完成进度"));
            args.Player.SendInfoMessage(GetString("/task pr 提交任务"));
            args.Player.SendInfoMessage(GetString("/task del 移除任务"));
            args.Player.SendInfoMessage(GetString("/task reset 清空完成任务"));
        }
    }
}