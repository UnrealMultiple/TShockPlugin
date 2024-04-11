namespace ProgressBag;

[AttributeUsage(AttributeTargets.Field)]
public class ProgressNameAttribute : Attribute
{
    public string[] Names { get; set; }

    public ProgressNameAttribute(params string[] names)
    {
        Names = names;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ProgressMapAttribute : Attribute
{
    public string Filed { get; set; }

    public object value { get; set; }

    public ProgressMapAttribute(string Filed, object value)
    {
        this.Filed = Filed;

        this.value = value;
    }
}

