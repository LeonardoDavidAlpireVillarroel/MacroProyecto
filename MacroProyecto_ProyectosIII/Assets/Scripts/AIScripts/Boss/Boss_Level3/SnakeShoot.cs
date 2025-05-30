using UnityEngine;

public class SnakeShoot : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public void SetTarget(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Aquí puedes añadir lógica al colisionar con el jugador, suelo, etc.
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Fruta golpeó al jugador!");
            // Aquí puedes hacer daño o algo más
        }

        Destroy(gameObject); // Destruye la fruta al chocar
    }
}
