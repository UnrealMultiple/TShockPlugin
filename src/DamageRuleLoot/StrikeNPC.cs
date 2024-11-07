namespace DamageRuleLoot;

public class StrikeNPC
{
    public int npcIndex;

    public int npcID;

    public string npcName = string.Empty;

    public static List<StrikeNPC> strikeNPC = new List<StrikeNPC>();

    public Dictionary<string, double> PlayerOrDamage = new Dictionary<string, double>();

    public double AllDamage = 0;

    public float value = 0f;

    public StrikeNPC() { }

    public StrikeNPC(int index, int id, string name, Dictionary<string, double> PlayerOrDamage, double allDamage, float value)
    {
        this.npcIndex = index;
        this.npcID = id;
        this.npcName = name;
        this.PlayerOrDamage = PlayerOrDamage;
        this.AllDamage = allDamage;
        this.value = value;
    }
}
