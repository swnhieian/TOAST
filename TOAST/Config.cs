using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TOAST
{
    static class Config
    {
        public static int candidateNum = 5;
        public static int candidateDefault = -1;
        public static int fitThreshold = 20;
        public static double keyWidth = 41;
        public static double keyHeight = 44;

        /////
        public static double inputTextBoxHeight = 50;
        public static double candidateHeight = inputTextBoxHeight;
        public static double inputTextBoxWidth = SystemParameters.PrimaryScreenWidth;
        public static double candidateInterval = 20;
        public static int inputFontSize = 30;
        public static int candidateFontSize = 30;
        /////
        public static Brush mainCanvasBackgroundInvalid = Brushes.Gray;
        public static Brush mainCanvasBackground = Brushes.Black;
        public static Brush candidateForeground = Brushes.White;
        public static Brush candidateBackground = Brushes.Black;
        public static Brush selectedCandidateForeground = Brushes.Black;
        public static Brush selectedCandidateBackground = Brushes.White;
        public static double logGaussianDistribution(double x, double mu, double sigma)
        {
            return -Math.Log(Math.Sqrt(2 * Math.PI)) - Math.Log(sigma) - (x - mu) * (x - mu) / (2 * sigma * sigma);
        }

        public static PositionParams registerPosition(List<Point> pointList, int lr)
        {
            int n = pointList.Count;
            double a = fitLine(pointList).Item1;
            PositionParams ret = new PositionParams(0,0,0,0,0,0,0);
            ret.RotateAngle = Math.Atan(a);// * 180 / Math.PI;
            ret.ScaleX = getDis(pointList[n - 1], pointList[0]) / ((n - 1) * keyWidth);
            ret.ScaleY = ret.ScaleX;
            ret.OffsetY = pointList[0].Y - 1.5 * ret.ScaleY * keyHeight;
            if (lr == 0)
            {
                ret.OffsetX = pointList[0].X - ret.ScaleX * keyWidth;
                ret.CenterX = keyWidth;
                ret.CenterY = 1.5 * keyHeight;
            } else
            {
                ret.OffsetX = pointList[0].X - ret.ScaleX * 6 * keyWidth;
                ret.CenterX = 6 * keyWidth;
                ret.CenterY = 1.5 * keyHeight;
            }
            return ret;
        }
        public static double getDis(Point pos1, Point pos2)
        {
            return Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
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
        public static void drawPoint(Canvas canvas, Point pos, int flag = 0)
        {
            Ellipse e = new Ellipse();
            e.Width = 5 + flag * 4;
            e.Height = 5 + flag* 4;
            e.Fill = Brushes.Yellow;
            if (flag == 1) e.Fill = Brushes.Red;
            canvas.Children.Add(e);
            Canvas.SetTop(e, pos.Y);
            Canvas.SetLeft(e, pos.X);
        }
    }
}
