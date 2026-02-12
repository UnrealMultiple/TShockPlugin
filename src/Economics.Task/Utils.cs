using Economics.Task.Model;
using Economics.Core.Extensions;
using Terraria;
using TShockAPI;

namespace Economics.Task;

public class UserTaskData
{
    private static readonly Dictionary<string, TaskContent> UserTask = new();
    public static bool Add(string Name, int TaskId)
    {
        var task = Config.Instance.GetTask(TaskId);
        if (task == null)
        {
            return false;
        }

        UserTask[Name] = task;
        return true;
    }

    public static TaskContent? GetUserTask(string Name)
    {
        return UserTask.TryGetValue(Name, out var task) ? task : null;
    }

    public static bool Remove(string Name)
    {
        return UserTask.Remove(Name);
    }

    public static void Clear()
    {
        UserTask.Clear();
    }

    public static bool HasTask(string Name)
    {
        return UserTask.ContainsKey(Name);
    }

    public static Dictionary<int, Item> GetTaskFishingItem(TSPlayer player, List<Core.Model.Item> items)
    {
        var res = new Dictionary<int, Item>();
        foreach (var item in items)
        {
            if (item.Prefix == 0)
            {
                for (var i = 0; i < player.TPlayer.inventory.Length; i++)
                {
                    var bgitem = player.TPlayer.inventory[i];
                    if (bgitem.type == item.netID)
                    {
                        res.Add(i, bgitem);
                    }
                }
            }
            else
            {
                for (var i = 0; i < player.TPlayer.inventory.Length; i++)
                {
                    var bgitem = player.TPlayer.inventory[i];
                    if (bgitem.type == item.netID && bgitem.prefix == item.Prefix)
                    {
                        res.Add(i, bgitem);
                    }
                }
            }
        }
        return res;
    }

    public static List<string> GetTaskProgress(TSPlayer ply)
    {
        var result = new List<string>();
        var task = GetUserTask(ply.Name);
        if (task != null)
        {
            result.Add(GetString($"{task.TaskName}完成进度"));
            task.TaskInfo.TallkNPC.ForEach(x =>
            {
                var talk = Plugin.TallkManager.TallkNpcByID(ply.Name, x) ? GetString("(已完成)").Color(TShockAPI.Utils.GreenHighlight) : GetString("(未完成)").Color(TShockAPI.Utils.RedHighlight);
                result.Add(GetString($"与{TShock.Utils.GetNPCById(x).FullName}进行对话{talk}"));
            });

            task.TaskInfo.KillNPCS.ForEach(x =>
            {
                var str = GetString($"击杀怪物 {TShock.Utils.GetNPCById(x.ID).FullName} ({Plugin.KillNPCManager.GetKillNpcsCountByID(ply.Name, x.ID)}/{x.Count})");
                result.Add(str);
            });



            task.TaskInfo.Items.ForEach(x =>
            {
                var stack = 0;
                if (x.Prefix == 0)
                {
                    var items = ply.TPlayer.inventory.Where(f => f.type == x.netID);
                    if (items.Any())
                    {
                        items.ForEach(n => stack += n.stack);
                    }
                    result.Add(GetString($"拥有{TShock.Utils.GetItemById(x.netID).Name} ({stack}/{x.Stack})"));
                }
                else
                {
                    var items = ply.TPlayer.inventory.Where(f => f.type == x.netID && f.prefix == x.Prefix);
                    if (items.Any())
                    {
                        items.ForEach(n => stack += n.stack);

                    }
                    result.Add(GetString($"拥有{TShock.Utils.GetPrefixById(x.Prefix)}{TShock.Utils.GetItemById(x.netID).Name} ({stack}/{x.Stack})"));
                }
            });
        }
        return result;
    }

    public static void FinishTask(TSPlayer player)
    {
        var task = GetUserTask(player.Name);
        if (task != null)
        {
            Remove(player.Name);
            Plugin.KillNPCManager.RemoveNpcKill(player.Name);
            Plugin.TallkManager.RemoveTallkNPC(player.Name);
            task.TaskInfo.Items.ForEach(x =>
            {
                var stack = x.Stack;
                for (var i = 0; i < player.TPlayer.inventory.Length; i++)
                {
                    var item = player.TPlayer.inventory[i];
                    if (x.Prefix == 0)
                    {
                        if (item.type == x.netID)
                        {
                            if (item.stack >= stack)
                            {
                                item.stack -= stack;
                                player.SendData(PacketTypes.PlayerSlot, "", player.Index, i);
                                break;
                            }
                            else
                            {
                                item.stack = 0;
                                player.SendData(PacketTypes.PlayerSlot, "", player.Index, i);
                                stack -= item.stack;
                            }
                        }
                    }
                    else
                    {
                        if (item.type == x.netID && item.prefix == x.Prefix)
                        {
                            if (item.stack >= stack)
                            {
                                item.stack -= stack;
                                player.SendData(PacketTypes.PlayerSlot, "", player.Index, i);
                                break;
                            }
                            else
                            {
                                item.stack = 0;
                                player.SendData(PacketTypes.PlayerSlot, "", player.Index, i);
                                stack -= item.stack;
                            }
                        }
                    }
                }
            });
            player.ExecCommand(task.Reward.Commands);
        }
    }

    public static bool DectTaskFinish(TSPlayer player)
    {
        var task = GetUserTask(player.Name);
        if (task != null)
        {
            foreach (var f in task.TaskInfo.KillNPCS)
            {
                if (!Plugin.KillNPCManager.KillNpcsByID(player.Name, f.ID, f.Count))
                {
                    return false;
                }
            }

            foreach (var f in task.TaskInfo.TallkNPC)
            {
                if (!Plugin.TallkManager.TallkNpcByID(player.Name, f))
                {
                    return false;
                }
            }

            foreach (var f in task.TaskInfo.Items)
            {
                if (f.Prefix > 0)
                {
                    var items = player.TPlayer.inventory.Where(x => x.type == f.netID && x.prefix == f.Prefix);
                    var stack = 0;
                    items.ForEach(x => stack += x.stack);
                    if (stack < f.Stack)
                    {
                        return false;
                    }
                }
                else
                {
                    var items = player.TPlayer.inventory.Where(x => x.type == f.netID);
                    var stack = 0;
                    items.ForEach(x => stack += x.stack);
                    if (stack < f.Stack)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}