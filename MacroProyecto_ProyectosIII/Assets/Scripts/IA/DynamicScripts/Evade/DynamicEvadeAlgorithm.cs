using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicEvadeAlgorithm : DynamicFleeAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic target;

        float limitPrediction;

        //CONSTRUCTOR
        public DynamicEvadeAlgorithm(float maxAcceleration, float thresholdDistance, float limitPrediction)
            : base(maxAcceleration, thresholdDistance)
        {
            this.limitPrediction = limitPrediction;
        }

        public new void SetAgent(AgKinematic agent) { this.agent = agent; }
        public new void SetTarget(AgKinematic target) { this.target = target; }


        public override SteeringOutput GetSteering()
        {
            Vector2 direction = target.position - agent.position;

            float distance = direction.Length();

            float currentSpeed = agent.velocity.Length();

            float finalPrediction;

            if (distance < 3.0f) // (elige un número razonable)
            {
                finalPrediction = 0f; // no predecir
            }
            else if (currentSpeed < (distance / limitPrediction))
            {
                finalPrediction = limitPrediction;
            }
            else
            {
                finalPrediction = distance / currentSpeed;
            }
            // Predecir futura posición
            Vector2 predictedPosition = target.position + target.velocity * finalPrediction;

            // Crear un "target fantasma" para el Seek
            AgKinematic predictedTarget = new AgKinematic(
                predictedPosition,
                target.orientation,
                target.velocity,
                target.rotation
            );

            // Actualizar el target de Seek
            base.SetAgent(this.agent);
            base.SetTarget(predictedTarget);

            // Ahora sí, usar el Seek normal para perseguir la posición predicha
            return base.GetSteering();
        }
    }
}
