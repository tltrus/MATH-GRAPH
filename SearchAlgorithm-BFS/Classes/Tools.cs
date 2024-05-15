using System.Windows;

namespace GraphBFS
{
    enum STATE
    {
        empty = -1,
        wall = -2,
        start = -6,
        finish = -4,
        way = -5
    }

    internal class Tools
    {
        // from Cell num to X and Y
        public static int[] FromNumtoXY(int Cellnum)
        {
            int y = Cellnum / Map.cols;      // Get row num
            int x = Cellnum - y * Map.cols; // Get Col num
            int[] XY = { x, y };
            return XY;
        }

        public static int FromXYtoNum(int x, int y, int cols) => y * cols + x;


        // from Cell num to Point
        public static Point FromNumtoXYPoint(int cellNum)
        {
            int y = cellNum / Map.cols;     // Get row num
            int x = cellNum - y * Map.cols; // Get Col num
            return new Point(x, y);
        }
    }
}
