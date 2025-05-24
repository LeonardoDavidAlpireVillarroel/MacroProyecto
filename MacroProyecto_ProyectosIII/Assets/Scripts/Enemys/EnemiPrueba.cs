using AIEngine.Movement.Components.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiPrueba : MonoBehaviour
{
    public int health = 1;
    [Header("Ataque Melee")]
    public float meleeRange = 2.5f;
    public float meleeCooldown = 2.0f;
    public int meleeDamage = 1;

    private Transform player;
    private Animator animator;
    private bool isAttacking = false;
    private bool canAttack = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player != null && canAttack && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= meleeRange)
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(meleeCooldown);

        isAttacking = false;
        canAttack = true;
    }

    public void ApplyDamage()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= meleeRange)
        {
            GameManager.Instance.LoseLifes();
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance.EnemyDefeated();

        LevelController nivel = FindFirstObjectByType<LevelController>();
        if (nivel != null) nivel.IncrementarEnemigosDerrotados();

        Destroy(gameObject);
    }
}
