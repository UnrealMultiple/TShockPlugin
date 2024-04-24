using System;
using Microsoft.Xna.Framework;

namespace MiniGamesAPI
{
    public class MiniCircle
    {
        public double x;

        public double y;

        public double radius;

        public MiniCircle(double x, double y, double radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public bool Contain(Point point)
        {
            double num = (float)point.X * 16f;
            double num2 = (float)point.Y * 16f;
            return Math.Abs((x - num) * (x - num) - (y - num2) * (y - num2)) <= radius * radius;
        }

        public bool Contain(float x, float y)
        {
            return Math.Abs((this.x - (double)x) * (this.x - (double)x) - (this.y - (double)y) * (this.y - (double)y)) <= radius * radius;
        }
    }
}