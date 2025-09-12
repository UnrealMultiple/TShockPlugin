using CaiBotLite.Moulds;
using CaiBotLite.Services;
using Newtonsoft.Json;
using On.OTAPI;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Program = Terraria.Program;

namespace CaiBotLite;

[ApiVersion(2, 1)]
// ReSharper disable once ClassNeverInstantiated.Global
public class CaiBotLite(Main game) : TerrariaPlugin(game)
{
    public static readonly Version VersionNum = new (2025, 08, 5, 0); //日期+版本号(0,1,2...)
    internal static int InitCode = -1;
    internal static bool DebugMode = Program.LaunchParameters.ContainsKey("-caidebug");
    private const string CharacterInfoKey = "CaiBotLite.CharacterInfo";
    public override string Author => "Cai,羽学,西江";
    public override string Description => "CaiBot官方机器人的适配插件";
    public override string Name => "CaiBotLitePlugin";
    
    public override Version Version => VersionNum;


    public override void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        Config.Settings.Read();
        Config.Settings.Write();
        Database.Init();
        ServerApi.Hooks.NetGetData.Register(this, Login.OnGetData, int.MaxValue);
        ServerApi.Hooks.GamePostInitialize.Register(this, GenBindCode);
        ServerApi.Hooks.NpcKilled.Register(this, OnNpcKilled);
        ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
        ServerApi.Hooks.GamePostUpdate.Register(this, OnGameUpdate);
        Hooks.MessageBuffer.InvokeGetData += Login.MessageBuffer_InvokeGetData; 
        GeneralHooks.ReloadEvent += GeneralHooksOnReloadEvent;
        PlayerHooks.PlayerPostLogin += PlayerHooksOnPlayerPostLogin;
        GetDataHandlers.KillMe.Register(KillMe, HandlerPriority.Highest);
        MapGenerator.Init();
        EconomicSupport.Init();
        BossLockSupport.Init();
        ProgressControlSupport.Init();
        WebsocketManager.Init();
        Commands.ChatCommands.Add(new Command("caibotlite.admin", CaiBotCommand, "caibotlite", "cbl"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method.DeclaringType?.Assembly == asm);
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
            ServerApi.Hooks.NetGetData.Deregister(this, Login.OnGetData);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, GenBindCode);
            ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKilled);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
            ServerApi.Hooks.GamePostUpdate.Deregister(this, OnGameUpdate);
            Hooks.MessageBuffer.InvokeGetData -= Login.MessageBuffer_InvokeGetData;
            GeneralHooks.ReloadEvent -= GeneralHooksOnReloadEvent;
            PlayerHooks.PlayerPostLogin -= PlayerHooksOnPlayerPostLogin;
            GetDataHandlers.KillMe.UnRegister(KillMe);
            MapGenerator.Dispose();
            WebsocketManager.StopWebsocket();
        }

        base.Dispose(disposing);
    }

    private static int _timer;

    private static void OnGameUpdate(EventArgs args)
    {
        if (_timer >= 60 * 60 * 5)
        {
            foreach (var player in TShock.Players.Where(x => x is { Active: true }))
            {
                var characterInfo = player.GetData<CaiCharacterInfo>(CharacterInfoKey);
                characterInfo?.CreatOrUpdate();
            }

            _timer = 0;
        }

        _timer++;
    }

    private static void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        var characterInfo = e.Player.GetData<CaiCharacterInfo>(CharacterInfoKey);
        if (characterInfo == null)
        {
            return;
        }

        characterInfo.Death++;
    }

    private static void OnServerLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        var characterInfo = player?.GetData<CaiCharacterInfo>(CharacterInfoKey);
        characterInfo?.CreatOrUpdate();
    }

    private static void PlayerHooksOnPlayerPostLogin(PlayerPostLoginEventArgs e)
    {
        var characterInfo = CaiCharacterInfo.GetByName(e.Player.Account.Name)
                            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident
                            ?? new () { AccountName = e.Player.Account.Name };
        e.Player.SetData(CharacterInfoKey, characterInfo);
    }

    private static void OnNpcKilled(NpcKilledEventArgs args)
    {
        if (!args.npc.boss)
        {
            return;
        }

        for (var i = 0; i < byte.MaxValue; i++)
        {
            if (!args.npc.playerInteraction[i])
            {
                continue;
            }

            var player = TShock.Players[i];
            // ReSharper disable once UseNullPropagation
            if (player == null)
            {
                return;
            }

            var characterInfo = player.GetData<CaiCharacterInfo>(CharacterInfoKey);
            if (characterInfo == null)
            {
                return;
            }

            var bossInfo = characterInfo.BossKills.FirstOrDefault(x => x.BossId == args.npc.type);
            if (bossInfo == null)
            {
                characterInfo.BossKills.Add(new BossKillInfo { AccountName = player.Account.Name, BossId = args.npc.type, KillCounts = 1 });
            }
            else
            {
                bossInfo.KillCounts++;
            }
        }
    }
    
    private static void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        Config.Settings.Read();
        e.Player.SendSuccessMessage("[CaiBotLite]配置文件已重载 :)");
    }
    
    private static void CaiBotCommand(CommandArgs args)
    {
        var plr = args.Player;

        if (args.Parameters.Count == 0)
        {
            ShowHelpText();
            return;
        }


        switch (args.Parameters[0].ToLowerInvariant())
        {
            case "test":
                _timer = 60 * 60 * 5;
                Console.WriteLine("你怎么知道Cai喜欢留一个测试命令?");
                break;
            case "reset":
                CaiCharacterInfo.CleanAll();
                Mail.CleanAll();
                plr.SendInfoMessage($"[CaiBotLite]已重置统计数据!");
                break;
            case "help":
                ShowHelpText();
                return;

            default:
                ShowHelpText();
                break;

            case "信息":
            case "info":
                plr.SendInfoMessage($"[CaiBot信息]\n" +
                                    $"插件版本: v{VersionNum}\n" +
                                    $"WebSocket状态: {WebsocketManager.WebSocket?.State}\n" +
                                    $"设置QQ群: {(Config.Settings.GroupNumber == 0L ? "未设置" : Config.Settings.GroupNumber)}\n" +
                                    $"绑定状态: {Config.Settings.Token != ""}\n" +
                                    $"Debug模式: {DebugMode}\n" +
                                    $"Economic API支持: {EconomicSupport.GetCoinsSupport}\n" +
                                    $"Economic RPG支持: {EconomicSupport.GetLevelNameSupport}\n" +
                                    $"Economic Skill支持: {EconomicSupport.GetSkillSupport}\n"
                );
                break;
            case "调试":
            case "debug":
                DebugMode = !DebugMode;
                plr.SendInfoMessage($"[CaiBotLite]调试模式已{(DebugMode ? "开启" : "关闭")}!");
                break;
            case "验证码":
            case "code":
                if (!string.IsNullOrEmpty(Config.Settings.Token))
                {
                    plr.SendInfoMessage("[CaiBotLite]服务器已绑定无法生成验证码!");
                    return;
                }

                GenBindCode(EventArgs.Empty);
                plr.SendInfoMessage("[CaiBotLite]验证码已生成,请在后台查看喵~");
                break;
            
            case "解绑":
            case "unbind":
                if (string.IsNullOrEmpty(Config.Settings.Token))
                {
                    plr.SendInfoMessage("[CaiBotLite]服务器没有绑定任何群哦!");
                    return;
                }
                Config.Settings.Token = string.Empty;
                Config.Settings.Write();
                WebsocketManager.WebSocket?.Dispose();
                GenBindCode(EventArgs.Empty);
                plr.SendInfoMessage("[CaiBotLite]验证码已生成,请在后台查看喵~");
                break;
            case "白名单":
            case "whitelist":
                Config.Settings.WhiteList = !Config.Settings.WhiteList;
                Config.Settings.Write();
                WebsocketManager.WebSocket?.Dispose();
                plr.SendInfoMessage($"[CaiBotLite]白名单已{(Config.Settings.WhiteList?"开启":"关闭")}!");
                break;
            case "群号":
            case "group":
                if (args.Parameters.Count < 2)
                {
                    plr.SendErrorMessage($"格式错误!" +
                                         $"正确格式: /caibotlite group <群号>");
                    return;
                }

                if (!long.TryParse(args.Parameters[1], out Config.Settings.GroupNumber))
                {
                    plr.SendErrorMessage($"无效参数,群号必须是长整数!");
                    return;
                }
                
                Config.Settings.Write();
                plr.SendInfoMessage($"[CaiBotLite]白名单提示群号已改为{Config.Settings.GroupNumber}");
                break;
                
                
        }

        return;

        void ShowHelpText()
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, plr, out var pageNumber))
            {
                return;
            }

            List<string> lines =
            [
                "/cbl debug CaiBot调试开关",
                "/cbl reset 重置数据统计",
                "/cbl code 生成并且展示验证码",
                "/cbl info 显示CaiBot的一些信息",
                "/cbl unbind 主动解除绑定",
                "/cbl whitelist 开关白名单",
                "/cbl group <群号> 设置踢出显示的群号",
                "/cbl test Cai保留用于测试的命令, 乱用可能会爆掉"
            ];

            PaginationTools.SendPage(
                plr, pageNumber, lines,
                new PaginationTools.Settings { HeaderFormat = GetString("帮助 ({0}/{1})："), FooterFormat = GetString("输入 {0}caibotlite help {{0}} 查看更多").SFormat(Commands.Specifier) }
            );
        }
    }


    public static void GenBindCode(EventArgs? args)
    {
        if (!string.IsNullOrEmpty(Config.Settings.Token))
        {
            return;
        }

        InitCode = new Random().Next(10000000, 99999999);
        TShock.Log.ConsoleError($"[CaiBotLite]您的服务器绑定码为: {InitCode}");
    }


    #region 加载前置

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName =
            $"embedded.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        var assemblyData = new byte[stream.Length];
        _ = stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
    }

    #endregion
}
