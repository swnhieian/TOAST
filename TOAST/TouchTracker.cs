using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TOAST
{
    enum TouchType { Type, SwipeRight, SwipeLeft, SwipeLeftLong};
    class TouchTracker
    {
        //List<Point> points;
        //Item1 time tick Item2 point pos
        List<Tuple<long, Point>> points;
        int select = -1;
        Point lastPos;
        long lastTick;
        public TouchTracker()
        {
            points = new List<Tuple<long, Point>>();
        }
        public void touchDown(Point pos)
        {
            points.Add(new Tuple<long, Point>(DateTime.Now.Ticks, pos));
        }
        public void touchMove(Point pos)
        {
            Point firstPos = points.First().Item2;
            points.Add(new Tuple<long, Point>(DateTime.Now.Ticks, pos));
            if (Select == -1 && Config.getDis(pos, firstPos) > 45 && pos.X > firstPos.X)
            {
                Select = 0;
                lastPos = pos;
                lastTick = DateTime.Now.Ticks;
            } else if (Select >= 0)
            {
                long tickNow = DateTime.Now.Ticks;
                int delta = (int)((pos.X - lastPos.X) / 20);
                Console.WriteLine(tickNow - lastTick);
                if (tickNow-lastTick>0 && tickNow - lastTick < 100000)
                {
                    delta = (int)((pos.X - lastPos.X) / (tickNow - lastTick) * 50000);
                }
                //Console.WriteLine((pos.X - lastPos.X) / (tickNow - lastTick));
                if (Math.Abs(delta) > 0)
                {
                    Select += delta;
                    lastPos = pos;
                    lastTick = tickNow;
                }               
                if (Select < 0) Select = 0;
                if (Select >= Config.candidateNum) Select = Config.candidateNum - 1;
                if (tickNow - points.First().Item1 < 1000000) Select = 0;//100 ms
            }
        }
        public void touchUp(Point pos)
        {
            points.Add(new Tuple<long, Point>(DateTime.Now.Ticks, pos));
        }
        public TouchType Type
        {
            get {
                Point firstP = points.First().Item2;
                Point lastP = points.Last().Item2;
                if (Config.getDis(firstP, lastP) > 200 && firstP.X > lastP.X) return TouchType.SwipeLeftLong;
                if (Config.getDis(firstP, lastP) > 50)
                {
                    if (firstP.X < lastP.X) return TouchType.SwipeRight;
                    return TouchType.SwipeLeft;
                }
                return TouchType.Type;
            }
        }
        public Point Pos
        {
            get { return points.Last().Item2; }
        }

        public int Select
        {
            get
            {
                return select;
            }

            set
            {
                select = value;
            }
        }
    }
}
