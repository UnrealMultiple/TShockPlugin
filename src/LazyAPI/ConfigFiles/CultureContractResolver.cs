using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Reflection;

namespace LazyAPI.ConfigFiles;
public class CultureContractResolver : DefaultContractResolver
{
    private readonly CultureInfo _culture;

    public CultureContractResolver(CultureInfo cultureInfo)
    {
        this._culture = cultureInfo;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        var languages = member.GetCustomAttributes<LocalizedPropertyNameAttribute>();
        property.PropertyName = languages.FirstOrDefault(x => x.Type == this._culture.Name)?.Text
            ?? languages.FirstOrDefault(x => x.Type == CultureType.Chinese)?.Text 
            ?? property.PropertyName;
        return property;
    }
}
