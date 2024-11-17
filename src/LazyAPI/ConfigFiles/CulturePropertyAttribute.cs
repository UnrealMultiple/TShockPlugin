

namespace LazyAPI.ConfigFiles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class CulturePropertyAttribute : Attribute
{
    public string Text { get;}

    public CultureType Type { get;}

    public CulturePropertyAttribute(CultureType type, string chinese)
    {
        this.Text = chinese;
        this.Type = type;
    }
}
