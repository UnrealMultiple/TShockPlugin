using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CreateSpawn;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("出生建筑");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 6);


    private bool create = false;
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        On.Terraria.WorldBuilding.GenerationProgress.End += this.GenerationProgress_End;
        Commands.ChatCommands.Add(new Command("create.copy", this.copy, "cb"));
        Commands.ChatCommands.Add(new Command("create.copy", this.CreateBuilding, "create"));
        ServerApi.Hooks.GamePostInitialize.Register(this, this.GamePost);
    }

    private void GamePost(EventArgs args)
    {
        if (this.create)
        {
            this.SpawnBuilding();
        }
    }


    private void GenerationProgress_End(On.Terraria.WorldBuilding.GenerationProgress.orig_End orig, Terraria.WorldBuilding.GenerationProgress self)
    {
        this.create = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            On.Terraria.WorldBuilding.GenerationProgress.End -= this.GenerationProgress_End;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.copy || x.CommandDelegate == this.CreateBuilding);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.GamePost);
        }

        base.Dispose(disposing);
    }

    private void CreateBuilding(CommandArgs args)
    {
        this.SpawnBuilding();
        args.Player.SendInfoMessage(GetString("建筑已创建!"));
    }

    private void copy(CommandArgs args)
    {
        if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "set")
        {
            switch (args.Parameters[1])
            {
                case "1":
                    args.Player.AwaitingTempPoint = 1;
                    args.Player.SendInfoMessage(GetString("请选择复制区域的左上角"));
                    break;
                case "2":
                    args.Player.AwaitingTempPoint = 2;
                    args.Player.SendInfoMessage(GetString("请选择复制区域的右下角"));
                    break;
                default:
                    args.Player.SendInfoMessage(GetString($"正确指令：/cb set <1/2> --选择复制的区域"));
                    args.Player.SendInfoMessage("/cb save");
                    break;
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "save")
        {
            if (args.Player.TempPoints[0].X == 0 || args.Player.TempPoints[1].X == 0)
            {
                args.Player.SendInfoMessage(GetString("您还没有选择复制的区域！"));
            }
            else
            {
                this.CopyBuilding(args.Player.TempPoints[0].X, args.Player.TempPoints[0].Y, args.Player.TempPoints[1].X, args.Player.TempPoints[1].Y);
                args.Player.SendInfoMessage(GetString("保存成功!"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(GetString($"/cb set <1/2> --选择复制的区域"));
            args.Player.SendInfoMessage("/cb save");
        }
    }


    private void CopyBuilding(int x1, int y1, int x2, int y2)
    {
        var Building = new List<Building>();
        Config.Instance.CentreX = (x2 - x1) / 2;
        Config.Instance.CountY = y2 - y1;
        for (var i = x1; i < x2; i++)
        {
            for (var j = y1; j < y2; j++)
            {
                var t = Main.tile[i, j];
                Building.Add(new Building()
                {
                    bTileHeader = t.bTileHeader,
                    bTileHeader2 = t.bTileHeader2,
                    bTileHeader3 = t.bTileHeader3,
                    frameX = t.frameX,
                    frameY = t.frameY,
                    liquid = t.liquid,
                    sTileHeader = t.sTileHeader,
                    type = t.type,
                    wall = t.wall
                });
            }
        }
        Map.SaveMap(Building);
    }

    public void SpawnBuilding()
    {
        Task.Run(() =>
        {
            var Building = Map.LoadMap();
            //出生点X
            var spwx = Main.spawnTileX;
            //出生点Y
            var spwy = Main.spawnTileY;
            //计算左X
            var x1 = spwx - Config.Instance.CentreX + Config.Instance.AdjustX;
            //计算左Y
            var y1 = spwy - Config.Instance.CountY + Config.Instance.AdjustY;
            //计算右x
            var x2 = Config.Instance.CentreX + spwx + Config.Instance.AdjustX;
            //计算右y
            var y2 = spwy + Config.Instance.AdjustY;
            var n = 0;
            for (var i = x1; i < x2; i++)
            {
                for (var j = y1; j < y2; j++)
                {
                    var t = Main.tile[i, j];
                    t.bTileHeader = Building[n].bTileHeader;
                    t.bTileHeader2 = Building[n].bTileHeader2;
                    t.bTileHeader3 = Building[n].bTileHeader3;
                    t.frameX = Building[n].frameX;
                    t.frameY = Building[n].frameY;
                    t.liquid = Building[n].liquid;
                    t.sTileHeader = Building[n].sTileHeader;
                    t.type = Building[n].type;
                    t.wall = Building[n].wall;
                    n++;
                    TSPlayer.All.SendTileSquareCentered(i, j);
                }
            }
        });
    }
}