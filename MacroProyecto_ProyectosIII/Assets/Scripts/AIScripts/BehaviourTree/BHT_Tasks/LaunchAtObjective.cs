//using AIEngine.Decision.BehaviourTree;
//using AIEngine.Movement.Components.Algorithms;
//using UnityEngine;

//public class LaunchAtObjective : BHT_Task
//{
//    private readonly CmpPathFollowing cmpPathFollowing;
//    private readonly Transform target;
//    private readonly float launchTriggerDistance;
//    private readonly float meleeDistance;

//    private float launchSpeed = 5f;
//    private float originalSpeed = 0f;
//    private float recoveryTime = 0.4f;

//    private bool isSpeedBoosted = false;
//    private float speedBoostTimer = 0f;

//    private float cooldown = 5f;
//    private float cooldownTimer = 0f;
//    private bool isOnCooldown = false;

//    public LaunchAtObjective(CmpPathFollowing cmpPathFollowing, Transform target, float launchTriggerDistance, float meleeDistance)
//    {
//        this.cmpPathFollowing = cmpPathFollowing;
//        this.target = target;
//        this.launchTriggerDistance = launchTriggerDistance;
//        this.meleeDistance = meleeDistance;
//    }

//    public override bool Run()
//    {
//        EnemiPrueba enemi = cmpPathFollowing.GetComponent<EnemiPrueba>();
//        if (enemi != null && enemi.isStuned) return false;

//        float distance = Vector3.Distance(cmpPathFollowing.transform.position, target.position);

//        // Cooldown
//        if (isOnCooldown)
//        {
//            cooldownTimer += Time.deltaTime;
//            if (cooldownTimer >= cooldown)
//            {
//                isOnCooldown = false;
//                cooldownTimer = 0f;
//            }
//            return false;
//        }

//        // Recuperación de velocidad original después de impulso
//        if (isSpeedBoosted)
//        {
//            speedBoostTimer += Time.deltaTime;
//            if (speedBoostTimer >= recoveryTime)
//            {
//                cmpPathFollowing.SetSpeed(originalSpeed);
//                isSpeedBoosted = false;
//                isOnCooldown = true;
//                cooldownTimer = 0f;
//                return true;
//            }
//            return false;
//        }

//        // Condición de impulso: entre melee y launch
//        if (distance <= launchTriggerDistance && distance > meleeDistance)
//        {
//            originalSpeed = cmpPathFollowing.GetSpeed();
//            cmpPathFollowing.SetSpeed(launchSpeed);
//            isSpeedBoosted = true;
//            speedBoostTimer = 0f;
//            return false;
//        }

//        return false;
//    }
//}
using AIEngine.Decision.BehaviourTree;
using AIEngine.Movement.Components.Algorithms;
using UnityEngine;

public class LaunchAtObjective : BHT_Task
{
    private readonly CmpPathFollowing cmpPathFollowing;
    private readonly Transform target;
    private readonly float launchTriggerDistance;
    private readonly float meleeDistance;

    private float launchSpeed = 5f;
    private float originalSpeed = 0f;
    private float recoveryTime = 0.4f;

    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;

    private float cooldown = 5f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    public LaunchAtObjective(CmpPathFollowing cmpPathFollowing, Transform target, float launchTriggerDistance, float meleeDistance)
    {
        this.cmpPathFollowing = cmpPathFollowing;
        this.target = target;
        this.launchTriggerDistance = launchTriggerDistance;
        this.meleeDistance = meleeDistance;
    }

    public override bool Run()
    {
        EnemiPrueba enemi = cmpPathFollowing.GetComponent<EnemiPrueba>();
        if (enemi != null && enemi.isStuned)
            return false;

        float distance = Vector3.Distance(cmpPathFollowing.transform.position, target.position);

        // Cooldown
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldown)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
            return false;
        }

        // Recuperar velocidad original después del lanzamiento
        if (isSpeedBoosted)
        {
            speedBoostTimer += Time.deltaTime;
            if (speedBoostTimer >= recoveryTime)
            {
                cmpPathFollowing.SetSpeed(originalSpeed);
                cmpPathFollowing.StopLaunch();
                cmpPathFollowing.UpdatePath(cmpPathFollowing.GetCurrentPath());

                isSpeedBoosted = false;
                isOnCooldown = true;
                cooldownTimer = 0f;
                return true;
            }
            return false;
        }

        // Lanzamiento si está en rango adecuado
        if (distance <= launchTriggerDistance && distance > meleeDistance)
        {
            originalSpeed = cmpPathFollowing.GetSpeed();
            cmpPathFollowing.SetSpeed(launchSpeed);

            Vector3 directionToTarget = (target.position - cmpPathFollowing.transform.position).normalized;
            cmpPathFollowing.Launch(directionToTarget);
            cmpPathFollowing.StopFollowing();

            isSpeedBoosted = true;
            speedBoostTimer = 0f;
            return false;
        }

        return false;
    }
}

