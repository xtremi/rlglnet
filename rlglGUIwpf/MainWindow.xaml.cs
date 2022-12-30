using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Forms;
using rlglnet;


namespace rlglGUIwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        rlglnet.rlglBaseApp rlglBaseApp;

        public MainWindow()
        {
            InitializeComponent();
            RlglInit(); 
        }



        void RlglInit()
        {
            rlglBaseApp = new rlglnet.rlglBaseApp(); 
            rlglBaseApp.InitWindow(new GlmNet.vec2(400, 200));
            SizeChanged += MainWindowSizedChanged;
            LocationChanged += MainWindowMoved;

            CompositionTarget.Rendering += RlglLoop;

        }

        private void MainWindowMoved(object sender, EventArgs e)
        {
            adjustGLFWwindowPosition();
        }

        private void MainWindowSizedChanged(object sender, SizeChangedEventArgs e)
        {
            adjustGLFWwindowPosition();
        }


        Point RealPixelsToWpf(Point p)
        {
            Matrix transform = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            //transform.Invert();
            return transform.Transform(p);
        }
        private void adjustGLFWwindowPosition()
        {
            Point topLeftPosition = new Point(Left + Width, Top);
            topLeftPosition = RealPixelsToWpf(topLeftPosition);

            GLFW.Window glfwWindow = rlglBaseApp.window;
            int left, top, right, bottom;
            GLFW.Glfw.GetWindowFrameSize(glfwWindow, out left, out top, out right, out bottom);

            //Not sure if the "left" is the correct offset to align the two windows in x ("top" is correct for y alignment)
            GLFW.Glfw.SetWindowPosition(glfwWindow, (int)topLeftPosition.X - left, (int)topLeftPosition.Y + top);
        }

        private void RlglLoop(object sender, EventArgs e)
        {
            rlglBaseApp.loop();
        }

        void OnClickButtonStart(object sender, RoutedEventArgs e)
        {
            int tmp = 1;
        }


    }
}
