using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicSeek : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float thresholdDistance;

        private DynamicSeekAlgorithm seek;
        private void Start()
        {
            seek = new DynamicSeekAlgorithm(maxAcceleration,thresholdDistance);

            var myself = gameObject.GetComponent<CmpKinematic>();
            seek.SetAgent(myself.GetAgent());
            seek.SetTarget(target.GetAgent());
        }
        public SteeringOutput GetSteering()
        {
            return seek.GetSteering();
        }
    }
}
