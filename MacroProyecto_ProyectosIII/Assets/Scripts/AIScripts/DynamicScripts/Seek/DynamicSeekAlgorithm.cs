using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicSeekAlgorithm
    {
        AgKinematic agent;
        AgKinematic target;

        protected float maxAcceleration;
        protected float thresholdDistance; // Distancia mínima para frenar

        //CONSTRUCTOR
        public DynamicSeekAlgorithm(float maxAcceleration,float thresholdDistance)
        {
            this.maxAcceleration = maxAcceleration;
            this.thresholdDistance = thresholdDistance;
        }

        public void SetAgent(AgKinematic agent) { this.agent = agent; }
        public void SetTarget(AgKinematic target) { this.target = target; }


        public virtual SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();
            sOut.linear= target.position - agent.position;

            Vector2 direction = target.position - agent.position;

            // Si está muy cerca, frenamos
            if (direction.Length() < thresholdDistance)
            {
                agent.velocity = Vector2.Zero;
                sOut.linear = Vector2.Zero;
                sOut.angular = 0f;
                return sOut;
            }

            direction = Vector2.Normalize(direction);
            sOut.linear = direction * maxAcceleration;
            sOut.angular = 0f;

            return sOut;
        }
    }
}
