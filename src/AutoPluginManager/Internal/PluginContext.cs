namespace AutoPluginManager.Internal;
internal class PluginContext
{
    public string AssemblyName { get; set; } = string.Empty;

    public byte[] AssemblyBuffer { get; set; } = Array.Empty<byte>();

    public byte[] PdbBuffer { get; set; } = Array.Empty<byte>();

    public byte[] ReadmdBuffer { get; set; } = Array.Empty<byte>();
}
