using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicAlignAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic target;

        private float maxAngularAcceleration;
        private float maxRotation;
        private float targetRange;
        private float decelerationRange;
        private float timeToTarget;
        public DynamicAlignAlgorithm(float maxAngularAcceleration, float maxRotation, float targetRange, float decelerationRange, float timeToTarget)
        {
            this.maxAngularAcceleration = maxAngularAcceleration;
            this.maxRotation = maxRotation;
            this.targetRange = targetRange;
            this.decelerationRange = decelerationRange;
            this.timeToTarget = timeToTarget;
        }

        public void SetAgent(AgKinematic agent) => this.agent = agent;
        public void SetTarget(AgKinematic target) => this.target = target;

        public virtual SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();

            float rotation = target.orientation - agent.orientation;

            rotation = MapToMinusPiToPi(rotation);  // Aseguramos que esté en el intervalo [0, 2π]


            if (Math.Abs(rotation) < targetRange)
            {
                agent.rotation = 0f;
                agent.orientation = target.orientation;

                sOut.angular = 0f;
                sOut.linear = Vector2.Zero;
                return sOut;
            }

            float targetRotation;

            if (Math.Abs(rotation) < decelerationRange)
            {
                targetRotation = maxRotation * (Math.Abs(rotation) / decelerationRange);
            }
            else 
            {
                targetRotation = maxRotation;
            }

            sOut.angular = (targetRotation * rotation) / Math.Abs(rotation);

            sOut.angular = (sOut.angular - agent.rotation) / timeToTarget;

            if (Math.Abs(sOut.angular) > maxAngularAcceleration)
            {
                sOut.angular = (sOut.angular / Math.Abs(sOut.angular)) * maxAngularAcceleration;
            }
            sOut.linear = Vector2.Zero;

            return sOut;
        }

        public static float MapToMinusPiToPi(float angle)
        {
            angle = (float)(angle % (2 * Math.PI));
            if (angle < 0)
                angle += (float)(2 * Math.PI);
            if (angle > Math.PI)
                angle -= (float)(2 * Math.PI);
            return angle;
        }
    }
}
