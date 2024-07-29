using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CreateSpawn;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => "出生建筑";

    public override string Name => "CreateSpawn";

    public override Version Version => new(1, 0, 0, 1);


    private readonly string SavePath = Path.Combine(TShock.SavePath, "Create.json");

    public Config Config = new();

    private bool create = false;
    public Plugin(Main game) : base(game)
    {
    }

    private GeneralHooks.ReloadEventD _reloadHandler;//这个大概还是有问题
    public override void Initialize()
    {
        LoadConfig();
        _reloadHandler = (_) => LoadConfig();
        On.Terraria.WorldBuilding.GenerationProgress.End += (_, _) => create = true;
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += _reloadHandler;
        Commands.ChatCommands.Add(new Command("create.copy", copy, "cb"));
        Commands.ChatCommands.Add(new Command("create.copy", CreateBuilding, "create"));
        ServerApi.Hooks.GamePostInitialize.Register(this, (_) =>
        {
            if (create)
                SpawnBuilding();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            On.Terraria.WorldBuilding.GenerationProgress.End -= (_, _) => create = true;
            TShockAPI.Hooks.GeneralHooks.ReloadEvent -= _reloadHandler;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == copy || x.CommandDelegate == CreateBuilding);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, (_) =>
            {
                if (create)
                    SpawnBuilding();
            });
        }

        base.Dispose(disposing);
    }

    private void CreateBuilding(CommandArgs args)
    {
        SpawnBuilding();
        args.Player.SendInfoMessage("建筑已创建!");
    }

    private void copy(CommandArgs args)
    {
        if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "set")
        {
            switch (args.Parameters[1])
            {
                case "1":
                    args.Player.AwaitingTempPoint = 1;
                    args.Player.SendInfoMessage("请选择复制区域的左上角");
                    break;
                case "2":
                    args.Player.AwaitingTempPoint = 2;
                    args.Player.SendInfoMessage("请选择复制区域的右下角");
                    break;
                default:
                    args.Player.SendInfoMessage($"正确指令：/cb set <1/2> --选择复制的区域");
                    args.Player.SendInfoMessage("/cb save");
                    break;
            }
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "save")
        {
            if (args.Player.TempPoints[0].X == 0 || args.Player.TempPoints[1].X == 0)
            {
                args.Player.SendInfoMessage("您还没有选择复制的区域！");
            }
            else
            {
                CopyBuilding(args.Player.TempPoints[0].X, args.Player.TempPoints[0].Y, args.Player.TempPoints[1].X, args.Player.TempPoints[1].Y);
                args.Player.SendInfoMessage("保存成功!");
            }
        }
        else
        {
            args.Player.SendInfoMessage($"/cb set <1/2> --选择复制的区域");
            args.Player.SendInfoMessage("/cb save");
        }
    }


    private void CopyBuilding(int x1, int y1, int x2, int y2)
    {
        var Building = new List<Building>();
        Config.centreX = (x2 - x1) / 2;
        Config.CountY = y2 - y1;
        for (int i = x1; i < x2; i++)
        {
            for (int j = y1; j < y2; j++)
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
        Config.Write(SavePath);
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
            var x1 = spwx - Config.centreX + Config.AdjustX;
            //计算左Y
            var y1 = spwy - Config.CountY + Config.AdjustY;
            //计算右x
            var x2 = Config.centreX + spwx + Config.AdjustX;
            //计算右y
            var y2 = spwy + Config.AdjustY;
            var n = 0;
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
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

    public void LoadConfig()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                Config = Config.Read(SavePath);
            }
            catch (Exception e)
            {
                TShock.Log.ConsoleError("配置文件读取错误:{0}", e.ToString());
            }
        }
        Config.Write(SavePath);
    }
}
