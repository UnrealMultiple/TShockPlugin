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
public class FlexibleAttribute : Attribute
{

}


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionsAttribute(string perm) : Attribute
{
    public string perm = perm;
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AliasAttribute(params string[] aliases) : Attribute
{
    public HashSet<string> alias = [.. aliases];
}

[AttributeUsage(AttributeTargets.Class)]
public class HelpTextAttribute(string helpText) : Attribute
{
    public string helpText = helpText;
}

[AttributeUsage(AttributeTargets.Method)]
public class UsageAttribute(string usage) : Attribute
{
    public string usage = usage;
}