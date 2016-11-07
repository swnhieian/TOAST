using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TOAST
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Keyboard kbd;
        TouchAnalyzer touchAnalyzer = new TouchAnalyzer();
        TextBlock[] candidates = new TextBlock[Config.candidateNum];
        RichTextBox inputTextBox = new RichTextBox();
        Paragraph inputParagraph = new Paragraph();

        private List<Tuple<string, double, string>> candidateList = new List<Tuple<string, double, string>>();
        int selectedCandidateNo = 0;



        public MainWindow()
        {
            double angle, x, y;
            angle = 90;
            x = 20;
            y = 30;
            double a = 0;
            double b = 0;
            RotateTransform rot = new RotateTransform(angle, a, b);
            Point p = new Point(x, y);
            Point transP = rot.Transform(p);
            Point calcP = new Point(a + (x - a) * Math.Cos(angle) - (y - b) * Math.Sin(angle), b + (x - a) * Math.Sin(angle) + (y - b) * Math.Cos(angle));
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            
            mainCanvas.Background = Brushes.Red;
            mainCanvas.PreviewMouseUp += MainCanvas_MouseUp;
            mainCanvas.TouchDown += MainCanvas_TouchDown;
            mainCanvas.TouchMove += MainCanvas_TouchMove;
            mainCanvas.TouchUp += MainCanvas_TouchUp;
            mainCanvas.KeyUp += MainCanvas_KeyUp;
            this.KeyUp += MainCanvas_KeyUp;
            kbd = new Keyboard();
            kbd.CandidateChange += Kbd_CandidateChange;
            kbd.LeftSpace += Kbd_LeftSpace;
            kbd.RightSpace += Kbd_RightSpace;
            kbd.drawKeyboard(mainCanvas);
            touchAnalyzer.registerHandPosition += TouchAnalyzer_registerHandPosition;
            touchAnalyzer.type += TouchAnalyzer_type;
            touchAnalyzer.swipeLeft += TouchAnalyzer_swipeLeft;
            touchAnalyzer.swipeRight += TouchAnalyzer_swipeRight;

            for (int i = 0; i < Config.candidateNum; i++)
            {
                candidates[i] = new TextBlock()
                {
                    Width = 200,
                    Height = 50,
                    FontSize = 20,
                    Background = Brushes.Black,
                    Foreground = Brushes.White
                };
                mainCanvas.Children.Add(candidates[i]);
                Canvas.SetTop(candidates[i], Config.inputTextBoxHeight);
                Canvas.SetLeft(candidates[i], candidates[i].Width * i + 20 * i);
                candidates[i].TouchDown += ((sender, args) =>
                {
                    //Console.WriteLine("candidate touch down");
                    //Console.WriteLine("select one word:{0},{1}", args.GetTouchPoint(mainCanvas).Position.X, args.GetTouchPoint(mainCanvas).Position.Y);
                    TextBlock tb = args.Source as TextBlock;
                    int no = candidates.ToList().IndexOf(tb);
                    select(no);
                    args.Handled = true;
                });
                candidates[i].TouchMove += ((sender, args) =>
                {
                    //Console.WriteLine("candidate touch move");
                    args.Handled = true;
                });
                candidates[i].TouchUp += ((sender, args) =>
                {
                    //Console.WriteLine("candidate touch up");
                    args.Handled = true;
                });
            }


            FlowDocument flowDoc = new FlowDocument();
            inputTextBox.Document = flowDoc;
            inputParagraph.FontFamily = new FontFamily("Courier New");
            inputParagraph.FontSize = Config.inputFontSize;
            flowDoc.Blocks.Add(inputParagraph);
            inputTextBox.Width = Config.inputTextBoxWidth;
            inputTextBox.Height = Config.inputTextBoxHeight ;
            mainCanvas.Children.Add(inputTextBox);
            Canvas.SetTop(inputTextBox, 0);
        }
        private void select(int no)
        {
            inputParagraph.Inlines.Add(new Run(candidates[no].Text + " "));
            kbd.select(candidateList[no]);
            selectedCandidateNo = 0;
            renderCandidate();
        }

        private void Kbd_RightSpace(object sender)
        {
            if (candidateList.Count == 0) return;
            select(selectedCandidateNo);
        }

        private void Kbd_LeftSpace(object sender)
        {
            if (candidateList.Count == 0) return;
            selectedCandidateNo = (selectedCandidateNo + 1) % candidateList.Count;
            renderCandidate();
        }

        private void TouchAnalyzer_swipeRight(object sender)
        {
            select(selectedCandidateNo);
        }

        private void TouchAnalyzer_swipeLeft(object sender)
        {
            if (kbd.InputLength > 0)
            {
                kbd.reset();
            } else
            {
                /// to do : 
                /// delete input word
                inputParagraph.Inlines.Remove(inputParagraph.Inlines.LastInline);
            }
            selectedCandidateNo = 0;
            renderCandidate();
        }

        private void MainCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                inputParagraph.Inlines.Clear();
            }

        }

        private void TouchAnalyzer_type(object sender, Point pos)
        {
            kbd.type(pos);
        }

        private void TouchAnalyzer_registerHandPosition(object sender, PositionParams leftPositionParams, PositionParams rightPositionParams)
        {
            Console.WriteLine("here register!!!!!!");
            kbd.LeftKeyboardParams = leftPositionParams;
            kbd.RightKeyboardParams = rightPositionParams;
            //Console.WriteLine("leftPositionParams:{0}, {1}", leftPositionParams.RotateAngle, leftPositionParams.RotTransform.Angle);
            //Console.WriteLine("rightPositionParams:{0}, {1}", rightPositionParams.RotateAngle, rightPositionParams.RotTransform.Angle);
            kbd.drawKeyboard(mainCanvas);
            kbd.reset();
        }

        private void MainCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            Canvas _canvas = (Canvas)sender as Canvas;
            if (_canvas != null && e.TouchDevice.Captured == _canvas)
            {
                int id = e.TouchDevice.Id;
                Point pos = e.GetTouchPoint(mainCanvas).Position;
                touchAnalyzer.touchUp(pos, id);
                _canvas.ReleaseTouchCapture(e.TouchDevice);
            }
        }

        private void MainCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            Canvas _canvas = (Canvas)sender as Canvas;
            if (_canvas != null)
            {
                int id = e.TouchDevice.Id;
                Point pos = e.GetTouchPoint(mainCanvas).Position;
                touchAnalyzer.touchMove(pos, id);
            }            
        }

        private void MainCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            Canvas _canvas = (Canvas)sender as Canvas;
            if (_canvas != null)
            {
                e.TouchDevice.Capture(_canvas);
                int id = e.TouchDevice.Id;
                Point pos = e.GetTouchPoint(mainCanvas).Position;
                touchAnalyzer.touchDown(pos, id);
            }            
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //kbd.type(e.GetPosition(mainCanvas));
        }

        private void Kbd_CandidateChange(object sender, CandidateChangeEventArgs args)
        {
            candidateList.Clear();
            int t = 0;
            int len = args.CandidateList.Count;
            while (t < len && candidateList.Count < Config.candidateNum)
            {
                string word = args.CandidateList[t].Item1;
                if (!candidateList.Exists(x => x.Item1 == word))
                {
                    candidateList.Add(args.CandidateList[t]);
                }
                t++;
            }
            for (int i = 0; i < candidateList.Count; i++)
            {
                candidates[i].Text = candidateList[i].Item1;
                candidates[i].Visibility = Visibility.Visible;
            }
            for (int i = candidateList.Count; i<Config.candidateNum; i++)
            {
                candidates[i].Text = "";
                candidates[i].Visibility = Visibility.Hidden;
            }
            renderCandidate();
        }
        private void renderCandidate()
        {
            for (int i=0; i<candidateList.Count; i++)
            {
                candidates[i].Foreground = (i == selectedCandidateNo) ? Config.selectedCandidateForeground : Config.candidateForeground;
                candidates[i].Background = (i == selectedCandidateNo) ? Config.selectedCandidateBackground : Config.candidateBackground;

            }
        }

    }
}
