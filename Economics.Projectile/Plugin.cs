using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Economics.Projectile;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Projectile.json");

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        GetDataHandlers.NewProjectile.Register(OnProj);
        ServerApi.Hooks.NetGetData.Register(this, GetData);
        
    }

    private void GetData(GetDataEventArgs args)
    {
        if(args.MsgID == PacketTypes.MinionAttackTargetUpdate)
        {
            var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
            var who = reader.ReadByte();
            var npcid = reader.ReadInt16();
            Console.WriteLine(npcid);
            Console.WriteLine(Main.npc[npcid].FullName);
        }
    }

    private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        throw new NotImplementedException();
    }
}
