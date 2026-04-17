namespace DeltaForce.Protocol.Serialization;

internal class StringSerializer : FieldSerializer<string>
{
    protected override string ReadOverride(BinaryReader br) => br.ReadString();
    protected override void WriteOverride(BinaryWriter bw, string value) => bw.Write(value);
}
