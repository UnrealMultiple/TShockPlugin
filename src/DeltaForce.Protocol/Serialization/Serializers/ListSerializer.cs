namespace DeltaForce.Protocol.Serialization;

internal class ListSerializer<T> : FieldSerializer<List<T>>
{
    protected override List<T> ReadOverride(BinaryReader br)
    {
        var length = br.ReadInt32();
        var list = new List<T>(length);
        var elementSerializer = PacketSerializer.RequestFieldSerializer(typeof(T), null);

        for (int i = 0; i < length; i++)
        {
            list.Add((T)elementSerializer.Read(br));
        }

        return list;
    }

    protected override void WriteOverride(BinaryWriter bw, List<T> value)
    {
        bw.Write(value.Count);
        var elementSerializer = PacketSerializer.RequestFieldSerializer(typeof(T), null);

        foreach (var item in value)
        {
            elementSerializer.Write(bw, item);
        }
    }
}
