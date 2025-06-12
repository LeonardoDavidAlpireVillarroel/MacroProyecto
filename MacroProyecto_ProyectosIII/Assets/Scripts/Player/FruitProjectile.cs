using UnityEngine;

public class FruitProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            BossSpawner bossSpawner = FindFirstObjectByType<BossSpawner>();
            if (bossSpawner != null)
            {
                bossSpawner.ApplyDamageToTotalHealth(GameManager.Instance.fuerza);
            }

            Destroy(gameObject);
        }


        if (collision.gameObject.CompareTag("EnemigoTutorial"))
        {
            EnemiTutorial enemigo = collision.gameObject.GetComponent<EnemiTutorial>();
            if (enemigo != null)
            {
                enemigo.TakeDamage(1, TipoAtaque.Disparo);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
