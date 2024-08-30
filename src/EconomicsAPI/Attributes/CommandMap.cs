namespace EconomicsAPI.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandMap : Attribute
{
    public string Name { get; }

    public List<string> Permission { get; }

    public CommandMap(string name, params string[] perm)
    {
        Name = name;
        Permission = perm.ToList();
    }
}
