using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MathGraph
{
    internal class DFS : xFS
    {

        public DFS(Graph g, Pen pathPen)
        {
            N = g.N;
            this.g = g;
            path = new List<int>();

            // инициализация переменных 
            Parents = new int[N];
            this.pathPen = pathPen;

        }

        public void Calculation(int v)
        {
            Stack<int> S = new Stack<int>(); // создание очереди
            S.Push(v);
            // Основной цикл перебора вершин графа
            while (S.Count != 0)
            {
                //взять из очереди очередную вершину 
                v = S.Pop();
                for (int i = 0; i < N; i++)
                {
                    if ((g.adjacencyMatrix[v, i] != 0) && !Parents.Contains(i))
                    {
                        Parents[i] = v; // v – предок для i
                        S.Push(i);
                    }
                }
            }
        }

        public void Drawing(DrawingContext dc)
        {
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    Point p0 = g.nodes[path[i]];
                    Point p1 = g.nodes[path[i + 1]];
                    dc.DrawLine(pathPen, p0, p1);
                }
            }

            for (int i = 0; i < N; ++i)
            {
                // Draw point
                Point p = g.GetPoint(i);

                if (path != null && path.Contains(i))
                {
                    dc.DrawEllipse(Brushes.Black, pathPen, new Point(p.X, p.Y), 15, 15);
                }
            }
        }
    }
}
