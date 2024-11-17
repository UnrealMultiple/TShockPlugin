using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace LazyAPI.ConfigFiles;
public class LocalizationContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        var language = member.GetCustomAttribute<LocalizationPropertyAttribute>();
        if (language != null)
        {
            property.PropertyName = Terraria.Localization.Language.ActiveCulture.LegacyId switch
            {
                1 => language.English,
                7 => language.Chinese,
                _ => language.Chinese
            };
        }
        return property;
    }
}
