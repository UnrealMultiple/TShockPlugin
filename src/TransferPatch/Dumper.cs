using System.Reflection;

namespace TransferPatch;

public class Dumper(string file) : IDisposable
{
    private readonly StreamWriter _sw = new(file);

    private readonly static BindingFlags Flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

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
        if (processedTypes.Contains(type) ||
            type.Assembly != targetAssembly)
        {
            return;
        }
        processedTypes.Add(type);

        // 处理字段
        foreach (var field in type.GetFields(Flag))
        {
            this._sw.WriteLine($"\"{FormatTypeName(field.DeclaringType)}.{field.Name}\" : \"\"");
            this.ProcessMemberType(field.FieldType, targetAssembly, processedTypes);
        }

        // 处理属性
        foreach (var prop in type.GetProperties(Flag))
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

        if (memberType.IsGenericType)
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

    private static string FormatTypeName(Type? type)
    {
        if (type == null)
        {
            return string.Empty;
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
            var backtickIndex = name.IndexOf('`');
            if (backtickIndex > 0)
            {
                name = name[..backtickIndex];
            }
            
            return $"{type.Namespace}.{name}`{genericDef.GetGenericArguments().Length}";
        }

        return type.FullName ?? type.Name;
    }

    public void Dispose()
    {
        this._sw?.Dispose();
        GC.SuppressFinalize(this);
    }
}