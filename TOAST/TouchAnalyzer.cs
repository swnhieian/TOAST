using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace TOAST
{
    delegate void RegisterHandPositionHandler(object sender, PositionParams leftPositionParams, PositionParams rightPositionParams);
    class TouchAnalyzer
    {
        Dictionary<int, TouchTracker> screenPoints;
        public event RegisterHandPositionHandler registerHandPosition;
        public TouchAnalyzer()
        {
            screenPoints = new Dictionary<int, TouchTracker>();
        }
        private TouchTracker getTouchTracker(int id)
        {
            Console.WriteLine("touch point num:{0}", screenPoints.Count);
            TouchTracker touchTracker = null;
            if (screenPoints.TryGetValue(id, out touchTracker))
            {
                return touchTracker;
            }
            if (touchTracker == null)
            {
                touchTracker = new TouchTracker();
                screenPoints.Add(id, touchTracker);
            }
            return touchTracker;
        }
        public void touchDown(Point pos, int id)
        {
            TouchTracker touchTracker = getTouchTracker(id);
            if (touchTracker == null) return;
            touchTracker.touchDown(pos);
            if (screenPoints.Count == 8)
            {
                //register two hand
                var sortedPoints = (from t in screenPoints orderby t.Value.Pos.X select t.Value.Pos).ToList();
                PositionParams left = Config.registerPosition(sortedPoints.Take(4).ToList(), 0);
                //sortedPoints.RemoveRange(0, 4);
                List<Point> rightList = new List<Point>();
                rightList.Add(sortedPoints[4]);
                rightList.Add(sortedPoints[5]);
                rightList.Add(sortedPoints[6]);
                rightList.Add(sortedPoints[7]);
                PositionParams right = Config.registerPosition(rightList, 1);
                registerHand(left, right);
            }
        }
        public void touchMove(Point pos, int id)
        {
            TouchTracker touchTracker = getTouchTracker(id);
            if (touchTracker == null) return;
            touchTracker.touchMove(pos);
        }
        public void touchUp(Point pos, int id)
        {
            TouchTracker touchTracker = getTouchTracker(id);
            if (touchTracker == null) return;
            touchTracker.touchUp(pos);
            screenPoints.Remove(id);
        }

        private void registerHand(PositionParams leftPosition, PositionParams rightPosition)
        {
            if (registerHandPosition!= null)
            {
                registerHandPosition(this, leftPosition, rightPosition);
            }
        }

    }
}
