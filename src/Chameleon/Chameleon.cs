using LazyAPI;
using Newtonsoft.Json;
using System.IO.Streams;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace Chameleon;

[ApiVersion(2, 1)]
public class Chameleon : LazyPlugin
{
    private const string WaitPwd4Reg = "reg-pwd";

    public const ushort Size = 10;

    public static readonly List<string> PrepareList = new(); //已经发送过公告的玩家，等待注册

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "mistzzt,肝帝熙恩";

    public override string Description => GetString("账户系统交互替换方案");

    public override Version Version => new Version(1, 1, 1);


    public Chameleon(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, OnGetData, int.MaxValue);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInit, int.MaxValue);

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

        if (player == null)
        {
            return;
        }

        if (player.IsLoggedIn)
        {
            return;
        }

        if (player.RequiresPassword && type != PacketTypes.PasswordSend)
        {
            args.Handled = true;
            return;
        }

        if (type is PacketTypes.ContinueConnecting2 or PacketTypes.PasswordSend)
        {
            using var data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
            args.Handled = type == PacketTypes.ContinueConnecting2 ? HandleConnecting(player) : HandlePassword(player, data.ReadString());
        }
    }

    private static bool HandleConnecting(TSPlayer player)
    {
        var account = TShock.UserAccounts.GetUserAccountByName(player.Name);
        if (account is null)
        {
            // 只有未注册的玩家才会看到强制提示
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

        player.DataWhenJoined = new PlayerData(true);
        player.DataWhenJoined.CopyCharacter(player);

        do
        {
            // uuid自动登录 已注册part.2
            if (TShock.Config.Settings.DisableUUIDLogin)
            {
                TShock.Log.ConsoleInfo(GetString($"[Chameleon] `DisableUUIDLogin`被设置为true，不允许玩家`{player.Name}`使用UUID登录,"), Microsoft.Xna.Framework.Color.Yellow);
                break;
            }
            if (account.UUID != player.UUID)
            {
                TShock.Log.ConsoleInfo(GetString($"[Chameleon] 玩家`{player.Name}`UUID登录失败，需要验证密码。"));
                break;
            }
            if (Configuration.Instance.VerifyloginIP &&
                JsonConvert.DeserializeObject<string[]>(account.KnownIps) is var knownIps &&
                (knownIps is null || player.IP != knownIps.Last()))
            {
                TShock.Log.ConsoleInfo(GetString($"[Chameleon] 玩家`{player.Name}`的IP与上次登录IP地址不符，需要验证密码。"));
                break;
            }

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

            player.SendSuccessMessage(GetString($"[Chameleon] 已经验证`{account.Name}`登录完毕。"));
            TShock.Log.ConsoleInfo(GetString($"[Chameleon] `{player.Name}`成功验证登录。"));
            PlayerHooks.OnPlayerPostLogin(player);
            return true;
#pragma warning disable CS0162
        } while (false);
#pragma warning disable CS0162
        // 使用密码登录 part.2
        player.RequiresPassword = true;
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

                player.SendSuccessMessage(GetString($"[Chameleon] 已经验证`{account.Name}`登录完毕。"));
                TShock.Log.ConsoleInfo(GetString($"[Chameleon] `{player.Name}`成功验证登录。"));
                TShock.UserAccounts.SetUserAccountUUID(account, player.UUID);
                PlayerHooks.OnPlayerPostLogin(player);
                return true;
            }
            Kick(player, Configuration.Instance.VerificationFailedMessage, GetString("密码错误..."));
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
                Kick(player, GetString($"密码位数不能少于{TShock.Config.Settings.MinimumPasswordLength}个字符。"), GetString("注册失败"));
                return true;
            }
            player.SendSuccessMessage(GetString($"[Chameleon] 账户`{account.Name}`注册成功。"));
            player.SendSuccessMessage(GetString($"[Chameleon] 你的密码是: [c/FF0000:{password}]"));
            TShock.UserAccounts.AddUserAccount(account);
            TShock.Log.ConsoleInfo(GetString($"[Chameleon] 玩家{player.Name}注册了新账户：{account.Name}"));

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

            player.SendSuccessMessage(GetString($"[Chameleon] 已经验证`{account.Name}`登录完毕。"));
            TShock.Log.ConsoleInfo(GetString($"[Chameleon] `{player.Name}`成功验证登录。"));
            TShock.UserAccounts.SetUserAccountUUID(account, player.UUID);
            PlayerHooks.OnPlayerPostLogin(player);
            return true;
        }

        // 系统预留账户名
        Kick(player, GetString("此用户名被禁用。"), GetString("请更换人物名"));
        return true;
    }

    private static void AddToList(string playerName)
    {

        PrepareList.Add(playerName);

        if (PrepareList.Count > 10)
        {
            PrepareList.RemoveAt(0);
        }

    }

    private static void Kick(TSPlayer player, string msg, string custom)
    {
        if (!player.ConnectionAlive)
        {
            return;
        }

        player.SilentKickInProgress = true;
        player.Disconnect($"{custom}：{msg}");
        TShock.Log.ConsoleInfo(GetString($"[Chameleon] 向`{player.Name}`发送通知完毕."));
    }
}