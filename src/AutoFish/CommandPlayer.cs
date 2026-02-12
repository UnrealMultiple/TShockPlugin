using AutoFish;
using AutoFish.Utils;
using LazyAPI.Attributes;
using TShockAPI;

namespace AutoFishR;

[Command("af")]
[Permissions("autofish")]
internal class CommandPlayer
{
    public static void Help(CommandArgs args)
    {
        args.Player.SendSuccessMessage(GetString($"[i:3455][c/AD89D5:自][c/D68ACA:动][c/DF909A:钓][c/E5A894:鱼][i:3454]"));
        args.Player.SendSuccessMessage(GetString($"/af help -- 查看自动钓鱼菜单"));
        args.Player.SendSuccessMessage(GetString($"/af status -- 查看个人状态"));
        args.Player.SendSuccessMessage(GetString($"/af fish -- 开启丨关闭[c/4686D4:自动钓鱼]功能"));
        args.Player.SendSuccessMessage(GetString($"/af buff -- 开启丨关闭[c/F6B152:钓鱼BUFF]"));
        args.Player.SendSuccessMessage(GetString($"/af multi -- 开启丨关闭[c/87DF86:多钩功能]"));
        args.Player.SendSuccessMessage(GetString($"/af hook 数字 -- 设置个人钩子上限 (<= 全局上限)"));
        args.Player.SendSuccessMessage(GetString($"/af anim -- 开启丨关闭[c/8EC4F4:跳过上鱼动画]"));
        args.Player.SendSuccessMessage(GetString($"/af quest -- 开启丨关闭[c/F4D58D:屏蔽任务鱼]"));
        args.Player.SendSuccessMessage(GetString($"/af list -- 列出消耗模式[c/F5F251:指定物品表]"));
        args.Player.SendSuccessMessage(GetString($"/af rules [页码] -- 查看[c/F9D77E:自定义钓鱼规则]"));
        args.Player.SendSuccessMessage(GetString($"/af bait -- 开启丨关闭[c/92C5EC:保护贵重鱼饵]"));
        args.Player.SendSuccessMessage(GetString($"/af baitlist -- 查看贵重鱼饵列表"));
    }

    [Alias("status")]
    [RealPlayer]
    public static void Status(CommandArgs args)
    {
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        Uitls.SendGradientMessage(args.Player, GetString($"功能：{(playerData.AutoFishEnabled ? "开启" : "关闭")}"));
        Uitls.SendGradientMessage(args.Player, GetString($"BUFF：{(playerData.BuffEnabled ? "开启" : "关闭")}"));
        Uitls.SendGradientMessage(args.Player, GetString($"多钩：{(playerData.MultiHookEnabled ? "开启" : "关闭")}, 钩子上限：{playerData.HookMaxNum}"));
        Uitls.SendGradientMessage(args.Player, GetString($"不钓怪物：{(playerData.BlockMonsterCatch ? "开启" : "关闭")}"));
        Uitls.SendGradientMessage(args.Player, GetString($"跳过上鱼动画：{(playerData.SkipFishingAnimation ? "开启" : "关闭")}"));
        Uitls.SendGradientMessage(args.Player, GetString($"屏蔽任务鱼：{(playerData.BlockQuestFish ? "开启" : "关闭")}"));
        Uitls.SendGradientMessage(args.Player, GetString($"保护贵重鱼饵：{(playerData.ProtectValuableBaitEnabled ? "开启" : "关闭")}"));
        if (Configuration.Instance.BaitRewards.Count != 0 && playerData.CanConsume())
        {
            var (minutes, seconds) = playerData.GetRemainTime();
            Uitls.SendGradientMessage(args.Player, GetString($"消耗模式：开启，剩余：{minutes}分{seconds}秒"));
        }
    }

