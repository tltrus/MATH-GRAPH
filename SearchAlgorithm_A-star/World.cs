using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;
using System.Globalization;


namespace Astar
{
    public class Map
    {
        public int rows = 25;
        public int cols = 25;
        public Point pStart;
        public Point pFinish;
        public int[,] map;
        public int cellsize = 15;      // cell size
        int offs_x = 0;
        int offs_y = 0;
        bool triggerStartFinish;

        public Map()
        {
            map = new int[rows, cols];
            pFinish = new Point();
            pStart = new Point();
        }

        public void CreateWay(List<Point> points)
        {
            foreach (var point in points)
            {
                map[(int)point.Y, (int)point.X] = 4; // way
            }
        }

        public int GetCellindex(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            return (Y * cols) + X;
        }

        public void SetWall(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            if ((X >= cols) || (Y >= rows)) return; // if click out of field

            map[Y, X] = (map[Y, X] == 1) ? 0 : 1; // trigger set\reset cell wall
        }

        public void SetStartFinish(Point p)
        {
            int X = (int)p.X / cellsize;
            int Y = (int)p.Y / cellsize;

            if ((X >= cols) || (Y >= rows)) return; // if click out of field

            if (triggerStartFinish)
            {
                // Finish
                map[(int)pFinish.Y, (int)pFinish.X] = 0; // reset old cell
                map[Y, X] = (map[Y, X] == 3) ? 0 : 3; // trigger set\reset cell
                pFinish.X = X;
                pFinish.Y = Y;
                triggerStartFinish = false;
            }
            else
            {
                // Start
                map[(int)pStart.Y, (int)pStart.X] = 0; // reset old cell
                map[Y, X] = (map[Y, X] == 2) ? 0 : 2; // trigger set\reset cell
                pStart.X = X;
                pStart.Y = Y;
                triggerStartFinish = true;
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
                    if (map[y, x] == 0)
                    {
                        brush = Brushes.White;
                    }
                    else
                    // Wall
                    if (map[y, x] == 1)
                    {
                        brush = Brushes.Black;
                    }
                    else
                    // Start
                    if (map[y, x] == 2)
                    {
                        brush = Brushes.Green;
                        text = "S";
                        textPos = new Point(offs_x + 3, offs_y);
                    }
                    else
                    // Finish
                    if (map[y, x] == 3)
                    {
                        brush = Brushes.SandyBrown;
                        text = "F";
                        textPos = new Point(offs_x + 3, offs_y);
                    }
                    else
                    // Way
                    if (map[y, x] == 4)
                    {
                        brush = Brushes.SandyBrown;
                    }

                    Pen pen = new Pen(Brushes.Gray, 0.5);
                    dc.DrawRectangle(brush, pen, rect);

                    FormattedText formattedText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black,
                            VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                    dc.DrawText(formattedText, textPos);
                }
            }
        }

        public void Clear() => map = new int[rows, cols];
    }
}

