using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static MathGraph.Graph;

namespace MathGraph
{
    class Graph
    {
        // A class to represent a weighted edge in graph 
        public class Edge
        {
            public int src, dest, weight;
            public Edge()
            {
                src = dest = weight = 0;
            }
        };
        public class Node
        {
            public int id;
            public Point pos;

            public Node()
            {
                pos = new Point();
            }
        }
        public List<Edge> edges;
        int[] dist;
        public int[] predecessor; // Инициализировать предшественника
        List<Node> nodes;
        int N = 7; // nodes
        int E; // edges
        public List<Node> path;

        // Creates a graph with V vertices and E edges 
        public Graph()
        {
            edges = new List<Edge>();
            nodes = new List<Node>();
            path = new List<Node>();
        }

        public void AddEdge(int src, int dest, int weight)
        {
            Edge edge = new Edge()
            {
                src = src,
                dest = dest,
                weight = weight
            };

            edges.Add(edge);

            Node node = new Node()
            {
                id = src
            };

            if (ListContains(nodes, src).Count() == 0) nodes.Add(node);

            node = new Node()
            {
                id = dest
            };

            if (ListContains(nodes, dest).Count() == 0) nodes.Add(node);

            E = edges.Count; 

            // Set position for each node
            foreach(var n in nodes)
            {
                n.pos = GetPoint(n.id);
            }
        }

        public void SetConnections()
        {
            if (nodes is null) return;

            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    if (i == j) continue;

                    var p = MainWindow.rnd.NextDouble();
                    var w = MainWindow.rnd.Next(-2, 11);

                    if (p < 0.3)
                    {
                        AddEdge(src: i, dest: j, weight: w);
                    }
                }
            }
        }

        private IEnumerable<Node> ListContains(List<Node> nodes, int id) => nodes.Where(n => n.id == id);

        // The main function that finds shortest distances from src 
        // to all other vertices using Bellman-Ford algorithm. The 
        // function also detects negative weight cycle 
        public bool BellmanFord(Graph graph, int src)
        {
            //int V = graph.V, E = graph.E;
            dist = new int[N];
            // Инициализировать предшественника
            predecessor = new int[N];

            // Step 1: Initialize distances from src to all other 
            // vertices as INFINITE 
            for (int i = 0; i < N; ++i)
                dist[i] = int.MaxValue;
            dist[src] = 0;

            // Step 2: Relax all edges |V| - 1 times. A simple 
            // shortest path from src to any other vertex can 
            // have at-most |V| - 1 edges 
            for (int i = 1; i < N; ++i)
            {
                for (int j = 0; j < E; ++j)
                {
                    int u = graph.edges[j].src;
                    int v = graph.edges[j].dest;
                    int weight = graph.edges[j].weight;
                    if (dist[u] != int.MaxValue && dist[u] + weight < dist[v])
                    {
                        dist[v] = dist[u] + weight;
                        predecessor[v] = u;
                    }
                }
            }

            // Step 3: check for negative-weight cycles. The above 
            // step guarantees shortest distances if graph doesn't 
            // contain negative weight cycle. If we get a shorter 
            // path, then there is a cycle. 
            for (int j = 0; j < E; ++j)
            {
                int u = graph.edges[j].src;
                int v = graph.edges[j].dest;
                int weight = graph.edges[j].weight;
                if (dist[u] != int.MaxValue && dist[u] + weight < dist[v])
                {
                    Console.WriteLine("Graph contains negative weight cycle");
                    return false;
                }
            }

            return true;
        }

        // Вывод расстояний от источника [A] до каждой вершины графа 
        public string PrintPathes()
        {
            string temp = "";

            for (int i = 0; i < N; ++i)
            {
                int d = dist[i];
                string str = d.ToString();
                if (d == int.MaxValue)
                    str = "\u221E"; // infinity symbol for non connected nodes
                temp += "from 0 to " + i + " --> [ " + str + " ]\n";
            }

            return temp + "\n";
        }

        // вывод пути из массива predecessor
        public List<int> PrintPath(int v_begin, int v_end)
        {
            List<int> list = new List<int>();
            list.Add(v_end);
            int counter = 1;
            int u = predecessor[v_end];

            while (counter < nodes.Count)
            {
                list.Add(u);

                if (list.Count == 2 && u == v_begin && edges.Where(a => (a.src == u) && (a.dest == v_end)).Count() == 0) // trick to check no connection between src and dest
                {
                    return new List<int>() { -1 };
                }

                if (u == v_begin) return list;

                u = predecessor[u];

                counter++;
            }

            return new List<int>() { -1 };
        }

        public string PrintWeights()
        {
            string temp = "\n";

            for (int i = 0; i < edges.Count; ++i)
                temp += "[ " + edges[i].src + ", " + edges[i].dest + " ] --> " + edges[i].weight + "\n";

            return temp + "\n";
        }

        public void Drawing(DrawingContext dc)
        {
            int r = 15;

            // Draw edges
            for (int i = 0; i < edges.Count; ++i)
            {
                (Point a, Point b) = GetPointsFromEdge(i);
                dc.DrawLine(new Pen(Brushes.Gray, 1), a, b);

                var cross = Utils.IntersectionLineVsCircle(a, b, b, r + 5);
                if (cross.Length > 0)
                    dc.DrawEllipse(Brushes.Black, null, new Point(cross[1].X, cross[1].Y), 3, 3);
            }

            // Draw nodes
            for (int i = 0; i < nodes.Count; ++i)
            {
                // Draw point
                Point p = GetPoint(i);
                Pen pen = new Pen(Brushes.LightGray, 5);
                dc.DrawEllipse(Brushes.Black, pen, new Point(p.X, p.Y), r, r);

                // Draw labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 11, Brushes.Black,
                                                                VisualTreeHelper.GetDpi(MainWindow.visual).PixelsPerDip);
                dc.DrawText(formattedText, new Point(p.X + 5, p.Y - r - 15));
            }
        }
        public void DrawingPath(DrawingContext dc, List<int> path)
        {
            int r = 15;

            // Draw path
            for (int i = 0; i < path.Count; ++i)
            {
                int id = path[i];
                Point p = GetPoint(id);
                Pen pen = new Pen(Brushes.Lime, 5);
                dc.DrawEllipse(Brushes.Black, pen, new Point(p.X, p.Y), r, r);
            }
        }

        private (Point, Point) GetPointsFromEdge(int i)
        {
            var node_a = nodes.Where(n => n.id == edges[i].src).First();
            var node_b = nodes.Where(n => n.id == edges[i].dest).First();

            var p0 = (node_a).pos;
            var p1 = (node_b).pos;
            return (p0, p1);
        }

        // For placement the nodes on canvas
        private Point GetPoint(int i)
        {
            double angle = Utils.Map(i % N, 0, N, 0, 2 * Math.PI);
            double r = MainWindow.height / 2.4;
            Point p = new Point(r * Math.Cos(angle + Math.PI) + MainWindow.width / 2, r * Math.Sin(angle + Math.PI) + MainWindow.height / 2);
            return p;
        }
    }
}
