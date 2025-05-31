using UnityEngine;

public class GridNodeGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector3 startPoint = Vector3.zero;
    public float width = 20f;
    public float depth = 20f;
    public float spacing = 1f;

    [Header("Script Template")]
    [Tooltip("Arrastra aquí un GameObject con el script que quieras usar como plantilla.")]
    public MonoBehaviour scriptTemplate; // El componente que quieres clonar

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (scriptTemplate == null)
        {
            Debug.LogError("Por favor arrastra un GameObject con el script deseado al campo 'Script Template'.");
            return;
        }

        System.Type scriptType = scriptTemplate.GetType();

        // Limpiar hijos anteriores
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

                node.AddComponent(scriptType); // Añade el tipo seleccionado
            }
        }
    }
}
