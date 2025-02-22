using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace MathGraph
{
    class Graph
    {
        public List<Point> nodes;
        public int N; // nodes
        public int[,] adjacencyMatrix;
        Random rnd;

        public Graph(int nodesNum, Random rnd)
        {
            this.rnd = rnd;
            N = nodesNum;
            nodes = new List<Point>();

            for (int i = 0; i < N; ++i)
            {
                Point node = GetPoint(i);
                nodes.Add(node);
            }

            adjacencyMatrix = MakeAdjacencyMatrix(N);
            //adjacencyMatrix = new int[,]
            //{
            //    { 0, 1, 0, 0, 1 },
            //    { 1, 0, 1, 1, 1 },
            //    { 0, 1, 0, 1, 0 },
            //    { 0, 1, 1, 0, 1 },
            //    { 1, 1, 0, 1, 0 }
            //};
            //adjacencyMatrix = new int[,]
            //{
            //    { 0, 0, 1, 0, 0 },
            //    { 0, 0, 1, 0, 0 },
            //    { 1, 1, 0, 1, 1 },
            //    { 0, 0, 1, 0, 1 },
            //    { 0, 0, 1, 1, 0 }
            //};
        }

        // For placement the nodes on canvas
        public Point GetPoint(int i)
        {
            double angle = Utils.Map(i % N, 0, N, 0, 2 * Math.PI);
            double r = MainWindow.height / 2.4;
            return new Point(r * Math.Cos(angle + Math.PI) + MainWindow.width / 2, r * Math.Sin(angle + Math.PI) + MainWindow.height / 2);
        }

        public string GetMatrix() => ToString(adjacencyMatrix);

        // Make Adjacency matrix
        private int[,] MakeAdjacencyMatrix(int N)
        {
            int[,] m = new int[N, N];
            
            for (int i = 0; i < N; i++)
            {
                for (int j = i; j < N; j++)
                {
                    if (i == j) continue;

                    int val = 0;
                    if (rnd.NextDouble() < 0.4)
                    {
                        val = 1;
                    }
                    m[i, j] = val;
                    m[j, i] = val;
                }
            }
            return m;
        }

        private string ToString<T>(T[,] matrix, int padding = 2)
        {
            string s = "";
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    string str = (string)Convert.ChangeType(matrix[i, j], typeof(string));
                    s += str.PadLeft(padding) + " ";
                }
                s += Environment.NewLine;
            }
            return s;
        }

        public void Drawing(DrawingContext dc, List<int> path = null)
        {
            int r = 15;

            for (int i = 0; i < N; ++i)
            {
                for (int j = 1; j < N; ++j)
                {
                    if (adjacencyMatrix[i, j] == 1)
                    {
                        // Draw edges
                        Point p0 = nodes[i];
                        Point p1 = nodes[j];
                        Pen pen = new Pen(Brushes.LightGray, 2);
                        dc.DrawLine(pen, p0, p1);
                    }
                }
            }

            for (int i = 0; i < N; ++i)
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
    }
}
