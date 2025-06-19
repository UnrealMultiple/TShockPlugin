using LazyAPI.Attributes;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace ServerTools.Command;

[Command("readplayer")]
[Permissions("servertool.readplayer.use")]
public class ReaderPlayer
{
    [Main]
    public static void Read(CommandArgs args, string file, string playerName)
    {
        if (!File.Exists(file))
        {
            args.Player.SendErrorMessage(GetString($"角色文件{Path.GetFileName(file)}不存在无法读取!"));
            return;
        }
        try
        {
            ReadPlayerCopyCharacter(file, playerName);
            args.Player.SendSuccessMessage(GetString($"{Path.GetFileName(file)} 读取成功，写入角色{playerName}!"));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"读取过程错误:{ex}"));
        }
    }

    [Main]
    public static void Read(CommandArgs args, string file)
    {
        if (!File.Exists(file))
        {
            args.Player.SendErrorMessage(GetString($"角色文件{Path.GetFileName(file)}不存在无法读取!"));
            return;
        }
        try
        {
            ReadPlayerCopyCharacter(file);
            args.Player.SendSuccessMessage(GetString($"{Path.GetFileName(file)} 读取成功，写入数据库!"));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"读取过程错误:{ex}"));
        }
    }

    [Main]
    public static void Read(CommandArgs args)
    {
        var files = Directory.GetFiles(Plugin.ReaderPath);
        foreach (var file in files)
        {
            try
            {
                ReadPlayerCopyCharacter(file);
                args.Player.SendSuccessMessage(GetString($"{Path.GetFileName(file)} 读取成功，写入数据库!"));
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"读取过程错误:{ex}"));
            }
        }
    }

    public static void Help(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString($"使用方法:"));
        args.Player.SendInfoMessage(GetString("/readplayer"));
        args.Player.SendInfoMessage(GetString("/readplayer [文件名]"));
        args.Player.SendInfoMessage(GetString("/readplayer [文件名] [目标角色]"));
    }

    private static void ReadPlayerCopyCharacter(string path, string? name = null)
    {
        var data = Player.LoadPlayer(path, false);
        if (!string.IsNullOrEmpty(name))
        {
            data.Player.name = name;
        }
        var tsPlayer = new TSPlayer(byte.MaxValue - 1);
        //下面的的参数是必要设置的
        typeof(TSPlayer).GetField("FakePlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.Instance)?.SetValue(tsPlayer, data.Player);
        tsPlayer.PlayerData = new();
        tsPlayer.IsLoggedIn = true;
        tsPlayer.State = 10;
        tsPlayer.Account = GetOrGenerateAccount(data.Player);
        //保存数据
        tsPlayer.PlayerData.CopyCharacter(tsPlayer);
        TShock.CharacterDB.InsertPlayerData(tsPlayer);
    }

    private static UserAccount GetOrGenerateAccount(Player player)
    {
        var ac = TShock.UserAccounts.GetUserAccountByName(player.name);
        if (ac != null)
        {
            return ac;
        }
        TShock.UserAccounts.AddUserAccount(new UserAccount()
        {
            Name = player.name,
            Group = TShock.Config.Settings.DefaultGuestGroupName
        });
        var tempAccount = TShock.UserAccounts.GetUserAccountByName(player.name);
        return tempAccount;
    }
}
