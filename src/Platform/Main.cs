using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Platform;

[ApiVersion(2, 1)]
public class Platform : TerrariaPlugin
{

    public override string Author => "Cai";

    public override string Description => GetString("判断玩家设备");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 0, 5);

    public Platform(Main game)
    : base(game)
    {
        base.Order = int.MinValue;
    }
    public static PlatformType[] Platforms { get; set; } = new PlatformType[256];
    public static string GetPlatform(TSPlayer plr)
    {
        return Platforms[plr.Index].ToString();
    }
    public override void Initialize()
    {
        On.OTAPI.Hooks.MessageBuffer.InvokeGetData += this.OnGetData;
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        Commands.ChatCommands.Add(new Command("Platform", this.PlatformCommand, "Platform", "判断玩家设备"));
    }

    private void PlatformCommand(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            var plys = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (plys.Count > 0)
            {
                var platform = PlatformTool.GetPlatform(plys[0]);
                args.Player.SendInfoMessage(GetString($"玩家 `{plys[0].Name}` 的设备是: {platform}"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("目标玩家不在线!"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("语法错误"));
            args.Player.SendInfoMessage(GetString("/platform <玩家名字/序号id>"));
        }
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }
        TShock.Log.ConsoleInfo(GetString($"[Platform]玩家{TShock.Players[args.Who].Name}游玩平台:{Platforms[args.Who]}"));

    }


    private bool OnGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {
        try
        {
            if (messageType == 1)
            {
                Platforms[instance.whoAmI] = PlatformType.PC;
            }

            if (messageType == 150)
            {
                instance.ResetReader();
                instance.reader.BaseStream.Position = start + 1;
                var PlayerSlot = instance.reader.ReadByte();
                var Platform = instance.reader.ReadByte();
                Platforms[instance.whoAmI] = (PlatformType) Platform;
                //Console.WriteLine($"[PE]PlayerSlot={PlayerSlot},Plat={Platform}");
            }
        }
        catch { }

        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }
    public enum PlatformType : byte // TypeDefIndex: 5205
    {
        PE = 0,
        Stadia = 1,
        XBOX = 2,
        PSN = 3,
        Editor = 4,
        Switch = 5,
        PC = 233
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PlatformCommand);
        }
        base.Dispose(disposing);
    }


}
public static class PlatformTool
{
    public static string GetPlatform(this TSPlayer plr)
    {
        return Platform.Platforms[plr.Index].ToString();
    }

}