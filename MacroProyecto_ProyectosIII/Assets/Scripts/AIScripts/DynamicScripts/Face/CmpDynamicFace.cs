using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicFace : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAngularAcceleration;
        [SerializeField] private float maxRotation;
        [SerializeField] private float targetRange;
        [SerializeField] private float decelerationRange;
        [SerializeField] private float timeToTarget;

        private DynamicFaceAlgorithm DynamicFace;

        private void Start()
        {
            DynamicFace = new DynamicFaceAlgorithm(maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget);

            var myself = GetComponent<CmpKinematic>();
            DynamicFace.SetAgent(myself.GetAgent());
            DynamicFace.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return DynamicFace.GetSteering();
        }
    }
}
