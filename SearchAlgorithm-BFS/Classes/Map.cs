using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;
using System.Globalization;

namespace GraphBFS
{
    public class Map
    {
        public int rows = 20;
        public static int cols = 20;
        public Point pStart;
        public Point pFinish;
        public static int iStart;
        public static int iFinish;
        public static int[,] map;
        public int cellsize = 19;      // cell size
        int offs_x = 0;
        int offs_y = 0;
        bool triggerStartFinish;

        public Map()
        {
            pFinish = new Point();
            pStart = new Point();

            Init();
        }

        private void Init()
        {
            map = new int[rows, cols];

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                {
                    map[y, x] = GetCellindex(x, y);
                }
        }

        public void CreateWay(List<Point> points)
        {
            foreach (var point in points)
            {
                map[(int)point.Y, (int)point.X] = -5; // way
            }
        }

        public int GetCellindex(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            return (Y * cols) + X;
        }

        public int GetCellindex(int x, int y) => (y * cols) + x;

        public void SetWall(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            if ((X >= cols) || (Y >= rows)) return; // if click out of field

            map[Y, X] = (map[Y, X] == (int)STATE.wall) ? (int)STATE.empty : (int)STATE.wall; // trigger set\reset cell wall
        }

        public void SetStartFinish(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            if ((X >= cols) || (Y >= rows)) return; // if click out of field

            if (triggerStartFinish)
            {
                // Finish
                map[(int)pFinish.Y, (int)pFinish.X] = (int)STATE.empty; // reset old cell
                map[Y, X] = (map[Y, X] == (int)STATE.finish) ? (int)STATE.empty : (int)STATE.finish; // trigger set\reset cell
                pFinish.X = X;
                pFinish.Y = Y;
                triggerStartFinish = false;
                iFinish = Tools.FromXYtoNum(X, Y, cols);
            }
            else
            {
                // Start
                map[(int)pStart.Y, (int)pStart.X] = (int)STATE.empty; // reset old cell
                map[Y, X] = (map[Y, X] == (int)STATE.start) ? (int)STATE.empty : (int)STATE.start; // trigger set\reset cell
                pStart.X = X;
                pStart.Y = Y;
                triggerStartFinish = true;
                iStart = Tools.FromXYtoNum(X, Y, cols);
            }
        }

        public void Draw(DrawingVisual visual, DrawingContext dc)
        {
            for (int y = 0; y < rows; y++)
            {
                offs_y = y * cellsize;
                for (int x = 0; x < cols; x++)
                {
                    offs_x = x * cellsize;

                    Brush brush = Brushes.White;
                    var text = "";
                    Point textPos = new Point();

                    Rect rect = new Rect()
                    {
                        X = offs_x,
                        Y = offs_y,
                        Width = cellsize,
                        Height = cellsize
                    };

                    // Empty
                    if (map[y, x] == (int)STATE.empty)
                    {
                        brush = Brushes.White;
                    }
                    else
                    // Wall
                    if (map[y, x] == (int)STATE.wall)
                    {
                        brush = Brushes.Black;
                    }
                    else
                    // Start
                    if (map[y, x] == (int)STATE.start)
                    {
                        brush = Brushes.Green;
                        text = "S";
                    }
                    else
                    // Finish
                    if (map[y, x] == (int)STATE.finish)
                    {
                        brush = Brushes.SandyBrown;
                        text = "F";
                    }
                    else
                    // Way
                    if (map[y, x] == (int)STATE.way)
                    {
                        brush = Brushes.SandyBrown;
                    }
                    else
                    // Numbers
                    if (map[y, x] > 0)
                    {
                        text = map[y, x].ToString();
                    }

                    Pen pen = new Pen(Brushes.Gray, 0.5);
                    dc.DrawRectangle(brush, pen, rect);

                    textPos = new Point(offs_x + 3, offs_y + 3);
                    FormattedText formattedText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight, new Typeface("Verdana"), 8, Brushes.Black,
                            VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                    dc.DrawText(formattedText, textPos);
                }
            }
        }

        public void Clear() => Init();
    }
}

