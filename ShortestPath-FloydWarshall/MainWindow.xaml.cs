using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;


namespace MathGraph
{
    /*
     Алгоритм Флойда (алгоритм Флойда–Уоршелла) 
     — алгоритм нахождения длин кратчайших путей между всеми парами вершин во взвешенном ориентированном графе. 
     Работает корректно, если в графе нет циклов отрицательной величины, а в случае, когда такой цикл есть, позволяет найти хотя бы один такой цикл. 
     Алгоритм работает за Θ(n3) времени и использует Θ(n2) памяти. Разработан в 1962 году.

     Код взят с сайте английской википедии  
     https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm
    */

    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        DrawingVisual visual;
        DrawingContext dc;
        DispatcherTimer timer = new DispatcherTimer();

        int INF = 99999; // бесконечность
        
        int[,] M;       // Матрица смежности направленного взвешенного графа
        int[,] dist;     
        int[,] next;    

        List<int> path = new List<int>();
        List<Point> nodes;
        int width, height, S, n;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            Init();
            Algoritm();

            // Из 0 в 2
            path = GetPath(0, 2);
            lb1.Content = "From 0 to 2:   [" + String.Join(" ", path) + "]";

            Drawing(path);
        }

        private void Init()
        {
            rtbConsole.Document.Blocks.Clear();

            rtbConsole.AppendText("Adjacency matrix:\r\r");

            n = 7; // nodes num
            
            // Add nodes
            nodes = new List<Point>();

            for (int i = 0; i < n; ++i)
            {
                double x = rnd.Next(width);
                double y = rnd.Next(height);

                Point node = new Point(x, y);
                nodes.Add(node); 
            }

            // Adjacency matrix
            M = new int[n, n];
            int s = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (i == j) continue;

                    if (rnd.NextDouble()  < 0.6)
                    {
                        s = (int)Math.Sqrt((nodes[j].X - nodes[i].X) * (nodes[j].X - nodes[i].X) + (nodes[j].Y - nodes[i].Y) * (nodes[j].Y - nodes[i].Y));
                        M[i, j] = s;
                        M[j, i] = s;
                    }
                    else
                    {
                        M[i, j] = INF;
                        M[j, i] = INF;
                    }
                }
            }

            // Draw Adjacency matrix
            for (int i = 0; i < n; i++)
            {
                var str = "";

                for (int j = 0; j < n; j++)
                {
                    var value = "";

                    if (M[i, j] == INF)
                    {
                        value = "inf";
                    }
                    else
                        value = M[i, j].ToString();

                    str += value + "\t";
                }

                rtbConsole.AppendText(str + "\r");
            }

            //// Матрица смежности направленного взвешенного графа
            //M = new int[,]   {
            //                            {0,     1,      6,  INF },
            //                            {INF,   0,      4,  1 },
            //                            {INF,   INF,    0,  INF },
            //                            {INF,   INF,    1,  0 },
            //                        };

            dist = new int[n, n]; // создаем вторую матрицу, которую будем менять
            next = new int[n, n];


            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    dist[i, j] = M[i, j];
                    next[i, j] = j;
                }
        }

        // Floyd–Warshall algorithm
        private void Algoritm()
        {
            for (int k = 0; k < n; ++k) //текущая вершина, используемая для улучшения
            {
                for (int i = 0; i < n; ++i)
                    for (int j = 0; j < n; ++j)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            //Массив dist содержит длины кратчайших путей между всеми парами вершин
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k]; // спец массив, хранящий вершины, по которым помжно отследить предков. По нему ищется маршрут
                        }
                    }
            }
        }

        // Вычисление маршрута из матрицы next
        private List<int> GetPath(int u, int v)
        {
            List<int> path = new List<int>();

            if (next[u, v] == null) return path; // !!!! Нужно подумать как исправить ЭТО

            path.Add(u);
            int u_ = u;

            while(u_ != v)
            {
                u_ = next[u_, v];
                path.Add(u_);

                // return if no connection between u and v
                if (u_ == v && M[u, v] == INF && path.Count == 2)
                {
                    path = new List<int>();
                    path.Add(-1);
                    return path;
                }
            }
            return path;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Algoritm();

            // Из 0 в 5
            path = GetPath(0, 5);
            lb1.Content = "From 0 to 5:   [" + String.Join(" ", path) + "]";

            Drawing(path);
        }

        private void Tick(object sender, EventArgs e) => Drawing();

        private void Drawing(List<int> path = null)
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                // Draw edges
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j) continue;

                        if (M[i, j] != 0 && M[i, j] != INF)
                        {
                            Pen pen = new Pen(Brushes.DarkGray, 1);
                            Point p0 = nodes[i];
                            Point p1 = nodes[j];
                            dc.DrawLine(pen, p0, p1);
                        }
                    }
                }

                // Draw path
                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Pen pen = new Pen(Brushes.LimeGreen, 2);
                        Point p0 = nodes[path[i]];
                        Point p1 = nodes[path[i + 1]];
                        dc.DrawLine(pen, p0, p1);
                    }
                }

                // Draw point + labeling
                for (int i = 0; i < nodes.Count; ++i)
                {
                    // Draw point
                    double size = 5;

                    Brush brush = null;
                    if (path != null && path.Contains(i))
                        brush = Brushes.LimeGreen;
                    else
                        brush = Brushes.DarkGray; // new SolidColorBrush(Color.FromArgb(255, 1, 64, 225));

                    dc.DrawEllipse(Brushes.Black, new Pen(brush, 3), nodes[i], size, size);

                    // Draw labeling
                    FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                    FlowDirection.LeftToRight, new Typeface("Verdana"), 11, Brushes.Black,
                                                                    VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                    dc.DrawText(formattedText, new Point(nodes[i].X, nodes[i].Y - 20));
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }

    }
}
