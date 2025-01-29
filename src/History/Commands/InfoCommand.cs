using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace History.Commands;

public class InfoCommand : HCommand
{
    private readonly string account;
    private readonly int radius;
    private readonly int time;

    public InfoCommand(string account, int time, int radius, TSPlayer sender)
        : base(sender)
    {
        this.account = account;
        this.time = time;
        this.radius = radius;
    }

    public override void Execute()
    {
        var actions = new List<Action>();
        var lookupTime = (int) (DateTime.UtcNow - History.Date).TotalSeconds - this.time;

        var plrX = this.sender.TileX;
        var plrY = this.sender.TileY;
        var lowX = plrX - this.radius;
        var highX = plrX + this.radius;
        var lowY = plrY - this.radius;
        var highY = plrY + this.radius;
        var XYReq = string.Format("XY / 65536 BETWEEN {0} AND {1} AND XY & 65535 BETWEEN {2} AND {3}", lowX, highX, lowY, highY);

        using (var reader =
            History.Database.QueryReader("SELECT Action, XY FROM History WHERE Account = @0 AND Time >= @1 AND " + XYReq + " AND WorldID = @2",
            this.account, lookupTime, Main.worldID))
        {
            while (reader.Read())
            {
                actions.Add(new Action
                {
                    action = (byte) reader.Get<int>("Action"),
                    x = reader.Get<int>("XY") >> 16,
                    y = reader.Get<int>("XY") & 0xffff,
                });
            }
        }

        for (var i = 0; i >= 0 && i < History.Actions.Count; i++)
        {
            var action = History.Actions[i];
            if (action.account == this.account && action.time >= lookupTime &&
                lowX <= action.x && lowY <= action.y && action.x <= highX && action.y <= highY)
            {
                actions.Add(action);
            }
        }
        // 0 actions escape
        if (actions.Count == 0)
        {
            this.sender.SendInfoMessage(GetString("{0}在指定区域中未执行任何图格操作."), this.account);
            return;
        }

        // Done
        List<Action> TilePlaced = new List<Action>(), TileDestroyed = new List<Action>(), TileModified = new List<Action>(),
                    WallPlaced = new List<Action>(), WallDestroyed = new List<Action>(), WallModified = new List<Action>(),
                    WirePlaced = new List<Action>(), WireDestroyed = new List<Action>(), WireModified = new List<Action>(),
                    ActuatorPlaced = new List<Action>(), ActuatorDestroyed = new List<Action>(), ActuatorModified = new List<Action>(),
                    Painted = new List<Action>(), SignModified = new List<Action>(), Other = new List<Action>();
        foreach (var action in actions)
        {
            switch (action.action)
            {
                case 0:
                case 4:
                    if (TileDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (TilePlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        TileModified.Add(action);
                        TilePlaced.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    TileDestroyed.Add(action);
                    break;
                case 1:
                    if (TilePlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (TileDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        TileModified.Add(action);
                        TileDestroyed.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    TilePlaced.Add(action);
                    break;
                case 2:
                    if (WallDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (WallPlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        WallModified.Add(action);
                        WallPlaced.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    WallDestroyed.Add(action);
                    break;
                case 3:
                    if (WallPlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (WallDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        WallModified.Add(action);
                        WallDestroyed.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    WallPlaced.Add(action);
                    break;
                case 5:
                case 10:
                case 12:
                case 16:
                    WirePlaced.Add(action);
                    break;
                case 6:
                case 11:
                case 13:
                case 17:
                    WireDestroyed.Add(action);
                    break;
                case 7:
                case 14:
                    TileModified.Add(action); //slope/pound
                    break;
                case 8:
                    if (ActuatorPlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (ActuatorDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        ActuatorModified.Add(action);
                        ActuatorDestroyed.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    ActuatorPlaced.Add(action);
                    break;
                case 9:
                    if (ActuatorDestroyed.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    if (ActuatorPlaced.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        ActuatorModified.Add(action);
                        ActuatorPlaced.RemoveAll(act => act.x.Equals(action.x) && act.y.Equals(action.y));
                        break;
                    }
                    ActuatorDestroyed.Add(action);
                    break;
                case 25:
                case 26:
                    if (Painted.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    Painted.Add(action);
                    break;
                case 27:
                    if (SignModified.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    SignModified.Add(action);
                    break;
                default:
                    if (Other.Exists(act => act.x.Equals(action.x) && act.y.Equals(action.y)))
                    {
                        break;
                    }
                    Other.Add(action);
                    break;
            }
        }
        actions.Clear();
        var InfoPrep = new StringBuilder();
        InfoPrep.Append(this.account);

        if (TileModified.Count > 0)
        {
            InfoPrep.Append(GetString($" 修改 {TileModified.Count} 物块,"));
        }

        if (TileDestroyed.Count > 0)
        {
            InfoPrep.Append(GetString($" 破环 {TileDestroyed.Count} 物块,"));
        }

        if (TilePlaced.Count > 0)
        {
            InfoPrep.Append(GetString($" 放置 {TilePlaced.Count} 物块,"));
        }

        if (WallModified.Count > 0)
        {
            InfoPrep.Append(GetString($" 修改 {WallModified.Count} 墙,"));
        }

        if (WallDestroyed.Count > 0)
        {
            InfoPrep.Append(GetString($" 破坏 {WallDestroyed.Count} 墙,"));
        }

        if (WallPlaced.Count > 0)
        {
            InfoPrep.Append(GetString($" 放置 {WallPlaced.Count} 墙,"));
        }

        if (WireModified.Count > 0)
        {
            InfoPrep.Append(GetString($" 修改 {WireModified.Count} 电线,"));
        }

        if (WireDestroyed.Count > 0)
        {
            InfoPrep.Append(GetString($" 破坏 {WireDestroyed.Count} 电线,"));
        }

        if (WirePlaced.Count > 0)
        {
            InfoPrep.Append(GetString($" 放置 {WirePlaced.Count} 电线,"));
        }

        if (ActuatorModified.Count > 0)
        {
            InfoPrep.Append(GetString($" 修改 {ActuatorModified.Count} 致动器,"));
        }

        if (ActuatorDestroyed.Count > 0)
        {
            InfoPrep.Append(GetString($" 破坏 {ActuatorDestroyed.Count} 致动器,"));
        }

        if (ActuatorPlaced.Count > 0)
        {
            InfoPrep.Append(GetString($" 放置 {ActuatorPlaced.Count} 致动器,"));
        }

        if (Painted.Count > 0)
        {
            InfoPrep.Append(GetString($" 涂漆 {Painted.Count} 物块/墙,"));
        }

        if (SignModified.Count > 0)
        {
            InfoPrep.Append(GetString($" 修改 {SignModified.Count} 告示牌,"));
        }

        if (Other.Count > 0)
        {
            InfoPrep.Append(GetString($" {Other.Count} 其他修改,"));
        }

        InfoPrep.Length--;
        InfoPrep.Append(GetString("."));
        this.sender.SendInfoMessage(InfoPrep.ToString());
    }
}