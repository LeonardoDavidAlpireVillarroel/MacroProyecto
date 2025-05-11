using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicArrive : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float targetRange;
        [SerializeField] private float decelerationRange;
        [SerializeField] private float timeToTarget;

        private DynamicArriveAlgorithm DynamicArrive;

        private void Start()
        {
            DynamicArrive = new DynamicArriveAlgorithm(maxSpeed,maxAcceleration,targetRange, decelerationRange,timeToTarget);

            var myself = GetComponent<CmpKinematic>();
            DynamicArrive.SetAgent(myself.GetAgent());
            DynamicArrive.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return DynamicArrive.GetSteering();
        }
    }
}
