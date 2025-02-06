using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace VeinMiner;

[ApiVersion(2, 1)]
public partial class VeinMiner : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; 
    public override Version Version => new (1, 6, 1, 0);
    public override string Author => "Megghy, YSpoof, Maxthegreat99, 肝帝熙恩, Cai";
    public override string Description => GetString("连锁挖矿插件！");

    internal static Config Config = new();

    public VeinMiner(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config.Load();
        Commands.ChatCommands.Add(new Command("veinminer", VMCmd, "veinminer", "vm"));
        GetDataHandlers.TileEdit += OnTileEdit;
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += Config.Load;
        ServerApi.Hooks.ServerJoin.Register(this, OnPlayerJoin);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.TileEdit -= OnTileEdit;
            TShockAPI.Hooks.GeneralHooks.ReloadEvent -= Config.Load;
            ServerApi.Hooks.ServerJoin.Deregister(this, OnPlayerJoin);
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method?.DeclaringType?.Assembly == asm);
        }

        base.Dispose(disposing);
    }
    
    private void VMCmd(CommandArgs args)
    {
        var tsp = args.Player;
        var result = tsp.GetData<VMStatus>("VeinMiner");
        if (args.Parameters.Count >= 1)
        {
            result.EnableBroadcast = !result.EnableBroadcast;
            tsp.SendMessage(GetString($"[c/95CFA6:<VeinMiner> 挖矿消息已{(result.EnableBroadcast ? GetString("开启") : GetString("关闭"))}]."), Color.White);
        }
        else
        {
            result.Enable = !result.Enable;
            tsp.SendMessage(GetString($"[c/95CFA6:<VeinMiner> 已{(result.Enable ? GetString("开启") : GetString("关闭! | 要仅关闭挖矿消息提示请输入：/vm {任意参数}"))}.]"), Color.White);
        }
    }



    static void OnPlayerJoin(JoinEventArgs args)
    {
        if (TShock.Players[args.Who] is { } plr)
        {
            plr.SetData("VeinMiner", new VMStatus());
        }
    }

    static void OnTileEdit(object? sender, GetDataHandlers.TileEditEventArgs args)
    {
        if (Main.tile[args.X, args.Y] is not { } tile || !args.Player.HasPermission("veinminer") || !Config.Enable || !args.Player.GetData<VMStatus>("VeinMiner").Enable || !Config.TargetTile.Contains(tile.type) || args.Action != GetDataHandlers.EditAction.KillTile || args.EditData != 0)
        {
            return;
        }

        args.Handled = true;
        Mine(args.Player, args.X, args.Y, tile.type);
    }

    static void Mine(TSPlayer plr, int x, int y, int type)
    {
        var list = GetVein(new(), x, y, type);
        var count = list.Count;
        var item = Utils.GetItemFromTile(x, y);
        var mineableList = list.Where(point => point.Y <= 0 || !Config.IgnoreAboveTile.Contains(Main.tile[point.X, point.Y - 1].type)).ToList();

        var mineCount = mineableList.Count;
        if (mineCount > 0)
        {
            list = mineableList;

            if (Config.Exchange.Where(e => e.Type == type && mineCount >= e.MinSize).ToList() is { Count: > 0 } exchangeList)
            {
                exchangeList.ForEach(e =>
                {
                    if (e.Item.Count <= plr.GetBlankSlot())
                    {
                        if (plr.IsSpaceEnough(item.netID, mineCount))
                        {
                            e.Item.ForEach(ex => plr.GiveItem(ex.Key, ex.Value));
                            if (e.OnlyGiveItem)
                            {
                                mineCount = KillTileAndSend(list, true);
                            }
                            else
                            {
                                GiveItem();
                            }

                            plr.SendInfoMessage(GetString($"[c/95CFA6:<VeinMiner>] 已给予奖励物品"));
                        }
                        else
                        {
                            GiveItem();
                        }
                        return;
                    }

                    plr.SendInfoMessage(GetString($"[c/95CFA6:<VeinMiner>] 背包已满, 还需空位：[c/95CFA6:{e.Item.Count}]."));
                    plr.SendTileSquareCentered(x, y, 1);
                });
            }
            else
            {
                GiveItem();
            }
            void GiveItem()
            {
                if (plr.GetData<VMStatus>("VeinMiner").EnableBroadcast && mineCount > 1)
                {
                    plr.SendMessage(GetString($"[c/95CFA6:<VeinMiner>] 正在挖掘[c/95CFA6:{mineCount}块{(item.type == 0 ? GetString("未知"):item.Name)}]."), Color.White);
                }
                if (Config.PutIntoInventory)
                {
                    if (plr.IsSpaceEnough(item.netID, mineCount))
                    {
                        mineCount = KillTileAndSend(list, true);
                        plr.GiveItem(item.netID, mineCount);
                        plr.SendMessage(GetString($"[c/95CFA6:<VeinMiner>] 挖掘了[c/95CFA6:{mineCount}块{(item.type == 0 ? GetString("未知") : item.Name)}]."), Color.White);

                    }
                    else
                    {
                        WorldGen.KillTile(x, y);
                        plr.SendInfoMessage(GetString($"[c/95CFA6:<VeinMiner>] 背包已满, 需要[c/95CFA6:{mineCount}]个空位以放入[c/95CFA6:{item.Name}]."));
                    }
                }
                else
                {
                    mineCount = KillTileAndSend(list, false);
                }
            }
        }
        else if (count > 0)
        {
            plr.SendMessage(GetString($"[c/95CFA6:<VeinMiner>] 无法挖取矿石, 可能是因为矿石上方有不可破坏的物体."), Color.White);
        }
    }

    private static int KillTileAndSend(List<Point> list, bool noItem)
    {
        if (!list.Any())
        {
            return 0;
        }

        var killCount = 0;
        list.ForEach(p =>
        {
            WorldGen.KillTile(p.X, p.Y, false, false, noItem);
            NetMessage.SendData((int)PacketTypes.Tile, -1, -1, null, 4, p.X, p.Y, false.GetHashCode());
        });
        list.ForEach(p =>
        {

            var tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.active())
            {
                killCount++;
            }

        });
        return killCount;
    }

    private static List<Point> GetVein(List<Point> list, int x, int y, int type)
    {

        if (!list.Any(p => p.Equals(new Point(x, y))) && Main.tile[x, y] is { } tile && tile.active() && tile.type == type)
        {
            if (list.Count > 5000) //最大连锁块数
            {
                return list;
            }

            list.Add(new(x, y));
            list = GetVein(list, x + 1, y, type);
            list = GetVein(list, x - 1, y, type);
            list = GetVein(list, x, y + 1, type);
            list = GetVein(list, x, y - 1, type);
            list = GetVein(list, x + 1, y + 1, type);
            list = GetVein(list, x + 1, y - 1, type);
            list = GetVein(list, x - 1, y + 1, type);
            list = GetVein(list, x - 1, y - 1, type);
        }

        return list;
    }
}