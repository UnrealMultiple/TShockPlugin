using LazyAPI;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Generation;
using TerrariaApi.Server;
using TShockAPI;

namespace CreateSpawn;

[ApiVersion(2, 1)]
public class CreateSpawn(Main game) : LazyPlugin(game)
{
    public override string Name => "复制建筑";

    public override string Author => "少司命 羽学";

    public override Version Version => new(1, 0, 1, 0);

    public override string Description => "使用指令复制区域建筑,支持保存建筑文件、跨地图粘贴";

    public bool IsNewWorld;

    public override void Initialize()
    {
        ExtractData();
        ServerApi.Hooks.GamePostInitialize.Register(this, this.GamePost);
        On.Terraria.WorldGen.AddGenerationPass_string_WorldGenLegacyMethod += this.WorldGen_AddGenerationPass_string_WorldGenLegacyMethod;
        base.Initialize();
    }

    private void WorldGen_AddGenerationPass_string_WorldGenLegacyMethod(On.Terraria.WorldGen.orig_AddGenerationPass_string_WorldGenLegacyMethod orig, string name, WorldGenLegacyMethod method)
    {
        orig(name, method);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.GamePost);
            On.Terraria.WorldGen.AddGenerationPass_string_WorldGenLegacyMethod -= this.WorldGen_AddGenerationPass_string_WorldGenLegacyMethod;
        }
        base.Dispose(disposing);
    }


    private void GamePost(EventArgs args)
    {
        if (this.IsNewWorld)
        {
            var name = "DefaultBuild";
            var clip = Map.LoadClip(name);
            if (clip == null)
            {
                TShock.Utils.Broadcast($"未找到名为 {name} 的建筑!", 240, 250, 150);
                return;
            }

            var spwx = Main.spawnTileX; // 出生点 X（单位是图格）
            var spwy = Main.spawnTileY; // 出生点 Y
            var startX = spwx - Config.Instance.CentreX + Config.Instance.AdjustX;
            var startY = spwy - Config.Instance.CountY + Config.Instance.AdjustY;
            Utils.SpawnBuilding(TSPlayer.Server, startX, startY, clip);
            this.IsNewWorld = false;
        }
    }

    private static void ExtractData()
    {
        if (Directory.Exists(Map.Paths))
        {
            return;
        }
        Directory.CreateDirectory(Map.Paths);
        var assembly = Assembly.GetExecutingAssembly();
        var resource = $"{assembly.GetName().Name}.Resources.DefaultBuild_cp.map";
        using var stream = assembly.GetManifestResourceStream(resource);
        using var fileStream = File.Create(Path.Combine(Map.Paths, "DefaultBuild_cp.map"));
        stream?.CopyTo(fileStream);
    }
}