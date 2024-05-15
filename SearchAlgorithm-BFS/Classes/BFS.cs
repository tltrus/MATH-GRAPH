using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace GraphBFS
{
    // Breadth First Search
    public class BFS
    {
        public int[] Mark; // массив пометок 
        public int[] Parent; // массив предков
        private List<string> path;

        private void Calculate(int[,] g)
        {
            int vertexNum = g.GetLength(0);

            Mark = new int[vertexNum]; // массив пометок 
            Parent = new int[vertexNum]; // массив предков 
            for (int i = 0; i < vertexNum; i++)
            {
                Mark[i] = 0;
                Parent[i] = 0;
            }

            Queue<int> Q = new Queue<int>(); // создание очереди 
            int v = Map.iStart; // начальная вершина 
            Mark[v] = 1; // пометим нач. вершину 
            Q.Enqueue(v); // поместим нач. вершину в очередь 

            string aaa = "0 ";
            while (Q.Count != 0) //Пока очередь не исчерпана 
            {
                //взять из очереди очередную вершину 
                v = Q.Dequeue();

                for (int i = 0; i < vertexNum; i++)
                {
                    if ((g[v, i] != 0) && (Mark[i] == 0))
                    {
                        // все непомеченные вершины, 
                        Mark[i] = 1; // смежные с текущей, помечаются 
                        Q.Enqueue(i); // и помещаются в конец очереди 
                        Parent[i] = v; // v – предок открытой вершины 
                        //Console.Write("{0} ", i);
                        aaa += i + " ";

                    }
                }
                Mark[v] = 2; // вершина обработана 
            }
        }

        public void CreatePath(int[,] g, int v_begin, int v_end)
        {
            Calculate(g);

            path = new List<string>();
            // Индекс массива Parent[] - это номер вершины (от 0 до...)
            // Значение индекса массива Parent[] - это предок вершины
            int u = Parent[v_end];
            path.Add(u.ToString());
            while (u != v_begin)
            {
                path.Add(Parent[u].ToString());

                u = Parent[u];
            }
            path.Reverse();

            foreach (var p in path)
            {
                Point point = Tools.FromNumtoXYPoint(int.Parse(p)); // From num to Point(x, y)
                Map.map[(int)point.Y, (int)point.X] = (int)STATE.way;
            }
        }

        public string GetPath() => string.Join(" ", path);
    }
}
