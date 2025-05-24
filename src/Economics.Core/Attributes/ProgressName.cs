namespace Economics.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ProgressName : Attribute
{
    public string[] Names { get; set; }

    public ProgressName(params string[] names)
    {
        this.Names = names;
    }
}