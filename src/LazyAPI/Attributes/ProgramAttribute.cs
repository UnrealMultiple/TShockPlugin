namespace LazyAPI.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ProgressNameAttribute : Attribute
{
    public string[] Names { get; set; }

    public ProgressNameAttribute(params string[] names)
    {
        this.Names = names;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ProgressMapID : Attribute
{
    public int[] ID { get; set; }

    public ProgressMapID(params int[] id)
    {
        this.ID = id;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ProgressMapAttribute : Attribute
{
    public string Filed { get; set; }

    public Type Target { get; set; }

    public object Value { get; set; }

    public ProgressMapAttribute(string Filed, Type target, object value)
    {
        this.Filed = Filed;

        this.Target = target;

        this.Value = value;
    }
}
