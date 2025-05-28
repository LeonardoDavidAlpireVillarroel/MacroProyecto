using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicFlee : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float thresholdDistance;

        private DynamicFleeAlgorithm flee;
        private void Start()
        {
            flee = new DynamicFleeAlgorithm(maxAcceleration, thresholdDistance);

            var myself = gameObject.GetComponent<CmpKinematic>();
            flee.SetAgent(myself.GetAgent());
            flee.SetTarget(target.GetAgent());
        }
        public SteeringOutput GetSteering()
        {
            return flee.GetSteering();
        }
    }
}
