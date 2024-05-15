using System;
using System.Collections.Generic;

namespace GraphBFS
{
    internal class AdjacencyMatrix
    {
        public int[,] adjacency;
        private void AddToAdjacencyMatrix(int[,] original, Dictionary<Tuple<int, int>, int> coordinate2NodeNum, Tuple<int, int> fromCoord, int deltaX, int deltaY)
        {
            Tuple<int, int> toCoord = new Tuple<int, int>(fromCoord.Item1 + deltaX, fromCoord.Item2 + deltaY);
            try
            { // quick and dirty way of catching out of range coordinates
                if (original[toCoord.Item1, toCoord.Item2] != (int)STATE.wall)
                {
                    int fromNodeNum = coordinate2NodeNum[fromCoord];
                    int toNodeNum = coordinate2NodeNum[toCoord];
                    adjacency[fromNodeNum, toNodeNum] = 1;
                    adjacency[toNodeNum, fromNodeNum] = 1;
                }
            }
            catch
            {
            }
        }
        public string CreateAdjacencyMatrix(int[,] map)
        {
            Dictionary<int, Tuple<int, int>> nodeNum2Coordinate = new Dictionary<int, Tuple<int, int>>();
            Dictionary<Tuple<int, int>, int> coordinate2NodeNum = new Dictionary<Tuple<int, int>, int>();
            int nodeCount = 0;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Tuple<int, int> coord = new Tuple<int, int>(i, j);
                    nodeNum2Coordinate.Add(nodeCount, coord);
                    coordinate2NodeNum.Add(coord, nodeCount);
                    nodeCount++;
                }
            }

            // Now create the adacency matrix
            adjacency = new int[nodeCount, nodeCount];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] != (int)STATE.wall)
                    {
                        Tuple<int, int> fromCoord = new Tuple<int, int>(i, j);
                        // Check connections
                        AddToAdjacencyMatrix(map, coordinate2NodeNum, fromCoord, -1, 0); // UP
                        AddToAdjacencyMatrix(map, coordinate2NodeNum, fromCoord, +1, 0); // DOWN
                        AddToAdjacencyMatrix(map, coordinate2NodeNum, fromCoord, 0, -1); // LEFT
                        AddToAdjacencyMatrix(map, coordinate2NodeNum, fromCoord, 0, +1); // UP
                    }
                }
            }

            return "Adjacency matrix was created";
        }
    }
}
