namespace Economics.Core.Utils;

public struct FrameTimer(int intervalFrames, bool triggerOnFirstTick = false)
{
    public int RemainingFrames = triggerOnFirstTick ? 0 : intervalFrames;
    public int IntervalFrames = intervalFrames;

    public bool TickTock()
    {
        this.RemainingFrames--;
        if (this.RemainingFrames > 0)
        {
            return false;
        }

        this.RemainingFrames += this.IntervalFrames;
        if (this.RemainingFrames < 0)
        {
            this.RemainingFrames = 0;
        }

        return true;
    }

    public void Reset(bool triggerOnNextTick = false)
    {
        this.RemainingFrames = triggerOnNextTick ? 0 : this.IntervalFrames;
    }
}
