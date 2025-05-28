using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpSeek : MonoBehaviour,ISteeringProvider
    {
        [SerializeField] private CmpStatic target;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxAngularSpeed;

        private SeekAlgorithm seek;
        private void Start()
        {
            seek = new SeekAlgorithm(maxSpeed, maxAngularSpeed);

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
