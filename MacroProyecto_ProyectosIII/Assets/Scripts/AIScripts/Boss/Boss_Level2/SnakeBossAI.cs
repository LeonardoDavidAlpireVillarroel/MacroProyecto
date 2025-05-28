using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SnakeBossAI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Attack Settings")]
    public GameObject holePrefab;
    public GameObject shadowPrefab;  // Prefab de la sombra para el hoyo
    public float timeBeforeAppear = 2f;
    public float stayDuration = 2f;
    public float appearForce = 5f;
    public float timeBetweenAttacks = 5f;

    [Header("References")]
    public Animator animator;
    private Transform player;
    private Rigidbody rb;

    private Renderer rend;
    private Collider col;

    private Vector3 targetPosition;
    private bool isAttacking = false;
    private bool isDead = false;

    private GameObject shadowInstance;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(BossRoutine());
    }

    private IEnumerator BossRoutine()
    {
        while (!isDead)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                yield return StartCoroutine(AttackSequence());
                isAttacking = false;
            }

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator AttackSequence()
    {
        Vector3 playerPos = player.position;
        targetPosition = new Vector3(playerPos.x, 0f, playerPos.z);

        // Sonido de terremoto
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound3D("Earthquake", targetPosition);
        }

        // Mostrar sombra vibrando y luego instanciar hoyo (ya instancia hoyo dentro)
        yield return StartCoroutine(ShowShadowAndSpawnHole(targetPosition));

        // Instanciar hoyo y guardarlo para destruir luego
        GameObject hole = Instantiate(holePrefab, targetPosition, Quaternion.identity);

        Collider holeCollider = hole.GetComponent<Collider>();
        if (holeCollider != null)
            holeCollider.enabled = false;

        // 4. Impulsar jugador hacia arriba y lateralmente si está muy cerca
        Collider[] hits = Physics.OverlapSphere(targetPosition, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody playerRb = hit.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDir = (playerRb.position - targetPosition);
                    pushDir.y = 0;
                    if (pushDir == Vector3.zero)
                    {
                        pushDir = Vector3.right;
                    }
                    pushDir.Normalize();

                    Vector3 force = pushDir * appearForce + Vector3.up * appearForce;

                    playerRb.AddForce(force, ForceMode.Impulse);
                }
            }
        }

        // 5. Preparar serpiente: poner un poco bajo el suelo (ej: y = -0.5f)
        transform.position = targetPosition + new Vector3(0, -0.5f, 0);

        // Activar visuales y collider para que se vea y colisione
        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;

        rb.isKinematic = true;  // Desactivar física para controlar posición manualmente
        gameObject.SetActive(true);

        // Activar collider del hoyo después
        if (holeCollider != null)
            holeCollider.enabled = true;

        // 6. Subir la serpiente con animación y movimiento suave
        animator.SetTrigger("Appear");

        // Subir lentamente de y = -0.5f a y = 0.5f (ejemplo)
        float elapsed = 0f;
        float appearTime = 0.5f;
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPosition + new Vector3(0, 0.5f, 0);
        while (elapsed < appearTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / appearTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(stayDuration);

        StartCoroutine(DestroyHoleAfterSeconds(hole, stayDuration + 1f));

        // 7. Bajar la serpiente (desaparecer)
        animator.SetTrigger("Disappear");

        elapsed = 0f;
        float disappearTime = 0.5f;
        startPos = transform.position;
        endPos = targetPosition + new Vector3(0, -0.5f, 0);
        while (elapsed < disappearTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / disappearTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        // Desactivar visuales y collider (pero no todo el objeto)
        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;
        rb.isKinematic = true;

        Destroy(hole);
    }

    // Nueva coroutine para simular vibración lateral + sombra fade in/out
    private IEnumerator ShowShadowAndSpawnHole(Vector3 position)
    {
        // Instanciar sombra con rotación -90 grados en X para que quede tumbada
        GameObject shadow = Instantiate(shadowPrefab, position + Vector3.up * 0.01f, Quaternion.Euler(-90f, 0f, 0f));

        Renderer shadowRenderer = shadow.GetComponent<Renderer>();
        Color originalColor = shadowRenderer.material.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // Escala original para referencia
        Vector3 originalScale = shadow.transform.localScale;

        // Inicialmente transparente y con escala 0 (o muy pequeña)
        shadowRenderer.material.color = transparentColor;
        shadow.transform.localScale = Vector3.zero;

        float fadeDuration = timeBeforeAppear;
        float elapsed = 0f;

        float moveAmount = 0.1f;
        float moveSpeed = 20f;  // Vibración más rápida

        while (elapsed < fadeDuration)
        {
            // Fade in alpha
            float alpha = Mathf.Lerp(0, originalColor.a, elapsed / fadeDuration);
            shadowRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Escalar desde 0 hasta escala original
            shadow.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsed / fadeDuration);

            // Movimiento lateral oscilante
            float xOffset = Mathf.Sin(elapsed * moveSpeed) * moveAmount;
            shadow.transform.position = position + new Vector3(xOffset, 0.01f, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Al terminar, desaparecer la sombra de golpe
        Destroy(shadow);
    }

    private IEnumerator DestroyHoleAfterSeconds(GameObject holeObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (holeObj != null)
            Destroy(holeObj);
    }


    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        animator.SetTrigger("Die");
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        Destroy(gameObject, 3f);
    }
}
