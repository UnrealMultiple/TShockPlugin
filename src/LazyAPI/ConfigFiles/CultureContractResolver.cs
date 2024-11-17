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
        var languages = member.GetCustomAttributes<CulturePropertyAttribute>();
        if (languages.Any())
        {
            var language = this._culture.LCID switch
            {
                (int) CultureType.ZH_CN => languages.FirstOrDefault(x => x.Type == CultureType.ZH_CN),
                (int) CultureType.EN_US => languages.FirstOrDefault(x => x.Type == CultureType.EN_US),
                _ => languages.FirstOrDefault(x => x.Type == CultureType.ZH_CN),
            };
            if (language != null)
            { 
                property.PropertyName = language.Text;
            }
        }
        return property;
    }
}
