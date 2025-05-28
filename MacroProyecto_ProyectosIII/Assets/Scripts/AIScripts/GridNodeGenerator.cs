using UnityEngine;
using UnityEditor; // Para MonoScript
using System;

public class GridNodeGenerator : MonoBehaviour
{
    [SerializeField] private MonoScript scriptToAttach; // Arrastra aquí el script que quieres añadir a cada objeto
    public Vector3 startPoint = Vector3.zero;
    public float width = 20f;
    public float depth = 20f;
    public float spacing = 1f;

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (scriptToAttach == null)
        {
            Debug.LogError("Por favor arrastra un script al campo 'Script To Attach'.");
            return;
        }

        Type scriptType = scriptToAttach.GetClass();
        if (scriptType == null || !typeof(MonoBehaviour).IsAssignableFrom(scriptType))
        {
            Debug.LogError("El script asignado no es un MonoBehaviour válido.");
            return;
        }

        // Limpiar objetos anteriores
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        int countX = Mathf.FloorToInt(width / spacing) + 1;
        int countZ = Mathf.FloorToInt(depth / spacing) + 1;

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 position = startPoint + new Vector3(x * spacing, 0, z * spacing);
                GameObject node = new GameObject($"nodeComponent_{x}_{z}");
                node.transform.SetParent(transform);
                node.transform.position = position;

                // Añadir el script seleccionado
                node.AddComponent(scriptType);
            }
        }
    }
}
