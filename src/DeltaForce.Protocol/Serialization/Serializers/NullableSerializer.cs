namespace DeltaForce.Protocol.Serialization;

internal class NullableSerializer<T> : FieldSerializer<T?> where T : struct
{
    private static readonly Dictionary<Type, Func<IFieldSerializer>> NullableSerializers = new()
    {
        [typeof(int)] = () => new PrimitiveFieldSerializer<int>(),
        [typeof(long)] = () => new PrimitiveFieldSerializer<long>(),
        [typeof(short)] = () => new PrimitiveFieldSerializer<short>(),
        [typeof(byte)] = () => new PrimitiveFieldSerializer<byte>(),
        [typeof(bool)] = () => new PrimitiveFieldSerializer<bool>(),
        [typeof(float)] = () => new PrimitiveFieldSerializer<float>(),
        [typeof(double)] = () => new PrimitiveFieldSerializer<double>(),
        [typeof(Guid)] = () => new GuidSerializer()
    };

    private readonly IFieldSerializer _underlyingSerializer;

    public NullableSerializer()
    {
        if (!NullableSerializers.TryGetValue(typeof(T), out var factory))
            throw new NotSupportedException($"Nullable<{typeof(T)}> is not supported");

        _underlyingSerializer = factory();
    }

    protected override T? ReadOverride(BinaryReader br)
    {
        var hasValue = br.ReadBoolean();
        if (!hasValue)
            return null;

        return (T)_underlyingSerializer.Read(br);
    }

    protected override void WriteOverride(BinaryWriter bw, T? value)
    {
        bw.Write(value.HasValue);
        if (value.HasValue)
        {
            _underlyingSerializer.Write(bw, value.Value);
        }
    }
}
