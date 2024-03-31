using System.Collections.Generic;
using System.Windows;

namespace GraphLee
{
    enum STATE
    {
        empty = -1,
        wall = -2,
        start = 0,
        finish = -4,
        way = -5
    }
    
    public class Alg_Lee
    {
        public static void PathCalculation(int[,] map, Point start, Point finish)
        {
            bool add = true;
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int x, y, step = 0;

            while (add == true)
            {
                add = false;
                for (y = 0; y < rows; y++)
                    for (x = 0; x < cols; x++)
                    {
                        if (map[y, x] == step)
                        {
                            if (x - 1 > -1 && map[y, x - 1] != (int)STATE.wall && map[y, x - 1] == (int)STATE.empty)
                            {
                                map[y, x - 1] = step + 1;
                            }
                            if (y - 1 > -1 && map[y - 1, x] != (int)STATE.wall && map[y - 1, x] == (int)STATE.empty)
                            {
                                map[y - 1, x] = step + 1;
                            }
                            if (x + 1 < cols && map[y, x + 1] != (int)STATE.wall && map[y, x + 1] == (int)STATE.empty)
                            {
                                map[y, x + 1] = step + 1;
                            }
                            if (y + 1 < rows && map[y + 1, x] != (int)STATE.wall && map[y + 1, x] == (int)STATE.empty)
                            {
                                map[y + 1, x] = step + 1;
                            }
                        }
                    }
                step++;
                add = true;
                if (map[(int)finish.Y, (int)finish.X] != -4)
                    add = false;
                if (step > cols * rows)
                    add = false;
            }
        }

        public static List<Point> PathFind(int[,] map, Point start, Point finish)
        {
            List<Point> result = new List<Point>(); 

            int pY = (int)finish.Y;
            int pX = (int)finish.X;

            var p1 = FindNearNeighbour(map, pY, pX);
            int steps = map[(int)p1.Y, (int)p1.X]; // get count of steps

            for (int i = steps; i > 0; i--)
            {
                var p = FindNearNeighbour(map, pY, pX);
                result.Add(p);
                pY = (int)p.Y;
                pX = (int)p.X;
            }

            return result;
        }

        private static Point FindNearNeighbour(int[,] map, int y, int x)
        {
            int min = 9999;
            Point result = new Point();
            int value = 0;

            if (x - 1 > -1)
            {
                value = map[y, x - 1];
                if (value < min && value > 0)
                {
                    min = value;
                    result.X = x - 1;
                    result.Y = y;
                }
            }

            if (x + 1 < map.GetLength(1))
            {
                value = map[y, x + 1];
                if (value < min && value > 0)
                {
                    min = value;
                    result.X = x + 1;
                    result.Y = y;
                }
            }

            if (y - 1 > -1)
            {
                value = map[y - 1, x];
                if (value < min && value > 0)
                {
                    min = value;
                    result.X = x;
                    result.Y = y - 1;
                }
            }

            if (y + 1 < map.GetLength(0))
            {
                value = map[y + 1, x];
                if (value < min && value > 0)
                {
                    min = value;
                    result.X = x;
                    result.Y = y + 1;
                }
            }

            return result;
        }
    }
}
