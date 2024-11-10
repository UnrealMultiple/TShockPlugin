using Terraria;
using TShockAPI;

namespace AutoFish.Utils;

public class Tools
{
    #region 判断浮漂跳动
    public static bool BobbersActive(int whoAmI)
    {
        using (var enumerator = Main.projectile.Where((p) => p.active && p.owner == whoAmI && p.bobber).GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                _ = enumerator.Current;
                return true;
            }
        }
        return false;
    }
    #endregion
}
