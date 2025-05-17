using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DwTP;

[ApiVersion(2, 1)]
public class dwTP : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学";
    public override Version Version => new Version(1, 0, 3);
    public override string Description => GetString("用/dw命令传送到微光湖、地牢、神庙、花苞、宝藏袋位置");
    #endregion

    #region 注册与释放
    public dwTP(Main game) : base(game) { }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("dw.use", this.dwCmd, "dw", "定位"));
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKilled);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.CommandDelegate == this.dwCmd);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKilled);
        }
        base.Dispose(disposing);
    }

    #endregion

    #region Boss死亡记录宝藏袋位置
    private bool Bag = false;
    private int BagX = 0;
    private int BagY = 0;
    private void OnNpcKilled(NpcKilledEventArgs e)
    {
        if (e.npc.boss)
        {
            this.Bag = true;
        }

        if (this.Bag)
        {
            this.BagX = (int) e.npc.position.X;
            this.BagY = (int) e.npc.position.Y;
        }
    }
    #endregion

    #region 指令方法
    private void dwCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(GetString($"[i:3455][c/AD89D5:定][c/D68ACA:位][c/DF909A:传][c/E5A894:送][i:3454] [C/BFDFEA:by] [c/7CAEDD:羽学][i:3459]\n") +
                GetString($"/dw wg ——传送到[c/E2E4C4:微光湖]附近\n") +
                GetString($"/dw sm ——传送到[c/E2E4C4:神庙]附近\n") +
                GetString($"/dw hb ——传送到[c/E2E4C4:花苞]附近\n") +
                GetString($"/dw bag ——传送到[c/E2E4C4:宝藏袋]附近\n") +
                GetString($"/dw dl ——传送到[c/E2E4C4:地牢]附近[骷髅王前/石后]"), 247, 244, 150);
        }

        else if (args.Parameters.Count == 1 && (args.Parameters[0] == "wg" || args.Parameters[0] == "微光"))
        {
            var flag = false;
            var x2 = 0;
            var y2 = 0;

            if (!flag)
            {
                for (var x = 0; x < Main.maxTilesX; x++)
                {
                    for (var y = 0; y < Main.maxTilesY; y++)
                    {
                        var tile = Main.tile[x, y];
                        if (tile != null && tile.liquidType() == Terraria.ID.LiquidID.Shimmer)
                        {
                            x2 = x;
                            y2 = y - 3;

                            if (!tile.active())
                            {
                                WorldGen.PlaceTile(x, y, 38, false, true, -1, 0);
                            }

                            flag = true;
                            break;
                        }
                    }
                }
            }

            if (flag)
            {
                args.Player.SendMessage(GetString($"已将您传送到[c/F25156:微光湖]附近([c/E2E4C4:{x2} {y2}])"), 222, 192, 223);
                args.Player.Teleport(x2 * 16, y2 * 16, 10);
            }
            return;
        }

        else if (args.Parameters.Count == 1 && (args.Parameters[0] == "sm" || args.Parameters[0] == "神庙"))
        {
            var flag = false;
            var x2 = 0;
            var y2 = 0;

            if (!flag)
            {
                for (var x = 0; x < Main.maxTilesX; x++)
                {
                    for (var y = 0; y < Main.maxTilesY; y++)
                    {
                        var tile = Main.tile[x, y];
                        if (tile == null)
                        {
                            return;
                        }

                        if (tile.type == Terraria.ID.TileID.ClosedDoor && tile.wall == Terraria.ID.WallID.LihzahrdBrickUnsafe)
                        {
                            x2 = x + 6;
                            y2 = y - 3;
                            flag = true;
                            break;
                        }
                        else if (tile.type == 237)
                        {
                            x2 = x + 6;
                            y2 = y - 3;
                            flag = true;
                            break;
                        }
                    }
                }
            }

            if (flag)
            {
                args.Player.SendMessage(GetString($"已将您传送到[c/F25156:神庙]附近([c/E2E4C4:{x2} {y2}])"), 222, 192, 223);
                args.Player.Teleport(x2 * 16, y2 * 16, 10);
            }
            return;
        }

        else if (args.Parameters.Count == 1 && (args.Parameters[0] == "hb" || args.Parameters[0] == "花苞"))
        {
            var flag = false;
            var x2 = 0;
            var y2 = 0;

            if (!flag)
            {
                for (var x = 0; x < Main.maxTilesX; x++)
                {
                    for (var y = 0; y < Main.maxTilesY; y++)
                    {
                        var tile = Main.tile[x, y];
                        if (tile != null && tile.type == 238)
                        {
                            x2 = x;
                            y2 = y - 3;
                            flag = true;
                            break;
                        }
                    }
                }
            }

            if (flag)
            {
                args.Player.SendMessage(GetString($"已将您传送到[c/F25156:花苞]附近([c/E2E4C4:{x2} {y2}])"), 222, 192, 223);
                args.Player.Teleport(x2 * 16, y2 * 16, 10);
            }
            else
            {
                args.Player.SendMessage(GetString($"世纪之花[c/F25156:未生长],无法获取[c/E2E4C4:花苞坐标]"), 222, 192, 223);
            }
            return;
        }

        else if (args.Parameters.Count == 1 && (args.Parameters[0] == "dl" || args.Parameters[0] == "地牢"))
        {
            var flag = false;
            var npcY = 0;
            var npcX = 0;
            if (!flag)
            {
                for (var i = 0; i < Main.npc.Length; i++)
                {
                    var npc = Main.npc[i];
                    if (npc.active)
                    {
                        if (npc.type == 37)
                        {
                            npcY = (int) npc.position.Y;
                            npcX = (int) npc.position.X;
                            flag = true;
                            break;
                        }
                        else if (npc.type == 438)
                        {
                            npcY = (int) npc.position.Y;
                            npcX = (int) npc.position.X;
                            flag = true;
                            break;
                        }
                    }
                }
            }

            if (flag)
            {
                args.Player.SendMessage(GetString($"已将您传送到[c/F25156:地牢]附近([c/E2E4C4:{npcX / 16} {npcY / 16}])"), 222, 192, 223);
                args.Player.Teleport(npcX, npcY, 10);
            }
            else
            {
                args.Player.SendMessage(GetString($"地牢守护者[c/F25156:已击败],无法获取[c/E2E4C4:地牢坐标]"), 222, 192, 223);
            }

            return;
        }

        else if (args.Parameters.Count == 1 && (args.Parameters[0] == "bag" || args.Parameters[0] == "宝藏袋"))
        {
            if (this.Bag)
            {
                args.Player.SendMessage(GetString($"已将您传送到[c/F25156:宝藏袋]附近([c/E2E4C4:{this.BagX / 16} {this.BagY / 16}])"), 222, 192, 223);
                args.Player.Teleport(this.BagX, this.BagY, 10);
            }
            return;
        }
    }
    #endregion
}