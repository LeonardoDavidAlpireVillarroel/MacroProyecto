using AIEngine.Movement.Agents;
using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicPathFollowing : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float thresholdDistance;
        [SerializeField] private PathFollowing path;
        [SerializeField] private float objectivePhase;
        private int index;

        private DynamicPathFollowingAlgorithm pathFollowing;

        private void Start()
        {
            pathFollowing = new DynamicPathFollowingAlgorithm(maxAcceleration, thresholdDistance, path, objectivePhase,index);

            var myself = GetComponent<CmpKinematic>();
            pathFollowing.SetAgent(myself.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return pathFollowing.GetSteering();
        }

    }
}
