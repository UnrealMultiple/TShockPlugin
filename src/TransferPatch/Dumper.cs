using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TransferPatch;

public class Dumper(string file) : IDisposable
{
    private readonly StreamWriter _sw = new(file);
    public void DumpType(Type rootType)
    {
        var processedTypes = new HashSet<Type>();
        var targetAssembly = rootType.Assembly;
        this.DumpTypeInternal(rootType, targetAssembly, processedTypes);
        this._sw.Flush();
        this._sw.Close();
    }

    private void DumpTypeInternal(Type type, Assembly targetAssembly, HashSet<Type> processedTypes)
    {
        // 跳过已处理类型、基础类型和非目标程序集类型
        if (processedTypes.Contains(type) || 
            IsBasicType(type) || 
            type.Assembly != targetAssembly)
        {
            return;
        }
        processedTypes.Add(type);

        // 处理字段
        foreach (var field in GetRelevantFields(type))
        {
            this._sw.WriteLine($"\"{FormatTypeName(field.DeclaringType)}.{field.Name}\" : \"\"");
            this.ProcessMemberType(field.FieldType, targetAssembly, processedTypes);
        }

        // 处理属性
        foreach (var prop in GetRelevantProperties(type))
        {
            this._sw.WriteLine($"\"{FormatTypeName(prop.DeclaringType)}.{prop.Name}\" : \"\"");
            this.ProcessMemberType(prop.PropertyType, targetAssembly, processedTypes);
        }
    }

    private void ProcessMemberType(Type memberType, Assembly targetAssembly, HashSet<Type> processedTypes)
    {
        // 处理数组元素类型
        if (memberType.IsArray)
        {
            var elementType = memberType.GetElementType();
            if (elementType != null && elementType.Assembly == targetAssembly)
            {
                this.DumpTypeInternal(elementType, targetAssembly, processedTypes);
            }
            return;
        }

        if (IsGenericCollection(memberType))
        { 
            foreach(var args in memberType.GetGenericArguments())
            {
                if (args.Assembly == targetAssembly)
                {
                    this.DumpTypeInternal(args, targetAssembly, processedTypes);
                }
            }
            return;
        }

        if (memberType.Assembly == targetAssembly)
        {
            this.DumpTypeInternal(memberType, targetAssembly, processedTypes);
        }
    }

    // 核心：格式化类型名称（处理泛型和嵌套类型）
    private static string FormatTypeName(Type? type)
    {
        if (type == null)
        {
            return "null";
        }

        // 处理嵌套类型
        if (type.IsNested)
        {
            return $"{FormatTypeName(type.DeclaringType)}/{type.Name}";
        }

        // 处理泛型类型
        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            var name = genericDef.Name;
            
            // 保留反引号和参数数量，去除具体类型参数
            var backtickIndex = name.IndexOf('`');
            if (backtickIndex > 0)
            {
                name = name[..backtickIndex];
            }
            
            return $"{type.Namespace}.{name}`{genericDef.GetGenericArguments().Length}";
        }

        // 普通类型
        return type.FullName ?? type.Name;
    }
    
    private static bool IsGenericCollection(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDef = type.GetGenericTypeDefinition();
        return genericTypeDef == typeof(List<>) ||
               genericTypeDef == typeof(Dictionary<,>) ||
               genericTypeDef == typeof(HashSet<>) ||
               genericTypeDef == typeof(Queue<>) ||
               genericTypeDef == typeof(Stack<>) ||
               genericTypeDef == typeof(LinkedList<>) ||
               genericTypeDef == typeof(SortedSet<>) ||
               genericTypeDef == typeof(ICollection<>) ||
               genericTypeDef == typeof(IEnumerable<>) ||
               genericTypeDef == typeof(IList<>);
    }

    private static IEnumerable<FieldInfo> GetRelevantFields(Type type)
    {
        return type.GetFields(
            BindingFlags.Public | 
            BindingFlags.Instance |
            BindingFlags.DeclaredOnly
        );
    }

    private static IEnumerable<PropertyInfo> GetRelevantProperties(Type type)
    {
        return type.GetProperties(
            BindingFlags.Public | 
            BindingFlags.Instance | 
            BindingFlags.DeclaredOnly
        );
    }

    private static bool IsBasicType(Type type)
    {
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(object) || 
               type == typeof(ValueType) || 
               type == typeof(Enum) || 
               type == typeof(void) || 
               type.IsEnum;
    }

    public void Dispose()
    {
        this._sw?.Dispose();
        GC.SuppressFinalize(this);
    }
}