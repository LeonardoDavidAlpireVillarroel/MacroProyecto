using System.Numerics;
using System.Collections.Generic;

public class Node
{
    public int id;
    public Vector2 position;
    public List<Node> neighbors = new List<Node>();

    public Node(int id, Vector2 position)
    {
        this.id = id;
        this.position = position;
    }
}
