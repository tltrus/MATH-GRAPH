using System;
using System.Collections.Generic;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            Init();

            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0,0,0,0, 10);
        }

        private void Init()
        {
            Graph = new Graph(rnd, width, height);
            nodes_count = 200;
            m = rnd.Next(1, 8);
            Graph.Initialization(nodes_num: nodes_count, m_param: m);

            rtbConsole.Document.Blocks.Clear();
            rtbConsole.AppendText("Graph construction / Mix of Barabási–Albert network model and nearest neighbor\nm parameter is " + m + "\nnodes number is " + nodes_count);

            Drawing();
        }

        private void Tick(object sender , EventArgs e)
        {
            Graph.SetLabel(rnd.Next(nodes_count));
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

        private void btnSetLabel_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
                timer.Start();
            else
                timer.Stop();

                //for (int i = 0; i < nodes_count; ++i)
                //{
                //    Graph.SetLabel(i);
                //}
                int i = rnd.Next(nodes_count);
            Graph.SetLabel(i);
            Drawing();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) => Init();
    }
}
