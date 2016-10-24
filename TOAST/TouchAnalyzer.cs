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
        Dictionary<int, Point> screenPoints;
        public event RegisterHandPositionHandler registerHandPosition;
        public TouchAnalyzer()
        {
            screenPoints = new Dictionary<int, Point>();
        }
        public void touchDown(Point pos, int id)
        {
            Debug.Assert(!screenPoints.ContainsKey(id));
            screenPoints.Add(id, pos);
            if (screenPoints.Count == 8)
            {
                //register two hand
                var sortedPoints = from t in screenPoints orderby t.Value.X select t.Value;
                PositionParams left = Config
                registerHand(null);

            }
        }
        public void touchMove(Point pos, int id)
        {
            Debug.Assert(screenPoints.ContainsKey(id));
        }
        public void touchUp(Point pos, int id)
        {
            Debug.Assert(screenPoints.ContainsKey(id));
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
