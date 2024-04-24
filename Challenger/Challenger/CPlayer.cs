using TShockAPI;

namespace Challenger
{
    public class CPlayer
    {
        public TSPlayer? me;

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
            get
            {
                return me != null && me.Active && index == me.Index && index == me.TPlayer.whoAmI && me.TPlayer != null && me.TPlayer.active && _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public CPlayer(TSPlayer? me, bool tips)
        {
            if (me == null)
            {
                this.me = new TSPlayer(-1);
                index = 255;
                this.tips = true;
                isActive = false;
            }
            else
            {
                this.me = me;
                index = me.Index;
                this.tips = tips;
                isActive = true;
            }
        }
    }
}