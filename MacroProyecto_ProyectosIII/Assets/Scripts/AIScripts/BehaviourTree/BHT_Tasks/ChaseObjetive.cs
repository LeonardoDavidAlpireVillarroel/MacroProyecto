//using AIEngine.Decision.BehaviourTree;
//using AIEngine.Movement.Components.Algorithms;
//using UnityEngine;

//public class ChaseObjective : BHT_Task
//{
//    private readonly PathAssignerToPathFollowingPoints pathAssigner;
//    private readonly CmpPathFollowing cmpPathFollowing;
//    private readonly GameObject objective;

//    public ChaseObjective(PathAssignerToPathFollowingPoints pathAssigner, CmpPathFollowing cmpPathFollowing, GameObject objective)
//    {
//        this.pathAssigner = pathAssigner;
//        this.cmpPathFollowing = cmpPathFollowing;
//        this.objective = objective;
//    }

//    public override bool Run()
//    {
//        if (pathAssigner == null || cmpPathFollowing == null || objective == null)
//            return false;

//        pathAssigner.AssignPath();
//        cmpPathFollowing.objective = objective.transform;
//        return true;
//    }

//    public void Stop()
//    {
//        cmpPathFollowing.StopFollowing();
//    }
//}

using AIEngine.Decision.BehaviourTree;
using AIEngine.Movement.Components.Algorithms;
using UnityEngine;

public class ChaseObjective : BHT_Task
{
    private readonly PathAssignerToPathFollowingPoints pathAssigner;
    private readonly CmpPathFollowing cmpPathFollowing;
    private readonly GameObject target;
    private readonly float chaseDistance;

    public ChaseObjective(PathAssignerToPathFollowingPoints pathAssigner, CmpPathFollowing cmpPathFollowing, GameObject target, float chaseDistance)
    {
        this.pathAssigner = pathAssigner;
        this.cmpPathFollowing = cmpPathFollowing;
        this.target = target;
        this.chaseDistance = chaseDistance;
    }

    public override bool Run()
    {
        if (pathAssigner == null || cmpPathFollowing == null || target == null)
            return false;

        float distance = Vector3.Distance(cmpPathFollowing.transform.position, target.transform.position);
        if (distance > chaseDistance)
            return false;

        pathAssigner.AssignPath();
        cmpPathFollowing.objective = target.transform;

        return true;
    }

    public void Stop()
    {
        cmpPathFollowing.StopFollowing();
    }
}
