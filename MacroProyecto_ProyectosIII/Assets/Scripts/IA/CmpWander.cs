using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpWander : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float maxAngularSpeed = 2f;

        private WanderAlgorithm wander;

        private void Start()
        {
            wander = new WanderAlgorithm(maxSpeed, maxAngularSpeed);

            var myself = GetComponent<CmpStatic>();
            wander.SetAgent(myself.GetAgent());
        }

        public SteeringOutput GetSteering()
        {
            return wander.GetSteering();
        }
    }
}
