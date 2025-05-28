using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicVelMatchingAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic target;

        private float maxAcceleration;
        private float timeToTarget;
        public DynamicVelMatchingAlgorithm(float maxAcceleration, float timeToTarget)
        {
            this.maxAcceleration = maxAcceleration;
            this.timeToTarget = timeToTarget;
        }

        public void SetAgent(AgKinematic agent) => this.agent = agent;
        public void SetTarget(AgKinematic target) => this.target = target;

        public SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();


            Vector2 velocityDifference = target.velocity - agent.velocity;

            sOut.linear = velocityDifference / timeToTarget;

            if (sOut.linear.Length() > maxAcceleration)
            {
                sOut.linear = Vector2.Normalize(sOut.linear) * maxAcceleration;
            }

            return sOut;
        }

    }


}
