using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace DTEntryBlock;

[ApiVersion(2, 1)]
public class DTEntryBlock : TerrariaPlugin
{
    public override string Author => "肝帝熙恩";
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Description => GetString("阻止玩家在击败骷髅王/世纪之花前进入地牢/神庙");
    public override Version Version => new Version(1, 1, 8);
    public static Configuration Config = null!;
    Color orangeColor = new Color(255, 165, 0);

    public DTEntryBlock(Main game) : base(game)
    {
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);

    }
    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage(GetString("[阻止进入地牢或神庙] 重新加载配置完毕。"));
    }
    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdatePhysics);
        LoadConfig();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdatePhysics);
        }
        base.Dispose(disposing);
    }
    private void OnPlayerUpdatePhysics(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        this.CheckPlayerLocation(e.Player);
    }

    private void CheckPlayerLocation(TSPlayer player)
    {
        if (player == null || !player.Active || player.Dead)
        {
            return;
        }

        // 检查是否在地牢环境
        if (this.IsPlayerInDungeon(player) && !player.HasPermission("skullking.bypass") && Config.PreventPlayersEnterDungeon)
        {
            // 检查骷髅王是否被击败
            if (!NPC.downedBoss3)
            {

                if (Config.TeleportPlayersEnterDungeonForUnkilledSkullKing)
                {
                    player.SendMessage(GetString("因为在没击败骷髅王的时候探索地牢，你被传送到出生点."), this.orangeColor);
                    player.Teleport(Main.spawnTileX * 16, Main.spawnTileY * 16);
                    player.TPlayer.ZoneDungeon = false;
                }
                if (Config.KillPlayersEnterDungeonForUnkilledSkullKing)
                {
                    player.KillPlayer();
                    player.SendMessage(GetString("因为在没击败骷髅王的时候探索地牢，你被击杀了."), this.orangeColor);
                    player.TPlayer.ZoneDungeon = false;

                }
            }
        }
        // 检查是否在神庙环境
        if (this.IsPlayerInTemple(player) && !player.HasPermission("Plant.bypass") && Config.PreventPlayersEnterTemple)
        {
            // 检查世纪之花是否被击败
            if (!NPC.downedPlantBoss)
            {
                if (Config.TeleportPlayersEnterTempleForUnkilledPlantBoss)
                {
                    player.SendMessage(GetString("禁止在没击败世纪之花的时候探索神庙，你被传送到出生点"), this.orangeColor);
                    player.Teleport(Main.spawnTileX * 16, Main.spawnTileY * 16);
                    player.TPlayer.ZoneLihzhardTemple = false;
                }
                if (Config.KillPlayersEnterTempleForUnkilledPlantBoss)
                {
                    player.SendMessage(GetString("禁止在没击败世纪之花的时候探索神庙，你被击杀"), this.orangeColor);
                    player.KillPlayer();
                    player.TPlayer.ZoneLihzhardTemple = false;
                }
            }
        }
    }

    private bool IsPlayerInDungeon(TSPlayer player)
    {
        return Main.drunkWorld
            ? player.TPlayer.ZoneDungeon && !(player.TPlayer.position.Y / 16f < Main.dungeonY + 40)
            : player.TPlayer.ZoneDungeon;
    }

    private bool IsPlayerInTemple(TSPlayer player)
    {
        return player.TPlayer.ZoneLihzhardTemple;
    }
}