using LazyAPI.Attributes;
using TShockAPI;

namespace AutoFish;

[Command("afa")]
[Permissions("autofish.admin")]
internal class CommandAdmin
{
    #region toggle
    [Alias("fish")]
    public static void ToggleAutoFish(CommandArgs args)
    {
        Configuration.Instance.GlobalAutoFishFeatureEnabled = !Configuration.Instance.GlobalAutoFishFeatureEnabled;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalAutoFishFeatureEnabled ? "开启" : "关闭")}]自动钓鱼功能。"));
        Configuration.Instance.SaveTo();
    }

    [Alias("buff")]
    public static void ToggleAutoFishBuff(CommandArgs args)
    {
        Configuration.Instance.GlobalBuffFeatureEnabled = !Configuration.Instance.GlobalBuffFeatureEnabled;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalBuffFeatureEnabled ? "开启" : "关闭")}]自动钓鱼BUFF。"));
        Configuration.Instance.SaveTo();
    }

    [Alias("multi")]
    public static void ToggleAutoFishMulti(CommandArgs args)
    {
        Configuration.Instance.GlobalMultiHookFeatureEnabled = !Configuration.Instance.GlobalMultiHookFeatureEnabled;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalMultiHookFeatureEnabled ? "开启" : "关闭")}]多钩功能。"));
        Configuration.Instance.SaveTo();
    }

    [Alias("mod")]
    public static void ToggleAutoFishMod(CommandArgs args)
    {
        Configuration.Instance.GlobalConsumptionModeEnabled = !Configuration.Instance.GlobalConsumptionModeEnabled;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalConsumptionModeEnabled ? "开启" : "关闭")}]消耗模式。"));
        Configuration.Instance.SaveTo();
    }

    [Alias("monster")]
    public static void ToggleAutoFishMonster(CommandArgs args)
    {
        Configuration.Instance.GlobalBlockMonsterCatch = !Configuration.Instance.GlobalBlockMonsterCatch;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalBlockMonsterCatch ? "开启" : "关闭")}]不钓怪物。"));
        Configuration.Instance.SaveTo();
    }

    [Alias("anim")]
    public static void ToggleAutoFishAnim(CommandArgs args)
    {
        Configuration.Instance.GlobalSkipFishingAnimation = !Configuration.Instance.GlobalSkipFishingAnimation;
        args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{(Configuration.Instance.GlobalSkipFishingAnimation ? "开启" : "关闭")}]跳过上鱼动画。"));
        Configuration.Instance.SaveTo();
    }


    [Alias("debug")]
    public static void ToggleAutoFishDebug(CommandArgs args)
    {
        Plugin.DebugMode = !Plugin.DebugMode;
        args.Player.SendSuccessMessage(GetString($"已[c/92C5EC:{(Plugin.DebugMode ? "开启" : "关闭")}]debug模式。"));
        Configuration.Instance.SaveTo();
    }
    #endregion

    [Alias("del")]
    public static void DelBait(CommandArgs args, string type)
    {
        var selectItem = SelectItem(args.Player, type);
        if (selectItem == null)
        {
            return;
        }
        var msg = Configuration.Instance.BaitRewards.Remove(selectItem.type)
            ? GetString($"已成功从指定鱼饵表移出物品 : [i:{selectItem.type}]!")
            : GetString("物品 {0} 不在指定鱼饵表中!");
        args.Player.SendInfoMessage(msg);
        Configuration.Instance.SaveTo();
    }

    [Alias("duo")]
    public static void SetMultiHookMax(CommandArgs args, uint max)
    {
        Configuration.Instance.GlobalMultiHookMaxNum = (int) max;
        Configuration.Instance.SaveTo();
        args.Player.SendSuccessMessage(GetString($"已成功将多钓数量上限设置为: [c/92C5EC:{max}] 个！"));
    }

    [Alias("add", "set")]
    public static void AddFishItem(CommandArgs args, string type, uint count, uint minutes)
    {
        var selectItem = SelectItem(args.Player, type);
        if (selectItem == null)
        {
            return;
        }
        Configuration.Instance.BaitRewards[selectItem.type] = new Configuration.BaitReward
        {
            Count = (int) count,
            Minutes = (int) minutes
        };
        Configuration.Instance.SaveTo();
        args.Player.SendSuccessMessage(GetString($"已设置鱼饵 [i:{selectItem.type}] 的兑换规则：每{count}个 => {minutes}分钟"));
    }

    [Alias("time")]
    public static void Time(CommandArgs args, string type, uint minutes)
    {
        var selectItem = SelectItem(args.Player, type);
        if (selectItem == null)
        {
            return;
        }

        var bait = Configuration.Instance.BaitRewards[selectItem.type];
        bait.Minutes = (int) minutes;
        Configuration.Instance.SaveTo();
        args.Player.SendSuccessMessage(GetString($"已设置鱼饵 [i:{selectItem.type}] 的兑换规则：每{bait.Count}个 => {minutes}分钟"));
    }

    public static void Help(CommandArgs args)
    {
        args.Player.SendSuccessMessage(GetString($"[i:3455][c/AD89D5:自][c/D68ACA:动][c/DF909A:钓][c/E5A894:鱼][i:3454]"));
        args.Player.SendSuccessMessage(GetString($"请使用 /afa 进行管理员命令。"));
        args.Player.SendSuccessMessage(GetString($"[自动钓鱼 - 管理员命令]"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa buff -- 开启丨关闭全局钓鱼BUFF"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa multi -- 开启丨关闭多线模式"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa duo 数字 -- 设置多线的钩子数量上限"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa mod -- 开启丨关闭消耗模式"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa set 物品名 数量 分钟 -- 设置鱼饵兑换规则"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa time 物品名 分钟 -- 修改鱼饵兑换时长"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa add 物品名 数量 分钟 -- 添加指定鱼饵"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa del 物品名 -- 移除指定鱼饵"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa monster -- 开启丨关闭不钓怪物"));
        args.Player.SendSuccessMessage(GetString($"[全局] /afa anim -- 开启丨关闭跳过上鱼动画"));
        args.Player.SendSuccessMessage(GetString($"[导出] /afa exportstats -- 查看导出统计信息"));
        args.Player.SendSuccessMessage(GetString($"[导出] /afa export -- 导出游戏钓鱼规则到 export.yml 文件"));
    }

    private static Terraria.Item? SelectItem(TSPlayer player, string type)
    {
        var items = TShock.Utils.GetItemByIdOrName(type);
        if (items.Count > 1)
        {
            player.SendMultipleMatchError(items.Select(i => i.Name));
            return null;
        }
        if (items.Count == 0)
        {
            player.SendErrorMessage(GetString("不存在该物品！"));
            return null;
        }
        return items[0];
    }
}
