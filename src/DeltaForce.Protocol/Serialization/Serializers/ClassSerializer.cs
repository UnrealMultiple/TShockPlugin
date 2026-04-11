using System.Reflection;

namespace DeltaForce.Protocol.Serialization;

internal class ClassSerializer<T> : FieldSerializer<T> where T : class, new()
{
    private readonly List<(PropertyInfo prop, IFieldSerializer serializer)> _properties;

    public ClassSerializer()
    {
        _properties = [.. typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && !p.IsDefined(typeof(IgnoreAttribute), false))
            .Select(p => (p, PacketSerializer.RequestFieldSerializer(p.PropertyType, p)))];
    }

    protected override T ReadOverride(BinaryReader br)
    {
        var obj = new T();

        foreach (var (prop, serializer) in _properties)
        {
            var value = serializer.Read(br);
            prop.SetValue(obj, value);
        }

        return obj;
    }

    protected override void WriteOverride(BinaryWriter bw, T value)
    {
        foreach (var (prop, serializer) in _properties)
        {
            var propValue = prop.GetValue(value);
            serializer.Write(bw, propValue);
        }
    }
}
