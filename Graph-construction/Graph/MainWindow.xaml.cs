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

        List<int> schema = new List<int>{ 3, 3, 3, 2, 4, 4, 5, 3, 4 };
        List<Node> nodes;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            Init();

            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0,0,0,0,500);
        }

        private void Init()
        {
            schema = GetRandomSchema();
            nodes = new List<Node>();

            rtbConsole.Document.Blocks.Clear();
            rtbConsole.AppendText("Schema is [" + String.Join(",", schema) + "]\n");

            for (int i = 0; i < schema.Count; ++i)
            {
                double x = rnd.Next(20, width - 20);
                double y = rnd.Next(20, height - 20);

                Node node = new Node(schema[i]);
                node.pos = new Point(x, y);
                node.id = i;
                node.ConnectedNotify += (str) =>
                {
                    rtbConsole.AppendText(str);
                };

                nodes.Add(node);
            }

            foreach(var n in nodes)
            {
                n.SetConnections(nodes);
            }

            Drawing();
        }

        private List<int> GetRandomSchema()
        {
            List<int> result = new List<int>();

            int rnd_count = rnd.Next(5, 15);
            for (int i = 0; i < rnd_count; ++i)
            {
                int rnd_value = rnd.Next(1, 10);
                result.Add(rnd_value);
            }

            return result;
        }

        private void Tick(object sender , EventArgs e) => Drawing();

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                foreach (Node node in nodes)
                {
                    node.DrawingEdges(dc, nodes);
                }

                foreach (Node node in nodes)
                {
                    node.DrawingNodes(dc);
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Drawing();
        }
    }
}
