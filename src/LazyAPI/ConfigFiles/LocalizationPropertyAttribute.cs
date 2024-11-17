

namespace LazyAPI.ConfigFiles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class LocalizationPropertyAttribute : Attribute
{
    public string Text { get;}

    public LocalizationType Type { get;}

    public LocalizationPropertyAttribute(string chinese, LocalizationType type)
    {
        this.Text = chinese;
        this.Type = type;
    }
}
