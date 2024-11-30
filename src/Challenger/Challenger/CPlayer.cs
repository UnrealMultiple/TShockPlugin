using TShockAPI;

namespace Challenger;

public class CPlayer
{
    public TSPlayer me;

    public bool tips;

    public int index = 255;

    private bool _isActive;

    public int ExtraLife = 0;

    public int ExtraMana = 0;

    public bool FossilArmorEffectProj = false;

    public int FossilArmorEffectProjIndex = -1;

    public int CrimsonArmorEffectTimer = 0;

    public int ShadowArmorEffectTimer = 0;

    public int SpiderArmorEffectTimer = 0;

    public bool HallowedArmorState = true;

    public bool ChlorophyteArmorEffectLife = false;

    public bool TurtleArmorEffectLife = false;

    public bool TikiArmorEffectLife = false;

    public bool BeetleArmorEffectLife = false;

    public bool SpectreArmorEffectLife = false;

    public int SpectreArmorEffectProjIndex = -1;

    public bool SpectreArmorEffectMana = false;

    public int CthulhuShieldTime = 0;

    public bool isActive
    {
        get => this.me != null && this.me.Active && this.index == this.me.Index && this.index == this.me.TPlayer.whoAmI && this.me.TPlayer != null && this.me.TPlayer.active && this._isActive;
        set => this._isActive = value;
    }

    public CPlayer(TSPlayer? me, bool tips)
    {
        if (me == null)
        {
            this.me = new TSPlayer(-1);
            this.index = 255;
            this.tips = true;
            this.isActive = false;
        }
        else
        {
            this.me = me;
            this.index = me.Index;
            this.tips = tips;
            this.isActive = true;
        }
    }
}