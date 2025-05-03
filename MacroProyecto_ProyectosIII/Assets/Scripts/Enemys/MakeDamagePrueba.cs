using UnityEngine;

public class MakeDamagePrueba : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLifes();
        }
    }
}
