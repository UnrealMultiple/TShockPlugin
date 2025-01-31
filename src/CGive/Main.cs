using Rests;
using TerrariaApi.Server;
using TShockAPI;

namespace CGive;

[ApiVersion(2, 1)]
public class Main : TerrariaPlugin
{
    public override string Author => "Leader";

    public override string Description => GetString("离线give");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 8);

    public Main(Terraria.Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("cgive.admin", this.cgive, "cgive"));
        ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInit);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        TShock.RestApi.Register("/getWarehouse", this.getWarehouse);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.cgive);
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnGameInit);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ((List<RestCommand>) typeof(Rest).GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(TShock.RestApi)!)
            .RemoveAll(x => x.Name == "/getWarehouse");
        }
        base.Dispose(disposing);
    }

    private object getWarehouse(RestRequestArgs args)
    {
        var text = args.Parameters["name"];
        var specifier = Commands.Specifier;
        if (TShock.UserAccounts.GetUserAccountsByName(text).Count == 0)
        {
            return new RestObject()
            {
                Error = GetString($"没有找到注册名为{text}的账户")
            };
        }
        var list = new List<Warehouse>();
        foreach (var item2 in CGive.GetCGive())
        {
            if (item2.who != text)
            {
                continue;
            }
            var array = item2.cmd.Split(' ');
            if (array.Length < 3 || (array[0].ToLower() != specifier + "give" && array[0].ToLower() != specifier + "g"))
            {
                continue;
            }
            var result = 0;
            if (array.Length >= 4 && !int.TryParse(array[3], out result))
            {
                continue;
            }
            var itemByIdOrName = TShock.Utils.GetItemByIdOrName(array[1]);
            if (itemByIdOrName.Count == 0)
            {
                continue;
            }
            var item = itemByIdOrName[0];
            if (array.Length == 3)
            {
                list.Add(new Warehouse(item.maxStack, item.netID));
                continue;
            }
            if (array.Length == 4)
            {
                list.Add(new Warehouse(result, item.netID));
                continue;
            }
            if (array.Length == 5)
            {
                var prefixByIdOrName = TShock.Utils.GetPrefixByIdOrName(array[4]);
                if (prefixByIdOrName.Count != 0)
                {
                    list.Add(new Warehouse(result, item.netID, prefixByIdOrName[0]));
                }
            }

        }
        return list.Count == 0
            ? new RestObject()
            {
                Error = GetString("该玩家没有仓库")
            }
            : new RestObject("200")
            {
                { "response", "获取成功" },
                { "data", list }
            };
    }

    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        foreach (var item in CGive.GetCGive())
        {
            if (item.who == "-1")
            {
                var given = new Given
                {
                    Name = TShock.Players[args.Who].Name,
                    id = item.id
                };
                if (!given.IsGiven())
                {
                    item.who = given.Name;
                    if (item.Execute())
                    {
                        given.Save();
                    }
                }
            }
            else if (item.Execute())
            {
                item.Del();
            }
        }
    }

    private void OnGameInit(EventArgs args)
    {
        Data.Init();
    }

    private void cgive(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("/cgive personal [命令] [被执行者]"));
            args.Player.SendInfoMessage(GetString("/cgive all [执行者] [命令]"));
            args.Player.SendInfoMessage(GetString("/cgive list,列出所有离线命令"));
            args.Player.SendInfoMessage(GetString("/cgive del [id],删除指定id的离线命令"));
            args.Player.SendInfoMessage(GetString("/cgive reset,重置所有数据"));
            return;
        }
        switch (args.Parameters[0])
        {
            case "reset":
                Data.Command("delete from CGive,Given");
                args.Player.SendSuccessMessage(GetString("成功删除所有数据"));
                break;
            case "del":
            {
                if (int.TryParse(args.Parameters[1], out var netID))
                {
                    var cGive2 = new CGive
                    {
                        id = netID
                    };
                    cGive2.Del();
                    args.Player.SendSuccessMessage(GetString("已执行删除"));
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("ID输入错误!"));
                }
                break;
            }
            case "list":
            {
                foreach (var item in CGive.GetCGive())
                {
                    var player = args.Player;
                    player.SendInfoMessage(GetString($"执行者:{item.Executer} 被执行者:{item.who} 命令:{item.cmd} id:{item.id}"));
                }
                break;
            }
            case "all":
            {
                var executer2 = args.Parameters[1];
                var cmd2 = args.Parameters[2];
                var who2 = "-1";
                var cGive3 = new CGive
                {
                    who = who2,
                    Executer = executer2,
                    cmd = cmd2
                };
                cGive3.Execute();
                break;
            }
            case "personal":
            {
                var executer = "Server";
                var who = args.Parameters[2];
                var cmd = args.Parameters[1];
                var cGive = new CGive
                {
                    Executer = executer,
                    who = who,
                    cmd = cmd
                };
                if (!cGive.Execute())
                {
                    args.Player.SendInfoMessage(GetString("命令已保存"));
                    cGive.Save();
                }
                else
                {
                    args.Player.SendInfoMessage(GetString("命令执行成功！"));
                }
                break;
            }
        }
    }
}