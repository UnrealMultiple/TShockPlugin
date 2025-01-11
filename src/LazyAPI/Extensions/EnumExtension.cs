namespace LazyAPI.Extensions;
public static class EnumExtension
{
    public static T? GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value)!;
        return type.GetField(name)
                   ?.GetCustomAttributes(false)
                   .OfType<T>()
                   .FirstOrDefault();
    }
}
