using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.Globalization;
using Microsoft.Xna.Framework;
using LazyAPI.Extensions;

namespace TrCDK;

[ApiVersion(2, 1)]
public class TrCDK : TerrariaPlugin
{
    public override string Author => "Jonesn";
    public override string Description => GetString("CDK系统");
    public override string Name => "TrCDK";
    public override Version Version => new Version(1, 0, 0, 0);
    public TrCDK(Main game) : base(game) { }

    public override void Initialize()
    {
       
    }

    protected override void Dispose(bool Disposing)
    {
        
    }
}