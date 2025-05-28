//using UnityEngine;
//using System.Collections.Generic;
//using System;
//using AIEngine.Movement.Components.Algorithms; // Ojo: ¡System.Numerics, no UnityEngine!

//public class AgentPathController : MonoBehaviour
//{
//    [Header("Pathfinding")]
//    public GraphComponent graphComponent;
//    public NodeComponent startNode;
//    public NodeComponent targetNode;

//    [Header("Movimiento del agente")]
//    public GraphComponent pathData; // El componente que contiene los puntos a seguir

//    public PathFollowing pathToFollow;


//    void Start()
//    {
//        if (graphComponent == null || startNode == null || targetNode == null || pathData == null)
//        {
//            Debug.LogError("Faltan referencias en AgentPathController.");
//            return;
//        }

//        // 1. Construir grafo
//        Graph graph = graphComponent.BuildGraph();


//        // 2. Calcular camino con A*
//        AStar astar = new AStar();
//        Node start = graph.nodes.Find(n => n.id == startNode.nodeId);
//        Node end = graph.nodes.Find(n => n.id == targetNode.nodeId);
//        Debug.Log($"StartNode ID: {startNode.nodeId}, TargetNode ID: {targetNode.nodeId}");


//        if (start == null || end == null)
//        {
//            Debug.LogError("Start o End node no encontrados en el grafo.");
//            return;
//        }

//        List<Node> path = astar.FindPath(graph, start, end);

//        Debug.Log(path.Count);
//        if (path == null || path.Count == 0)
//        {
//            Debug.LogWarning("No se encontró un camino.");
//            return;
//        }
//        Debug.Log(path.Count);

//        foreach (var node in path)
//        {
//            Debug.Log($"Nodo en path: id={node.id} pos=({node.position.X}, {node.position.Y})");
//        }
//        // 3. Convertir path a Vector2 de Unity
//        UnityEngine.Vector2[] unityPath = new UnityEngine.Vector2[path.Count];


//        Console.WriteLine(path.Count);


//        for (int i = 0; i < path.Count; i++)
//        {
//            unityPath[i] = new UnityEngine.Vector2(path[i].position.X, path[i].position.Y);
//        }

//        // 4. Asignar al componente PathFollowing del agente
//        pathToFollow.unityVertexes = unityPath;
//    }
//}
using UnityEngine;
using System.Collections.Generic;

public class AgentPathController : MonoBehaviour
{
    [Header("Pathfinding")]
    public GraphComponent graphComponent;
    public NodeComponent startNode;
    public NodeComponent targetNode;

    public float speed = 3f;

    private List<Vector3> pathPositions = new List<Vector3>();
    private int currentTargetIndex = 0;
    private bool pathReady = false;

    void Start()
    {
        if (graphComponent == null || startNode == null || targetNode == null)
        {
            Debug.LogError("Faltan referencias en AgentPathController.");
            return;
        }

        Graph graph = graphComponent.BuildGraph();

        AStar astar = new AStar();
        Node start = graph.nodes.Find(n => n.id == startNode.nodeId);
        Node end = graph.nodes.Find(n => n.id == targetNode.nodeId);

        if (start == null || end == null)
        {
            Debug.LogError("Start o End node no encontrados en el grafo.");
            return;
        }

        List<Node> path = astar.FindPath(graph, start, end);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No se encontró un camino.");
            return;
        }

        // Guardar posiciones de nodos para moverse
        pathPositions.Clear();
        foreach (var node in path)
        {
            // Si tus nodos tienen posición en Vector2, convierte a Vector3 (ejemplo: z=0)
            Vector3 pos = new Vector3(node.position.X, 0, node.position.Y);
            pathPositions.Add(pos);
        }

        currentTargetIndex = 0;
        pathReady = true;
    }

    void Update()
    {
        if (!pathReady || pathPositions.Count == 0)
            return;

        // Obtener posición objetivo actual
        Vector3 targetPos = pathPositions[currentTargetIndex];

        // Mover agente hacia el nodo actual
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Cuando llegue cerca al nodo actual, pasar al siguiente
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentTargetIndex++;

            // Si ya llegó al último nodo
            if (currentTargetIndex >= pathPositions.Count)
            {
                pathReady = false;
                Debug.Log("Llegó al destino final.");
            }
        }
    }
}
