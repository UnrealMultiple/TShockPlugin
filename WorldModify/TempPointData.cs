using Microsoft.Xna.Framework;

namespace WorldModify
{
    internal class TempPointData
    {
        public Point[] TempPoints = (Point[])(object)new Point[2];

        public Rectangle rect = default;

        public int AwaitingTempPoint { get; set; }
    }
}
