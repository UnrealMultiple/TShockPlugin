using System.Reflection;
using LazyAPI;
using Terraria;
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

    public bool IsNewWorld;

    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.GamePost);
        On.Terraria.WorldBuilding.GenerationProgress.End += this.GenerationProgress_End;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.GamePost);
            On.Terraria.WorldBuilding.GenerationProgress.End -= this.GenerationProgress_End;
        }

        base.Dispose(disposing);
    }

    private void GenerationProgress_End(On.Terraria.WorldBuilding.GenerationProgress.orig_End orig, Terraria.WorldBuilding.GenerationProgress self)
    {
        orig(self);
        this.IsNewWorld = true;
    }


    private void GamePost(EventArgs args)
    {
        if (this.IsNewWorld)
        {
            if (Config.Instance.AutoSpawnBuilds.Count == 0)
            {
                this.IsNewWorld = false;
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

            this.IsNewWorld = false;
        }
    }
}