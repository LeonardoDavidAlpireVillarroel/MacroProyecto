using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicAlign : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAngularAcceleration;
        [SerializeField] private float maxRotation;
        [SerializeField] private float targetRange;
        [SerializeField] private float decelerationRange;
        [SerializeField] private float timeToTarget;

        private DynamicAlignAlgorithm DynamicAlign;

        private void Start()
        {
            DynamicAlign = new DynamicAlignAlgorithm(maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget);

            var myself = GetComponent<CmpKinematic>();
            DynamicAlign.SetAgent(myself.GetAgent());
            DynamicAlign.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return DynamicAlign.GetSteering();
        }
    }
}
