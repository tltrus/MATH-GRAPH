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
        public static DrawingVisual visual;
        public DrawingContext dc;
        DispatcherTimer timer = new DispatcherTimer();

        List<int> path;
        public static int width, height;
        Graph Graph;
        BFS BFS;
        DFS DFS;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            Init();
            Drawing(path);
        }

        private void Init()
        {
            rtbConsole.Document.Blocks.Clear();

            path = new List<int>();
            int num = rnd.Next(4, 15);
            Graph = new Graph(num, rnd);

            rtbConsole.AppendText("\rAdjacency matrix:\r\r");
            rtbConsole.AppendText(Graph.GetMatrix());
            rtbConsole.AppendText("\rNodes: " + Graph.N + "\r");

            // BFS

            rtbConsole.AppendText("\rBREADTH FIRST SEARCH algorithm (BFS)\r", "Blue");
            BFS = new BFS(Graph, new Pen(Brushes.Blue, 8));
            BFS.Calculation();
            path = BFS.GetPathList(0, 3);
            string str = "path from " + 0 + " to " + 3 + " is [ ";
            string pathStr = String.Join(", ", path);
            str += pathStr + " ]\r";
            rtbConsole.AppendText(str);
            rtbConsole.AppendText(BFS.ParentToString());

            // DFS

            rtbConsole.AppendText("\rDEPTH FIRST SEARCH algorithm (DFS)\r", "LimeGreen");
            DFS = new DFS(Graph, new Pen(Brushes.LimeGreen, 5));
            DFS.Calculation(0);
            path = DFS.GetPathList(0, 3);
            str = "path from " + 0 + " to " + 3 + " is [ ";
            pathStr = String.Join(", ", path);
            str += pathStr + " ]\r";
            rtbConsole.AppendText(str);
            rtbConsole.AppendText(DFS.ParentToString());
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Drawing(path);
        }

        private void Drawing(List<int> path = null)
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                Graph.Drawing(dc, path);
                BFS.Drawing(dc);
                DFS.Drawing(dc);

                dc.Close();
                g.AddVisual(visual);
            }
        }

    }
}
