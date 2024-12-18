using LazyAPI;
using Newtonsoft.Json;
using System.IO.Streams;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Chameleon;

[ApiVersion(2, 1)]
public class Chameleon : LazyPlugin
{
    public const string WaitPwd4Reg = "reg-pwd";

    public const ushort Size = 10;

    public static string[] PrepareList = new string[Size];

    public override string Name => "Chameleon";

    public override string Author => "mistzzt，肝帝熙恩修复1449";

    public override string Description => "账户系统交互替换方案";

    public override Version Version => new Version(1, 0, 5);


    public Chameleon(Main game) : base(game)
    {
        this.Order = 1;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, OnGetData, 9999);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInit, 9999);

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInit);

        }
        base.Dispose(disposing);
    }



    private static void OnPostInit(EventArgs args)
    {
        if (!string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword) || !string.IsNullOrEmpty(Netplay.ServerPassword))
        {
            TShock.Log.ConsoleError(GetString("[Chameleon] 在启用本插件的情况下，服务器密码功能将失效。"));
        }

        if (TShock.Config.Settings.DisableLoginBeforeJoin)
        {
            TShock.Log.ConsoleError(GetString("[Chameleon] 在启用本插件的情况下，入服前登录将被强制开启。"));
            TShock.Config.Settings.DisableLoginBeforeJoin = true;
        }

        if (!TShock.Config.Settings.RequireLogin && !TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            TShock.Log.ConsoleError(GetString("[Chameleon] 在启用本插件的情况下，注册登录将被强制开启。"));
            TShock.Config.Settings.RequireLogin = true;
        }
    }

    private static void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var type = args.MsgID;

        var player = TShock.Players[args.Msg.whoAmI];
        if(player == null)
        {
            return;
        }
        if (player.IsLoggedIn == true)
        {
            return;
        }

        if (player.RequiresPassword == true && type != PacketTypes.PasswordSend)
        {
            args.Handled = true;
            return;
        }

        if (type is PacketTypes.ContinueConnecting2 or PacketTypes.PasswordSend)
        {
            using (var data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1))
            {
                if (type == PacketTypes.ContinueConnecting2)
                {
                    args.Handled = HandleConnecting(player);
                }
                else if (type == PacketTypes.PasswordSend)
                {
                    args.Handled = HandlePassword(player, data.ReadString());
                }
            }
        }
    }

    private static bool HandleConnecting(TSPlayer player)
    {
        var account = TShock.UserAccounts.GetUserAccountByName(player.Name);
        player.DataWhenJoined = new PlayerData(player);
        player.DataWhenJoined.CopyCharacter(player);

        if (account != null)
        {
            // uuid自动登录 已注册part.2
            if (!TShock.Config.Settings.DisableUUIDLogin)
            {
                if (account.UUID == player.UUID)
                {
                    var knownIps = JsonConvert.DeserializeObject<List<string>>(account.KnownIps);
                    //player.SendInfoMessage(GetString($"knownIps：{knownIps},knownIps[^1]:{knownIps[^1]},player.IP：{player.IP}"));
                    if (knownIps != null && player.IP == knownIps[^1])
                    {
                        if (player.State == 1)
                        {
                            player.State = 2;
                        }

                        NetMessage.SendData((int) PacketTypes.WorldInfo, player.Index);

                        player.PlayerData = TShock.CharacterDB.GetPlayerData(player, account.ID);

                        var group = TShock.Groups.GetGroupByName(account.Group);

                        player.Group = group;
                        player.tempGroup = null;
                        player.Account = account;
                        player.IsLoggedIn = true;
                        player.IsDisabledForSSC = false;

                        if (Main.ServerSideCharacter)
                        {
                            if (player.HasPermission(Permissions.bypassssc))
                            {
                                player.PlayerData.CopyCharacter(player);
                                TShock.CharacterDB.InsertPlayerData(player);
                            }
                            player.PlayerData.RestoreCharacter(player);
                        }
                        player.LoginFailsBySsi = false;

                        if (player.HasPermission(Permissions.ignorestackhackdetection))
                        {
                            player.IsDisabledForStackDetection = false;
                        }

                        if (player.HasPermission(Permissions.usebanneditem))
                        {
                            player.IsDisabledForBannedWearable = false;
                        }

                        player.SendSuccessMessage(GetString($"已经验证{account.Name}登录完毕。"));
                        TShock.Log.ConsoleInfo(player.Name + GetString("成功验证登录。"));
                        PlayerHooks.OnPlayerPostLogin(player);
                        return true;
                    }
                    else
                    {
                        TShock.Log.ConsoleInfo(player.Name + GetString("IP地址不存在或与上次登录IP地址不符。"));
                    }
                }
                else
                {
                    TShock.Log.ConsoleInfo(player.Name + GetString("登录UUID与上次不同。"));
                }
            }
            else
            {
                    TSPlayer.Server.SendMessage(GetString("DisableUUIDLogin被设置为true，无法通过UUID自动登录。"),Microsoft.Xna.Framework.Color.Yellow);
            }

            // 使用密码登录 part.2
            player.RequiresPassword = true;
            NetMessage.SendData((int) PacketTypes.PasswordRequired, player.Index);
            return true;
        }

        if (Configuration.Instance.EnableForcedHint && !PrepareList.Contains(player.Name))
        {
            AddToList(player.Name);
            Kick(player, string.Join("\n", Configuration.Instance.Hints), Configuration.Instance.Greeting);
            return true;
        }

        // 未注册 part.1
        player.SetData(WaitPwd4Reg, true);
        NetMessage.SendData((int) PacketTypes.PasswordRequired, player.Index);
        return true;
    }

    private static bool HandlePassword(TSPlayer player, string password)
    {
        var isRegister = player.GetData<bool>(WaitPwd4Reg);

        if (!player.RequiresPassword && !isRegister)
        {
            return true;
        }

        if (!isRegister && PlayerHooks.OnPlayerPreLogin(player, player.Name, password))
        {
            return true;
        }

        var account = TShock.UserAccounts.GetUserAccountByName(player.Name);
        if (account != null)
        {
            if (account.VerifyPassword(password))
            {
                player.RequiresPassword = false;
                player.PlayerData = TShock.CharacterDB.GetPlayerData(player, account.ID);

                if (player.State == 1)
                {
                    player.State = 2;
                }

                NetMessage.SendData((int) PacketTypes.WorldInfo, player.Index);

                var group = TShock.Groups.GetGroupByName(account.Group);

                player.Group = group;
                player.tempGroup = null;
                player.Account = account;
                player.IsLoggedIn = true;
                player.IsDisabledForSSC = false;

                if (Main.ServerSideCharacter)
                {
                    if (player.HasPermission(Permissions.bypassssc))
                    {
                        player.PlayerData.CopyCharacter(player);
                        TShock.CharacterDB.InsertPlayerData(player);
                    }
                    player.PlayerData.RestoreCharacter(player);
                }
                player.LoginFailsBySsi = false;

                if (player.HasPermission(Permissions.ignorestackhackdetection))
                {
                    player.IsDisabledForStackDetection = false;
                }

                if (player.HasPermission(Permissions.usebanneditem))
                {
                    player.IsDisabledForBannedWearable = false;
                }

                player.SendSuccessMessage(GetString($"已经验证{account.Name}登录完毕。"));
                TShock.Log.ConsoleInfo(player.Name + GetString("成功验证登录。"));
                TShock.UserAccounts.SetUserAccountUUID(account, player.UUID);
                PlayerHooks.OnPlayerPostLogin(player);
                return true;
            }
            Kick(player, Configuration.Instance.VerificationFailedMessage, GetString("验证失败"));
            return true;
        }
        if (player.Name != TSServerPlayer.AccountName)
        {
            account = new UserAccount
            {
                Name = player.Name,
                Group = TShock.Config.Settings.DefaultRegistrationGroupName,
                UUID = player.UUID
            };
            try
            {
                account.CreateBCryptHash(password);
            }
            catch (ArgumentOutOfRangeException)
            {
                Kick(player, GetString("密码位数不能少于") + TShock.Config.Settings.MinimumPasswordLength + GetString("个字符。"), GetString("注册失败"));
                return true;
            }
            player.SendSuccessMessage(GetString("账户{0}注册成功。"), account.Name);
            player.SendSuccessMessage(GetString("你的密码是{0}"), password);
            TShock.UserAccounts.AddUserAccount(account);
            TShock.Log.ConsoleInfo(GetString("玩家{0}注册了新账户：{1}"), player.Name, account.Name);

            player.RequiresPassword = false;
            player.SetData(WaitPwd4Reg, false);
            player.PlayerData = TShock.CharacterDB.GetPlayerData(player, account.ID);

            if (player.State == 1)
            {
                player.State = 2;
            }

            NetMessage.SendData((int) PacketTypes.WorldInfo, player.Index);

            var group = TShock.Groups.GetGroupByName(account.Group);

            player.Group = group;
            player.tempGroup = null;
            player.Account = account;
            player.IsLoggedIn = true;
            player.IsDisabledForSSC = false;

            if (Main.ServerSideCharacter)
            {
                if (player.HasPermission(Permissions.bypassssc))
                {
                    player.PlayerData.CopyCharacter(player);
                    TShock.CharacterDB.InsertPlayerData(player);
                }
                player.PlayerData.RestoreCharacter(player);
            }
            player.LoginFailsBySsi = false;

            if (player.HasPermission(Permissions.ignorestackhackdetection))
            {
                player.IsDisabledForStackDetection = false;
            }

            if (player.HasPermission(Permissions.usebanneditem))
            {
                player.IsDisabledForBannedWearable = false;
            }

            player.SendSuccessMessage(GetString($"已经验证{account.Name}登录完毕。"));
            TShock.Log.ConsoleInfo(player.Name + GetString("成功验证登录。"));
            TShock.UserAccounts.SetUserAccountUUID(account, player.UUID);
            PlayerHooks.OnPlayerPostLogin(player);
            return true;
        }

        // 系统预留账户名
        Kick(player, GetString("该用户名已被占用。"), GetString("请更换人物名"));
        return true;
    }

    private static void AddToList(string playerName)
    {
        var index = 0;
        while (index < PrepareList.Length && !string.IsNullOrEmpty(PrepareList[index]))
        {
            index++;
        }

        PrepareList[index % PrepareList.Length] = playerName;
    }

    public static void Kick(TSPlayer player, string msg, string custom)
    {
        if (!player.ConnectionAlive)
        {
            return;
        }

        player.SilentKickInProgress = true;
        player.Disconnect($"{custom}：{msg}");
        TShock.Log.ConsoleInfo(GetString($"向{player.Name}发送通知完毕."));
    }
}