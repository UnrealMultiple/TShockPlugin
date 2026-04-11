namespace DeltaForce.Protocol.Serialization;

public abstract class FieldSerializer<T> : IFieldSerializer
{
    protected abstract T ReadOverride(BinaryReader br);
    protected abstract void WriteOverride(BinaryWriter bw, T t);

    public object Read(BinaryReader br) => ReadOverride(br);
    public void Write(BinaryWriter bw, object o) => WriteOverride(bw, (T)o);
}
