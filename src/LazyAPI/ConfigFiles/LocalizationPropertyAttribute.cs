

namespace LazyAPI.ConfigFiles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class LocalizationPropertyAttribute : Attribute
{
    public string Chinese { get;}

    public string? English { get;}

    public LocalizationPropertyAttribute(string chinese, string? english = null)
    {
        this.Chinese = chinese;
        this.English = english;
    }
}
