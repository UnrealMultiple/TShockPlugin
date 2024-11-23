using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger;

public class CNPC
{
    public NPC npc;

    private readonly int netID;

    private readonly int index;

    public float[] ai;

    public int state;

    public int LifeMax;

    public HashSet<string> AccOfObsidian;

    private bool _isActive;

    public bool isActive
    {
        get => this.npc != null && this.npc.netID == this.netID && this.npc.whoAmI == this.index && this.npc.active && this._isActive;
        set => this._isActive = value;
    }

    public CNPC()
    {
        this.npc = null!;
        this.netID = 0;
        this.index = 0;
        this.ai = new float[8];
        this.state = 0;
        this.LifeMax = 0;
        this.AccOfObsidian = new HashSet<string>();
        this.isActive = false;
    }

    public CNPC(NPC? npc)
    {
        if (npc == null)
        {
            this.npc = null!;
            this.netID = 0;
            this.index = 0;
            this.ai = new float[8];
            this.state = 0;
            this.LifeMax = 0;
            this.isActive = false;
        }
        else
        {
            this.npc = npc;
            this.netID = npc.netID;
            this.index = npc.whoAmI;
            this.ai = new float[8];
            this.state = 0;
            this.LifeMax = npc.life;
            this.isActive = npc.active;
        }
        this.AccOfObsidian = new HashSet<string>();
    }

    public CNPC(NPC? npc, float[] ai, int state)
    {
        if (npc == null)
        {
            this.npc = null!;
            this.netID = 0;
            this.index = 0;
            this.ai = new float[8];
            state = 0;
            this.LifeMax = 0;
            this.isActive = false;
        }
        else
        {
            this.npc = npc;
            this.netID = npc.netID;
            this.index = npc.whoAmI;
            this.ai = ai;
            this.state = state;
            this.LifeMax = npc.life;
            this.isActive = npc.active;
        }
        this.AccOfObsidian = new HashSet<string>();
    }

    public virtual void NPCAI()
    {
    }

    public virtual int SetState()
    {
        return 0;
    }

    public virtual void OnHurtPlayers(PlayerDamageEventArgs e)
    {
    }

    public virtual void OnKilled()
    {
        if (this.AccOfObsidian.Count == 0)
        {
            return;
        }
        try
        {
            if (this.npc.boss || this.npc.rarity > 1 || this.npc.lifeMax > 7000)
            {
                return;
            }
            foreach (var item in this.AccOfObsidian)
            {
                var players = TShock.Players;
                foreach (var val in players)
                {
                    if (val != null && val.Active && val.Name == item)
                    {
                        this.npc.NPCLoot_DropItems(val.TPlayer);
                    }
                }
            }
        }
        catch
        {
        }
    }

    public virtual void WhenHurtByPlayer(NpcStrikeEventArgs args)
    {
    }
}