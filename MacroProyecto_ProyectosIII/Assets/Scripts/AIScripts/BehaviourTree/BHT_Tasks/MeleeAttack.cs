using AIEngine.Decision.BehaviourTree;
using UnityEngine;

public class MeleeAttack : BHT_Task
{
    private Animator animator;
    private Transform target;
    private GameObject npc;
    private float cooldown;
    private float lastAttackTime = -Mathf.Infinity;
    private float meleeRange;

    private EnemiPrueba enemyData;

    public MeleeAttack(GameObject npc, Animator animator, Transform target, EnemiPrueba enemyData)
    {
        this.animator = animator;
        this.target = target;
        this.npc = npc;
        this.enemyData = enemyData;

        this.cooldown = enemyData.meleeCooldown;
        this.meleeRange = enemyData.meleeRange;
    }

    public override bool Run()
    {
        if (Time.time - lastAttackTime < cooldown)
            return false;

        float dist = Vector3.Distance(npc.transform.position, target.position);
        if (dist > meleeRange)
            return false;

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack");

        // Inflige daño al jugador (asumiendo un script llamado PlayerHealth con TakeDamage)
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(enemyData.meleeDamage);
        }

        return true;
    }
}
