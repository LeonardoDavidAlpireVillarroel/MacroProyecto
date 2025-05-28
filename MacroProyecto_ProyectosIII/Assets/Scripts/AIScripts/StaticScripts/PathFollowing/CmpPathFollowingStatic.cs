using AIEngine.Movement.Agents;
using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpPathFollowingStatic : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float thresholdDistance = 0.5f;
        [SerializeField] private float objectivePhase = 0f;

        [SerializeField] private PathFollowingPointsStatic path;

        private PathFollowingAlgorithmStatic pathFollowingAlgorithm;
        private CmpStatic cmpStatic;

        private int index;

        private void Start()
        {
            cmpStatic = GetComponent<CmpStatic>();

            index = 0;
            pathFollowingAlgorithm = new PathFollowingAlgorithmStatic(
                maxSpeed,
                thresholdDistance,
                path,
                objectivePhase,
                index
            );
            pathFollowingAlgorithm.SetAgent(cmpStatic.GetAgent());

            Debug.Log($"[CmpPathFollowing] Path inicializado: {path.Length} puntos");
        }

        public SteeringOutput GetSteering()
        {
            if (pathFollowingAlgorithm == null)
                return new SteeringOutput();

            var output = pathFollowingAlgorithm.GetSteering();

            index = pathFollowingAlgorithm.CurrentIndex;

            return output;
        }
    }
}
