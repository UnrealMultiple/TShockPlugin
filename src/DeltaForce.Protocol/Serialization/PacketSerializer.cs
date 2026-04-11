using System.Reflection;
using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Serialization;

public class PacketSerializer
{
    private readonly Dictionary<Type, Action<BinaryWriter, object>> _serializers = [];
    private readonly Dictionary<PacketType, Func<BinaryReader, object>> _deserializers = [];

    public PacketSerializer()
    {
        LoadPackets();
    }

    private void LoadPackets()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            RegisterPacket(type);
        }
    }

    public void RegisterPacket<T>() where T : INetPacket
    {
        RegisterPacket(typeof(T));
    }

    private void RegisterPacket(Type type)
    {
        if (type.IsAbstract || !typeof(INetPacket).IsAssignableFrom(type)) return;

        var serializers = new List<Action<BinaryWriter, object>>();
        var deserializers = new List<Action<object, BinaryReader>>();

        foreach (var prop in type.GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite) continue;
            if (prop.IsDefined(typeof(IgnoreAttribute))) continue;

            var ser = RequestFieldSerializer(prop.PropertyType, prop);

            var currentProp = prop;
            serializers.Add((bw, o) => ser.Write(bw, currentProp.GetValue(o)));
            deserializers.Add((o, br) => currentProp.SetValue(o, ser.Read(br)));
        }

        var inst = Activator.CreateInstance(type) as INetPacket;

        if (serializers.Count > 0)
        {
            _serializers[type] = (bw, o) =>
            {
                foreach (var s in serializers)
                    s(bw, o);
            };
        }

        if (deserializers.Count > 0)
        {
            _deserializers[inst!.PacketID] = br =>
            {
                var result = Activator.CreateInstance(type)!;
                foreach (var d in deserializers)
                    d(result, br);
                return result;
            };
        }
    }

    private static readonly Dictionary<Type, IFieldSerializer> FieldSerializers = new()
    {
        [typeof(string)] = new StringSerializer(),
        [typeof(Guid)] = new GuidSerializer(),
        [typeof(byte[])] = new ByteArraySerializer(),
        [typeof(DateTime)] = new DateTimeSerializer()
    };

    public static IFieldSerializer RequestFieldSerializer(Type type, PropertyInfo? prop)
    {
        if (prop?.GetCustomAttribute<SerializerAttribute>() is { } serializerAttr)
            return (IFieldSerializer)Activator.CreateInstance(serializerAttr.SerializerType)!;

        if (type.GetCustomAttribute<DefaultSerializerAttribute>() is { } defaultSerializerAttr)
            return (IFieldSerializer)Activator.CreateInstance(defaultSerializerAttr.SerializerType)!;

        if (FieldSerializers.TryGetValue(type, out var cachedSerializer))
            return cachedSerializer;
        var serializer = CreateSerializer(type);
        FieldSerializers[type] = serializer;
        return serializer;
    }

    private static IFieldSerializer CreateSerializer(Type type) => type switch
    {
        _ when type.IsPrimitive || type.IsEnum => CreateGenericSerializer(typeof(PrimitiveFieldSerializer<>), type),

        _ when type.IsArray => CreateGenericSerializer(typeof(ArraySerializer<>), type.GetElementType()!),

        _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) 
            => CreateGenericSerializer(typeof(ListSerializer<>), type.GetGenericArguments()[0]),

        _ when Nullable.GetUnderlyingType(type) is { } underlyingType 
            => CreateGenericSerializer(typeof(NullableSerializer<>), underlyingType),

        _ when type.IsClass && type != typeof(object) 
            => CreateGenericSerializer(typeof(ClassSerializer<>), type),

        _ => throw new NotSupportedException($"Type {type} is not supported for serialization")
    };

    private static IFieldSerializer CreateGenericSerializer(Type genericSerializerType, Type typeArgument)
    {
        var concreteType = genericSerializerType.MakeGenericType(typeArgument);
        return (IFieldSerializer)Activator.CreateInstance(concreteType)!;
    }

    public byte[] Serialize(INetPacket packet)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.Write((short)0);
        bw.Write((byte)packet.PacketID);

        if (_serializers.TryGetValue(packet.GetType(), out var f))
        {
            f(bw, packet);
            var length = (short)ms.Position;
            ms.Position = 0;
            bw.Write(length);
            return ms.ToArray();
        }

        return [];
    }

    public INetPacket? Deserialize(BinaryReader br)
    {
        var length = br.ReadInt16();
        var packetType = (PacketType)br.ReadByte();

        if (_deserializers.TryGetValue(packetType, out var f))
        {
            return f(br) as INetPacket;
        }

        return null;
    }
}
