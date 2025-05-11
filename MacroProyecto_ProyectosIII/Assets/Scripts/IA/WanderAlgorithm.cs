using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class WanderAlgorithm
    {
        private AgStatic agent;
        private float maxSpeed;
        private float maxAngularSpeed;

        private Random random;

        public WanderAlgorithm(float maxSpeed, float maxAngularSpeed)
        {
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            random = new Random();
        }

        public void SetAgent(AgStatic agent) { this.agent = agent; }

        public SteeringOutput GetSteering()
        {
            SteeringOutput sOut = new SteeringOutput();

            // Dirección según orientación actual
            Vector2 orientationVec = new Vector2(MathF.Cos(agent.orientation), MathF.Sin(agent.orientation));
            sOut.linear = orientationVec * maxSpeed;

            // Dirección angular aleatoria: -1 o 1
            float randomDir = random.Next(0, 2) == 0 ? -1f : 1f;
            sOut.angular = maxAngularSpeed * randomDir;

            return sOut;
        }
    }
}
