using Microsoft.Xna.Framework;
using MonoMod.Core;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using TerrariaApi.Server;

namespace AutoClassificationQuickStack;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "xuyuwtu";
    public override string Description => GetString("快速堆叠时根据配置放入箱子");
    public override Version Version => new Version(1, 2);
    #endregion

    #region 注册重载释放
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        // 应用 Detour，使新的 PutItemInNearbyChest 方法生效。
        this._detour.Apply();

        // 注册重载事件处理器，以便在服务器重载时更新配置。
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += this.OnReload;
        // 注册游戏初始化后的事件处理器，确保在游戏完全启动后执行某些操作。
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnGamePostInitialize);
    }

    // 释放资源的方法，在插件卸载或程序结束时调用。
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 清理 Detour 和事件处理器。
            this._detour.Dispose();
            TShockAPI.Hooks.GeneralHooks.ReloadEvent -= this.OnReload;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnGamePostInitialize);
        }
    }

    // 游戏初始化后的事件处理器，首次运行时调用一次。
    private void OnGamePostInitialize(EventArgs args)
    {
        // 触发重载事件以初始化配置。
        this.OnReload(null!);
        // 取消注册自身，避免重复触发。
        ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnGamePostInitialize);
    }
    #endregion

    #region 配置文件重载
    // 存储分类信息的字典，键是物品 ID，值是与之可以一起存放的物品 ID 数组。
    private static readonly Dictionary<int, int[]> ClassificationInfos = new();
    private void OnReload(TShockAPI.Hooks.ReloadEventArgs e)
    {
        // 获取最新的配置信息。
        var config = Configuration.GetConfig();
        // 如果有玩家发送了重载命令，则向该玩家发送成功消息。
        e?.Player.SendSuccessMessage($"[{nameof(AutoClassificationQuickStack)}]{GetString("重载完成")}");

        // 清空当前的分类信息，并从新配置中重新填充。
        ClassificationInfos.Clear();
        foreach (var info in config.ClassificationInfos)
        {
            foreach (var key in info.Keys)
            {
                ClassificationInfos.Add(key, info.Values);
            }
        }
    }
    #endregion

    // 创建一个 Detour 对象，用于拦截 Chest.PutItemInNearbyChest 方法，并替换为自定义逻辑。
    private readonly ICoreDetour _detour = DetourFactory.Current.CreateDetour(
        typeof(Chest).GetMethod(nameof(PutItemInNearbyChest))
            ?? throw new InvalidOperationException($"{GetString("找不到方法")} {nameof(PutItemInNearbyChest)}"),
        new Func<Item, Vector2, int, Item>(PutItemInNearbyChest).Method,
        applyByDefault: false
    );
    // 自定义的 PutItemInNearbyChest 方法，实现物品分类和堆叠逻辑。
    internal static Item PutItemInNearbyChest(Item putItem, Vector2 position, int playerID)
    {
        // 如果是在多人模式下作为客户端运行，则直接返回原物品。
        if (Main.netMode == 1)
        {
            return putItem;
        }

        // 遍历所有箱子，寻找合适的放置位置。
        for (var i = 0; i < Main.maxChests; i++)
        {
            // 跳过无效或被占用的箱子。
            if (Main.chest[i] == null || Chest.IsPlayerInChest(i) || Chest.IsLocked(Main.chest[i].x, Main.chest[i].y))
            {
                continue;
            }

            // 计算箱子中心点的位置。
            var chestCenter = new Vector2((Main.chest[i].x * 16) + 16, (Main.chest[i].y * 16) + 16);

            // 检查箱子是否在合理范围内且可以通过快捷堆叠功能访问。
            if ((chestCenter - position).Length() >= 600f || !OTAPI.Hooks.Chest.InvokeQuickStack(playerID, putItem, i))
            {
                continue;
            }

            var hasSameAs = false;
            var hasAir = false;
            var chest = Main.chest[i];

            // 检查箱子内是否有相同类型的物品或空位。
            for (var j = 0; j < chest.item.Length; j++)
            {
                if (chest.item[j].IsAir)
                {
                    hasAir = true;
                }
                else if (putItem.IsTheSameAs(chest.item[j]))
                {
                    hasSameAs = true;
                    var remainingStack = chest.item[j].maxStack - chest.item[j].stack;
                    if (remainingStack > 0)
                    {
                        if (remainingStack > putItem.stack)
                        {
                            remainingStack = putItem.stack;
                        }
                        // 显示物品转移动画。
                        Chest.VisualizeChestTransfer(position, chestCenter, putItem, remainingStack);
                        putItem.stack -= remainingStack;
                        chest.item[j].stack += remainingStack;
                        if (putItem.stack <= 0)
                        {
                            putItem.SetDefaults();
                            return putItem;
                        }
                    }
                }
            }

            // 如果没有空位或者物品已经全部放入，则跳过后续处理。
            if (!hasAir || putItem.stack <= 0)
            {
                continue;
            }

            // 检查附近是否存在框架（如装饰品框架），并且尝试根据分类信息匹配物品。
            var checkY = chest.y - 2;
            if (!hasSameAs && WorldGen.InWorld(chest.x, checkY, 10))
            {
                var id = TEItemFrame.Find(chest.x, checkY);
                if (id != -1 && ClassificationInfos.TryGetValue(((TEItemFrame)TileEntity.ByID[id]).item.type, out var itemIDs) && Array.BinarySearch(itemIDs, putItem.type) >= 0)
                {
                    hasSameAs = true;
                }
            }

            // 如果找到了匹配的分类信息，则继续尝试放置物品。
            if (!hasSameAs)
            {
                continue;
            }

            // 尝试将物品放入第一个空槽。
            for (var k = 0; k < chest.item.Length; k++)
            {
                if (chest.item[k].type == 0 || chest.item[k].stack == 0)
                {
                    Chest.VisualizeChestTransfer(position, chestCenter, putItem, putItem.stack);
                    chest.item[k] = putItem.Clone();
                    putItem.SetDefaults();
                    return putItem;
                }
            }
        }

        // 如果没有找到合适的位置，则返回未改变的物品。
        return putItem;
    }
}