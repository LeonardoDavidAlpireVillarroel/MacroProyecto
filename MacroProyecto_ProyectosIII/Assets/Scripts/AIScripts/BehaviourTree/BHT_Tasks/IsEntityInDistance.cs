using AIEngine.Decision.BehaviourTree;
using UnityEngine;

public class IsEntityInDistance : BHT_Task
{
    private readonly GameObject entity;
    private readonly GameObject myself;
    private readonly float distance;

    public IsEntityInDistance(GameObject entity, GameObject myself, float distance)
    {
        this.entity = entity;
        this.myself = myself;
        this.distance = distance;
    }

    public override bool Run()
    {
        return Vector3.Distance(myself.transform.position, entity.transform.position) < distance;
    }
}
