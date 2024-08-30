namespace PermaBuff;
internal class Playerbuffs
{
    public static readonly Dictionary<string, HashSet<int>> PlayerBuffs = new();

    public static void AddBuff(string Name, int buffid, bool insertDB = true)
    {
        if (!PlayerBuffs.TryGetValue(Name, out var buffs) || buffs == null)
        {
            PlayerBuffs[Name] = new HashSet<int>();
        }

        PlayerBuffs[Name].Add(buffid);
        if (insertDB)
        {
            DB.AddBuff(Name, buffid);
        }
    }

    public static void DelBuff(string Name, int buffid)
    {
        if (GetBuffs(Name).Contains(buffid))
        {
            PlayerBuffs[Name].Remove(buffid);
            DB.Delbuff(Name, buffid);
        }
    }

    public static HashSet<int> GetBuffs(string Name)
    {
        if (!PlayerBuffs.TryGetValue(Name, out var buffs) || buffs == null)
        {
            PlayerBuffs[Name] = new HashSet<int>();
        }

        return PlayerBuffs[Name];
    }

}