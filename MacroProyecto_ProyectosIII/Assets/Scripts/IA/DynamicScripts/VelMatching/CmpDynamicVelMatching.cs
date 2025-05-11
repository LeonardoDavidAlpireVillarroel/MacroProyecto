using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicVelMatching : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float timeToTarget;

        private DynamicVelMatchingAlgorithm DynamicVelMatching;

        private void Start()
        {
            DynamicVelMatching = new DynamicVelMatchingAlgorithm(maxAcceleration, timeToTarget);

            var myself = GetComponent<CmpKinematic>();
            DynamicVelMatching.SetAgent(myself.GetAgent());
            DynamicVelMatching.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return DynamicVelMatching.GetSteering();
        }
    }
}
