using UnityEngine;

public class FruitShoot : MonoBehaviour
{
    public GameObject fruitPrefab;    // Prefab asignado desde el inspector
    public float shootSpeed = 10f;     // Velocidad de disparo de la fruta
    public float fruitLifetime = 5f;   // Tiempo de vida de la fruta en segundos

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo presionado
        {
            ShootFruit();
        }
    }

    void ShootFruit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Plano XZ

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 shootDirection = (targetPoint - transform.position).normalized;

            // Instanciamos la fruta
            GameObject newFruit = Instantiate(fruitPrefab, transform.position, Quaternion.identity);

            // Asegurarse de que tiene Rigidbody
            Rigidbody rb = newFruit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = newFruit.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.linearVelocity = shootDirection * shootSpeed;

            // Ignorar colisión con el Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Collider fruitCollider = newFruit.GetComponent<Collider>();
                Collider playerCollider = player.GetComponent<Collider>();

                if (fruitCollider != null && playerCollider != null)
                {
                    Physics.IgnoreCollision(fruitCollider, playerCollider);
                }
            }

            // Destruir la fruta después de cierto tiempo
            Destroy(newFruit, fruitLifetime);
        }
    }
}
