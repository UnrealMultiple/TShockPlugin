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

    public static void ClearAll()
    {
        var BuffList = new List<string>(PlayerBuffs.Keys);
        foreach (var Name in BuffList)
        {
            var buffs = PlayerBuffs[Name];
            foreach (var buffId in buffs.ToList())
            {
                buffs.Remove(buffId);
                DB.Delbuff(Name, buffId);
            }

            if (buffs.Count == 0)
            {
                PlayerBuffs.Remove(Name);
            }
        }

        DB.ClearTable();
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