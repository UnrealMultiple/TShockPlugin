namespace DataSync;

[AttributeUsage(AttributeTargets.Field)]
internal class AliasAttribute : Attribute
{
    public string[] Alias { get; }

    public AliasAttribute(params string[] alias)
    {
        this.Alias = alias;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal class MappingAttribute : Attribute
{
    public string Key { get; }
    public object Value { get; }
    public MappingAttribute(string key, object value)
    {
        this.Key = key;
        this.Value = value;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal class MatchAttribute : Attribute
{
    public int[] NPCID { get; }

    public MatchAttribute(params int[] match)
    {
        this.NPCID = match;
    }
}