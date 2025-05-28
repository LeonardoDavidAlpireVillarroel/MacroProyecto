using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class ArriveAlgorithm
    {
        private AgStatic agent;
        private AgStatic target;

        private float maxSpeed;
        private float radius;
        private float timeToTarget;

        public ArriveAlgorithm(float maxSpeed, float radius, float timeToTarget)
        {
            this.maxSpeed = maxSpeed;
            this.radius = radius;
            this.timeToTarget = timeToTarget;
        }

        public void SetAgent(AgStatic agent) => this.agent = agent;
        public void SetTarget(AgStatic target) => this.target = target;

        public SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();

            Vector2 direction = target.position - agent.position;
            float distance = direction.Length();

            if (distance < radius)
            {
                sOut.linear = Vector2.Zero;
                sOut.angular = 0f;
                return sOut;
            }

            Vector2 desiredVelocity = direction / timeToTarget;

            if (desiredVelocity.Length() > maxSpeed)
            {
                desiredVelocity = Vector2.Normalize(desiredVelocity) * maxSpeed;
            }

            sOut.linear = desiredVelocity;

            // Orientación hacia la dirección de movimiento
            float desiredOrientation = ForceOrientation(agent.orientation, desiredVelocity);
            float rotation = desiredOrientation - agent.orientation;
            sOut.angular = MapToRange(rotation);

            return sOut;
        }

        private float ForceOrientation(float current, Vector2 velocity)
        {
            return velocity.LengthSquared() > 0.0001f ? MathF.Atan2(velocity.Y, velocity.X) : current;
        }

        private float MapToRange(float radians)
        {
            const float TWO_PI = 2 * MathF.PI;
            while (radians > MathF.PI) radians -= TWO_PI;
            while (radians < -MathF.PI) radians += TWO_PI;
            return radians;
        }
    }
}
