using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace GroundCraft;

public sealed partial class GroundCraft
{
    private void GroundCraftInfo(CommandArgs args)
    {
        bool enabled;
        int recipeCount;
        float clusterRadiusTiles;
        int requiredStableScans;
        int maxCraftsPerClusterPerScan;
        bool requireExactIngredientTypes;
        bool animateConsumedItems;
        lock (_stateLock)
        {
            enabled = _config.Enabled;
            recipeCount = _recipes.Count;
            clusterRadiusTiles = _config.ClusterRadiusTiles;
            requiredStableScans = _config.RequiredStableScans;
            maxCraftsPerClusterPerScan = _config.MaxCraftsPerClusterPerScan;
            requireExactIngredientTypes = _config.RequireExactIngredientTypes;
            animateConsumedItems = _config.AnimateConsumedItems;
        }

        args.Player.SendInfoMessage(GetString($"地上合成：{(enabled ? "已启用" : "已关闭")}，启用配方 {recipeCount} 条。"));
        args.Player.SendInfoMessage(GetString($"材料丢在 {clusterRadiusTiles:0.##} 格内，静止 {requiredStableScans} 次扫描后尝试合成；每堆每次最多合成 {maxCraftsPerClusterPerScan} 批。"));
        args.Player.SendInfoMessage(GetString($"精确材料匹配={(requireExactIngredientTypes ? "开启" : "关闭")}，螺旋动画={(animateConsumedItems ? "开启" : "关闭")}。"));
        args.Player.SendInfoMessage(GetString("命令：/gcrecipes [页码|搜索]、/gcenv、/gcaudit、/gcreload。"));
        args.Player.SendInfoMessage(GetString($"配置：{ConfigPath}；配方：{RecipesPath}。"));
    }

    private void GroundCraftRecipes(CommandArgs args)
    {
        int page = 1;
        string query = "";
        if (args.Parameters.Count > 0)
        {
            if (!int.TryParse(args.Parameters[0], out page))
                query = string.Join(" ", args.Parameters);
        }

        List<DropRecipe> recipes;
        lock (_stateLock)
            recipes = _recipes.ToList();

        List<DropRecipe> matches = string.IsNullOrWhiteSpace(query)
            ? recipes
            : recipes.Where(recipe => MatchesRecipeQuery(recipe, query)).ToList();

        const int pageSize = 6;
        int maxPage = Math.Max(1, (int)Math.Ceiling(matches.Count / (double)pageSize));
        page = Math.Clamp(page, 1, maxPage);

        string queryPart = string.IsNullOrWhiteSpace(query) ? "" : GetString($"，搜索“{query}”");
        args.Player.SendInfoMessage(GetString($"地上合成配方{queryPart}：第 {page}/{maxPage} 页，共 {matches.Count} 条。"));

        foreach (DropRecipe recipe in matches.Skip((page - 1) * pageSize).Take(pageSize))
            args.Player.SendInfoMessage(RecipeLine(recipe));
    }

