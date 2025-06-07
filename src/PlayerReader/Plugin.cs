using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace PlayerReader;

[ApiVersion(2, 1)]
public class Plugin(Main game) : TerrariaPlugin(game)
{
    public override string Author => "少司命";

    public override string Description => GetString("通过存档读取玩家数据库!");

    public override string Name => "ReaderPlayer";

    public override Version Version => new(1, 0, 0, 0);

    private const string ReaderPath = "ReaderPlayers";

    public override void Initialize()
    {
        if (!Directory.Exists(ReaderPath))
        {
            Directory.CreateDirectory(ReaderPath);
        }

        Commands.ChatCommands.Add(new Command("tshock.readplayer.use", this.ReaderCmd, "readplayer"));
    }

    private void ReaderCmd(CommandArgs args)
    {
        if (args.Parameters.Count >= 1)
        {
            var path = Path.Combine(ReaderPath, args.Parameters[0]);
            if (!File.Exists(path))
            {
                args.Player.SendErrorMessage(GetString($"不存在的文件:{args.Parameters[0]}"));
                return;
            }
            try
            {
                ReadPlayerCopyCharacter(path);
                args.Player.SendSuccessMessage(GetString($"{path} 读取成功，写入数据库!"));
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"读取过程错误:{ex}"));
            }
        }
        else
        { 
            var files = Directory.GetFiles(ReaderPath);
            if (files.Length == 0)
            {
                args.Player.SendErrorMessage(GetString("没有文件可以被读取!"));
                return;
            }
            try
            {
                foreach (var file in files)
                {
                    ReadPlayerCopyCharacter(file);
                    args.Player.SendSuccessMessage(GetString($"{Path.GetFileName(file)} 读取成功，写入数据库!"));
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"读取过程错误:{ex}"));
            }
        }
        
    }

    private static void ReadPlayerCopyCharacter(string path)
    {
        var data = Player.LoadPlayer(path, false);
        var tsPlayer = new TSPlayer(byte.MaxValue - 1);
        //下面的的参数是必要设置的
        typeof(TSPlayer).GetField("FakePlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.Instance)?.SetValue(tsPlayer, data.Player);
        tsPlayer.PlayerData = new();
        tsPlayer.IsLoggedIn = true;
        tsPlayer.State = 10;
        tsPlayer.Account = GenerateAccount(data.Player);
        //保存数据
        tsPlayer.PlayerData.CopyCharacter(tsPlayer);
        TShock.CharacterDB.InsertPlayerData(tsPlayer);
    }

    private static UserAccount GenerateAccount(Player player)
    {
        var ac = TShock.UserAccounts.GetUserAccountByName(player.name);
        if (ac != null)
        {
            return ac;
        }
        var accounts = TShock.UserAccounts.GetUserAccounts().OrderByDescending(a => a.ID);
        var tempac =  new UserAccount()
        {
            ID = accounts.First().ID + 1,
            Name = player.name,
            Group = TShock.Config.Settings.DefaultGuestGroupName
        };
        TShock.UserAccounts.AddUserAccount(tempac);
        return tempac;
    }
}
