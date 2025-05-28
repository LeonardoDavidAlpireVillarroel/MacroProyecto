using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicArriveAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic target;

        private float maxSpeed;
        private float maxAcceleration;
        private float targetRange;
        private float decelerationRange;
        private float timeToTarget;
        public DynamicArriveAlgorithm(float maxSpeed,float maxAcceleration,float targetRange, float decelerationRange, float timeToTarget)
        {
            this.maxSpeed = maxSpeed;
            this.maxAcceleration = maxAcceleration;
            this.targetRange = targetRange;
            this.decelerationRange = decelerationRange;
            this.timeToTarget = timeToTarget;
        }

        public void SetAgent(AgKinematic agent) => this.agent = agent;
        public void SetTarget(AgKinematic target) => this.target = target;

        public SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();

            Vector2 direction = target.position - agent.position;
            float distance = direction.Length();

            if (distance < targetRange)
            {
                agent.velocity = Vector2.Zero;
                sOut.linear = Vector2.Zero;
                sOut.angular = 0f;
                return sOut;
            }
            float targetSpeed;
            if (distance < decelerationRange)
            {
                // Interpolamos: velocidad proporcional a la distancia
                targetSpeed = maxSpeed * (distance / decelerationRange);
            }
            else
            {
                // Si está fuera del rango de desaceleración, usamos velocidad máxima
                targetSpeed = maxSpeed;
            }

            sOut.linear = Vector2.Normalize(direction) * targetSpeed;

            // Aceleración necesaria
            sOut.linear = (sOut.linear - agent.velocity) / timeToTarget;

            // Limitamos si excede la aceleración máxima
            if (sOut.linear.Length() > maxAcceleration)
            {
                sOut.linear = Vector2.Normalize(sOut.linear) * maxAcceleration;
            }

            sOut.angular = 0f;
            return sOut;

        }

    }
}
