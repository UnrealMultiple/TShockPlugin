using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent;

[assembly: InternalsVisibleTo("AdditionalPylons.SmokeTests")]

namespace AdditionalPylons;

/// <summary>
/// FreePylons-style condition hooks. Only these two predicates are bypassed,
/// and only while the config flag is on and Main.netMode is server.
/// </summary>
internal sealed class PylonConditionHooks : IDisposable
{
    private Hook? npcHook;
    private Hook? biomeHook;
    private bool disposed;

    private delegate bool NpcPredicate(TeleportPylonsSystem self, TeleportPylonInfo info, int count);
    private delegate bool NpcDetour(NpcPredicate original, TeleportPylonsSystem self, TeleportPylonInfo info, int count);
    private delegate bool BiomePredicate(TeleportPylonsSystem self, TeleportPylonInfo info, Player player);
    private delegate bool BiomeDetour(BiomePredicate original, TeleportPylonsSystem self, TeleportPylonInfo info, Player player);

    public bool IsInstalled => npcHook is not null && biomeHook is not null;

    public void Install()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        if (this.IsInstalled) return;

        var npcMethod = RequirePredicate("DoesPylonHaveEnoughNPCsAroundIt", typeof(TeleportPylonInfo), typeof(int));
        var biomeMethod = RequirePredicate("DoesPylonAcceptTeleportation", typeof(TeleportPylonInfo), typeof(Player));
        try
        {
            this.npcHook = new Hook(npcMethod, (NpcDetour)IgnoreNpcRequirement);
            this.biomeHook = new Hook(biomeMethod, (BiomeDetour)IgnoreBiomeRequirement);
        }
        catch
        {
            this.RemoveHooks();
            throw;
        }
    }

    private static MethodInfo RequirePredicate(string name, params Type[] parameters)
    {
        var type = typeof(TeleportPylonsSystem);
        var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null, types: parameters, modifiers: null);
        if (method is null || method.ReturnType != typeof(bool) || method.ContainsGenericParameters)
            throw new NotSupportedException(
                $"AdditionalPylons: unsupported OTAPI signature {type.FullName}.{name}; loaded {type.Assembly.FullName}. Expected TShock 6.1.0 dependencies.");
        return method;
    }

    private static bool IsKnownPylon(TeleportPylonInfo info) =>
        info.TypeOfPylon >= 0 && info.TypeOfPylon < TeleportPylonType.Count;

    private static bool IgnoreNpcRequirement(NpcPredicate original, TeleportPylonsSystem self,
        TeleportPylonInfo info, int count) =>
        Configuration.Instance.NoTownEnvironment && Main.netMode == 2 && IsKnownPylon(info) || original(self, info, count);

    private static bool IgnoreBiomeRequirement(BiomePredicate original, TeleportPylonsSystem self,
        TeleportPylonInfo info, Player player) =>
        Configuration.Instance.NoTownEnvironment && Main.netMode == 2 && IsKnownPylon(info) || original(self, info, player);

    private void RemoveHooks()
    {
        try { this.biomeHook?.Dispose(); }
        finally
        {
            this.biomeHook = null;
            try { this.npcHook?.Dispose(); }
            finally { this.npcHook = null; }
        }
    }

    public void Dispose()
    {
        if (this.disposed) return;
        this.disposed = true;
        this.RemoveHooks();
    }
}
