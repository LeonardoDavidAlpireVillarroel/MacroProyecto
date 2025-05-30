
////using AIEngine.Movement.Algorithms;
////using AIEngine.Movement.Components.Agents;
////using AIEngine.Movement.Output;
////using UnityEngine;

////namespace AIEngine.Movement.Components.Algorithms
////{
////    [RequireComponent(typeof(CmpStatic))]
////    public class CmpPathFollowing : MonoBehaviour, ISteeringProvider
////    {
////        [SerializeField] public float maxSpeed;
////        [SerializeField] private float thresholdDistance;
////        [SerializeField] private float objectivePhase;

////        private PathFollowingPoints path;
////        private int index;
////        private PathFollowingAlgorithm pathFollowing;

////        private CmpStatic cmpStatic;

////        public Transform objective; // objetivo a seguir
////        public float stopNearObjectiveDistance = 1.0f;

////        private void Start()
////        {
////            cmpStatic = GetComponent<CmpStatic>();
////        }


////        private bool isActive = false;

////        public void UpdatePath(PathFollowingPoints newPath)
////        {
////            if (newPath == null || newPath.Length == 0)
////            {
////                Debug.LogWarning("[CmpPathFollowing] Path nulo o vacío.");
////                isActive = false;
////                return;
////            }

////            path = newPath;
////            index = 0;
////            pathFollowing = new PathFollowingAlgorithm(maxSpeed, thresholdDistance, path, objectivePhase, index);
////            pathFollowing.SetAgent(cmpStatic.GetAgent());

////            isActive = true;

////        }

////        public void StopFollowing()
////        {
////            isActive = false;
////            pathFollowing = null;
////        }

////        //public SteeringOutput GetSteering()
////        //{
////        //    if (!isActive || pathFollowing == null)
////        //        return new SteeringOutput(); // sin movimiento

////        //    return pathFollowing.GetSteering();
////        //}
////        public SteeringOutput GetSteering()
////        {
////            if (!isActive || pathFollowing == null)
////                return new SteeringOutput(); // sin movimiento

////            // Detener si está suficientemente cerca del objetivo
////            if (objective != null)
////            {
////                float distanceToObjective = Vector3.Distance(transform.position, objective.position);
////                if (distanceToObjective < stopNearObjectiveDistance)
////                {
////                    return new SteeringOutput(); // sin movimiento
////                }
////            }

////            return pathFollowing.GetSteering();
////        }
////    }
////}

//using AIEngine.Movement.Algorithms;
//using AIEngine.Movement.Components.Agents;
//using AIEngine.Movement.Output;
//using UnityEngine;

//namespace AIEngine.Movement.Components.Algorithms
//{
//    [RequireComponent(typeof(CmpStatic))]
//    public class CmpPathFollowing : MonoBehaviour, ISteeringProvider
//    {
//        [SerializeField] public float maxSpeed;
//        [SerializeField] private float thresholdDistance;
//        [SerializeField] private float objectivePhase;

//        private PathFollowingPoints path;
//        private int index;
//        private PathFollowingAlgorithm pathFollowing;

//        private CmpStatic cmpStatic;

//        public Transform objective;
//        public float stopNearObjectiveDistance = 1.0f;

//        private bool isActive = false;

//        // --- NUEVO: Variables para Launch ---
//        private bool isLaunching = false;
//        private Vector3 launchVelocity;

//        public float GetSpeed()
//        {
//            return maxSpeed;
//        }

//        public void SetSpeed(float newSpeed)
//        {
//            maxSpeed = newSpeed;

//            if (pathFollowing != null)
//            {
//                pathFollowing.SetMaxSpeed(newSpeed);
//            }
//        }

//        // --- NUEVOS MÉTODOS para el lanzamiento ---
//        public void Launch(Vector3 velocity)
//        {
//            isLaunching = true;
//            launchVelocity = velocity;
//        }

//        public void StopLaunch()
//        {
//            isLaunching = false;
//        }

