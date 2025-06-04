namespace Economics.Core.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ProgressMap : Attribute
{
    public string Filed { get; set; }

    public object value { get; set; }

    public ProgressMap(string Filed, object value)
    {
        this.Filed = Filed;

        this.value = value;
    }
}