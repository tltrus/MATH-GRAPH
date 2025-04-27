using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace MathGraph
{
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        DispatcherTimer timer = new DispatcherTimer();
        int width, height;
        public static DrawingVisual visual;
        DrawingContext dc;

        Graph Graph;
        int nodes_count, m;
        Point? mouse;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            Init();

            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0,0,0,0, 300);
        }

        private void Init()
        {
            timer.Stop();
            
            Graph = new Graph(rnd, width, height);
            Graph.MessageNotify += msg =>
            {
                rtbConsole.AppendText(msg);
            };
            Graph.BestNotify += (msg) =>
            {
                timer.Stop();
                rtbConsole.AppendText(".\n=END=");
                rtbConsole.AppendText(".\nNearest node is " + msg);
            };
            nodes_count = 200;
            m = 4; // rnd.Next(1, 8);
            Graph.Initialization(nodes_num: nodes_count, m_param: m);

            rtbConsole.Document.Blocks.Clear();
            rtbConsole.AppendText("Navigable Small World (NSW) algorithm / Looking for nearest node to point q from any random node"
                                    + ".\n   m parameter is " + m 
                                    + ".\n   Nodes number is " + nodes_count
                                    + ".\n1. Click mouse button to set any point (q)"
                                    + ".\n2. Press button \"Find nearest node\"\n");

            Drawing();
        }

        private void Tick(object sender , EventArgs e)
        {
            int state = Graph.NSWDynamic();
            if (state == 3) Graph.SetQPoint((Point)mouse);

            Drawing();
        }

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                Graph.Draw(dc);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
            if (mouse is null) return;

            if (!timer.IsEnabled)
            {
                Graph.Reset();
                rtbConsole.AppendText(".\n=STARTING=");
                timer.Start();
            }
            else
                timer.Stop();
        }

        private void g_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mouse = e.GetPosition(g);
            Graph.SetQPoint((Point)mouse);
            Drawing();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) => Init();

    }
}
