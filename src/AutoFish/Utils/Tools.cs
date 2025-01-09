using Terraria;

namespace AutoFish.Utils;

public class Tools
{
    #region 判断浮漂跳动
    public static bool BobbersActive(int whoAmI)
    {
        return Main.projectile.Any((p) => p.active && p.owner == whoAmI && p.bobber);
    }
    #endregion

    #region 根据玩家数量动态调整限制上限 5人100次,10人50次,20人25次,20人以上10次
    public static int GetLimit(int plrs)
    {
        return plrs <= 5 ? 100 : plrs <= 10 ? 50 : plrs <= 20 ? 25 : 10;
    }
    #endregion
}
