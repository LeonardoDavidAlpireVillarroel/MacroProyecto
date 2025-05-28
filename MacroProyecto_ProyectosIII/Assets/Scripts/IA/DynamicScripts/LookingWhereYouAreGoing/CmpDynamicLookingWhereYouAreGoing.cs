using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicLookingWhereYouAreGoing : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAngularAcceleration;
        [SerializeField] private float maxRotation;
        [SerializeField] private float targetRange;
        [SerializeField] private float decelerationRange;
        [SerializeField] private float timeToTarget;

        private DynamicLookingWhereYouAreGoingAlgorithm DynamicLookingWhereYouAreGoing;

        private void Start()
        {
            DynamicLookingWhereYouAreGoing = new DynamicLookingWhereYouAreGoingAlgorithm(maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget);

            var myself = GetComponent<CmpKinematic>();
            DynamicLookingWhereYouAreGoing.SetAgent(myself.GetAgent());
            DynamicLookingWhereYouAreGoing.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return DynamicLookingWhereYouAreGoing.GetSteering();
        }
    }
}