    private static bool MatchesRecipeQuery(DropRecipe recipe, string query)
    {
        if (recipe.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || ItemName(recipe.OutputType).Contains(query, StringComparison.OrdinalIgnoreCase))
            return true;

        return recipe.Ingredients.Keys.Any(type => ItemName(type).Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    private void GroundCraftEnvironment(CommandArgs args)
    {
        if (args.Player == TSPlayer.Server)
        {
            args.Player.SendErrorMessage(GetString("该命令需要玩家在游戏内执行。"));
            return;
        }

        EnvironmentSnapshot snapshot;
        lock (_stateLock)
            snapshot = ProbeEnvironment(PlayerCenter(args.Player.TPlayer));

        args.Player.SendInfoMessage(GetString($"当前层级：{snapshot.Layer}。"));
        args.Player.SendInfoMessage(GetString($"当前生物群系：{FormatTags(snapshot.Biomes)}。"));
        args.Player.SendInfoMessage(GetString($"附近液体：{FormatTags(snapshot.Liquids)}。"));
        args.Player.SendInfoMessage(GetString($"Boss 进度示例：困难模式={(Main.hardMode ? "是" : "否")}，任意机械Boss={(NPC.downedMechBossAny ? "已击败" : "未击败")}。"));
    }

    private void GroundCraftReload(CommandArgs args)
    {
        ReloadFiles(logToConsole: true);
        int recipeCount;
        lock (_stateLock)
            recipeCount = _recipes.Count;

        args.Player.SendSuccessMessage(GetString($"GroundCraft 已热重载：启用配方 {recipeCount} 条。无需重启服务器。"));
    }

    private void GroundCraftAudit(CommandArgs args)
    {
        lock (_stateLock)
        {
            args.Player.SendInfoMessage(GetString($"读取={_audit.Seen}，禁用={_audit.Disabled}，格式通过={_audit.ImportAccepted}，安全通过={_audit.SafetyAccepted}，启用={_audit.ActiveRecipes}。"));
            SendReasons(args.Player, GetString("导入拒绝"), _audit.ImportRejects);
            SendReasons(args.Player, GetString("安全拒绝"), _audit.SafetyRejects);
            args.Player.SendInfoMessage(GetString($"运行统计：扫描={_runtime.Scans}，材料堆={_runtime.Clusters}，合成批次={_runtime.CraftBatches}，合成次数={_runtime.Crafts}，未匹配={_runtime.NoMatches}，额外材料拒绝={_runtime.ExtraItemTypeRejects}，条件不符={_runtime.ConditionMisses}。"));
        }
    }

    private static string RecipeLine(DropRecipe recipe)
    {
        string ingredients = string.Join(" + ", recipe.Ingredients
            .OrderBy(p => ItemName(p.Key), StringComparer.OrdinalIgnoreCase)
            .Select(p => $"{ItemName(p.Key)} x{p.Value}"));

        return GetString($"{recipe.Id}: {ingredients} => {ItemName(recipe.OutputType)} x{recipe.OutputStack} @ {StationName(recipe.RequiredTiles)} {ConditionName(recipe.Conditions)}");
    }

    private static string StationName(IReadOnlyCollection<int> tileTypes)
    {
        if (tileTypes.Count == 0)
            return GetString("任意地点");

        return string.Join("/", tileTypes.Select(tileType => tileType switch
        {
            TileID.WorkBenches => GetString("工作台"),
            TileID.Furnaces => GetString("熔炉"),
            TileID.Hellforge => GetString("地狱熔炉"),
            TileID.Anvils => GetString("铁砧/铅砧"),
            TileID.Sawmill => GetString("锯木机"),
            TileID.Loom => GetString("织布机"),
            TileID.Kegs => GetString("酒桶"),
            TileID.Bottles => GetString("瓶子/炼药桌"),
            TileID.CookingPots => GetString("烹饪锅"),
            TileID.TinkerersWorkbench => GetString("工匠作坊"),
            TileID.Extractinator => GetString("提炼机"),
            TileID.MythrilAnvil => GetString("秘银砧/山铜砧"),
            _ => GetString($"图块 {tileType}")
        }));
    }

    private static string ConditionName(ConditionSet conditions)
    {
        List<string> parts = new();
        if (conditions.Layers.Length > 0)
            parts.Add(GetString($"层级={string.Join("/", conditions.Layers)}"));
        if (conditions.Biomes.Length > 0)
            parts.Add(GetString($"地形={string.Join("/", conditions.Biomes)}"));
        if (conditions.Liquids.Length > 0)
            parts.Add(GetString($"液体={string.Join("+", conditions.Liquids)}"));
        if (conditions.AnyLiquids.Length > 0)
            parts.Add(GetString($"任一液体={string.Join("/", conditions.AnyLiquids)}"));
        if (!conditions.BossProgress.IsEmpty)
            parts.Add(GetString("进度条件"));

        return parts.Count == 0 ? "" : GetString($"[{string.Join("，", parts)}]");
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0 ? GetString("无") : string.Join(", ", tags.OrderBy(v => v, StringComparer.OrdinalIgnoreCase));
    }

    private static void SendReasons(TSPlayer player, string title, IReadOnlyDictionary<string, int> reasons)
    {
        if (reasons.Count == 0)
        {
            player.SendInfoMessage(GetString($"{title}：无。"));
            return;
        }

        player.SendInfoMessage(GetString($"{title}：{FormatReasons(reasons)}"));
    }

    private static string FormatReasons(IReadOnlyDictionary<string, int> reasons)
    {
        if (reasons.Count == 0)
            return GetString("无");

        return string.Join(", ", reasons.OrderByDescending(p => p.Value).ThenBy(p => p.Key).Take(8).Select(p => $"{ReasonName(p.Key)}={p.Value}"));
    }

    private static string ReasonName(string reason)
    {
        return reason switch
        {
            "invalid_output" => GetString("产物非法"),
            "empty_ingredients" => GetString("没有材料"),
            "invalid_ingredient" => GetString("材料非法"),
            "unknown_layer" => GetString("未知层级"),
            "unknown_biome" => GetString("未知地形"),
            "unknown_liquid" => GetString("未知液体"),
            "unknown_boss_progress" => GetString("未知Boss进度"),
            "needs_two_material_types" => GetString("材料种类不足"),
            "coins" => GetString("涉及钱币"),
            "input_is_output" => GetString("材料和产物相同"),
            "output_over_max_stack" => GetString("产物超过最大堆叠"),
            "ambiguous_same_inputs" => GetString("同材料同条件多结果歧义"),
            _ => reason
        };
    }

}
