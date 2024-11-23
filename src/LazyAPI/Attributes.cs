namespace LazyAPI;

[AttributeUsage(AttributeTargets.Class)]
public class RestAttribute : Attribute
{
    public HashSet<string> alias;

    public RestAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public HashSet<string> alias;

    public CommandAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : Attribute
{
}
