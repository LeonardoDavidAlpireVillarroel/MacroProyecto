using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicPathFollowingAlgorithm : DynamicSeekAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic reusableTarget = new AgKinematic(Vector2.Zero, 0f, Vector2.Zero, 0f);
        private PathFollowing path;
        private float objectivePhase;
        private int index;

        public DynamicPathFollowingAlgorithm(float maxAcceleration, float thresholdDistance, PathFollowing path, float objectivePhase, int index)
            : base(maxAcceleration, thresholdDistance)
        {
            this.objectivePhase = objectivePhase;
            this.path = path;
            this.index = index;
        }

        public new void SetAgent(AgKinematic agent)
        {
            this.agent = agent;
            base.SetAgent(agent);
        }

        public override SteeringOutput GetSteering()
        {
            if (agent == null || path == null || path.Length == 0)
                return new SteeringOutput();

            var agentPos = new UnityEngine.Vector2(agent.position.X, agent.position.Y);

            // Buscar el punto más cercano solo si estamos lejos del objetivo
            index = path.GetClosestPoint(agentPos, index);

            int targetIndex = System.Math.Clamp(index + (int)objectivePhase, 0, path.Length - 1);
            var targetPos = path.GetPointPosition(targetIndex);
            var target2D = new Vector2(targetPos.x, targetPos.y);

            // Si estamos cerca del objetivo, avanzar al siguiente
            float distance = Vector2.Distance(agent.position, target2D);
            if (distance < thresholdDistance && targetIndex < path.Length - 1)
            {
                index++;
                targetIndex = System.Math.Clamp(index + (int)objectivePhase, 0, path.Length - 1);
                targetPos = path.GetPointPosition(targetIndex);
                target2D = new Vector2(targetPos.x, targetPos.y);
            }

            reusableTarget.position = target2D;
            base.SetTarget(reusableTarget);

            return base.GetSteering();
        }
    }
}
