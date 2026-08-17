using TShockAPI;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private void FishRandCommand(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            ShowStatus(args.Player);
            return;
        }

        switch (args.Parameters[0].ToLowerInvariant())
        {
            case "reload":
            case "r":
                if (!args.Player.HasPermission(AdminPermission))
                {
                    args.Player.SendErrorMessage("你没有重载随机渔获配置的权限。");
                    return;
                }

                if (TryLoadConfig(out string? error))
                    args.Player.SendSuccessMessage(_loadSummary);
                else
                    args.Player.SendErrorMessage($"随机渔获配置重载失败：{error}");
                break;

            case "sample":
            case "preview":
                ShowSample(args);
                break;

            case "stages":
                ShowStages(args.Player);
                break;

            case "stage":
                ShowStageDetails(args);
                break;

            default:
                args.Player.SendInfoMessage("用法：/fishrand [sample [数量]|stages|stage <阶段>|reload]");
                break;
        }
    }

    private void ShowStatus(TSPlayer player)
    {
        string mode = UseRandomNpcMode() ? "随机生物" : "进度物品";
        if (UseRandomNpcMode())
        {
            player.SendInfoMessage($"随机渔获：{(_config.Enabled ? "开启" : "关闭")}；模式：{mode}；当前生物池：{_npcPool.Count} 个。");
        }
        else
        {
            FishingLootStage? activeStage = ResolveActiveStage();
            LootPool pool = BuildCurrentPool();
            string stageText = activeStage == null ? "无阶段" : $"{activeStage.Name} ({activeStage.Id})";
            player.SendInfoMessage($"随机渔获：{(_config.Enabled ? "开启" : "关闭")}；模式：{mode}；当前阶段：{stageText}；当前候选：{pool.Count} 个。");
        }

        player.SendInfoMessage(_loadSummary);
        player.SendInfoMessage($"配置文件：{_configPath}");
    }

    private void ShowSample(CommandArgs args)
    {
        int count = 8;
        if (args.Parameters.Count >= 2)
            int.TryParse(args.Parameters[1], out count);

        count = Math.Clamp(count, 1, 20);
        if (UseRandomNpcMode())
        {
            if (_npcPool.Count == 0)
            {
                args.Player.SendErrorMessage("当前没有可用随机生物。");
                return;
            }

            List<string> npcs = new(count);
            for (int i = 0; i < count; i++)
            {
                PendingNpcChoice? choice = _npcPool.Next();
                if (choice == null)
                    break;

                npcs.Add(choice.DisplayName);
            }

            args.Player.SendInfoMessage($"随机生物预览 {npcs.Count} 个：{string.Join("，", npcs)}");
            return;
        }

        LootPool pool = BuildCurrentPool();
        FishingLootStage? activeStage = ResolveActiveStage();
        if (pool.Count == 0)
        {
            args.Player.SendErrorMessage("当前阶段没有可用渔获候选。");
            return;
        }

        List<string> items = new(count);
        for (int i = 0; i < count; i++)
        {
            PendingLootChoice? choice = pool.Next();
            if (choice == null)
                break;

            items.Add($"{choice.DisplayName} x{choice.MinStack}-{choice.MaxStack}");
        }

        string stageText = activeStage == null ? "无阶段" : activeStage.Name;
        args.Player.SendInfoMessage($"当前阶段 [{stageText}] 随机预览 {items.Count} 个：{string.Join("，", items)}");
    }

    private void ShowStages(TSPlayer player)
    {
        if (UseRandomNpcMode())
        {
            player.SendInfoMessage($"当前是随机生物模式：{_npcPool.Count} 个可钓生物。");
            return;
        }

        if (player == TSPlayer.Server || !player.Active)
        {
            player.SendInfoMessage("查看阶段需要在游戏内执行。");
            return;
        }

        FishingLootStage? activeStage = ResolveActiveStage();

        player.SendInfoMessage("阶段渔获表：");
        foreach (FishingLootStage stage in _config.Stages)
        {
            string state = stage == activeStage
                ? "当前"
                : StageUnlocked(stage)
                    ? "已过"
                    : "未解锁";

            player.SendInfoMessage($"[{state}] {stage.Name} ({stage.Id}) - {stage.Items.Count} 项 - {stage.Description}");
        }

        player.SendInfoMessage("使用 /fishrand stage <阶段ID> 查看该阶段的具体物品。");
    }

    private void ShowStageDetails(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage("用法：/fishrand stage <阶段ID>");
            return;
        }

        string requested = args.Parameters[1].Trim();
        FishingLootStage? stage = _config.Stages.FirstOrDefault(item =>
            item.Id.Equals(requested, StringComparison.OrdinalIgnoreCase));
        if (stage == null)
        {
            args.Player.SendErrorMessage($"找不到阶段：{requested}");
            return;
        }

        string state = stage == ResolveActiveStage()
            ? "当前"
            : StageUnlocked(stage) ? "已解锁" : "未解锁";
        args.Player.SendInfoMessage($"[{state}] {stage.Name} ({stage.Id})：{stage.Description}");
        foreach (LootEntry entry in stage.Items)
        {
            string range = entry.MinStack == entry.MaxStack
                ? entry.MinStack.ToString()
                : $"{entry.MinStack}-{entry.MaxStack}";
            args.Player.SendInfoMessage($"{entry.Name} x{range}，权重 {entry.Weight}");
        }
    }
}
