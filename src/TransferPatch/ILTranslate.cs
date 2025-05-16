using Mono.Cecil;
using Mono.Collections.Generic;
using Newtonsoft.Json;

namespace TransferPatch;

public class ILTranslate : IDisposable
{
    private readonly string _file;

    private readonly AssemblyDefinition _assemblyDefinition;

    private const string JsonNameSpace = "Newtonsoft.Json";

    public event Func<FieldDefinition, string, string>? SetField;

    public event Func<PropertyDefinition, string, string>? SetProperty;

    public ILTranslate(Stream stream, string fileName)
    {
        this._file = fileName;
        this._assemblyDefinition = AssemblyDefinition.ReadAssembly(stream);
    }

    public void Patch(params string[] classNames)
    {
        foreach (var className in classNames)
        { 

            var targetType = this.FindTargetType(className) ?? throw new NullReferenceException(GetString("未找到目标类!"));
            var jsonPropCtor = this.GenerateJsonPropertyConstructor();
            foreach (var prop in targetType.Properties)
            {
                this.AddJsonPropertyAttribute(prop, jsonPropCtor, className);
            }

            foreach (var field in targetType.Fields)
            {
                this.AddJsonPropertyAttributeToField(field, jsonPropCtor, className);
            }
        }
        
        this._assemblyDefinition.Write(this._file);
    }

    private void AddJsonPropertyAttribute(PropertyDefinition prop, MethodReference constructor, string className)
    {
        this.RemoveExistingAttributes(prop.CustomAttributes);
        var jsonAttribute = new CustomAttribute(constructor);
        var jsonPropertyName = SetProperty?.Invoke(prop, className);
        jsonAttribute.ConstructorArguments.Add(
            new CustomAttributeArgument(prop.Module.TypeSystem.String, jsonPropertyName));
        prop.CustomAttributes.Add(jsonAttribute);
    }

    private void AddJsonPropertyAttributeToField(FieldDefinition field, MethodReference constructor, string className)
    {
        this.RemoveExistingAttributes(field.CustomAttributes);
        var jsonAttribute = new CustomAttribute(constructor);
        var jsonPropertyName = SetField?.Invoke(field, className);
        jsonAttribute.ConstructorArguments.Add(
            new CustomAttributeArgument(field.Module.TypeSystem.String, jsonPropertyName));
        field.CustomAttributes.Add(jsonAttribute);
    }

    private void RemoveExistingAttributes(Collection<CustomAttribute> attributes)
    {
        var toRemove = attributes
            .Where(attr => attr.AttributeType.FullName == typeof(JsonPropertyAttribute).FullName)
            .ToList();

        foreach (var attr in toRemove)
        {
            attributes.Remove(attr);
        }
    }

    private TypeDefinition? FindTargetType(string targetClassName)
    {
        foreach (var type in this._assemblyDefinition.MainModule.Types)
        {
            if (type.FullName == targetClassName)
            {
                return type;
            }

            if (type.HasGenericParameters &&
                type.FullName == targetClassName)
            {
                return type;
            }

            if (type.NestedTypes.Count > 0)
            {
                foreach (var nestedType in type.NestedTypes)
                {
                    if (nestedType.FullName == targetClassName ||
                       (nestedType.HasGenericParameters &&
                        nestedType.FullName == targetClassName))
                    {
                        return nestedType;
                    }
                }
            }
        }
        return null;
    }

    private MethodReference GenerateJsonPropertyConstructor()
    {
        var jsonAssembly = this._assemblyDefinition.MainModule.AssemblyReferences.First(a => a.Name == JsonNameSpace) ?? throw new NullReferenceException(GetString("无法获取JsonPropertyAttribute"));
        var attributeType = new TypeReference(
            JsonNameSpace,
            nameof(JsonPropertyAttribute),
            this._assemblyDefinition.MainModule,
            jsonAssembly
        );

        return new MethodReference(
            ".ctor",
            this._assemblyDefinition.MainModule.TypeSystem.Void,
            attributeType)
        {
            HasThis = true,
            Parameters = { new ParameterDefinition(this._assemblyDefinition.MainModule.TypeSystem.String) }
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
