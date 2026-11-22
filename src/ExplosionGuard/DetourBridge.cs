using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;

namespace ExplosionGuard
{
    /// <summary>原版 Projectile.ExplodeTiles 的签名（所有爆炸毁图的必经之路）。</summary>
    public delegate void ExplodeTilesOrig(Projectile self, Vector2 compareSpot,
        int radius, int minI, int maxI, int minJ, int maxJ, bool wallSplode, bool explodeHardmodeOres);

    /// <summary>detour 回调签名：首参为原版调用委托（MonoMod 约定）。</summary>
    internal delegate void ExplodeTilesDetourDelegate(ExplodeTilesOrig orig, Projectile self, Vector2 compareSpot,
        int radius, int minI, int maxI, int minJ, int maxJ, bool wallSplode, bool explodeHardmodeOres);

    /// <summary>
    /// Projectile.ExplodeTiles 的 MonoMod 桥接：本文件是整个插件唯一触碰 MonoMod 类型的文件。
    /// 绑定失败时 TryCreate 抛异常，由 Initialize 捕获并降级为无操作。
    /// </summary>
    internal static class DetourBridge
    {
        public static IDisposable? TryCreate(Delegate detour)
        {
            var mi = typeof(Projectile).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == nameof(Projectile.ExplodeTiles)
                    && m.GetParameters().Length == 8
                    && m.GetParameters()[0].ParameterType == typeof(Vector2))
                ?? throw new MissingMemberException(typeof(Projectile).FullName, "ExplodeTiles(...)");

            return new Hook(mi, detour);
        }
    }
}
