namespace LazyAPI.Attributes;

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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RealPlayerAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method)]
public class MainAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionsAttribute : Attribute
{
    public string perm;

    public PermissionsAttribute(string perm)
    {
        this.perm = perm;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AliasAttribute : Attribute
{
    public HashSet<string> alias;

    public AliasAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}


