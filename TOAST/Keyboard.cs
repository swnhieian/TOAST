using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public delegate void SpaceHandler(object sender);
    enum TypeClass { Type, LeftSpace, RightSpace};
    class Keyboard
    {
        public event CandidateChangeHandler CandidateChange;
        public event SpaceHandler LeftSpace;
        public event SpaceHandler RightSpace;
        //
        private PositionParams leftKeyboardParams, rightKeyboardParams;
        private ParameterFitter parameterFittter;

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
        public int InputLength
        {
            get
            {
                return currentInputPoints.Count;
            }
        }

        public Keyboard()
        {
            LeftKeyboardParams = new PositionParams(0, 150, 1, 1, 0, 0, 0);
            RightKeyboardParams = new PositionParams(300, 150, 1, 1, 0, 0, 0);
            wordPredictor = new WordPredictor(this);
            currentInputPoints = new List<Point>();
            parameterFittter = new ParameterFitter(this);
        }


        public Keyboard(PositionParams leftKeyboardParams, PositionParams rightKeyboardParams)
        {
            this.LeftKeyboardParams = leftKeyboardParams;
            this.RightKeyboardParams = rightKeyboardParams;
        }
        public TypeClass getTypeClass(Point pos)
        {
            Point actualLP = leftKeyboardParams.inverseTransform(pos);
            Point actualRP = rightKeyboardParams.inverseTransform(pos);
            bool isL = false, isR = false;
            if (actualLP.X < letterPosX['n'-'a']-0.5*Config.keyWidth && actualLP.Y > letterPosY['n'-'a'] + 0.5*Config.keyHeight)
            {
                isL = true;
            }
            if (actualRP.X >= letterPosX['n'-'a']-0.5*Config.keyWidth && actualRP.Y > letterPosY['n'-'a'] + 0.5*Config.keyHeight)
            {
                isR = true;
            }
            if (isL && !isR)
            {
                return TypeClass.LeftSpace;
            }
            if (isR && !isL)
            {
                return TypeClass.RightSpace;
            }
            return TypeClass.Type;
        }

        public void type(Point pos)
        {
            switch (getTypeClass(pos))
            {
                case TypeClass.Type:
                    currentInputPoints.Add(pos);
                    updateInput();
                    break;
                case TypeClass.LeftSpace:
                    if (LeftSpace != null) LeftSpace(this);
                    break;
                case TypeClass.RightSpace:
                    if (RightSpace != null) RightSpace(this);
                    break;
                default:
                    break;
            }            
        }
        public void select(Tuple<string, double, string> cand)
        {
            Debug.Assert(cand.Item1.Length == currentInputPoints.Count);
            parameterFittter.select(currentInputPoints, cand);
            this.reset();
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
            temp_canvas = canvas;
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
                        Width = Config.keyWidth * positionParams.ScaleX,
                        Height = Config.keyHeight * positionParams.ScaleY,
                        Stroke = Brushes.Black,
                        Margin = new Thickness(0),
                        Fill = Brushes.Blue,
                        StrokeThickness = 0.5
                    };
                    keys[i] = rect;
                    canvas.Children.Add(rect);
                } else
                {
                    rect.Width = Config.keyWidth * positionParams.ScaleX;
                    rect.Height = Config.keyHeight * positionParams.ScaleY;
                }
                Canvas.SetTop(rect, pos.Y);
                Canvas.SetLeft(rect, pos.X);
                //positionParams.RotTransform = new RotateTransform(positionParams.RotateAngle);
                //rect.RenderTransform = positionParams.RotTransform;
                rect.RenderTransform = new RotateTransform(positionParams.RotateAngle*180/Math.PI);
            }
        }
        public Canvas temp_canvas = null;
        public void drawPoint(Canvas canvas, Point pos)
        {
            Ellipse e = new Ellipse();
            e.Width = 5;
            e.Height = 5;
            e.Fill = Brushes.Yellow;
            temp_canvas.Children.Add(e);
            Canvas.SetTop(e, pos.Y);
            Canvas.SetLeft(e, pos.X);           
        }

    }
}
