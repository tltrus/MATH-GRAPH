using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MathGraph
{
    class Graph
    {
        public delegate void MessageHandler(string msg);
        public event MessageHandler MessageNotify;
        public delegate void BestHandler(string msg);
        public event BestHandler BestNotify;

        public class Node
        {
            public Point pos;
            public int r = 5; // radius
            public int label;
            public int id;
            public int radar = 120;

            public Node(int x, int y, int label, int id)
            {
                pos = new Point(x, y);
                this.label = label;
                this.id = id;
            }
        }
        public List<Node> nodes;
        public class Edge
        {
            public int from, to;
        }
        public List<Edge> edges;
        public int nodes_num;
        private Random rnd;
        private int width, height;
        private int m_parameter; // value of m parameter (m<=m_0)
        private Point? qPoint = null;
        private List<int> visited, path, closest_nodes;
        private int state;
        private int current_node, closest_node;
        private int counter;


        public Graph(Random random, int width, int height)
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
            rnd = random;
            this.width = width;
            this.height = height;
            visited = new List<int>();
            path = new List<int>();
            closest_nodes = new List<int>();
            state = 0;
        }
        public void Initialization(int nodes_num, int m_param)
        {
            m_parameter = m_param;
            this.nodes_num = nodes_num;

            nodes.Clear();
            edges.Clear();

            for (int i = 0; i < nodes_num; ++i)
            {
                AddNode(i);
            }

            for (int i = 0; i < nodes_num; ++i)
            {
                for (int j = 0; j < m_parameter; ++j)
                    AddEdge(i);
            }
        }
        public void Reset()
        {
            visited = new List<int>();
            path = new List<int>();
            closest_nodes = new List<int>();
            state = 0;
            counter = 0;
        }

        // ADD NODE
        private void AddNode(int i, int x = 0, int y = 0)
        {
            if (x == 0) 
                x = rnd.Next(10, width - 10);
            if (y == 0)
                y = rnd.Next(10, height - 10);

            Node node = new Node(x, y, 0, i);
            nodes.Add(node);
        }

        // SET Q
        public void SetQPoint(Point q) => qPoint = q;

        // ADD EDGE
        public void AddEdge(int current_node)
        {
            int second_node = GetNearestNode(current_node);

            if (current_node == second_node) return;

            bool isEdgeInGraph = CheckEdgeInGraph(current_node, second_node);
            if (!isEdgeInGraph)
            {
                edges.Add(new Edge() { from = current_node, to = second_node });
                edges.Add(new Edge() { from = second_node, to = current_node });
            }
        }
        private bool CheckEdgeInGraph(int n1, int n2)
        {
            int count = edges.Where(a => a.from == n1 && a.to == n2).Count();
            
            if (count > 0) return true;
            return false;
        }
        private int GetRadarNode(List<int> radar_nodes, int current_node)
        {
            // Get nodes degree
            var nodes_degr = radar_nodes.Count;

            var p_list = new List<Prob>();
            for (int i = 0; i < radar_nodes.Count; ++i)
            {
                // Barabasi algorithm
                int index = radar_nodes[i];
                double node_k = GetNodeDegree(index);
                double p = node_k / nodes_degr;

                bool isEdgeInGraph = CheckEdgeInGraph(current_node, index);
                if (!isEdgeInGraph)
                    p_list.Add(new Prob() { index = index, prob = p });
            }

            for (int i = 0; i < p_list.Count; ++i)
            {
                if (rnd.NextDouble() < p_list[i].prob)
                    return p_list[i].index;
            }
            return -1;
        }
        private int GetNearestNode(int current_node)
        {
            int node_id = 0;
            double dist = double.MaxValue;
            var radar_nodes = new List<int>();

            for (int i = 0; i < nodes_num; ++i)
            {
                if (i == current_node) continue;

                Node n0 = nodes[current_node];
                Node n1 = nodes[i];
                var dist_new = GetDist(n0.pos, n1.pos);

                // Get nodes inside radar
                if (dist_new < nodes[i].radar)
                {
                    radar_nodes.Add(i);
                }

                // Get nearest node
                if (dist_new < dist)
                {
                    dist = dist_new;
                    node_id = i;
                }
            }

            if (radar_nodes.Count > 0)
            {
                // if found big node
                int n = GetRadarNode(radar_nodes, current_node);
                if (n >= 0)
                    return n;
            }

            nodes[node_id].radar += 10; // increase radar size each cycle

            return node_id; // nearest node
        }
        private int GetNodeDegree(int n) => edges.Where(a => a.from == n).Count();

        // NSW
        private List<int> GetNeighbors(int node_index)
        {
            var neighbors = edges
                .Where(a => a.from == node_index)
                .Select(b => b.to)
                .ToList();
            return neighbors;
        }
        private double GetDist(Point p1, Point p2) => Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        private int GetQNearestNode(Point q, double parent_dist, List<int> neighbors)
        {
            var dist = double.MaxValue;
            int n_index = -1;

            for (int i = 0; i < neighbors.Count; ++i)
            {
                int index = neighbors[i];
                if (visited.Contains(index)) continue; // skip visited node
                
                var dist_new = GetDist(q, nodes[index].pos);
                if (dist_new < dist)
                {
                    dist = dist_new;
                    n_index = index;
                }
            }
            if (parent_dist < dist) return -1;

            return n_index;
        }
        public void NSWStatic()
        {
            path.Clear();
            visited.Clear();
            foreach (var n in nodes)
                n.label = 0;

            var current_node = rnd.Next(nodes_num);
            nodes[current_node].label = 1;
            path.Add(current_node);
            int closest_node = -1;

            while(current_node >= 0)
            {
                visited.Add(current_node);
                var neighbors = GetNeighbors(current_node);
                var current_dist = GetDist(nodes[current_node].pos, (Point)qPoint);
                var qNearestNode = GetQNearestNode((Point)qPoint, current_dist, neighbors);


                current_node = qNearestNode;

                if (qNearestNode != -1) // if no error
                {
                    nodes[qNearestNode].label = 1;
                    closest_node = qNearestNode;
                    path.Add(closest_node);
                }
            }

            MessageNotify?.Invoke("\nClosest node is " + closest_node);
        }
        /// <summary>
        /// Dynamic calculation.
        /// State machine is used.
        /// </summary>
        /// <returns>number of state</returns>
        public int NSWDynamic()
        {
            switch(state)
            {
                case 0:
                    path.Clear();
                    visited.Clear();
                    foreach (var n in nodes)
                        n.label = 0;

                    current_node = rnd.Next(nodes_num);
                    nodes[current_node].label = 1;
                    path.Add(current_node);
                    closest_node = -1;

                    state = 1;

                    break;

                case 1:
                    visited.Add(current_node);
                    var neighbors = GetNeighbors(current_node);
                    var current_dist = GetDist(nodes[current_node].pos, (Point)qPoint);
                    var qNearestNode = GetQNearestNode((Point)qPoint, current_dist, neighbors);


                    current_node = qNearestNode;

                    if (qNearestNode != -1) // if no error
                    {
                        nodes[qNearestNode].label = 1;
                        closest_node = qNearestNode;
                        path.Add(closest_node);
                    }

                    if (current_node < 0) state = 2;

                    break;

                case 2:
                    MessageNotify?.Invoke("\nClosest node is " + closest_node);
                    closest_nodes.Add(closest_node);
                    counter++;
                    state = 3;
                    break;
                case 3:
                    state = 0; // restart
                    break;
            }
            
            if (counter > 10) // Limit is 10
            {
                // Find result best node
                var query = from i in closest_nodes
                            group i by i into g
                            select new { g.Key, Count = g.Count() };

                int max = query.Max(g => g.Count);

                int best = query
                              .Where(g => g.Count == max)
                              .Select(g => g.Key).First();

                BestNotify?.Invoke(best.ToString());
            }

            return state;
        }

        // DRAWING
        public void Draw(DrawingContext dc)
        {
            // Draw connections
            for (int i = 0; i < edges.Count; ++i)
            {
                Point a = nodes[edges[i].from].pos;
                Point b = nodes[edges[i].to].pos;
                dc.DrawLine(new Pen(Brushes.LightGray, 0.5), a, b);
            }

            // Draw nodes
            for (int i = 0; i < nodes.Count; ++i)
            {
                // Draw point
                var p = nodes[i].pos;
                var r = nodes[i].r;
                Brush brush = null;
                switch (nodes[i].label)
                {
                    case 0:
                        brush = Brushes.DeepSkyBlue;
                        break;
                    case 1:
                        brush = Brushes.Green;
                        break;
                }
                dc.DrawEllipse(brush, null, p, r, r);

                // Draw radar
                //dc.DrawEllipse(null, new Pen(Brushes.LimeGreen, 0.5), p, nodes[i].radar, nodes[i].radar);

                // Draw labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 7, Brushes.Black,
                                                                VisualTreeHelper.GetDpi(MainWindow.visual).PixelsPerDip);
                dc.DrawText(formattedText, new Point(p.X + 5, p.Y - r - 15));
            }

            // Draw path
            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Point a = nodes[path[i]].pos;
                    Point b = nodes[path[i + 1]].pos;
                    dc.DrawLine(new Pen(Brushes.Green, 2), a, b);
                }
            }

            // Draw q
            if (qPoint != null)
                dc.DrawEllipse(Brushes.Red, null, (Point)qPoint, 5, 5);
        }
    }
    public class Prob
    {
        public int index;
        public double prob;
    }
}
