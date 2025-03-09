using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;


namespace MathGraph
{
    public partial class MainWindow : Window
    {
        public static Random rnd = new Random();
        public static DrawingVisual visual;
        DrawingContext dc;
        DispatcherTimer timer = new DispatcherTimer();

        Graph graph;

        public static int width, height;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            Init();

            graph.AddEdge(src: 0, dest: 1, weight: -1);
            graph.AddEdge(src: 0, dest: 2, weight: 4);
            graph.AddEdge(src: 1, dest: 2, weight: 3);
            graph.AddEdge(src: 1, dest: 3, weight: 2);
            graph.AddEdge(src: 1, dest: 4, weight: 2);
            graph.AddEdge(src: 3, dest: 2, weight: 5);
            graph.AddEdge(src: 3, dest: 1, weight: 1);
            graph.AddEdge(src: 4, dest: 5, weight: -2);
            graph.AddEdge(src: 5, dest: 6, weight: 1);

            //
            //graph.AddEdge(src: 0, dest: 4, weight: 5);
            //graph.AddEdge(src: 1, dest: 2, weight: 8);
            //graph.AddEdge(src: 1, dest: 3, weight: 8);
            //graph.AddEdge(src: 1, dest: 4, weight: 8);
            //graph.AddEdge(src: 1, dest: 5, weight: 10);
            //graph.AddEdge(src: 2, dest: 0, weight: 1);
            //graph.AddEdge(src: 3, dest: 6, weight: -2);
            //graph.AddEdge(src: 4, dest: 0, weight: 10);
            //graph.AddEdge(src: 4, dest: 6, weight: 4);
            //graph.AddEdge(src: 5, dest: 2, weight: 8);
            //graph.AddEdge(src: 5, dest: 4, weight: 5);
            //graph.AddEdge(src: 6, dest: 0, weight: 10);
            //graph.AddEdge(src: 6, dest: 3, weight: 0);
            //graph.AddEdge(src: 6, dest: 4, weight: 8);
            //graph.AddEdge(src: 0, dest: 6, weight: 2);

            Drawing();
        }

        private void Init()
        {
            rtbConsole.Document.Blocks.Clear();
            graph = new Graph();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            graph.SetConnections();
            Drawing();
        }

        private void Tick(object sender, EventArgs e) => Drawing();

        private void Drawing()
        {
            bool isPath = graph.BellmanFord(graph, 0);

            // Вывод весов
            rtbConsole.AppendText("Weights:\n");
            rtbConsole.AppendText(graph.PrintWeights());

            // Вывод расстояний от источника [A] до каждой вершины графа
            rtbConsole.AppendText("Path from src [0] to each vertex:\n");
            rtbConsole.AppendText(graph.PrintPathes());

            // Вывод короткого пути от источника до цели
            var path_list = graph.PrintPath(0, 3); 
            path_list.Reverse();
            var path_str = String.Join(",", path_list);
            if (!isPath) path_str = "neg. weight cycle";
            if (path_list.Contains(-1)) path_str = "no path";
            lb1.Content = "Shortest path from 0 to 3 --> [ " + path_str + " ]\n";

            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                graph.Drawing(dc);
                if (!path_list.Contains(-1) && isPath is true)
                    graph.DrawingPath(dc, path_list);

                dc.Close();
                g.AddVisual(visual);
            }
        }
    }
}
