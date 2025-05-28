using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GraphComponent : MonoBehaviour
{
    [Header("Carga automática")]
    public NodeComponent[] nodes;
    public ConnectionComponent[] connections;

    [ContextMenu("Auto-popular nodos y conexiones")]
    public void AutoPopulate()
    {
        nodes = GetComponentsInChildren<NodeComponent>(true);
        connections = GetComponentsInChildren<ConnectionComponent>(true);

        Debug.Log($"[GraphComponent] Nodos encontrados: {nodes.Length}, Conexiones encontradas: {connections.Length}", this);
    }

    [ContextMenu("Asignar IDs únicos a nodos")]
    public void AssignUniqueNodeIds()
    {
        AutoPopulate(); // Asegura que la lista esté actualizada

        int id = 0;
        HashSet<int> usedIds = new HashSet<int>();

        foreach (var node in nodes)
        {
            // Evita asignar IDs duplicados
            while (usedIds.Contains(id))
                id++;

            node.nodeId = id;
            usedIds.Add(id);
            id++;
        }

        Debug.Log($"[GraphComponent] IDs únicos asignados a {nodes.Length} nodos.");
    }

    public Graph BuildGraph()
    {
        var graph = new Graph();
        var nodeMap = new Dictionary<int, Node>();

        // Crear nodos
        foreach (var nodeComp in nodes)
        {
            var node = nodeComp.ToNode();
            nodeMap[node.id] = node;
            graph.nodes.Add(node);
        }

        // Crear conexiones (y sus opuestas)
        foreach (var connComp in connections)
        {
            if (!nodeMap.ContainsKey(connComp.from.nodeId) || !nodeMap.ContainsKey(connComp.to.nodeId))
            {
                Debug.LogWarning($"[GraphComponent] Conexión inválida entre nodos no registrados: {connComp.from?.nodeId} → {connComp.to?.nodeId}", connComp);
                continue;
            }

            var fromNode = nodeMap[connComp.from.nodeId];
            var toNode = nodeMap[connComp.to.nodeId];

            // Conexión original
            var conn = new Connection(fromNode, toNode, connComp.cost);
            graph.connections.Add(conn);

            // Conexión inversa
            var reverseConn = new Connection(toNode, fromNode, connComp.cost);
            graph.connections.Add(reverseConn);
        }

        return graph;
    }


    [ContextMenu("Conectar nodos automáticamente")]
    public void AutoConnectNeighbors()
    {
        float maxDistance = 7.1f;

        AutoPopulate();

        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogWarning("No hay nodos para conectar.");
            return;
        }

        int totalCreated = 0;

        for (int i = 0; i < nodes.Length; i++)
        {
            for (int j = i + 1; j < nodes.Length; j++)
            {
                var nodeA = nodes[i];
                var nodeB = nodes[j];

                float distance = Vector3.Distance(nodeA.transform.position, nodeB.transform.position);
                if (distance <= maxDistance)
                {
                    bool alreadyConnected = false;
                    foreach (var conn in connections)
                    {
                        if ((conn.from == nodeA && conn.to == nodeB) || (conn.from == nodeB && conn.to == nodeA))
                        {
                            alreadyConnected = true;
                            break;
                        }
                    }

                    if (!alreadyConnected)
                    {
                        var connGO = new GameObject($"Connection_{nodeA.nodeId}_{nodeB.nodeId}");
                        connGO.transform.SetParent(this.transform);
                        var connComp = connGO.AddComponent<ConnectionComponent>();
                        connComp.from = nodeA;
                        connComp.to = nodeB;
                        connComp.cost = distance;

                        totalCreated++;
                    }
                }
            }
        }

        AutoPopulate();
        Debug.Log($"[GraphComponent] Conectados automáticamente {totalCreated} pares de nodos.");
    }
}
