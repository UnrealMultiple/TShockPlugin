using Terraria;
using Terraria.ID;
using TShockAPI;

namespace GhostView.Utils;

public static class BuffUtils
{
    private static readonly int[] DebuffIds =
    [
        BuffID.Poisoned,
        BuffID.PotionSickness,
        BuffID.Darkness,
        BuffID.Cursed,
        BuffID.OnFire,
        BuffID.Bleeding,
        BuffID.Confused,
        BuffID.Slow,
        BuffID.Weak,
        BuffID.Silenced,
        BuffID.BrokenArmor,
        BuffID.Horrified,
        BuffID.TheTongue,
        BuffID.CursedInferno,
        BuffID.Frostburn,
        BuffID.Chilled,
        BuffID.Frozen,
        BuffID.Burning,
        BuffID.Suffocation,
        BuffID.Ichor,
        BuffID.Venom,
        BuffID.Midas,
        BuffID.Blackout,
        BuffID.ChaosState,
        BuffID.ManaSickness,
        BuffID.Wet,
        BuffID.Slimed,
        BuffID.Electrified,
        BuffID.MoonLeech,
        BuffID.Rabies,
        BuffID.Webbed,
        BuffID.ShadowFlame,
        BuffID.Stoned,
        BuffID.Obstructed,
        BuffID.VortexDebuff,
        BuffID.BoneJavelin,
        BuffID.StardustMinionBleed,
        BuffID.DryadsWardDebuff,
        BuffID.Daybreak,
        BuffID.WindPushed,
        BuffID.WitheredArmor,
        BuffID.WitheredWeapon,
        BuffID.OgreSpit,
        BuffID.NoBuilding,
        BuffID.BetsysCurse,
        BuffID.Oiled,
        BuffID.Lovestruck,
        BuffID.Stinky,
        BuffID.Hunger,
        BuffID.Starving,
        BuffID.Dazed
    ];


    public static void ClearDebuffs(TSPlayer? player)
    {
        if (player?.TPlayer is null || !player.Active)
        {
            return;
        }

        var tPlayer = player.TPlayer;

        foreach (var debuff in DebuffIds)
        {
            for (var i = 0; i < Player.maxBuffs; i++)
            {
                if (tPlayer.buffType[i] != debuff)
                {
                    continue;
                }

                tPlayer.ClearBuff(debuff);
                break;
            }
        }
    }
}