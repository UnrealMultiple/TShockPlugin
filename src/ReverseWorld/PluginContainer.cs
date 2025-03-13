using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ReverseWorld;

[ApiVersion(2, 1)]
public class PluginContainer : TerrariaPlugin
{
    public PluginContainer(Main game) : base(game) { }

    public override string Author => "1413, 肝帝熙恩适配1449";

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 0, 2);


    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("tofout.reverseworld", Method, "reverseworld", "rw", "反转世界")
        {
            HelpText = GetString("反转整个世界的方向和地形")
        });

        Commands.ChatCommands.Add(new Command("tofout.placelandmine", cmd => Code.Method(cmd.Player, cmd.Parameters), "placelandmine", "plm", "放置地雷")
        {
            HelpText = GetString("在玩家当前位置放置地雷")
        });
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == Method || c.Name == "plm");
        }
        base.Dispose(disposing);
    }

    public static void ReverseWorld(bool nokick = true)
    {
        var chestList = new List<Chest>(Main.chest);
        foreach (var chest in chestList)
        {
            if (chest != null)
            {
                ReverseChest(chest);
            }
        }

        var underworldLayer = Main.UnderworldLayer;
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < underworldLayer / 2; y++)
            {
                if (Main.tile[x, y] != null && Main.tile[x, underworldLayer - 1 - y] != null)
                {
                    var tempTile = (ITile) Main.tile[x, y].Clone();
                    Main.tile[x, y] = (ITile) Main.tile[x, underworldLayer - 1 - y].Clone();
                    Main.tile[x, underworldLayer - 1 - y] = tempTile;
                }
            }
        }

        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < underworldLayer; y++)
            {
                var tile = Main.tile[x, y];
                if (tile != null)
                {
                    var slope = tile.slope();
                    switch (slope)
                    {
                        case 1:
                            slope = 3;
                            break;
                        case 2:
                            slope = 4;
                            break;
                        case 3:
                            slope = 1;
                            break;
                        case 4:
                            slope = 2;
                            break;
                    }
                    tile.slope(slope);
                }
            }
        }
        for (var i = 0; i < TShock.Players.Length; i++)
        {
            var player = TShock.Players[i];
            if (player != null && player.Active)
            {
                if (nokick)
                {
                    Replenisher.UpdateSection(player.TileX, player.TileY, Main.maxTilesX, Main.maxTilesY, -1);
                }
                else
                {
                    player.Kick(GetString("世界已成功反转！请重新加入！"));
                }
            }
        }

        TShock.Utils.Broadcast(GetString("世界已成功反转！"), Color.Yellow);
    }


    private static void ReverseChest(Chest chest)
    {
        var maxX = Main.maxTilesX - 1;
        var maxY = Main.maxTilesY - 1;

        // 确保 chest.x 和 chest.y 在有效范围内
        if (chest.x >= 0 && chest.x < maxX && chest.y >= 0 && chest.y < maxY)
        {
            // 反转 chest 的位置
            chest.y = Main.UnderworldLayer - 1 - chest.y - 1;
        }
    }

    public static void Method(CommandArgs args)
    {
        var nokick = true;

        if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "kick")
        {
            nokick = false;
        }

        ReverseWorld(nokick);
    }

}

