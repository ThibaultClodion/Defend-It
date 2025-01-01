using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Tester
public static class AStar
{
    static List<int> wallValues = new List<int>()
    {
        (int)MapGrid.CellType.WALL,
        (int)MapGrid.CellType.CLOSEDOOR,
        (int)MapGrid.CellType.OPENDOOR
    };

    public class Node
    {
        public Vector2Int position { get; set; }
        public int DistanceStart { get; set; }
        public int DistanceEnd { get; set; }
        public bool isPast { get; set; }
    }

    public static List<Vector2> execute(int[,] map, Vector2Int posStart, Vector2Int posEnd)
    {
        List<Vector2> result = new List<Vector2>();
        List<Node> nodes = InitializeDijkstraList(map, posStart, posEnd);
        int indice = 0;
        int iter = 0;
        do
        {
            iter++;
            indice = GetFirst(nodes);
            if (nodes[indice].DistanceEnd.Equals(0))
            {
                nodes[indice].isPast = true;
                List<Node> path = GetBestPath(nodes);
                foreach (Node nodeBestPath in path)
                {
                    result.Add(nodeBestPath.position);
                }
                return result;
            }

            AddVoisin(ref nodes, indice, posStart, posEnd);
            nodes[indice].isPast = true;
        } while (iter < map.LongLength);

        List<Node> bestPath = GetBestPath(nodes);
        foreach (Node nodeBestPath in bestPath)
        {
            result.Add(nodeBestPath.position);
        }
        return result;
    }

    // Probleme pour dépasser en Z
    private static List<Node> GetBestPath(List<Node> nodes)
    {
        List<Node> result = new List<Node>();
        List<Node> filtredNodes = nodes.Where(x => x.isPast == true).ToList();
        List<Node> neighbours = new List<Node>();
        Node currentNode = null;

        foreach (Node node in filtredNodes)
        {
            if (node.DistanceEnd == 0)
            {
                currentNode = node;
                result.Add(currentNode);
                filtredNodes.Remove(currentNode);
                break;
            }
        }

        int it = 0;
        while (it < nodes.Count / 2)
        {
            neighbours = GetNeighbours(currentNode, filtredNodes);
            if (neighbours.Count == 0)
            {
                result.Remove(currentNode);
                currentNode = result[result.Count - 1];
            }
            else
            {
                neighbours = neighbours.OrderBy(x => x.DistanceStart).ToList();
                currentNode = neighbours[0];
                result.Add(currentNode);
            }

            filtredNodes.Remove(currentNode);
            it++;
            if (currentNode.DistanceStart == 1)
            {
                break;
            }
        }
        return result;
    }

    private static List<Node> GetNeighbours(Node element, List<Node> potentialNeighbours)
    {
        List<Node> neighbours = new List<Node>();
        foreach (Node potentialNeighbour in potentialNeighbours)
        {
            int distance = (int)GetDistance(element.position, potentialNeighbour.position);
            if (distance > 0 && distance < 2)
            {
                neighbours.Add(potentialNeighbour);
            }
        }
        return neighbours;
    }

    private static List<Node> InitializeDijkstraList(int[,] map, Vector2 posStart, Vector2 posEnd)
    {
        List<Node> result = new List<Node>();
        for (int x = 0; x < map.GetLongLength(0); x++)
        {
            for (int y = 0; y < map.GetLongLength(1); y++)
            {
                if (!wallValues.Contains(map[x, y]))
                {
                    Node node = new Node();
                    node.position = new Vector2Int(x, y);
                    if (GetDistance(node.position, posStart).Equals(0f))
                    {
                        node.DistanceStart = 0;
                        node.DistanceEnd = Mathf.RoundToInt(GetDistance(posStart, posEnd)); ;
                    }
                    else
                    {
                        node.DistanceStart = int.MaxValue / 4;
                        node.DistanceEnd = int.MaxValue / 4;
                    }
                    node.isPast = false;
                    result.Add(node);
                }
            }
        }
        return result;
    }

    private static float GetDistance(Vector2 vector, Vector2 vector2)
    {
        return Mathf.Abs(vector.x - vector2.x) + Mathf.Abs(vector.y - vector2.y);
    }

    private static int GetFirst(List<Node> nodes)
    {
        int min = int.MaxValue;
        int minDistanceEnd = int.MaxValue;
        List<int> indices = new List<int>();

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].isPast)
            {
                continue;
            }
            if (min > nodes[i].DistanceEnd + nodes[i].DistanceStart)
            {
                min = nodes[i].DistanceEnd + nodes[i].DistanceStart;
                minDistanceEnd = nodes[i].DistanceEnd;
                indices.Clear();
                indices.Add(i);
            }
            else if (min == nodes[i].DistanceEnd + nodes[i].DistanceStart)
            {
                if (minDistanceEnd > nodes[i].DistanceEnd)
                {
                    minDistanceEnd = nodes[i].DistanceEnd;
                    indices.Clear();
                    indices.Add(i);
                }
                else if (minDistanceEnd == nodes[i].DistanceEnd)
                {
                    indices.Add(i);
                }
            }
        }
        return indices[Random.Range(0, indices.Count)];
    }

    private static void AddVoisin(ref List<Node> list, int indice, Vector2 posStart, Vector2 posEnd)
    {
        Vector2 position = list[indice].position;
        for (int i = 0; i < list.Count; i++)
        {
            if (Mathf.RoundToInt(GetDistance(position, list[i].position)).Equals(1))
            {
                list[i].DistanceStart = Mathf.RoundToInt(GetDistance(list[i].position, posStart));
                list[i].DistanceEnd = Mathf.RoundToInt(GetDistance(list[i].position, posEnd));
            }
        }
    }

    private static bool VectorInList(List<Node> list, Vector2 vector)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].position.x.Equals(vector.x) && list[i].position.y.Equals(vector.y))
                return true;
        }
        return false;
    }

    private static bool AllIsCheck(List<Node> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isPast)
            {
                return false;
            }
        }
        return true;
    }
}
