using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using AIEngine.Utils;

public class PathFollowingPointsStatic:MonoBehaviour
    {
        public Vector2[] unityVertexes;
    private System.Numerics.Vector2[] numericsVertexes;
    public int Length => numericsVertexes?.Length ?? 0;

    private void Awake()
    {
        numericsVertexes = unityVertexes.ToNumericsArray();
    }

    public int GetClosestPoint(System.Numerics.Vector2 position, int lastVertex)
        {
            int closestVertex = lastVertex;
            float closestDistance = Vector2.Distance(position.ToUnity(), numericsVertexes[lastVertex].ToUnity());

            for (int i = 0; i < numericsVertexes.Length; i++)
            {
                float distance = Vector2.Distance(position.ToUnity(), numericsVertexes[i].ToUnity());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVertex = i;
                }
            }

            return closestVertex;
        }

    public System.Numerics.Vector2 GetPointPosition(int vertexIndex)
    {
        if (vertexIndex >= 0 && vertexIndex < numericsVertexes.Length)
        {
            return numericsVertexes[vertexIndex];
        }

        return System.Numerics.Vector2.Zero;
    }
}
