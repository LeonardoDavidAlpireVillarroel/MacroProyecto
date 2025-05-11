using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpArrive : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private CmpStatic target;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float radius;
        [SerializeField] private float timeToTarget;

        private ArriveAlgorithm arrive;

        private void Start()
        {
            arrive = new ArriveAlgorithm(maxSpeed, radius, timeToTarget);

            var myself = GetComponent<CmpStatic>();
            arrive.SetAgent(myself.GetAgent());
            arrive.SetTarget(target.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return arrive.GetSteering();
        }
    }
}
