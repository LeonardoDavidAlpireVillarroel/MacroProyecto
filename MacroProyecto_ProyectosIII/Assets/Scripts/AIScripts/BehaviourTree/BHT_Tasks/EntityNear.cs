using AIEngine.Movement.Agents;
using AIEngine.Utils;
using UnityEngine;

namespace AIEngine.Decision.BehaviourTree.Tasks
{
    public class EntityNear : BHT_Task
    {
        private readonly float distance;
        private readonly GameObject entity;
        private readonly GameObject myself;

        public EntityNear(float distance, GameObject entity, GameObject myself)
        {
            this.distance = distance;
            this.entity = entity;
            this.myself = myself;
        }

        public override bool Run()
        {
            var separationDistance = myself.transform.position - entity.transform.position;
            return (separationDistance.magnitude < distance);
        }
    }
}


