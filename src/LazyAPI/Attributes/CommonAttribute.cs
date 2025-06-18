namespace LazyAPI.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RestAttribute(params string[] aliases) : Attribute
{
    public HashSet<string> alias = [.. aliases];
}

[AttributeUsage(AttributeTargets.Method)]
public class RestPathAttribute(string alias) : Attribute
{
    public string alias = alias;
}

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(params string[] aliases) : Attribute
{
    public HashSet<string> alias = [.. aliases];
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

[AttributeUsage(AttributeTargets.Method)]
public class KindAttribute : Attribute
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
public class AliasAttribute(params string[] aliases) : Attribute
{
    public HashSet<string> alias = [.. aliases];
}


