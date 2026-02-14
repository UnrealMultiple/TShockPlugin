using System.Diagnostics.CodeAnalysis;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Platform;

[ApiVersion(2, 1)]
// ReSharper disable once ClassNeverInstantiated.Global
public class Platform(Main game) : TerrariaPlugin(game)
{

    public override string Author => "Cai";

    public override string Description => GetString("判断玩家设备");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (2026, 02, 15, 0);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum PlatformType : byte
    {
        PE = 0,
        Stadia = 1,
        XBO = 2,
        PSN = 3,
        Editor = 4,
        Nintendo = 5,
        Steam = 6,
        GameCenter = 7,
        PC = 10
    }
    public static readonly PlatformType[] Platforms  = new PlatformType[Main.maxPlayers];
    public override void Initialize()
    {
        On.OTAPI.Hooks.MessageBuffer.InvokeGetData += this.OnGetData;
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        Commands.ChatCommands.Add(new Command("platform.use", PlatformCommand, "platform", "device"));
        PlayerHooks.PlayerChat += PlayerHooksOnPlayerChat;
    }

    private static void PlayerHooksOnPlayerChat(PlayerChatEventArgs e)
    {
        e.TShockFormattedText = e.TShockFormattedText.Replace("%platform%", e.Player.GetPlatform())
            .Replace("%device%", e.Player.GetPlatform());
    }

    private static void PlatformCommand(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (players.Count > 0)
            {
                var platform = players[0].GetPlatform();
                args.Player.SendInfoMessage(GetString($"玩家 `{players[0].Name}` 的设备是: {platform}"));
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

    private static void OnGreet(GreetPlayerEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }
        TShock.Log.ConsoleInfo(GetString($"[Platform]玩家{TShock.Players[args.Who].Name}游玩平台:{Platforms[args.Who]}"));

    }


    private bool OnGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {
        switch (messageType)
        {
            case (int)PacketTypes.ConnectRequest:
                Platforms[instance.whoAmI] = PlatformType.PC;
                break;
            case (int)PacketTypes.PlayerPlatformInfo:
            {
                instance.ResetReader();
                instance.reader.BaseStream.Position = start + 1;
                _ = instance.reader.ReadByte();
                var platformId = instance.reader.ReadByte();
                Platforms[instance.whoAmI] = (PlatformType) platformId;
                break;
            }
        }

        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == PlatformCommand);
        }
        base.Dispose(disposing);
    }


}