//        private void Start()
//        {
//            cmpStatic = GetComponent<CmpStatic>();
//        }

//        public void UpdatePath(PathFollowingPoints newPath)
//        {
//            if (newPath == null || newPath.Length == 0)
//            {
//                Debug.LogWarning("[CmpPathFollowing] Path nulo o vacío.");
//                isActive = false;
//                return;
//            }

//            path = newPath;
//            index = 0;
//            pathFollowing = new PathFollowingAlgorithm(maxSpeed, thresholdDistance, path, objectivePhase, index);
//            pathFollowing.SetAgent(cmpStatic.GetAgent());
//            isActive = true;
//        }

//        public void StopFollowing()
//        {
//            isActive = false;
//            pathFollowing = null;
//        }

//        public SteeringOutput GetSteering()
//        {
//            if (!isActive || pathFollowing == null)
//                return new SteeringOutput();

//            if (isLaunching)
//            {
//                // Devuelve velocidad fija para el lanzamiento
//                SteeringOutput launchSteering = new SteeringOutput();
//                launchSteering.linear = new System.Numerics.Vector2(launchVelocity.x, launchVelocity.z);
//                launchSteering.angular = 0f;
//                return launchSteering;
//            }

//            if (objective != null && Vector3.Distance(transform.position, objective.position) < stopNearObjectiveDistance)
//                return new SteeringOutput();

//            return pathFollowing.GetSteering();
//        }
//    }
//}

using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Agents;
using AIEngine.Movement.Output;
using UnityEngine;

namespace AIEngine.Movement.Components.Algorithms
{
    [RequireComponent(typeof(CmpStatic))]
    public class CmpPathFollowing : MonoBehaviour, ISteeringProvider
    {
        [SerializeField] public float maxSpeed;
        [SerializeField] private float thresholdDistance;
        [SerializeField] private float objectivePhase;

        private PathFollowingPoints path;
        private int index;
        private PathFollowingAlgorithm pathFollowing;
        private CmpStatic cmpStatic;

        public Transform objective;
        public float stopNearObjectiveDistance = 1.0f;

        private bool isActive = false;

        // --- NUEVO: Variables para Launch ---
        private bool isLaunching = false;
        private Vector3 launchDirection;

        private PathFollowingPoints lastPath;

        public float GetSpeed() => maxSpeed;

        public void SetSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
            if (pathFollowing != null)
                pathFollowing.SetMaxSpeed(newSpeed);
        }

        public void Launch(Vector3 direction)
        {
            isLaunching = true;
            launchDirection = direction.normalized;
        }

        public void StopLaunch()
        {
            isLaunching = false;
        }

        private void Start()
        {
            cmpStatic = GetComponent<CmpStatic>();
        }

        public void UpdatePath(PathFollowingPoints newPath)
        {
            if (newPath == null || newPath.Length == 0)
            {
                Debug.LogWarning("[CmpPathFollowing] Path nulo o vacío.");
                isActive = false;
                return;
            }

            path = newPath;
            lastPath = newPath;

            index = 0;
            pathFollowing = new PathFollowingAlgorithm(maxSpeed, thresholdDistance, path, objectivePhase, index);
            pathFollowing.SetAgent(cmpStatic.GetAgent());
            isActive = true;
        }

        public PathFollowingPoints GetCurrentPath() => lastPath;

        public void StopFollowing()
        {
            isActive = false;
            pathFollowing = null;
        }

        public SteeringOutput GetSteering()
        {
            if (!isActive || pathFollowing == null)
                return new SteeringOutput();

            if (isLaunching)
            {
                Vector3 dir3D = launchDirection * maxSpeed;
                return new SteeringOutput
                {
                    linear = new System.Numerics.Vector2(dir3D.x, dir3D.z),
                    angular = 0f
                };
            }

            if (objective != null && Vector3.Distance(transform.position, objective.position) < stopNearObjectiveDistance)
                return new SteeringOutput();

            return pathFollowing.GetSteering();
        }
    }
}
