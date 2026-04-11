namespace DeltaForce.Protocol.Serialization;

internal class ByteArraySerializer : FieldSerializer<byte[]>
{
    protected override byte[] ReadOverride(BinaryReader br)
    {
        var len = br.ReadInt32();
        return br.ReadBytes(len);
    }

    protected override void WriteOverride(BinaryWriter bw, byte[] value)
    {
        bw.Write(value.Length);
        bw.Write(value);
    }
}
