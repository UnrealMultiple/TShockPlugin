namespace DeltaForce.Protocol.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class SerializerAttribute : Attribute
{
    public Type SerializerType { get; }

    public SerializerAttribute(Type serializerType)
    {
        if (!typeof(IFieldSerializer).IsAssignableFrom(serializerType))
            throw new ArgumentException($"Type {serializerType} must implement IFieldSerializer");
        
        SerializerType = serializerType;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class DefaultSerializerAttribute : Attribute
{
    public Type SerializerType { get; }

    public DefaultSerializerAttribute(Type serializerType)
    {
        if (!typeof(IFieldSerializer).IsAssignableFrom(serializerType))
            throw new ArgumentException($"Type {serializerType} must implement IFieldSerializer");
        
        SerializerType = serializerType;
    }
}
