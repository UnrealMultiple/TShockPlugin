using LazyAPI;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static ServerTools.ModifyClientDetect;

namespace ServerTools;

[ApiVersion(2, 1)]
public partial class Plugin : LazyPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("服务器工具");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 2, 0, 0);

    public const string ReaderPath = "ReaderPlayers";

    public Plugin(Main game) : base(game)
    {
        if (!Directory.Exists(ReaderPath))
        {
            Directory.CreateDirectory(ReaderPath);
        }
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.PostInitialize);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.NetGetData.Register(this, this.GetData);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.NpcAIUpdate.Register(this, OnNPCUpdate);
        GetDataHandlers.NewProjectile.Register(this.NewProj);
        GetDataHandlers.ItemDrop.Register(this.OnItemDrop);
        GetDataHandlers.KillMe.Register(this.KillMe);
        GetDataHandlers.PlayerSpawn.Register(this.OnPlayerSpawn);
        GetDataHandlers.PlayerUpdate.Register(this.OnUpdate);
        HookManager.Add(typeof(TSRestPlayer).GetConstructor([typeof(string), typeof(TShockAPI.Group)])!, RestPlayerCtor);
        HookManager.Add(typeof(Commands).GetMethod("ViewAccountInfo", BindingFlags.NonPublic | BindingFlags.Static)!, ViewAccountInfo);
        OnTimer += this.OnUpdatePlayerOnline;
        On.OTAPI.Hooks.MessageBuffer.InvokeGetData += this.MessageBuffer_InvokeGetData;
        IL.Terraria.MessageBuffer.GetData += this.MessageBuffer_GetData;
        HandleCommandLine(Environment.GetCommandLineArgs());
    }

    private void MessageBuffer_GetData(ILContext il)
    {
        var cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld(typeof(MessageBuffer).GetField("readBuffer", BindingFlags.Instance | BindingFlags.Public)!),
            instr => instr.MatchLdarg(1),
            instr => instr.MatchLdelemU1(),
            instr => instr.MatchStloc(0)
        ))
        {
            cursor.Index += 5;
            cursor.EmitLdarg(0);
            cursor.EmitLdloc(0);
            cursor.EmitConvI4();
            cursor.EmitCall(typeof(HiddenChecker).GetMethod("CheckModify")!);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            HookManager.Clear();
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.PostInitialize);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.NetGetData.Deregister(this, this.GetData);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.NpcStrike.Deregister(this, OnStrike);
            ServerApi.Hooks.NpcAIUpdate.Deregister(this, OnNPCUpdate);
            GetDataHandlers.NewProjectile.UnRegister(this.NewProj);
            GetDataHandlers.ItemDrop.UnRegister(this.OnItemDrop);
            GetDataHandlers.KillMe.UnRegister(this.KillMe);
            GetDataHandlers.PlayerSpawn.UnRegister(this.OnPlayerSpawn);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnUpdate);
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.MessageBuffer_InvokeGetData;
            OnTimer -= this.OnUpdatePlayerOnline;
        }
        base.Dispose(disposing);
    }
}
