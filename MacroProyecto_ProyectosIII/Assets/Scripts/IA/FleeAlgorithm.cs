using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class FleeAlgorithm
    {
        AgStatic agent;
        AgStatic target;

        float maxSpeed;
        float maxAngularSpeed;
        private float panicRadius;



        //CONSTRUCTOR
        public FleeAlgorithm(float maxSpeed, float maxAngularSpeed, float panicRadius)
        {
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.panicRadius = panicRadius;
        }

        public void SetAgent(AgStatic agent) { this.agent = agent; }
        public void SetTarget(AgStatic target) { this.target = target; }


        public SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();

            Vector2 direction = agent.position - target.position;
            float distance = direction.Length();

            if (distance > panicRadius)
            {
                // Si está lejos del objetivo, no se mueve
                sOut.linear = Vector2.Zero;
                sOut.angular = 0f;
                return sOut;
            }

            direction = Vector2.Normalize(direction);
            sOut.linear = direction * maxSpeed;

            float desiredOrientation = ForceOrientation(agent.orientation, sOut.linear);
            float rotation = desiredOrientation - agent.orientation;
            sOut.angular = MapToRange(rotation);

            sOut.angular = MathF.Max(-maxAngularSpeed, MathF.Min(sOut.angular, maxAngularSpeed));

            return sOut;
        }
        private float ForceOrientation(float current, Vector2 velocity)
        {
            if (velocity.LengthSquared() > 0.0001f)
            {
                return MathF.Atan2(velocity.Y, velocity.X);
            }
            return current;
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

