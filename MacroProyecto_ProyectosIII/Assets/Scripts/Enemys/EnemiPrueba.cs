using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiPrueba : MonoBehaviour
{
    public int health = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLifes();
        }

        if (collision.gameObject.CompareTag("PlayerShoot"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.EnemyDefeated();

        LevelController nivel = FindFirstObjectByType<LevelController>();
        if (nivel != null)
        {
            nivel.IncrementarEnemigosDerrotados();
        }

        Destroy(gameObject);
    }
}
