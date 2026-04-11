namespace DeltaForce.Protocol.Serialization;

internal class PrimitiveFieldSerializer<T> : FieldSerializer<T>
{
    protected override T ReadOverride(BinaryReader br)
    {
        var method = typeof(BinaryReader).GetMethod($"Read{typeof(T).Name}")!;
        return (T)method.Invoke(br, null)!;
    }

    protected override void WriteOverride(BinaryWriter bw, T value)
    {
        var method = typeof(BinaryWriter).GetMethod($"Write", [typeof(T)])!;
        method.Invoke(bw, [value]);
    }
}
