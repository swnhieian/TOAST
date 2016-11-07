using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOAST
{
    enum TouchType { Type, SwipeRight, SwipeLeft};
    class TouchTracker
    {
        List<Point> points;
        public TouchTracker()
        {
            points = new List<Point>();
        }
        public void touchDown(Point pos)
        {
            points.Add(pos);
        }
        public void touchMove(Point pos)
        {
            points.Add(pos);
        }
        public void touchUp(Point pos)
        {
            points.Add(pos);
        }
        public TouchType Type
        {
            get {
                Point firstP = points.First();
                Point lastP = points.Last();
                if (Config.getDis(firstP, lastP) > 100)
                {
                    if (firstP.X < lastP.X) return TouchType.SwipeRight;
                    return TouchType.SwipeLeft;
                }
                return TouchType.Type;
            }
        }
        public Point Pos
        {
            get { return points.Last(); }
        }

    }
}
