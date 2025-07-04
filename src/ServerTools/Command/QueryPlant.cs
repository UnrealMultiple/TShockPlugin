using LazyAPI.Attributes;
using Terraria;
using TShockAPI;

namespace ServerTools.Command;


[Command("scp", "查花苞")]
[Permissions("servertool.query.wall")]
public class QuerPlant
{
    private static DateTime LastCommandUseTime = DateTime.Now;

    [Main]
    public static void Query(TSPlayer ply)
    {
        var Now = DateTime.Now;
        var cd = (Now - LastCommandUseTime).TotalSeconds;
        var count = 0;
        if (cd > 5)
        {
            var n = 1;
            int x = 0, y = 0;
            for (var i = 0; i < Main.tile.Width; i++)
            {
                for (var j = 0; j < Main.tile.Height; j++)
                {
                    var tile = Main.tile[i, j];
                    var sync = Math.Abs(i - x) + Math.Abs(j - y) > 2;
                    if (tile != null && tile.type == 238 && sync)
                    {
                        x = i;
                        y = j;
                        count++;
                        if (!TShock.Warps.Warps.Any(s => s.Position.X == i && s.Position.Y == j))
                        {
                            while (TShock.Warps.Warps.Any(x => x.Name == GetString("花苞") + n))
                            {
                                n++;
                            }
                            TShock.Warps.Add(i, j, GetString("花苞") + n);
                        }
                    }
                }
            }
            ply.SendInfoMessage(GetString("已为你搜索到{0}个花苞，输入/warp list可以查看结果"), count);
            LastCommandUseTime = Now;
        }
        else
        {
            ply.SendErrorMessage(GetString("你不能过于频繁的使用此指令!"));
        }
    }
}


[Command("rcp", "移除花苞")]
[Permissions("servertool.query.wall")]
public class RemovePlant
{
    [Main]
    public static void RWall(CommandArgs args)
    {
        TShock.Warps.Warps.FindAll(x => x.Name.StartsWith(GetString("花苞"))).ForEach(n => TShock.Warps.Remove(n.Name));
        args.Player.SendSuccessMessage(GetString("已移除所有花苞传送点!"));
    }
}
