using System;
using System.Collections.Generic;
using System.Numerics;

public class AStar
{
    class NodeRecord
    {
        public Node node;
        public Connection connection;
        public float costSoFar;
        public float estimatedTotalCost;

        public NodeRecord parent; // <-- Nuevo campo para backtracking
    }
    class NodeRecordComparer : IComparer<NodeRecord>
    {
        public int Compare(NodeRecord a, NodeRecord b)
        {
            return a.estimatedTotalCost.CompareTo(b.estimatedTotalCost);
        }
    }

    public List<Node> FindPath(Graph graph, Node start, Node goal)
    {
        List<NodeRecord> openList = new List<NodeRecord>();
        HashSet<Node> closedSet = new HashSet<Node>();

        var startRecord = new NodeRecord
        {
            node = start,
            connection = null,
            costSoFar = 0,
            estimatedTotalCost = Heuristic(start, goal)
        };

        openList.Add(startRecord);

        while (openList.Count > 0)
        {
            openList.Sort(new NodeRecordComparer());
            var current = openList[0];

            if (current.node == goal)
                return ReconstructPath(current);

            openList.RemoveAt(0);
            closedSet.Add(current.node);

            foreach (var connection in graph.GetConnections(current.node))
            {
                var toNode = connection.to;
                if (closedSet.Contains(toNode))
                    continue;

                float costSoFar = current.costSoFar + connection.cost;
                float estimatedTotal = costSoFar + Heuristic(toNode, goal);

                var existing = openList.Find(n => n.node == toNode);
                if (existing == null)
                {
                    openList.Add(new NodeRecord
                    {
                        node = toNode,
                        connection = connection,
                        costSoFar = costSoFar,
                        estimatedTotalCost = estimatedTotal,
                        parent = current  // Guardamos el nodo padre
                    });
                }
                else if (costSoFar < existing.costSoFar)
                {
                    existing.costSoFar = costSoFar;
                    existing.connection = connection;
                    existing.estimatedTotalCost = estimatedTotal;
                    existing.parent = current; // Actualizamos el padre también
                }
            }
        }

        return null; // No path found
    }

    private float Heuristic(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    private List<Node> ReconstructPath(NodeRecord endRecord)
    {
        List<Node> path = new List<Node>();
        NodeRecord current = endRecord;

        while (current != null)
        {
            path.Insert(0, current.node);
            current = current.parent;
        }

        return path;
    }
}
