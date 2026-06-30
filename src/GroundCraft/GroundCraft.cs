using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace GroundCraft;

[ApiVersion(2, 1)]
public sealed partial class GroundCraft : TerrariaPlugin
{
    private const int Anywhere = -1;
    private const string DefaultPlayerPermission = "tshock.canchat";
    private const string DefaultAdminPermission = "groundcraft.admin";
    private const int CraftAnimationTicks = 72;
    private const int CraftAnimationSyncEveryTicks = 1;
    private const int CraftAnimationNoGrabDelay = 120;
    private const float CraftAnimationLiftPixels = 104f;
    private const float CraftAnimationOrbitRadiusPixels = 30f;
    private const float CraftAnimationTurns = 1.65f;
    private const int ZenithAnimationTicks = 210;
    private const int ZenithAnimationSyncEveryTicks = 2;
    private const float ZenithAnimationLiftPixels = 240f;
    private const float ZenithAnimationOrbitRadiusPixels = 136f;
    private const float ZenithAnimationTurns = 4.5f;
    private const float ZenithAnimationParabolaPixels = 76f;
    private const int ZenithTrailEveryTicks = 30;
    private const int ZenithTrailOrderOffsetTicks = 3;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };

    private readonly List<Command> _commands = new();
    private readonly Dictionary<int, int> _stableScans = new();
    private readonly RuntimeStats _runtime = new();
    private readonly object _stateLock = new();
    private readonly List<CraftAnimation> _craftAnimations = new();
    private readonly HashSet<int> _lockedItemIndexes = new();

    private GroundCraftConfig _config = GroundCraftConfig.Default();
    private List<DropRecipe> _recipes = new();
    private RecipeAudit _audit = new();
    private int _ticks;
    private bool _hooked;
    private bool _netHooked;

    public override string Name => "GroundCraft";
    public override string Author => "愚蠢";
    public override string Description => GetString("地上合成：把掉落物丢在一起，根据 JSON 配方和环境/进度条件自动合成。");
    public override Version Version => new(1, 1, 0);

    private static string DataDirectory => Path.Combine(TShock.SavePath, "GroundCraft");
    private static string ConfigPath => Path.Combine(DataDirectory, "config.json");
    private static string RecipesPath => Path.Combine(DataDirectory, "recipes.json");

    public GroundCraft(Main game) : base(game)
    {
        Order = 28;
    }

    public override void Initialize()
    {
        ReloadFiles(logToConsole: true);

        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftInfo, "groundcraft", "gc")
        {
            HelpText = GetString("查看地上合成状态。")
        });
        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftRecipes, "gcrecipes", "gcr")
        {
            HelpText = GetString("查看地上合成配方。用法：/gcrecipes [页码|搜索]")
        });
        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftEnvironment, "gcenv")
        {
            HelpText = GetString("查看当前位置可用于地上合成的环境标签。")
        });
        Register(new Command(_config.AdminPermissionOrDefault(), GroundCraftReload, "gcreload")
        {
            HelpText = GetString("热重载 GroundCraft 的 config.json 和 recipes.json。")
        });
        Register(new Command(_config.AdminPermissionOrDefault(), GroundCraftAudit, "gcaudit")
        {
            HelpText = GetString("查看 GroundCraft 配方审核结果。")
        });

        ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        _hooked = true;
        ServerApi.Hooks.NetGetData.Register(this, OnGetData, int.MinValue);
        _netHooked = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_hooked)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                _hooked = false;
            }
            if (_netHooked)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                _netHooked = false;
            }

            ReleaseCraftAnimations();

            foreach (Command command in _commands)
                Commands.ChatCommands.Remove(command);

            _commands.Clear();
            TShock.Log.ConsoleInfo(GetString("[GroundCraft] 插件已卸载。"));
        }

        base.Dispose(disposing);
    }

    private void Register(Command command)
    {
        _commands.Add(command);
        Commands.ChatCommands.Add(command);
    }

}
