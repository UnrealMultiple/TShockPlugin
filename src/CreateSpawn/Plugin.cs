using System.Reflection;
using LazyAPI;
using Terraria;
using Terraria.WorldBuilding;
using TerrariaApi.Server;
using TShockAPI;

namespace CreateSpawn;

[ApiVersion(2, 1)]
public class CreateSpawn(Main game) : LazyPlugin(game)
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Author => "少司命 羽学 Eustia";

    public override Version Version => new (1, 0, 1, 1);

    public override string Description => "使用指令复制区域建筑,支持保存建筑文件、跨地图粘贴";
    
    private bool _isCreatingWorld;

    private bool _pendingSpawnAfterReset;

    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.GamePost);
        On.Terraria.WorldGen.CreateNewWorld += this.WorldGen_CreateNewWorld;
        On.Terraria.IO.WorldFile.LoadWorld += this.WorldFile_LoadWorld;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.GamePost);
            On.Terraria.WorldGen.CreateNewWorld -= this.WorldGen_CreateNewWorld;
            On.Terraria.IO.WorldFile.LoadWorld -= this.WorldFile_LoadWorld;
        }

        base.Dispose(disposing);
    }
    

    private Task WorldGen_CreateNewWorld(On.Terraria.WorldGen.orig_CreateNewWorld orig, GenerationProgress? progress, WorldGenerator.Controller? controller, WorldGen.WorldGenerationFinishCallback? afterGeneration)
    {
        this._isCreatingWorld = true;
        return orig(progress, controller, afterGeneration);
    }

    private void WorldFile_LoadWorld(On.Terraria.IO.WorldFile.orig_LoadWorld orig)
    {
        orig();
        if (!this._isCreatingWorld)
        {
            return;
        }

        this._isCreatingWorld = false;
        this._pendingSpawnAfterReset = true;
        this.TryAutoSpawn();
    }


    private void GamePost(EventArgs args)
    {
        if (this._pendingSpawnAfterReset)
        {
            this.TryAutoSpawn();
        }
    }

    private void TryAutoSpawn()
    {
        if (!this._pendingSpawnAfterReset)
        {
            return;
        }

        try
        {
            if (Config.Instance.AutoSpawnBuilds.Count == 0)
            {
                return;
            }

            var spwx = Main.spawnTileX;
            var spwy = Main.spawnTileY;
            var startX = spwx - Config.Instance.CentreX + Config.Instance.AdjustX;
            var startY = spwy - Config.Instance.CountY + Config.Instance.AdjustY;
            foreach (var name in Config.Instance.AutoSpawnBuilds.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var clip = Map.LoadClip(name);
                if (clip == null)
                {
                    TShock.Utils.Broadcast($"未找到名为 {name} 的建筑!", 240, 250, 150);
                    continue;
                }

                Utils.SpawnBuilding(TSPlayer.Server, startX, startY, clip);
            }
        }
        finally
        {
            this._pendingSpawnAfterReset = false;
        }
    }
}
