using Microsoft.Xna.Framework;

namespace EssentialsPlus;

public class PlayerInfo
{
    private readonly List<Vector2> backHistory = new List<Vector2>();
    private readonly CancellationTokenSource mute = new CancellationTokenSource();
    private CancellationTokenSource timeCmd = new CancellationTokenSource();

    public const string KEY = "EssentialsPlus_Data";

    public int BackHistoryCount => this.backHistory.Count;
    public CancellationToken MuteToken => this.mute.Token;
    public CancellationToken TimeCmdToken => this.timeCmd.Token;
    public string? LastCommand { get; set; }

    ~PlayerInfo()
    {
        this.mute.Cancel();
        this.mute.Dispose();
        this.timeCmd.Cancel();
        this.timeCmd.Dispose();
    }

    public void CancelTimeCmd()
    {
        this.timeCmd.Cancel();
        this.timeCmd.Dispose();
        this.timeCmd = new CancellationTokenSource();
    }
    public Vector2 PopBackHistory(int steps)
    {
        var vector = this.backHistory[steps - 1];
        this.backHistory.RemoveRange(0, steps);
        return vector;
    }
    public void PushBackHistory(Vector2 vector)
    {
        this.backHistory.Insert(0, vector);
        if (this.backHistory.Count > Config.Instance.BackPositionHistory)
        {
            this.backHistory.RemoveAt(this.backHistory.Count - 1);
        }
    }
}