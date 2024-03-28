using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;

namespace Astar
{
    // It's not my algorithm. I took it from another autor.
    public class A_star
    {
        public class PathNode
        {
            // Координаты точки на карте.
            public Point Position { get; set; }
            // Длина пути от старта (G).
            public int PathLengthFromStart { get; set; }
            // Точка, из которой пришли в эту точку.
            public PathNode CameFrom { get; set; }
            // Примерное расстояние до цели (H).
            public int HeuristicEstimatePathLength { get; set; }
            // Ожидаемое полное расстояние до цели (F).
            public int EstimateFullPathLength
            {
                get
                {
                    return PathLengthFromStart + HeuristicEstimatePathLength;
                }
            }
        }

        public static List<Point> FindPath(int[,] field, Point start, Point goal)
        {
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();

                if (currentNode.Position == goal)
                    return GetPathForNode(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
                {
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0) continue;
                    
                    var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else
                    if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        // Length between neighbour cells is 1.
        private static int GetDistBetweenNeighbours() =>  1;

        // The function of approximate estimation of the distance to the target:
        // To estimate the distance, using the length of the path without obstacles.
        private static int GetHeuristicPathLength(Point from, Point to) => Math.Abs((int)(from.X - to.X)) + Math.Abs((int)(from.Y - to.Y));

        // Get the list of Neighbours:
        private static Collection<PathNode> GetNeighbours(PathNode pathNode, Point goal, int[,] field)
        {
            var result = new Collection<PathNode>();

            Point[] neighbourPoints = new Point[4];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

            foreach (var point in neighbourPoints)
            {
                if (point.X < 0 || point.X >= field.GetLength(1)) continue;
                if (point.Y < 0 || point.Y >= field.GetLength(0)) continue;

                if ((field[(int)point.Y, (int)point.X] != 0) && (field[(int)point.Y, (int)point.X] != 2) && (field[(int)point.Y, (int)point.X] != 3)) continue;

                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + GetDistBetweenNeighbours(),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        // Get the way
        private static List<Point> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            // Go from End
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }
    }
}
