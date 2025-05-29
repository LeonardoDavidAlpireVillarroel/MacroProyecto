using AIEngine.Movement.Components.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiPrueba : MonoBehaviour
{
    public int health = 1;
    [Header("Ataque Melee")]
    public float meleeRange = 0.1f;
    public float meleeCooldown = 2.0f;
    public int meleeDamage = 1;

    private Transform player;
    private Animator animator;
    private bool isAttacking = false;
    private bool canAttack = true;

    private bool hasDealtDamage = false;

    public CmpPathFollowing cmpPathFollowing;

    private bool isStuned = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cmpPathFollowing = GetComponent<CmpPathFollowing>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player != null && canAttack && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= meleeRange && !isStuned)
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        canAttack = false;
        hasDealtDamage = false;

        animator.SetTrigger("Attack");

        cmpPathFollowing.maxSpeed = 20f;
        yield return new WaitForSeconds(0.5f);
        cmpPathFollowing.maxSpeed = 1f;

        yield return new WaitForSeconds(meleeCooldown - 0.5f);

        isAttacking = false;
        canAttack = true;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        isStuned = true;
        StartCoroutine(StunEnemy());

        if (health <= 0) Die();
    }

    private IEnumerator StunEnemy()
    {
        float originalSpeed = cmpPathFollowing.maxSpeed;

        cmpPathFollowing.maxSpeed = 0;
        yield return new WaitForSeconds(5f);
        cmpPathFollowing.maxSpeed = originalSpeed;
        isStuned = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerShoot"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLifes();
        }
    }

    void Die()
    {
        GameManager.Instance.EnemyDefeated();

        LevelController nivel = FindFirstObjectByType<LevelController>();
        if (nivel != null) nivel.IncrementarEnemigosDerrotados();

        Destroy(gameObject);
    }
}