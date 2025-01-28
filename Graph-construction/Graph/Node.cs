using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.CodeDom;
using System.Windows.Documents;

namespace MathGraph
{
    class Node
    {
        public delegate void ConnectedHandle(string str);
        public event ConnectedHandle ConnectedNotify;

        public List<int> connections; // indexes of Graph nodes list
        public Point pos;
        int N; // num of connections
        public int id;

        public bool IsEmptyForConnection
        {
            get
            {
                return connections.Count < N;
            }
        }

        public Node(int n)
        {
            N = n;
            connections = new List<int>();
        }

        public void SetConnections(List<Node> nodes)
        {
            for (int j = 0; j < nodes.Count; ++j)
            {
                if (j == id) continue; // skip if current node is in nodes list

                Node selected_node = nodes[j];

                bool isEmpty = selected_node.IsEmptyForConnection;
                bool isNotContain = !connections.Contains(selected_node.id);

                if (isEmpty && isNotContain && connections.Count < N)
                {
                    connections.Add(selected_node.id);              // save index of node from nodes list
                    nodes[selected_node.id].connections.Add(id);    // save index of current node
                }
            }

            if (connections.Count <= 0) return;

            string str = String.Join(",", connections.Select(i => i.ToString()).ToArray());
            ConnectedNotify?.Invoke("\nNode " + id + " is connected to nodes [" + str + "]");
        }

        private double GetDist(Point p)
        {
            return Math.Sqrt(Math.Pow(pos.X - p.X, 2) + Math.Pow(pos.Y - p.Y, 2));
        }

        public void DrawingEdges(DrawingContext dc, List<Node> nodes)
        {
            // Draw edges
            for (int i = 0; i < connections.Count; ++i)
            {
                Pen pen = new Pen(Brushes.LightGray, 1);

                int index = connections[i]; // get index of node inside connections array
                dc.DrawLine(pen, pos, nodes[index].pos);

            }
        }

        public void DrawingNodes(DrawingContext dc)
        {
            // Draw point
            double size = N + 2;
            Brush brush = new SolidColorBrush(Color.FromArgb(255, 1, 64, 225));
            dc.DrawEllipse(Brushes.Black, new Pen(Brushes.LightGray, 3), pos, size, size);

            // Draw labeling
            FormattedText formattedText = new FormattedText(id.ToString(), CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight, new Typeface("Verdana"), 11, Brushes.Black,
                                                            VisualTreeHelper.GetDpi(MainWindow.visual).PixelsPerDip);
            dc.DrawText(formattedText, new Point(pos.X, pos.Y - 20));
        }
    }

}
