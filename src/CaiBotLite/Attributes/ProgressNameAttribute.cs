namespace CaiBotLite.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ProgressNameAttribute(params string[]? names) : Attribute
{
    public string[] Names { get; } = names ?? [];
}