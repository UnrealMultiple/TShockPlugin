using System;
using System.Reflection;

namespace TrCDK
{
	public static class TypeExtensions
	{
		public static T CallPrivateMethod<T>(this Type _type, bool StaticMember, string Name, params object[] Params)
		{
			var bindingFlags = BindingFlags.NonPublic;
			bindingFlags |= (StaticMember ? BindingFlags.Static : BindingFlags.Instance);
			var method = _type.GetMethod(Name, bindingFlags);
			return (T)method.Invoke(StaticMember ? null : _type, Params);
		}

		public static T GetPrivateField<T>(this Type type, object Instance, string Name, params object[] Param)
		{
			var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			var field = type.GetField(Name, bindingAttr);
			if (field == null)
			{
				return default(T);
			}
			return (T)field.GetValue(Instance);
		}
	}
}
