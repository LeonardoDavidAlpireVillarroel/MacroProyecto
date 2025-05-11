using UnityEngine;

public class PathFollowing:MonoBehaviour
{
    public Vector2[] vertexes;

    public int Length => vertexes?.Length ?? 0;

    public int GetClosestPoint(Vector2 position, int lastVertex)
    {
        int closestVertex = lastVertex;
        float closestDistance = Vector2.Distance(position, vertexes[lastVertex]);

        for (int i = 0; i < vertexes.Length; i++)
        {
            float distance = Vector2.Distance(position, vertexes[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVertex = i;
            }
        }

        return closestVertex;
    }

    public Vector2 GetPointPosition(int vertexIndex)
    {
        if (vertexIndex >= 0 && vertexIndex < vertexes.Length)
        {
            return vertexes[vertexIndex];
        }
        return Vector2.zero;
    }
}
