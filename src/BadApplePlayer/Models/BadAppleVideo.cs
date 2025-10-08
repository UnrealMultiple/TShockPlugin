using System.IO.Compression;
using TShockAPI;

namespace BadApplePlayer.Models;

public class BadAppleVideo
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Fps { get; private set; }
    public int FrameCount { get; private set; }
    private List<byte[]> FramesData { get; set; } = new ();

    /// <summary>
    /// ZLib 解压并读取
    /// </summary>
    public void LoadCompressed(Stream stream)
    {
        using var decompressor = new ZLibStream(stream, CompressionMode.Decompress);
        using var reader = new BinaryReader(decompressor);

        this.Width = reader.ReadByte();
        this.Height = reader.ReadByte();
        var fpsInt = reader.ReadUInt16();
        this.Fps = fpsInt / 100;
        this.FrameCount = reader.ReadInt32();

        for (var i = 0; i < this.FrameCount; i++)
        {
            var frameSize = reader.ReadInt32();
            var frameData = reader.ReadBytes(frameSize);
            this.FramesData.Add(frameData);
        }
    }

    /// <summary>
    /// 数据差分
    /// </summary>
    public List<(int x, int y, bool isWhite)> GetFrameChanges(int frameIndex, bool[,]? previousFrame)
    {
        var changes = new List<(int x, int y, bool isWhite)>();
    
        if (frameIndex < 0 || frameIndex >= this.FrameCount)
        {
            return changes;
        }
    
        var frameData = this.FramesData[frameIndex];

        if (frameData.Length < 2)
        {
            return changes;
        }

        using var stream = new MemoryStream(frameData);
        using var reader = new BinaryReader(stream);

        try
        {
            var changeCount = reader.ReadUInt16();

            if (frameIndex == 0 || previousFrame == null)
            {
                for (var i = 0; i < changeCount; i++)
                {
                    var x = reader.ReadByte();
                    var y = reader.ReadByte();
                    
                    if (x < this.Width && y < this.Height)
                    {
                        changes.Add((x, y, true));
                    }
                }
            }
            else
            {
                for (var i = 0; i < changeCount; i++)
                {
                    var x = reader.ReadByte();
                    var y = reader.ReadByte();
                    var value = reader.ReadByte();
                    
                    if (x < this.Width && y < this.Height)
                    {
                        if (previousFrame[x, y] != (value == 1))
                        {
                            changes.Add((x, y, value == 1));
                        }
                    }
                }
            }
        }
        catch (EndOfStreamException) {     
            TShock.Log.Warn(GetString("[BadApplePlayer]: 数据读取异常"));
        }
        return changes;
    }}