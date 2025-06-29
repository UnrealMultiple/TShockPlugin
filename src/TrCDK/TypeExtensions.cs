using System;
using System.Reflection;

namespace TrCDK;

public static class TypeExtensions
{
    public static T CallPrivateMethod<T>(this Type _type, bool StaticMember, string Name, params object[] Params)
    {
        var bindingFlags = BindingFlags.NonPublic;
        bindingFlags |= StaticMember ? BindingFlags.Static : BindingFlags.Instance;
        var method = _type.GetMethod(Name, bindingFlags) ?? throw new InvalidOperationException(GetString($"找不到方法: {Name}"));
        var result = method.Invoke(StaticMember ? null : _type, Params);

        return result == null
            ? (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null
                ? default!
                : (T) (object) null!)
            : (T) result;
    }

    public static T GetPrivateField<T>(this Type type, object Instance, string Name, params object[] Param)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
        var field = type.GetField(Name, bindingAttr);

        if (field == null)
        {
            return default!;
        }

        var value = field.GetValue(Instance);

        return value == null
            ? (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null
                ? default!
                : (T) (object) null!)
            : (T) value;
    }
}