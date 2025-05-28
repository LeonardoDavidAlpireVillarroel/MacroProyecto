using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpFlee : MonoBehaviour,ISteeringProvider
    {
        [SerializeField] private CmpStatic target;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxAngularSpeed;
        [SerializeField] private float panicRadius = 5f;


        private FleeAlgorithm seek;
        private void Start()
        {
            seek = new FleeAlgorithm(maxSpeed, maxAngularSpeed, panicRadius);

            var myself = gameObject.GetComponent<CmpStatic>();
            seek.SetAgent(myself.GetAgent());
            seek.SetTarget(target.GetAgent());
        }
        public SteeringOutput GetSteering()
        {
            return seek.GetSteering();
        }
    }
}
