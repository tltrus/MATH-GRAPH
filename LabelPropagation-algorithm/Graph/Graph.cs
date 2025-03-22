using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MathGraph
{
    class Graph
    {
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

        public Graph(Random random, int width, int height)
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
            rnd = random;
            this.width = width;
            this.height = height;
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

        // ADD NODE
        private void AddNode(int i, int x = 0, int y = 0)
        {
            if (x == 0) 
                x = rnd.Next(10, width - 10);
            if (y == 0)
                y = rnd.Next(10, height - 10);
            int label = rnd.Next(3);

            Node node = new Node(x, y, label, i);
            nodes.Add(node);
        }

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

                var dist_new = Math.Sqrt((n1.pos.X - n0.pos.X) * (n1.pos.X - n0.pos.X) + (n1.pos.Y - n0.pos.Y) * (n1.pos.Y - n0.pos.Y));

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

        // SET LABELS
        public void SetLabel(int node_index)
        {
            List<int> neighbors = GetNeighbors(node_index);

            int new_label = GetLabel(neighbors);
            if (new_label == -1) return;

            nodes[node_index].label = new_label;
        }
        private List<int> GetNeighbors(int node_index)
        {
            var neighbors = edges
                .Where(a => a.from == node_index)
                .Select(b => b.to)
                .ToList();
            return neighbors;
        }
        private int GetLabel(List<int> neighbors)
        {
            // get label from most neighbors
            var labels = nodes
                .Where(a => neighbors.Any(i => a.id == i) && a.label != 0)
                .GroupBy(n => n.label)
                .Select(group => new {Group = group.Key, Count = group.Count()})
                .OrderByDescending(g => g.Count)
                .ToList();

            if (labels.Count == 2 && labels[0].Count == labels[1].Count)
            {
                if (rnd.NextDouble() < 0.5)
                {
                    return labels[0].Group;
                }
                return labels[1].Group;
            }
            else
            if (labels.Count > 0) return labels[0].Group;

            return -1;
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
                        brush = Brushes.LightGray;
                        break;
                    case 1:
                        brush = Brushes.Red;
                        break;
                    case 2:
                        brush = Brushes.Blue;
                        break;
                }
                dc.DrawEllipse(brush, null, p, r, r);

                // Draw radar
                //dc.DrawEllipse(null, new Pen(Brushes.LimeGreen, 0.5), p, nodes[i].radar, nodes[i].radar);

                // Draw labeling
                FormattedText formattedText = new FormattedText(i.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 8, Brushes.Black,
                                                                VisualTreeHelper.GetDpi(MainWindow.visual).PixelsPerDip);
                dc.DrawText(formattedText, new Point(p.X + 5, p.Y - r - 15));
            }
        }
    }
    public class Prob
    {
        public int index;
        public double prob;
    }
}
