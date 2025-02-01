using System.IO.Streams;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

//using PlaceholderAPI;
using static SwitchCommands.PluginCommands;

namespace SwitchCommands;

[ApiVersion(2, 1)]
public class SwitchCommands : TerrariaPlugin
{

    public static Database database = null!;
    public static SwitchPos switchPos = null!;

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "Johuan Cjx适配 羽学，肝帝熙恩优化";
    public override string Description => GetString("触发开关可以执行指令");
    public override Version Version => new Version(1, 2, 6);

    public SwitchCommands(Main game) : base(game) { }

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        GetDataHandlers.TileEdit.Register(OnEdit);
        PluginCommands.RegisterCommands();
        ServerApi.Hooks.NetGetData.Register(this, this.GetData);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= LoadConfig;
            GetDataHandlers.TileEdit.UnRegister(OnEdit);
            PluginCommands.UnregisterCommands();
            ServerApi.Hooks.NetGetData.Deregister(this, this.GetData);

        }
        base.Dispose(disposing);
    }

    #region 配置文件创建与重读加载方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        database = Database.Read(Database.databasePath);
        database.Write(Database.databasePath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage(GetString("[开关指令插件]重新加载配置完毕。"));
        }
    }
    #endregion

    #region 阻止没有权限的人破坏开关
    private static void OnEdit(object? sender, GetDataHandlers.TileEditEventArgs e)
    {
        var pos = new SwitchPos(e.X, e.Y);
        if (IsProtectedSwitch(pos))
        {
            if (e == null || !database.SwitchEnable || e.Player.HasPermission("switch.admin")) { return; }

            if (Main.tile[e.X, e.Y].type == 136 && e.Player.Active)
            {
                e.Player.SendMessage($"{database.SwitchText}", color: Microsoft.Xna.Framework.Color.Yellow);
                e.Player.SendTileSquareCentered(e.X, e.Y, 1);
                e.Handled = true;
            }
        }
    }

    private static bool IsProtectedSwitch(SwitchPos pos)
    {
        return database.switchCommandList.ContainsKey(pos.ToString());
    }
    #endregion

    private void GetData(GetDataEventArgs args)
    {
        using (var data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
        {
            var player = TShock.Players[args.Msg.whoAmI];
            switch (args.MsgID)
            {
                case PacketTypes.HitSwitch:
                    var pos = new SwitchPos(data.ReadInt16(), data.ReadInt16());
                    var tile = Main.tile[pos.X, pos.Y];

                    if (tile.type == TileID.Lever)
                    {
                        if (tile.frameX % 36 == 0)
                        {
                            pos.X++;
                        }

                        if (tile.frameY == 0)
                        {
                            pos.Y++;
                        }
                    }

                    if (database.switchCommandList.ContainsKey(pos.ToString()) && !string.IsNullOrEmpty(database.switchCommandList[pos.ToString()].show))
                    {
                        player.SendMessage(GetString($"开关说明：{database.switchCommandList[pos.ToString()].show}"), color: Microsoft.Xna.Framework.Color.Yellow);
                    }

                    var playerState = player.GetData<PlayerState>("PlayerState");

                    if (playerState == PlayerState.SelectingSwitch)
                    {
                        player.SetData("SwitchPos", pos);
                        player.SendSuccessMessage(GetString("成功绑定位于X：{0}、Y：{1} 的开关").SFormat(pos.X, pos.Y));
                        player.SendSuccessMessage(GetString("输入/开关 ，可查看子命令列表").SFormat(pos.X, pos.Y));
                        player.SetData("PlayerState", PlayerState.AddingCommands);

                        if (database.switchCommandList.ContainsKey(pos.ToString()))
                        {
                            player.SetData("CommandInfo", database.switchCommandList[pos.ToString()]);
                        }

                        return;
                    }

                    if (playerState == PlayerState.None)
                    {
                        if (database.switchCommandList.ContainsKey(pos.ToString()))
                        {
                            double seconds = 999999;

                            var cooldown = player.GetData<Dictionary<string, DateTime>>("冷却");

                            if (cooldown != null && cooldown.ContainsKey(pos.ToString()))
                            {
                                seconds = (DateTime.Now - player.GetData<Dictionary<string, DateTime>>("冷却")[pos.ToString()]).TotalMilliseconds / 1000;
                            }

                            if (seconds < database.switchCommandList[pos.ToString()].cooldown)
                            {
                                // player.SendErrorMessage("You must wait {0} more seconds before using this switch.".SFormat(database.switchCommandList[pos.ToString()].cooldown - seconds));
                                //冷却提示有点刷屏
                                return;
                            }

                            Group? currGroup = null;

                            var ignorePerms = database.switchCommandList[pos.ToString()].ignorePerms;

                            foreach (var cmd in database.switchCommandList[pos.ToString()].commandList)
                            {
                                if (ignorePerms)
                                {
                                    currGroup = player.Group;
                                    player.Group = new SuperAdminGroup();
                                }

                                var Place = cmd.ReplaceTags(player);//PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(cmd.ReplaceTags(player), player);
                                Commands.HandleCommand(player, Place);
                                if (ignorePerms)
                                {
                                    player.Group = currGroup;
                                }
                            }

                            if (cooldown == null)
                            {
                                cooldown = new Dictionary<string, DateTime>() { { pos.ToString(), DateTime.Now } };
                            }
                            else
                            {
                                cooldown[pos.ToString()] = DateTime.Now;
                            }

                            player.SetData("冷却", cooldown);
                        }
                    }

                    break;
            }
        }
    }
}

public static class StringManipulator
{

    public static string ReplaceTags(this string s, TSPlayer player)
    {
        var response = s.Split(' ').ToList();

        for (var x = response.Count - 1; x >= 0; x--)
        {
            if (response[x] == "$name")
            {
                response[x] = "\"" + player.Name + "\"";
            }
        }

        return string.Join(" ", response);

    }

}