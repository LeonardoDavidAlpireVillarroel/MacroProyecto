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
    }
}
