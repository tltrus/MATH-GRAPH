using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;


namespace MathGraph
{
    // Breadth First Algorithm(BFS) / Поиск в ширину
    internal class BFS : xFS
    {
        public int[] Marks; // массив пометок 
        Queue<int> Q;
        int v;


        public BFS(Graph g, Pen pathPen)
        {
            N = g.N;
            this.g = g;
            path = new List<int>();

            Marks = new int[N]; // массив пометок 
            Parents = new int[N]; // массив предков 

            Console.WriteLine("Вершины в порядке обхода");
            Q = new Queue<int>(); // создание очереди 
            v = 0; // задание начальной вершины 
            Marks[v] = 1; // пометим нач. вершину 
            Q.Enqueue(v); // поместим нач. вершину в очередь 

            Console.Write("{0} ", v);
            this.pathPen = pathPen;
        }

        public void Calculation()
        {
            while (Q.Count != 0) //Пока очередь не исчерпана 
            {
                //взять из очереди очередную вершину 
                v = Q.Dequeue();
                for (int i = 0; i < N; i++)
                {
                    if ((g.adjacencyMatrix[v, i] != 0) && (Marks[i] == 0))
                    {
                        // все непомеченные вершины, 
                        Marks[i] = 1; // смежные с текущей, помечаются 
                        Q.Enqueue(i); // и помещаются в конец очереди 
                        Parents[i] = v; // v – предок открытой вершины 
                        Console.Write("{0} ", i);
                    }
                }
                Marks[v] = 2; // вершина обработана 
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