    [Alias("fish")]
    [Permission("autofish.fish")]
    [RealPlayer]
    public static void Fish(CommandArgs args)
    {
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.AutoFishEnabled = !playerData.AutoFishEnabled;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.AutoFishEnabled ? "开启" : "关闭")}]自动钓鱼功能。"));
    }

    [Alias("buff")]
    [Permission("autofish.buff")]
    [RealPlayer]
    public static void Buff(CommandArgs args)
    {
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.BuffEnabled = !playerData.BuffEnabled;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.BuffEnabled ? "开启" : "关闭")}]自动钓鱼BUFF。"));
    }

    [Alias("multi")]
    [Permission("autofish.multihook")]
    [RealPlayer]
    public static void Multihook(CommandArgs args)
    {
        if (!Configuration.Instance.GlobalMultiHookFeatureEnabled)
        {
            args.Player.SendWarningMessage(GetString("管理员未开启多钩"));
            return;
        }
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.MultiHookEnabled = !playerData.MultiHookEnabled;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.MultiHookEnabled ? "开启" : "关闭")}]多钩。"));
    }

    [Alias("monster")]
    [Permission("autofish.filter.monster")]
    [RealPlayer]
    public static void Monster(CommandArgs args)
    {
        if (!Configuration.Instance.GlobalBlockMonsterCatch)
        {
            args.Player.SendWarningMessage(GetString("管理员未禁止钓起怪物"));
            return;
        }
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.BlockMonsterCatch = !playerData.BlockMonsterCatch;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.BlockMonsterCatch ? "禁止" : "允许")}]钓起怪物。"));
    }

    [Alias("anim")]
    [Permission("autofish.skipanimation")]
    [RealPlayer]
    public static void SkipFishingAnimation(CommandArgs args)
    {
        if (!Configuration.Instance.GlobalSkipFishingAnimation)
        {
            args.Player.SendWarningMessage(GetString("管理员未允许跳过钓鱼动画！"));
            return;
        }
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.SkipFishingAnimation = !playerData.SkipFishingAnimation;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.SkipFishingAnimation ? "跳过" : "恢复")}]钓鱼动画。"));
    }

    [Alias("list")]
    [RealPlayer]
    public static void List(CommandArgs args)
    {
        if (Configuration.Instance.BaitRewards.Count == 0)
        {
            args.Player.SendWarningMessage(GetString("管理员添加消耗模式指定物品！"));
            return;
        }
        var msg = Configuration.Instance.BaitRewards
            .OrderByDescending(b => b.Value)
            .Select(b => $"[i:{b.Key}]x{b.Value.Count} 时长:{b.Value.Minutes}");
        args.Player.SendInfoMessage(GetString($"消耗模式指定物品:"));
        args.Player.SendInfoMessage(string.Join("\n", msg));
    }

    [Alias("bait")]
    [RealPlayer]
    public static void Bait(CommandArgs args)
    {
        if (!Configuration.Instance.GlobalProtectValuableBaitEnabled)
        {
            args.Player.SendWarningMessage(GetString("管理员未添加指定消耗物品！"));
            return;
        }
        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.ProtectValuableBaitEnabled = !playerData.ProtectValuableBaitEnabled;
        args.Player.SendSuccessMessage(GetString($"你已[c/92C5EC:{(playerData.SkipFishingAnimation ? "开启" : "关闭")}]保护贵重鱼饵。"));
    }

    [Alias("baitlist")]
    [RealPlayer]
    public static void BaitList(CommandArgs args)
    {
        if (Configuration.Instance.ValuableBaitItemIds.Count == 0)
        {
            args.Player.SendWarningMessage(GetString("管理员未添加贵重鱼饵！"));
            return;
        }
        var msg = Configuration.Instance.ValuableBaitItemIds.Select(b => $"[i:{b}]");
        args.Player.SendInfoMessage(GetString($"贵重鱼饵列表:"));
        args.Player.SendInfoMessage(string.Join(",", msg));
    }


    [Alias("hook")]
    [Permission("autofish.multihook")]
    [RealPlayer]
    public static void Multihook(CommandArgs args, uint max)
    {
        if (!Configuration.Instance.GlobalMultiHookFeatureEnabled)
        {
            args.Player.SendWarningMessage(GetString("管理员未开启多钩功能！"));
            return;
        }
        if (max < 1)
        {
            args.Player.SendErrorMessage(GetString("最大数量不能小于1！"));
            return;
        }
        if (max > Configuration.Instance.GlobalMultiHookMaxNum)
        {
            args.Player.SendWarningMessage(GetString($"最大数量不能大于{Configuration.Instance.GlobalMultiHookMaxNum}"));
            return;
        }

        var playerData = Plugin.PlayerData.GetOrCreatePlayerData(args.Player.Name, Plugin.CreateDefaultPlayerData);
        playerData.HookMaxNum = (int) max;
        args.Player.SendSuccessMessage(GetString($"已将个人钩子上限设置为：{max} (全局上限 {Configuration.Instance.GlobalMultiHookMaxNum})"));
    }


}
