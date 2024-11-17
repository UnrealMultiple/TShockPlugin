using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace LazyAPI.ConfigFiles;
public class LocalizationContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        var languages = member.GetCustomAttributes<LocalizationPropertyAttribute>();
        if (languages.Any())
        {
            Console.WriteLine(Terraria.Localization.Language.ActiveCulture.CultureInfo.LCID);
            var language = Terraria.Localization.Language.ActiveCulture.CultureInfo.LCID switch
            {
                (int) LocalizationType.ZH_CN => languages.FirstOrDefault(x => x.Type == LocalizationType.ZH_CN),
                (int) LocalizationType.EN_US => languages.FirstOrDefault(x => x.Type == LocalizationType.EN_US),
                _ => languages.FirstOrDefault(x => x.Type == LocalizationType.ZH_CN),
            };
            if (language != null)
            { 
                property.PropertyName = language.Text;
            }
        }
        return property;
    }
}
