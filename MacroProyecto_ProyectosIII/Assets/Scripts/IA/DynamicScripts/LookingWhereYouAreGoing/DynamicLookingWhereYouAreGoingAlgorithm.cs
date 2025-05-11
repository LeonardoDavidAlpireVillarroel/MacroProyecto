using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.ComponentModel;
using System.Numerics;
using UnityEngine.UIElements;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicLookingWhereYouAreGoingAlgorithm : DynamicAlignAlgorithm
        {
        private AgKinematic agent;
        private AgKinematic target;

        private float maxAngularAcceleration;
        private float maxRotation;
        private float targetRange;
        private float decelerationRange;
        private float timeToTarget;
        public DynamicLookingWhereYouAreGoingAlgorithm(float maxAngularAcceleration, float maxRotation, float targetRange, float decelerationRange, float timeToTarget)
            : base(maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget)
        {
        }

        public new void SetAgent(AgKinematic agent) => this.agent = agent;
        public new void SetTarget(AgKinematic target) => this.target = target;

        public override SteeringOutput GetSteering()
        {
            if (agent == null || agent.velocity.LengthSquared() < 0.0001f)
                return new SteeringOutput(); 

            float orientation = (float)Math.Atan2(agent.velocity.X, agent.velocity.Y);

            AgKinematic target = new AgKinematic(agent.position, orientation, Vector2.Zero, 0f);

            base.SetAgent(agent);
            base.SetTarget(target);

            return base.GetSteering();
        }
    }
}

