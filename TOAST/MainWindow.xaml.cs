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

namespace TOAST
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Keyboard kbd;
        TouchAnalyzer touchAnalyzer = new TouchAnalyzer();
        TextBlock[] candidates = new TextBlock[5];
        public MainWindow()
        {
            InitializeComponent();
            for (int i=0; i<5; i++)
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
                Canvas.SetTop(candidates[i], 0);
                Canvas.SetLeft(candidates[i], candidates[i].Width * i + 20 * i);
            }
            mainCanvas.Background = Brushes.Red;
            mainCanvas.PreviewMouseUp += MainCanvas_MouseUp;
            mainCanvas.TouchDown += MainCanvas_TouchDown;
            mainCanvas.TouchMove += MainCanvas_TouchMove;
            mainCanvas.TouchUp += MainCanvas_TouchUp;
            kbd = new Keyboard();
            kbd.CandidateChange += Kbd_CandidateChange;
            kbd.drawKeyboard(mainCanvas);
            touchAnalyzer.registerHandPosition += TouchAnalyzer_registerHandPosition;
        }

        private void TouchAnalyzer_registerHandPosition(object sender, PositionParams leftPositionParams, PositionParams rightPositionParams)
        {
            kbd.LeftKeyboardParams = leftPositionParams;
            kbd.RightKeyboardParams = rightPositionParams;
        }

        private void MainCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            int id = e.TouchDevice.Id;
            Point pos = e.GetTouchPoint(mainCanvas).Position;
            touchAnalyzer.touchUp(pos, id);
        }

        private void MainCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            int id = e.TouchDevice.Id;
            Point pos = e.GetTouchPoint(mainCanvas).Position;
            touchAnalyzer.touchMove(pos, id);
        }

        private void MainCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            int id = e.TouchDevice.Id;
            Point pos = e.GetTouchPoint(mainCanvas).Position;
            touchAnalyzer.touchDown(pos, id);
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            kbd.type(e.GetPosition(mainCanvas));
        }

        private void Kbd_CandidateChange(object sender, CandidateChangeEventArgs args)
        {
            for (int i = 0; i < Math.Min(5, args.CandidateList.Count); i++)
                candidates[i].Text = args.CandidateList[i].Item1;
        }

    }
}
