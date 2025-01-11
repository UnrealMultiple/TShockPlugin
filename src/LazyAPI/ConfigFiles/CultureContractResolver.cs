using LazyAPI.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Reflection;

namespace LazyAPI.ConfigFiles;
public class CultureContractResolver : DefaultContractResolver
{
    private readonly CultureInfo _culture;

    private readonly Func<Type, object[]?, JsonConverter> CreateJsonConverterInstance;

    public CultureContractResolver(CultureInfo cultureInfo)
    {
        this._culture = cultureInfo;
        this.CreateJsonConverterInstance = (Func<Type, object[]?, JsonConverter>) Delegate.CreateDelegate(typeof(Func<Type, object[]?, JsonConverter>), typeof(JsonConvert).Assembly.GetType("Newtonsoft.Json.Serialization.JsonTypeReflector")?.GetMethod("CreateJsonConverterInstance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!);
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        var languages = member.GetCustomAttributes<LocalizedPropertyNameAttribute>();
        var currentLanguage = languages.FirstOrDefault(x => x.Type == this._culture.Name) ?? languages.FirstOrDefault(x => x.Type == CultureType.Chinese);
        if (currentLanguage != null)
        {
            property.PropertyName = currentLanguage.Text;
            property.Order = currentLanguage._order;
            property.Required = currentLanguage.Required;
            property.DefaultValueHandling = currentLanguage._defaultValueHandling;
            property.NullValueHandling = currentLanguage._nullValueHandling;
            property.ReferenceLoopHandling = currentLanguage._referenceLoopHandling;
            property.ObjectCreationHandling = currentLanguage._objectCreationHandling;
            property.TypeNameHandling = currentLanguage._typeNameHandling;
            property.IsReference = currentLanguage._isReference;
            property.ItemIsReference = currentLanguage._itemIsReference;
            property.ItemConverter = currentLanguage.ItemConverterType != null ? this.CreateJsonConverterInstance(currentLanguage.ItemConverterType, currentLanguage.ItemConverterParameters) : null;
            property.ItemReferenceLoopHandling = currentLanguage._itemReferenceLoopHandling;
            property.ItemTypeNameHandling = currentLanguage._itemTypeNameHandling;
        }
        return property;
    }
}
