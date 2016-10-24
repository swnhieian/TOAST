using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOAST
{
    class ParameterFitter
    {
        //'a' to 'z' line 0 1 2 on the keyboard
        private int[] lineNo = new int[] { 1, 2, 2, 1, 0, 1, 1, 1, 0, 1, 1, 1, 2, 2, 0, 0, 0, 0, 1, 0, 0, 2, 0, 2, 0, 2 };
        Keyboard kbd;
        public void select(List<Point> points, Tuple<string, double, string> cand)
        {

        }

        //using approximate approach
        public PositionParams fitParameter(List<Tuple<char, Point>> list)
        {
            List<Point>[] lines = new List<Point>[3];
            for (int i = 0; i < 3; i++) lines[i] = new List<Point>();
            foreach (var item in list)
            {
                lines[lineNo[item.Item1]].Add(item.Item2);
            }
            double C = 0;
            double count = 0;
            for (int i=0; i<3; i++)
            {
                if (lines[i].Count > 5)
                {
                    C += (lines[i].Count * Math.Atan(Config.fitLine(lines[i]).Item1));
                    count += lines[i].Count;
                }
            }
            C /= count;
            ////////////////////////////////////calculate A and B
            double sumx = 0;
            double sumy = 0;
            double sumxx = 0;
            double sumyy = 0;
            double suma = 0;
            double sumb = 0;
            double sumxacbs = 0;
            double sumybcas = 0;
            double n = list.Count;
            for (int i=0; i<n; i++)
            {
                sumx += kbd.letterPosX[list[i].Item1];
                sumxx += kbd.letterPosX[list[i].Item1] * kbd.letterPosX[list[i].Item1];
                sumy += kbd.letterPosY[list[i].Item1];
                sumyy += kbd.letterPosY[list[i].Item1] * kbd.letterPosY[list[i].Item1];
                suma += list[i].Item2.X;
                sumb += list[i].Item2.Y;
                sumxacbs += (kbd.letterPosX[list[i].Item1]*(list[i].Item2.X * Math.Cos(C) + list[i].Item2.Y * Math.Sin(C)));
                sumybcas += (kbd.letterPosY[list[i].Item1]*(list[i].Item2.Y * Math.Cos(C) - list[i].Item2.X * Math.Sin(C)));
            }
            double Ax = sumxacbs - sumx / n * (suma * Math.Cos(C) + sumb * Math.Sin(C));
            Ax /= (sumxx - sumx * sumx / n);
            double Ay = sumybcas + sumy / n * (suma * Math.Sin(C) - sumb * Math.Cos(C));
            Ay /= (sumyy - sumy * sumy / n);
            double Bx = suma / n - sumx * Math.Cos(C) / n * Ax + sumy * Math.Sin(C) / n * Ay;
            double By = sumb / n - sumx * Math.Sin(C) / n * Ax - sumy * Math.Cos(C) / n * Ay;
            return new PositionParams(Bx, By, Ax, Ay, C);

        }
    }
}
