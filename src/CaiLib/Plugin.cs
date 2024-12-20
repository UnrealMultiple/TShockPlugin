using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace CaiLib;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "Cai";

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(2024, 12, 18, 3);

    public Plugin(Main game) : base(game)
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            var assemblyData = new byte[stream.Length];
            stream.Read(assemblyData, 0, assemblyData.Length);
            return Assembly.Load(assemblyData);
        }
        return null;
    }

    public override void Initialize()
    {

    }
}