namespace BadApplePlayer.Models;

public class PlaybackSession(
    string positionName,
    PositionData position,
    BadAppleVideo video,
    bool loop,
    string startedBy)
{
    public string PositionName { get; set; } = positionName;
    public PositionData Position { get; set; } = position;
    public BadAppleVideo Video { get; set; } = video;
    public bool IsPlaying { get; private set; } = true;
    public bool IsPaused { get; set; } = false;
    public int CurrentFrame { get; set; } = 0;
    public bool Loop { get; set; } = loop;
    public string StartedBy { get; set; } = startedBy;

    public void Stop()
    {
        this.IsPlaying = false;
        this.IsPaused = false;
    }
}