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
    public class CandidateChangeEventArgs: EventArgs
    {
        private List<Tuple<string, double, string>> candidateList;
        public CandidateChangeEventArgs(List<Tuple<string, double, string>> candidateList) {
            this.CandidateList = candidateList;
        }
        public List<Tuple<string, double, string>> CandidateList
        {
            get
            {
                return candidateList;
            }

            set
            {
                candidateList = value;
            }
        }
    }
    public delegate void CandidateChangeHandler(object sender, CandidateChangeEventArgs args);
    class Keyboard
    {
        public event CandidateChangeHandler CandidateChange;
        //
        private PositionParams leftKeyboardParams, rightKeyboardParams;

        private WordPredictor wordPredictor;
        private List<Point> currentInputPoints;
        // Calculated from Standard Keyboard Image
        //public double[] letterPosX = new double[] { 743.5, 928.5, 846.5, 825.5, 806.5, 866.5, 907.5, 948.5, 1011.5, 989.5, 1030.5, 1071.5,
        //    1010.5, 969.5, 1052.5, 1093.5,724.5, 847.5, 784.5, 888.5, 970.5, 887.5, 765.5, 805.5, 929.5, 764.5};
        public double[] letterPosX = new double[] { 39.5, 224.5, 142.5, 121.5, 102.5, 162.5, 203.5, 244.5, 307.5, 285.5, 326.5, 367.5, 306.5, 265.5, 348.5, 389.5, 20.5, 143.5, 80.5, 184.5, 266.5, 183.5, 61.5, 101.5, 225.5, 60.5 };
        //public double[] letterPosY = new double[] { 616.4, 660.4, 660.4, 616.4, 572.4, 616.4, 616.4, 616.4, 572.4, 616.4, 616.4, 616.4,
        //    660.4, 660.4, 572.4, 572.4, 572.4, 572.4, 616.4, 572.4, 572.4, 660.4, 572.4, 660.4, 572.4, 660.4 };
        public double[] letterPosY = new double[] { 66.0, 110.0, 110.0, 66.0, 22.0, 66.0, 66.0, 66.0, 22.0, 66.0, 66.0, 66.0, 110.0, 110.0, 22.0, 22.0, 22.0, 22.0, 66.0, 22.0, 22.0, 110.0, 22.0, 110.0, 22.0, 110.0 };
        public PositionParams[] LeftRightPositionParams
        {
            get { return new PositionParams[2] { leftKeyboardParams, rightKeyboardParams }; }
        }
        internal PositionParams LeftKeyboardParams
        {
            get
            {
                return leftKeyboardParams;
            }

            set
            {
                leftKeyboardParams = value;
            }
        }

        internal PositionParams RightKeyboardParams
        {
            get
            {
                return rightKeyboardParams;
            }

            set
            {
                rightKeyboardParams = value;
            }
        }

        public Keyboard()
        {
            LeftKeyboardParams = new PositionParams(0, 0, 1, 1, 20, 0, 0);
            RightKeyboardParams = new PositionParams(300, 100, 1, 1, 0, 0, 0);
            wordPredictor = new WordPredictor(this);
            currentInputPoints = new List<Point>();

        }

        public Keyboard(PositionParams leftKeyboardParams, PositionParams rightKeyboardParams)
        {
            this.LeftKeyboardParams = leftKeyboardParams;
            this.RightKeyboardParams = rightKeyboardParams;
        }

        public void type(Point pos)
        {
           /* double leftDis = leftKeyboard.dist(leftKeyboardParams, pos);
            double rightDis = rightKeybaord.dist(rightKeyboardParams, pos);
            PositionParams actualParam = leftDis < rightDis ? leftKeyboardParams : rightKeyboardParams;
            Point typePos = actualParam.inverseTransform(pos);*/
            currentInputPoints.Add(pos);
            updateInput();
        }
        public void reset()
        {
            currentInputPoints.Clear();
            updateInput();
        }

        public void updateInput()
        {
            List<Tuple<string, double, string>> candidates = wordPredictor.predict(currentInputPoints);
            candidateChange(candidates);
        }


        protected virtual void onCandidateChange(CandidateChangeEventArgs e)
        {
            if (CandidateChange != null)
            {
                CandidateChange(this, e);
            }
        }
        public void candidateChange(List<Tuple<string, double, string>> candidates)
        {
            onCandidateChange(new CandidateChangeEventArgs(candidates));
        }
        Rectangle[] keys = null;
        public void drawKeyboard(Canvas canvas)
        {
            if (keys == null) keys = new Rectangle[26];
            string[] handCode = { "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "1", "1", "1",
        "1", "1", "0", "0", "0", "0", "1", "0", "0", "0", "1", "0"};
            for (int i=0; i<26; i++)
            {
                int lr = Convert.ToInt32(handCode[i]);
                Point oriPos = new Point(letterPosX[i], letterPosY[i]);
                PositionParams positionParams = LeftRightPositionParams[lr];
                Point pos = positionParams.transform(oriPos);
                Rectangle rect = keys[i];
                if (rect == null)
                {
                    rect = new Rectangle()
                    {
                        Width = 41 * positionParams.ScaleX,
                        Height = 44 * positionParams.ScaleY,
                        Stroke = Brushes.Black,
                        Margin = new Thickness(0),
                        Fill = Brushes.Blue,
                        StrokeThickness = 0.5
                    };
                    keys[i] = rect;
                    canvas.Children.Add(rect);
                } else
                {
                    rect.Width = 41 * positionParams.ScaleX;
                    rect.Height = 44 * positionParams.ScaleY;
                }
                Canvas.SetTop(rect, pos.Y);
                Canvas.SetLeft(rect, pos.X);
                //positionParams.RotTransform = new RotateTransform(positionParams.RotateAngle);
                //rect.RenderTransform = positionParams.RotTransform;
                rect.RenderTransform = new RotateTransform(positionParams.RotateAngle);
                Console.WriteLine("in render::{0}", positionParams.RotTransform.Angle);
            }
        }

    }
}
