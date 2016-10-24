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
        public static double logGaussianDistribution(double x, double mu, double sigma)
        {
            return -Math.Log(Math.Sqrt(2 * Math.PI)) - Math.Log(sigma) - (x - mu) * (x - mu) / (2 * sigma * sigma);
        }

        public static PositionParams registerPosition(List<Point> pointList)
        {
            int n = pointList.Count;
            double sumX = pointList.Sum((x) => { return x.X; });
            double sumY = pointList.Sum((x) => { return x.Y; });
            double sumXY = pointList.Sum((x) => { return x.X * x.Y; });
            double sumXX = pointList.Sum((x) => { return x.X * x.X; });
            double a = (sumXY - sumX * sumY / n) / (sumXX - sumX * sumX / n);
            double b = sumY / n - a * sumX / n;
            return new PositionParams(0,0,0,0,0);
        }
    }
}
