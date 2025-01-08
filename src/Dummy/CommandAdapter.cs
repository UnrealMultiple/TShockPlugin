using TShockAPI;

namespace Dummy;
internal class CommandAdapter
{
    private static readonly Dictionary<string, CommandDelegate> _actions = new()
    {
        { "remove", RemoveDummy },
        { "list", DummyList },
        { "reconnect", ReConnect }
    };

    public static void Adapter(CommandArgs args)
    {
        if (args.Parameters.Count >= 1)
        {
            var subcmd = args.Parameters[0].ToLower();
            if (_actions.TryGetValue(subcmd, out var action))
            {
                action(args);
                return;
            }
        }
        args.Player.SendInfoMessage(GetString("dummy remove [index] 移除目标假人"));
        args.Player.SendInfoMessage(GetString("dummy list 假人列表"));
        args.Player.SendInfoMessage(GetString("dummy reconnect [index] 重新连接"));
    }

    private static void ReConnect(CommandArgs args)
    {
        if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out var index))
        {
            var ply = Plugin._players[index];
            if (ply != null && !ply.Active)
            {
                ply.GameLoop("127.0.0.1", Plugin.Port, TShock.Config.Settings.ServerPassword);
            }
            else
            {
                args.Player.SendErrorMessage(GetString("目标假人不存在!"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("请输入正确的序号!"));
        }
    }

    private static void DummyList(CommandArgs args)
    {
        for (var i = 0; i < Plugin._players.Length; i++)
        {
            var player = Plugin._players[i];
            if (player != null && player.TSPlayer.Active)
            {
                args.Player.SendInfoMessage(GetString($"[{i}].{player.TSPlayer.Name} 连接状态: {player.Active}"));
            }
        }
    }

    private static void RemoveDummy(CommandArgs args)
    {
        if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out var index))
        {
            var ply = Plugin._players[index];
            if (ply != null && ply.TSPlayer.Active)
            {
                ply.Close();
                args.Player.SendSuccessMessage(GetString("假人移除成功!"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("目标假人不存在!"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("请输入正确的序号!"));
        }
    }
}
