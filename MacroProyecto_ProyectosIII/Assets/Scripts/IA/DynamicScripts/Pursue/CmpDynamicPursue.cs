using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpKinematic))]
    public class CmpDynamicPursue : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpKinematic target;
        [SerializeField] private float maxAcceleration;
        [SerializeField] private float thresholdDistance;
        [SerializeField] private float limitPrediction; // Límite de predicción

        private DynamicPursueAlgorithm pursue;
        private void Start()
        {
            pursue = new DynamicPursueAlgorithm(maxAcceleration, thresholdDistance, limitPrediction);

            var myself = gameObject.GetComponent<CmpKinematic>();
            pursue.SetAgent(myself.GetAgent());
            pursue.SetTarget(target.GetAgent());
        }
        public SteeringOutput GetSteering()
        {
            return pursue.GetSteering();
        }
    }
}
