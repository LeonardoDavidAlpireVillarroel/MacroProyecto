using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicEvade : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float thresholdDistance;
        [SerializeField] private float limitPrediction; // Límite de predicción

        private DynamicEvadeAlgorithm evade;
        private void Start()
        {
            evade = new DynamicEvadeAlgorithm(maxAcceleration, thresholdDistance, limitPrediction);

            var myself = gameObject.GetComponent<CmpKinematic>();
            evade.SetAgent(myself.GetAgent());
            evade.SetTarget(target.GetAgent());
        }
        public SteeringOutput GetSteering()
        {
            return evade.GetSteering();
        }
    }
}
