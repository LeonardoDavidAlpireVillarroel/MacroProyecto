using UnityEngine;

public class SnakeShoot : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLifesForBoss(3);
            Destroy(gameObject);
        }
    }
}
