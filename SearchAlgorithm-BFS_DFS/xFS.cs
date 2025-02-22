using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MathGraph
{
    internal class xFS
    {
        public int[] Parents; // массив предков
        public Graph g;
        public List<int> path;
        public Pen pathPen;
        public int N;

        public string ParentToString()
        {
            return "parents is [ " + String.Join(",", Parents) + " ] \r";
        }
        public List<int> GetPathList(int v_begin, int v_end)
        {
            int u = Parents[v_end];
            if (u == v_begin && g.adjacencyMatrix[u, v_end] == 1)
            {
                //Console.Write(" {0} {1} ", u, v_end);
                path.Add(u);
                path.Add(v_end);
                return path;
            }
            else
            if (Parents[v_end] == 0)
            {
                //Console.WriteLine(" Пути из {0} в {1} нет", v_begin, v_end);
                return new List<int> { -1 }; // no path
            }
            else
                GetPathList(v_begin, u);

            //Console.Write("{0} ", v_end);
            path.Add(v_end);
            return path;
        }
        public string GetPathString(int v_begin, int v_end)
        {
            this.path.Clear();

            string str = "path from " + v_begin + " to " + v_end + " is [ ";

            List<int> pathList = GetPathList(v_begin, v_end);
            string path = String.Join(", ", pathList);

            return str + path + " ]";
        }

    }
}
