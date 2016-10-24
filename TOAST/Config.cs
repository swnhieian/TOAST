using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOAST
{
    static class Config
    {
        public static double keyWidth = 41;
        public static double keyHeight = 44;
        public static double logGaussianDistribution(double x, double mu, double sigma)
        {
            return -Math.Log(Math.Sqrt(2 * Math.PI)) - Math.Log(sigma) - (x - mu) * (x - mu) / (2 * sigma * sigma);
        }

        public static PositionParams registerPosition(List<Point> pointList, int lr)
        {
            int n = pointList.Count;
            double a = fitLine(pointList).Item1;
            PositionParams ret = new PositionParams(0,0,0,0,0);
            ret.RotateAngle = Math.Atan(a);
            ret.ScaleX = (pointList[n - 1].X - pointList[0].X) / ((n - 1) * keyWidth);
            ret.ScaleY = 1;
            ret.OffsetY = pointList[0].Y - 1.5 * keyHeight;
            if (lr == 0)
            {
                ret.OffsetX = pointList[0].X - ret.ScaleX * keyWidth;
            } else
            {
                ret.OffsetX = pointList[0].X - ret.ScaleX * 7 * keyWidth;
            }
            return ret;
        }

        public static Tuple<double, double> fitLine(List<Point> pointList)
        {
            int n = pointList.Count;
            double sumX = pointList.Sum((x) => { return x.X; });
            double sumY = pointList.Sum((x) => { return x.Y; });
            double sumXY = pointList.Sum((x) => { return x.X * x.Y; });
            double sumXX = pointList.Sum((x) => { return x.X * x.X; });
            double a = (sumXY - sumX * sumY / n) / (sumXX - sumX * sumX / n);
            double b = sumY / n - a * sumX / n;
            return new Tuple<double, double>(a, b);
        }
    }
}
