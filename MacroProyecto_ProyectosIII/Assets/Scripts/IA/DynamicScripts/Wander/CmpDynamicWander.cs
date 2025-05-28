using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicWander : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private float maxAngularAcceleration;
        [SerializeField] private float maxRotation;
        [SerializeField] private float targetRange;
        [SerializeField] private float decelerationRange;
        [SerializeField] private float timeToTarget;
        [SerializeField] private float wanderDistance;
        [SerializeField] private float wanderRadius;
        [SerializeField] private float maxOrientationRatio;
        [SerializeField] private float maxAcceleration;

        private DynamicWanderAlgorithm dynamicWander;

        private void Start()
        {
            dynamicWander = new DynamicWanderAlgorithm(
                maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget,
                wanderDistance, wanderRadius, maxOrientationRatio, maxAcceleration);

            var myself = GetComponent<CmpKinematic>();
            dynamicWander.SetAgent(myself.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return dynamicWander.GetSteering();
        }
    }
}
