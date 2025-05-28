using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Algorithms
{
    public class PathFollowingAlgorithmStatic : SeekAlgorithm
    {
        private AgStatic agent;
        private AgStatic reusableTarget = new AgStatic(Vector2.Zero, 0f);

        private PathFollowingPointsStatic path;
        private float objectivePhase;
        private int index;
        private float thresholdDistance;
        public int CurrentIndex
        {
            get => index;
            set => index = value;
        }

        public PathFollowingAlgorithmStatic(
            float maxSpeed,
            float thresholdDistance,
            PathFollowingPointsStatic path,
            float objectivePhase,
            int index
        ) : base(maxSpeed, thresholdDistance)
        {
            this.path = path;
            this.objectivePhase = objectivePhase;
            this.thresholdDistance = thresholdDistance;
            this.index = index;
        }

        public new void SetAgent(AgStatic agent)
        {
            this.agent = agent;
            base.SetAgent(agent);
        }
        public override SteeringOutput GetSteering()
        {
            if (agent == null || path == null || path.Length == 0)
                return new SteeringOutput();

            if (index >= path.Length)
                return new SteeringOutput(); 

            Vector2 currentTargetPos = path.GetPointPosition(index);

            float distance = Vector2.Distance(agent.position, currentTargetPos);
            if (distance < thresholdDistance)
            {
                if (index < path.Length - 1)
                {
                    index++;
                    currentTargetPos = path.GetPointPosition(index);
                }
                else
                {
                    return new SteeringOutput();
                }
            }

            int targetIndex = System.Math.Clamp(index + (int)objectivePhase, 0, path.Length - 1);
            Vector2 targetPos = path.GetPointPosition(targetIndex);

            if (Vector2.Distance(targetPos, agent.position) < 0.001f)
                return new SteeringOutput();

            reusableTarget.position = targetPos;
            base.SetTarget(reusableTarget);
            return base.GetSteering();
        }

    }
}
