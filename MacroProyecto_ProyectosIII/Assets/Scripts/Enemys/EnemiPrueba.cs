//using System.Collections;
//using UnityEngine;
//using AIEngine.Movement.Components.Algorithms;

//public class EnemiPrueba : MonoBehaviour
//{
//    [Header("Estado del enemigo")]
//    public int health = 1;
//    public bool isStuned = false;

//    [Header("Ataque Melee")]
//    public float meleeRange = 0.1f;
//    public float meleeCooldown = 2.0f;
//    public int meleeDamage = 1;

//    [Header("Referencias")]
//    public Animator animator;
//    public CmpPathFollowing cmpPathFollowing;

//    private void Start()
//    {
//        animator = GetComponent<Animator>();
//        cmpPathFollowing = GetComponent<CmpPathFollowing>();
//    }

//    public void TakeDamage(int amount)
//    {
//        health -= amount;

//        if (!isStuned)
//        {
//            isStuned = true;
//            StartCoroutine(StunCoroutine());
//        }

//        if (health <= 0)
//        {
//            Die();
//        }
//    }

//    private IEnumerator StunCoroutine()
//    {
//        bool wasEnabled = cmpPathFollowing.enabled;
//        float originalSpeed = cmpPathFollowing.maxSpeed;

//        if (wasEnabled)
//            cmpPathFollowing.maxSpeed = 0;

//        yield return new WaitForSeconds(5f);

//        if (wasEnabled)
//            cmpPathFollowing.maxSpeed = originalSpeed;

//        isStuned = false;
//    }


//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("PlayerShoot"))
//        {
//            TakeDamage(1);
//            Destroy(collision.gameObject);
//        }

//        if (collision.gameObject.CompareTag("Player"))
//        {
//            GameManager.Instance.LoseLifes();
//        }
//    }

//    public void Die()
//    {
//        GameManager.Instance.EnemyDefeated();

//        LevelController nivel = FindFirstObjectByType<LevelController>();
//        if (nivel != null) nivel.IncrementarEnemigosDerrotados();

//        Destroy(gameObject);
//    }
//}

using System.Collections;
using UnityEngine;
using AIEngine.Movement.Components.Algorithms;

public class EnemiPrueba : MonoBehaviour
{
    [Header("Estado del enemigo")]
    public int health = 1;
    public bool isStuned = false;

    [Header("Ataque Melee")]
    public float meleeRange = 0.1f;
    public float meleeCooldown = 2.0f;
    public int meleeDamage = 1;

    [Header("Referencias")]
    public Animator animator;
    public CmpPathFollowing cmpPathFollowing;

    private void Start()
    {
        animator = GetComponent<Animator>();
        cmpPathFollowing = GetComponent<CmpPathFollowing>();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (!isStuned)
        {
            isStuned = true;
            StartCoroutine(StunCoroutine());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator StunCoroutine()
    {
        bool wasEnabled = cmpPathFollowing.enabled;
        float originalSpeed = cmpPathFollowing.maxSpeed;

        if (wasEnabled)
            cmpPathFollowing.maxSpeed = 0;

        yield return new WaitForSeconds(5f);

        if (wasEnabled)
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

    private void Die()
    {
        GameManager.Instance.EnemyDefeated();
        LevelController nivel = FindFirstObjectByType<LevelController>();
        if (nivel != null) nivel.IncrementarEnemigosDerrotados();
        Destroy(gameObject);
    }
}