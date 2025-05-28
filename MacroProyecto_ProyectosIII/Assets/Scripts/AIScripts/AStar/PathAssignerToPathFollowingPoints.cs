using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AIEngine.Movement.Components.Algorithms;

public class PathAssignerToPathFollowingPoints : MonoBehaviour
{
    [Header("Componentes de pathfinding")]
    public GraphComponent graphComponent;

    [Tooltip("Transform desde donde se buscará el nodo más cercano (ej: el jugador)")]
    public Transform pathOrigin;

    [Tooltip("Script que rastrea el nodo más cercano al objetivo en movimiento")]
    public NearestNodeTracker dynamicTargetTracker;

    [Header("Destino del path")]
    public PathFollowingPoints pathFollowing;

    [Header("Opciones de actualización")]
    public bool autoUpdate = true;
    public float updateInterval = 1f;

    private float timer;

    private IEnumerator Start()
    {
        if (graphComponent == null || pathFollowing == null || pathOrigin == null || dynamicTargetTracker == null)
        {
            Debug.LogError("Faltan referencias en PathAssignerToPathFollowingPoints.");
            enabled = false;
            yield break;
        }

        timer = updateInterval;

        yield return null;

        AssignPath(); 
    }

    private void Update()
    {
        if (!autoUpdate) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            AssignPath();
            timer = updateInterval;
        }
    }

    public void AssignPath()
    {
        Graph graph = graphComponent.BuildGraph();

        Node closestNode = FindClosestNode(graph, pathOrigin.position);

        NodeComponent dynamicNodeComp = dynamicTargetTracker.GetCurrentNearestNode();
        if (dynamicNodeComp == null)
        {
            Debug.LogWarning("[PathAssigner] No se ha detectado aún el nodo más cercano al destino dinámico.");
            return;
        }

        Node endNode = graph.nodes.Find(n => n.id == dynamicNodeComp.nodeId);

        if (closestNode == null || endNode == null)
        {
            Debug.LogWarning("[PathAssigner] Nodo más cercano o nodo destino inválido.");
            return;
        }

        AStar astar = new AStar();
        List<Node> path = astar.FindPath(graph, closestNode, endNode);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[PathAssigner] No se encontró un camino entre nodos.");
            return;
        }

        Vector2[] unityVertexes = new Vector2[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            unityVertexes[i] = new Vector2(path[i].position.X, path[i].position.Y);
        }

        pathFollowing.unityVertexes = unityVertexes;
        pathFollowing.InitializePath();


        CmpPathFollowing cmpPathFollowing = pathFollowing.GetComponent<CmpPathFollowing>();

        if (cmpPathFollowing != null)
        {
            cmpPathFollowing.UpdatePath(pathFollowing);
        }
    }

    private Node FindClosestNode(Graph graph, Vector3 origin)
    {
        Node closest = null;
        float minDist = float.MaxValue;

        foreach (var node in graph.nodes)
        {
            Vector3 nodePos = new Vector3(node.position.X, 0, node.position.Y);
            float dist = Vector3.Distance(origin, nodePos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}
