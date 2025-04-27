using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathGraph
{
    internal class Utils
    {
        private static double Constrain(double n, double low, double high)
        {
            return Math.Max(Math.Min(n, high), low);
        }

        public static double Map(double n, double start1, double stop1, double start2, double stop2, bool withinBounds = false)
        {
            double num = (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
            if (!withinBounds)
            {
                return num;
            }

            if (start2 < stop2)
            {
                return Constrain(num, start2, stop2);
            }

            return Constrain(num, stop2, start2);
        }

        static public Point[] IntersectionLineVsCircle(Point p1, Point p2, Point circle, double radius)
        {
            Point dp = new Point();
            Point[] sect;
            double a, b, c;
            double bb4ac;
            double mu1;
            double mu2;
            double u;

            //  get X and Y distances of the line segment
            dp.X = p2.X - p1.X;
            dp.Y = p2.Y - p1.Y;

            //  I don't understand the math beyond this
            a = dp.X * dp.X + dp.Y * dp.Y;
            b = 2 * (dp.X * (p1.X - circle.X) + dp.Y * (p1.Y - circle.Y));
            c = circle.X * circle.X + circle.Y * circle.Y;
            c += p1.X * p1.X + p1.Y * p1.Y;
            c -= 2 * (circle.X * p1.X + circle.Y * p1.Y);
            c -= radius * radius;
            bb4ac = b * b - 4 * a * c;

            u = ((circle.X - p1.X) * (p2.X - p1.X) + (circle.Y - p1.Y) * (p2.Y - p1.Y)) /
                     ((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

            //  u must be between 0 and 1 for the line segment to intersect
            if (u < 0 || u > 1)
            {
                return new Point[0];
            }

            //  if bb4ac is < 0, the line does not intersection
            if (bb4ac < 0 || Math.Abs(a) < Double.Epsilon)
            {
                //  no intersection
                return new Point[0];
            }
            //  if bb4ac is at 0, one intersection point
            else if (bb4ac >= -Double.Epsilon && bb4ac <= Double.Epsilon)
            {
                //  line intersects once
                sect = new Point[1];
                mu1 = -b / (2 * a);
                sect[0] = new Point(p1.X + mu1 * (p2.X - p1.X), p1.Y + mu1 * (p2.Y - p1.Y));
            }
            //  else bb4ac > 0, two intersection points
            else
            {
                mu1 = (-b + Math.Sqrt(bb4ac)) / (2 * a);
                mu2 = (-b - Math.Sqrt(bb4ac)) / (2 * a);

                sect = new Point[2];
                sect[0] = new Point(p1.X + mu1 * (p2.X - p1.X), p1.Y + mu1 * (p2.Y - p1.Y));
                sect[1] = new Point(p1.X + mu2 * (p2.X - p1.X), p1.Y + mu2 * (p2.Y - p1.Y));
            }

            return sect;
        }
    }
}
