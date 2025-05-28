//using UnityEngine;

//namespace AIEngine.Decision.BehaviourTree.Tasks
//{
//    public class ChaseObjective : BHT_Task
//    {
//        private readonly PathAssignerToPathFollowingPoints pathAssigner;

//        public ChaseObjective(PathAssignerToPathFollowingPoints pathAssigner)
//        {
//            this.pathAssigner = pathAssigner;
//        }

//        public override bool Run()
//        {
//            if (pathAssigner == null)
//            {
//                Debug.LogWarning("[ChaseObjective] No se asignó el pathAssigner.");
//                return false;
//            }

//            pathAssigner.AssignPath(); // Asigna o actualiza el camino
//            return true; // Siempre retorna true si se ejecutó correctamente
//        }
//    }
//}
using AIEngine.Decision.BehaviourTree;
using AIEngine.Movement.Components.Algorithms;

public class ChaseObjective : BHT_Task
{
    private readonly PathAssignerToPathFollowingPoints pathAssigner;
    private readonly CmpPathFollowing cmpPathFollowing;

    public ChaseObjective(PathAssignerToPathFollowingPoints pathAssigner, CmpPathFollowing cmpPathFollowing)
    {
        this.pathAssigner = pathAssigner;
        this.cmpPathFollowing = cmpPathFollowing;
    }

    public override bool Run()
    {
        if (pathAssigner == null || cmpPathFollowing == null)
        {
            return false;
        }

        pathAssigner.AssignPath();
        return true;
    }

    public void Stop()
    {
        cmpPathFollowing.StopFollowing();
    }
}

