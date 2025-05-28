
using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpPathFollowing : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float thresholdDistance;
        [SerializeField] private float objectivePhase;

        private PathFollowingPoints path;
        private int index;
        private PathFollowingAlgorithm pathFollowing;

        private CmpStatic cmpStatic;

        private void Start()
        {
            cmpStatic = GetComponent<CmpStatic>();
        }

        //public void UpdatePath(PathFollowingPoints newPath)
        //{
        //    if (newPath == null || newPath.Length == 0)
        //    {
        //        Debug.Log(newPath.Length);
        //        Debug.LogWarning("[CmpPathFollowing] Path nulo o vacío.");
        //        return;
        //    }

        //    path = newPath;
        //    index = 0;
        //    pathFollowing = new PathFollowingAlgorithm(maxSpeed, thresholdDistance, path, objectivePhase, index);
        //    pathFollowing.SetAgent(cmpStatic.GetAgent());

        //    Debug.Log($"[CmpPathFollowing] Path actualizado: {path.Length} puntos");
        //}

        //public SteeringOutput GetSteering()
        //{
        //    if (pathFollowing == null)
        //        return new SteeringOutput();

        //    return pathFollowing.GetSteering();
        //}

        private bool isActive = false;

        public void UpdatePath(PathFollowingPoints newPath)
        {
            if (newPath == null || newPath.Length == 0)
            {
                Debug.LogWarning("[CmpPathFollowing] Path nulo o vacío.");
                isActive = false;
                return;
            }

            path = newPath;
            index = 0;
            pathFollowing = new PathFollowingAlgorithm(maxSpeed, thresholdDistance, path, objectivePhase, index);
            pathFollowing.SetAgent(cmpStatic.GetAgent());

            isActive = true;

        }

        public void StopFollowing()
        {
            isActive = false;
            pathFollowing = null;
        }

        public SteeringOutput GetSteering()
        {
            if (!isActive || pathFollowing == null)
                return new SteeringOutput(); // sin movimiento

            return pathFollowing.GetSteering();
        }
    }
}
