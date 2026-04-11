namespace DeltaForce.Protocol.Serialization;

public interface IFieldSerializer
{
    object Read(BinaryReader br);
    void Write(BinaryWriter bw, object o);
}
