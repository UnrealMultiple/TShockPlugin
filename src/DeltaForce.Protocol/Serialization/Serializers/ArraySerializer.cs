namespace DeltaForce.Protocol.Serialization;

internal class ArraySerializer<T> : FieldSerializer<T[]>
{
    protected override T[] ReadOverride(BinaryReader br)
    {
        var length = br.ReadInt32();
        var array = new T[length];
        var elementSerializer = PacketSerializer.RequestFieldSerializer(typeof(T), null);

        for (int i = 0; i < length; i++)
        {
            array[i] = (T)elementSerializer.Read(br);
        }

        return array;
    }

    protected override void WriteOverride(BinaryWriter bw, T[] value)
    {
        bw.Write(value.Length);
        var elementSerializer = PacketSerializer.RequestFieldSerializer(typeof(T), null);

        foreach (var item in value)
        {
            elementSerializer.Write(bw, item);
        }
    }
}
