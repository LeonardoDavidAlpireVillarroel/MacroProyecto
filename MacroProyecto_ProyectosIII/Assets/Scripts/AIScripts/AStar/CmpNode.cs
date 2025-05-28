using UnityEngine;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;

public class NodeComponent : MonoBehaviour
{
    public int nodeId;

    public Node ToNode()
    {
        return new Node(nodeId, new Vector2(transform.position.x, transform.position.z));
    }
}
