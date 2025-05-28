using System.Collections.Generic;
using System.Numerics;

public class Graph
{
    public List<Node> nodes = new List<Node>();
    public List<Connection> connections = new List<Connection>();

    public List<Connection> GetConnections(Node node)
    {
        List<Connection> result = new List<Connection>();
        foreach (var conn in connections)
        {
            if (conn.from == node)
                result.Add(conn);
        }
        return result;
    }

    public Node FindClosestNode(Vector2 position)
    {
        Node closest = null;
        float minDistance = float.PositiveInfinity;

        foreach (Node node in nodes)
        {
            float distance = Vector2.Distance(position, node.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        return closest;
    }
}
