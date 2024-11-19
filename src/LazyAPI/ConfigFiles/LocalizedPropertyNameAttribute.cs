

namespace LazyAPI.ConfigFiles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class LocalizedPropertyNameAttribute : Attribute
{
    public string Text { get;}

    public string Type { get;}

    public LocalizedPropertyNameAttribute(string type, string text)
    {
        this.Text = text;
        this.Type = type;
    }
}
