using UnityEngine;

public class NearestNodeTracker : MonoBehaviour
{
    [Tooltip("Referencia al GraphComponent en la escena.")]
    public GraphComponent graph;

    private NodeComponent nearestNode;

    void Update()
    {
        if (graph == null || graph.nodes == null || graph.nodes.Length == 0)
        {
            //Debug.LogWarning("[NearestNodeTracker] No se ha asignado un grafo o no hay nodos.");
            return;
        }

        float closestDistance = float.MaxValue;
        NodeComponent closest = null;

        foreach (var node in graph.nodes)
        {
            float distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }

        if (closest != nearestNode)
        {
            nearestNode = closest;
        }
    }

    public NodeComponent GetCurrentNearestNode()
    {
        return nearestNode;
    }
}
