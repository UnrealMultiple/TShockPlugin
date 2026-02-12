using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;
using static PlayerSpeed.PlayerSpeed;

namespace PlayerSpeed;

public class Commands
{
    #region 主体指令方法
    public static void vel(CommandArgs args)
    {
        var plr = args.Player;
        if (!Configuration.Instance.Enabled || plr == null)
        {
            return;
        }

        if (args.Parameters.Count == 0)
        {
            HelpCmd(plr);
            return;
        }

        if (args.Parameters.Count >= 1 && plr.HasPermission("vel.admin"))
        {
            switch (args.Parameters[0].ToLower())
            {
                case "on":

                    Configuration.Instance.Enabled = true;
                    Configuration.Instance.SaveTo();
                    plr.SendInfoMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:启用]加速功能。"));
                    break;

                case "off":
                    Configuration.Instance.Enabled = false;
                    Configuration.Instance.SaveTo();
                    plr.SendInfoMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:关闭]加速功能。"));
                    break;

                case "mess":
                    Configuration.Instance.Mess = !Configuration.Instance.Mess;
                    plr.SendSuccessMessage(Configuration.Instance.Mess ?
                        GetString($"玩家 [{plr.Name}] 的已[c/92C5EC:启用]玩家速度播报") :
                        GetString($"玩家 [{plr.Name}] 的已[c/92C5EC:关闭]玩家速度播报"));
                    Configuration.Instance.SaveTo();
                    break;

                case "boss":
                    Configuration.Instance.KilledBoss = !Configuration.Instance.KilledBoss;
                    plr.SendSuccessMessage(Configuration.Instance.KilledBoss ?
                        GetString($"玩家 [{plr.Name}] 的已[c/92C5EC:启用]自动进度模式") :
                        GetString($"玩家 [{plr.Name}] 的已[c/92C5EC:关闭]自动进度模式"));
                    Configuration.Instance.SaveTo();
                    break;

                case "del":
                    if (args.Parameters.Count >= 1)
                    {
                        var other = args.Parameters[1]; // 指定的玩家名字
                        DB.DeleteData(other);
                        plr.SendSuccessMessage(GetString($"已[c/E8585B:删除] {other} 的数据！"));
                    }
                    return;

                case "reset":

                    if (Configuration.Instance.BossList != null && Configuration.Instance.KilledBoss)
                    {
                        for (var i = 0; i < Configuration.Instance.BossList.Count; i++)
                        {
                            if (Configuration.Instance.BossList[i].Enabled)
                            {
                                Configuration.Instance.BossList[i].Enabled = false;
                            }
                        }

                        Configuration.Instance.SaveTo();
                        plr.SendInfoMessage(GetString("已清除所有玩家数据,重置进度模式。"));
                    }
                    else
                    {
                        plr.SendInfoMessage(GetString("已清除所有玩家数据"));
                    }

                    DB.ClearData();
                    break;

                case "set":
                case "s":
                    if (args.Parameters.Count >= 2)
                    {
                        Parse(args.Parameters, out var ItemVal, 1);
                        UpdatePT(args, ItemVal);
                    }
                    else
                    {
                        plr.SendMessage(GetString("参数: 速度([c/F24F62:sd]) 高度([c/48DCB8:h]) 间隔([c/4898DC:r]) 冷却([c/FE7F53:t])\n") +
                            GetString("次数([c/DBF34E:c]) 加跳跃物品([c/59E32B:add]) 删跳跃物品([c/F14F63:del])\n") +
                            GetString("格式为:[c/48DCB8:/vel s sd 20 add 恐慌项链…]\n") +
                            GetString("确保属性后有正确的数字或名字,任意组合"), 244, 255, 150);
                    }
                    break;
                default:
                    HelpCmd(plr);
                    break;
            }
            return;
        }
    }
    #endregion

    #region 解析输入参数的属性名 通用方法
    private static void UpdatePT(CommandArgs args, Dictionary<string, string> itemValues)
    {
        var UpdateItem = new List<int>(Configuration.Instance.ArmorItem!);

        var mess = new StringBuilder();
        mess.Append(GetString($"修改冲刺:"));
        foreach (var kvp in itemValues)
        {
            string prop;
            switch (kvp.Key.ToLower())
            {
                case "sd":
                case "speed":
                case "速度":
                    if (float.TryParse(kvp.Value, out var speed))
                    {
                        Configuration.Instance.Speed = speed;
                    }

                    prop = GetString("速度");
                    break;
                case "gd":
                case "h":
                case "height":
                case "高度":
                    if (float.TryParse(kvp.Value, out var height))
                    {
                        Configuration.Instance.Height = height;
                    }

                    prop = GetString("高度");
                    break;
                case "t":
                case "sj":
                case "time":
                case "冷却":
                case "时间":
                    if (int.TryParse(kvp.Value, out var t))
                    {
                        Configuration.Instance.CoolTime = t;
                    }

                    prop = GetString("冷却");
                    break;
                case "c":
                case "ct":
                case "count":
                case "次数":
                    if (int.TryParse(kvp.Value, out var ct))
                    {
                        Configuration.Instance.Count = ct;
                    }

                    prop = GetString("次数");
                    break;
                case "r":
                case "range":
                case "间隔":
                    if (double.TryParse(kvp.Value, out var r))
                    {
                        Configuration.Instance.Range = r;
                    }

                    prop = GetString("间隔");
                    break;
                case "add":
                case "添加物品":
                    var add = TShock.Utils.GetItemByIdOrName(kvp.Value);
                    if (add.Count == 0)
                    {
                        args.Player.SendInfoMessage(GetString($"找不到名为 {kvp.Value} 的物品."));
                        return;
                    }
                    else if (add.Count > 1)
                    {
                        args.Player.SendMultipleMatchError(add.Select(i => i.Name));
                        return;
                    }

                    var newItemId = add[0].type;
                    if (!UpdateItem.Contains(newItemId))
                    {
                        UpdateItem.Add(newItemId);
                        prop = GetString("添加物品");
                    }
                    else
                    {
                        prop = GetString("物品已存在");
                    }
                    break;
                case "del":
                case "移除物品":
                    if (int.TryParse(kvp.Value, out var remove))
                    {
                        prop = UpdateItem.Remove(remove) ? GetString("移除物品") : GetString($"物品 {remove} 不存在.");
                    }
                    else
                    {
                        var ToRemove = TShock.Utils.GetItemByIdOrName(kvp.Value);
                        if (ToRemove.Count == 0)
                        {
                            args.Player.SendInfoMessage(GetString($"找不到名为 {kvp.Value} 的物品."));
                            return;
                        }
                        else if (ToRemove.Count > 1)
                        {
                            args.Player.SendMultipleMatchError(ToRemove.Select(i => i.Name));
                            return;
                        }

                        var del = ToRemove[0];
                        prop = UpdateItem.Remove(del.type) ? GetString("移除物品") : GetString("物品不存在");
                    }
                    break;
                default:
                    prop = kvp.Key;
                    break;
            }
            mess.AppendFormat("[c/94D3E4:{0}]([c/FF6975:{1}]) ", prop, kvp.Value);
        }

        // 将修改后的列表复制回 触发跳跃加速的物品ID表
        Configuration.Instance.ArmorItem = new List<int>(UpdateItem);
        Configuration.Instance.SaveTo();
        TShock.Utils.Broadcast(mess.ToString(), 255, 244, 150);
    }
    #endregion

    #region 解析输入参数的距离 如:da 1
    private static void Parse(List<string> parameters, out Dictionary<string, string> itemValues, int Index)
    {
        itemValues = new Dictionary<string, string>();
        for (var i = Index; i < parameters.Count; i += 2)
        {
            if (i + 1 < parameters.Count) // 确保有下一个参数
            {
                var propertyName = parameters[i].ToLower();
                var value = parameters[i + 1];
                itemValues[propertyName] = value;
            }
        }
    }
    #endregion

    #region 菜单方法
    private static void HelpCmd(TSPlayer plr)
    {
        if (plr == null)
        {
            return;
        }

        if (plr.HasPermission("vel.admin"))
        {
            plr.SendMessage(GetString("[i:3455][c/AD89D5:玩][c/D68ACA:家][c/DF909A:速][c/E5A894:度][i:3454]\n") +
                            GetString("/vel on ——开启插件功能\n") +
                            GetString("/vel off ——关闭插件功能\n") +
                            GetString("/vel set ——设置相关参数\n") +
                            GetString("/vel boss ——进度模式开关\n") +
                            GetString("/vel mess ——播报系统开关\n") +
                            GetString("/vel del 玩家名 ——删除玩家数据\n") +
                            GetString("/vel reset ——清除所有数据"), Color.AntiqueWhite);
        }

        if (!Configuration.Instance.KilledBoss)
        {
            plr.SendInfoMessage(GetString($"速度:[c/4EA4F2:{Configuration.Instance.Speed}] 高度:[c/FF5265:{Configuration.Instance.Height}] ") +
                GetString($"冷却:[c/48DCB8:{Configuration.Instance.CoolTime}] 间隔:[c/47DCBD:{Configuration.Instance.Range}]"));
        }
        else
        {
            var boss = GetMaxSpeed(Configuration.Instance.BossList);
            if (boss != null && boss.Enabled)
            {
                plr.SendInfoMessage(GetString($"最高速度:[c/4EA4F2:{boss.Speed}] 最大高度:[c/FF5265:{boss.Height}] ") +
                    GetString($"冷却:[c/48DCB8:{boss.CoolTime}秒] 使用次数:[c/47DCBD:{boss.Count}秒]"));
            }
            else
            {
                plr.SendInfoMessage(GetString("当前为进度模式,服务器未击败相关BOSS"));
            }
        }
    }
    #endregion
}