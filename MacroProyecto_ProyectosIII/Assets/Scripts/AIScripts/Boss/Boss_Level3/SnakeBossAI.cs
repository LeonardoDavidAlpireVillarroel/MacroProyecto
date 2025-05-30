using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SnakeBossAI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public int maxHealth = 4;
    public int currentHealth;

    [Header("Appear Settings")]
    public GameObject holePrefab;
    private GameObject currentHole;
    public GameObject shadowPrefab;
    private GameObject currentShadow;
    public float timeBeforeAppear = 2f;
    public float stayDuration = 2f;
    public float pushForce = 1f;

    [Header("Shooting")]
    public GameObject playerTarget;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float shootInterval = 1.5f;

    private Coroutine shootingCoroutine;

    [Header("References")]
    public Animator animator;
    private Rigidbody rb;
    private Renderer rend;
    private Collider col;

    private Vector3 spawnPosition;

    [Header("Spawner Reference")]
    public BossSpawner bossSpawner;

    private bool isDead = false;
    private bool isVisible = false;
    private bool isAppearing = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        rend.enabled = false;
        col.enabled = false;
        rb.isKinematic = true;
        currentHealth = maxHealth;
        gameObject.SetActive(false);
    }

    public void AppearAtPosition(Vector3 position)
    {
        if (isDead) return;

        spawnPosition = position;
        gameObject.SetActive(true);
        StartCoroutine(AppearanceSequence());
    }

    public bool IsVisible()
    {
        return isVisible;
    }

    public bool IsAppearing()
    {
        return isAppearing;
    }

    private IEnumerator AppearanceSequence()
    {
        isAppearing = true;  // Antes de empezar a aparecer

        rend.enabled = false;
        col.enabled = false;

        yield return StartCoroutine(ShowShadowAndSpawnHole(spawnPosition));

        currentHole = Instantiate(holePrefab, spawnPosition, Quaternion.identity);
        Collider holeCol = currentHole.GetComponent<Collider>();
        if (holeCol) holeCol.enabled = false;

        transform.position = spawnPosition + new Vector3(0, -0.5f, 0);

        rend.enabled = true;
        col.enabled = true;

        rb.isKinematic = true;

        if (holeCol) holeCol.enabled = true;

        animator.SetTrigger("Appear");

        float elapsed = 0f;
        float appearTime = 0.5f;
        Vector3 startPos = transform.position;
        Vector3 endPos = spawnPosition + new Vector3(0, 0.5f, 0);

        while (elapsed < appearTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / appearTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        isAppearing = false;  // Terminó la aparición
        isVisible = true;

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(stayDuration);

        shootingCoroutine = StartCoroutine(ShootingLoop());
    }

    private IEnumerator ShootingLoop()
    {
        while (isVisible)
        {
            ShootAtPlayer();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootAtPlayer()
    {
        if (playerTarget == null) return;

        Vector3 playerPos = playerTarget.transform.position;

        Vector3 spawnPos = shootPoint.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        SnakeShoot snakeShoot = bullet.GetComponent<SnakeShoot>();
        if (snakeShoot != null)
        {
            snakeShoot.SetTarget(playerPos);
        }
    }

    private IEnumerator ShowShadowAndSpawnHole(Vector3 position)
    {
        currentShadow = Instantiate(shadowPrefab, position + Vector3.up * 0.01f, Quaternion.Euler(-90f, 0f, 0f));
        Renderer shadowRenderer = currentShadow.GetComponent<Renderer>();

        Color originalColor = shadowRenderer.material.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        Vector3 originalScale = currentShadow.transform.localScale;

        shadowRenderer.material.color = transparentColor;
        currentShadow.transform.localScale = Vector3.zero;

        float fadeDuration = timeBeforeAppear;
        float elapsed = 0f;

        float moveAmount = 0.1f;
        float moveSpeed = 20f;

        while (elapsed < fadeDuration)
        {
            if (isDead)  // Si muere, destruye sombra y termina
            {
                Destroy(currentShadow);
                currentShadow = null;
                yield break;
            }

            float alpha = Mathf.Lerp(0, originalColor.a, elapsed / fadeDuration);
            shadowRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            currentShadow.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsed / fadeDuration);

            float xOffset = Mathf.Sin(elapsed * moveSpeed) * moveAmount;
            currentShadow.transform.position = position + new Vector3(xOffset, 0.01f, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(currentShadow);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PushPlayer(collision.gameObject));
        }
    }

    private IEnumerator PushPlayer(GameObject player)
    {
        Vector3 direction = (player.transform.position - transform.position);
        direction.y = 0;
        direction.Normalize();

        float distance = 1f;
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startPos = player.transform.position;
        Vector3 endPos = startPos + direction * distance;

        GameManager.Instance.LoseLifes();

        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;
    }

    public void Disappear()
    {
        //if (!isVisible) return;

        //StopAllCoroutines();
        //StartCoroutine(DisappearSequence());
        if (!isVisible && !isAppearing) return;
        ForceDisappear();
    }



    public void ForceDisappear()
    {
        StopAllCoroutines();  // Detiene aparición o disparo

        if (currentShadow != null)
        {
            Destroy(currentShadow);
            currentShadow = null;
        }

        if (currentHole != null)
        {
            Destroy(currentHole);
            currentHole = null;
        }

        isVisible = false;
        isAppearing = false;
        animator.ResetTrigger("Appear");
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Disappear");

        rend.enabled = false;
        col.enabled = false;
        rb.isKinematic = true;

        gameObject.SetActive(false);
    }

    private IEnumerator DisappearSequence()
    {
        isVisible = false;

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }

        animator.SetTrigger("Disappear");

        float elapsed = 0f;
        float disappearTime = 0.5f;
        Vector3 startPos = transform.position;
        Vector3 endPos = spawnPosition + new Vector3(0, -0.5f, 0);

        while (elapsed < disappearTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / disappearTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        rend.enabled = false;
        col.enabled = false;
        rb.isKinematic = true;

        if (currentHole != null)
        {
            Destroy(currentHole);
            currentHole = null;
        }

        gameObject.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        // Permitir daño si está apareciendo o visible
        if (!isAppearing && !isVisible) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        if (bossSpawner != null)
        {
            bossSpawner.UpdateTotalBossHealth();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (currentShadow != null)
        {
            Destroy(currentShadow);
            currentShadow = null;
        }

        if (currentHole != null)
        {
            Destroy(currentHole);
            currentHole = null;
        }

        StopAllCoroutines();

        animator.SetTrigger("Die");

        col.enabled = false;
        rb.isKinematic = true;

        StartCoroutine(DisappearSequence());
    }

    public bool IsDead()
    {
        return isDead;
    }
}
