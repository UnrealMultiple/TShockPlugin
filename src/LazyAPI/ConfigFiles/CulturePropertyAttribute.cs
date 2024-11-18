

namespace LazyAPI.ConfigFiles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class CulturePropertyAttribute : Attribute
{
    public string Text { get;}

    public string Type { get;}

    public CulturePropertyAttribute(string type, string text)
    {
        this.Text = text;
        this.Type = type;
    }
}
