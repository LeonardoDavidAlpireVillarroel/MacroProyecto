using System.Collections;
using System.Collections.Generic;
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
    private GameObject currentHole;
    public GameObject shadowPrefab;
    public float timeBeforeAppear = 2f;
    public float stayDuration = 2f;
    public float pushForce = 1f;

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

    private IEnumerator AppearanceSequence()
    {
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

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(stayDuration);

        isVisible = true;
    }

    private IEnumerator ShowShadowAndSpawnHole(Vector3 position)
    {
        GameObject shadow = Instantiate(shadowPrefab, position + Vector3.up * 0.01f, Quaternion.Euler(-90f, 0f, 0f));
        Renderer shadowRenderer = shadow.GetComponent<Renderer>();

        Color originalColor = shadowRenderer.material.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        Vector3 originalScale = shadow.transform.localScale;

        shadowRenderer.material.color = transparentColor;
        shadow.transform.localScale = Vector3.zero;

        float fadeDuration = timeBeforeAppear;
        float elapsed = 0f;

        float moveAmount = 0.1f;
        float moveSpeed = 20f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, originalColor.a, elapsed / fadeDuration);
            shadowRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            shadow.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsed / fadeDuration);

            float xOffset = Mathf.Sin(elapsed * moveSpeed) * moveAmount;
            shadow.transform.position = position + new Vector3(xOffset, 0.01f, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(shadow);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PushPlayer(collision.gameObject));
        }
        else if (collision.gameObject.CompareTag("PlayerShoot"))
        {
            if (bossSpawner != null)
            {
                bossSpawner.DespawnAllBosses();
                Destroy(collision.gameObject);
            }
        }
    }

    private IEnumerator PushPlayer(GameObject player)
    {
        Vector3 direction = (player.transform.position - transform.position);
        direction.y = 0;
        direction.Normalize();

        float distance = 1f; // cuánta distancia quieres empujar
        float duration = 0.2f; // cuánto tarda en hacer el empuje
        float elapsed = 0f;

        Vector3 startPos = player.transform.position;
        Vector3 endPos = startPos + direction * distance;

        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            GameManager.Instance.LoseLifes();
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;
    }

    public void Disappear()
    {
        if (!isVisible) return;

        StopAllCoroutines();
        StartCoroutine(DisappearSequence());
    }

    private IEnumerator DisappearSequence()
    {
        isVisible = false;

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

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        col.enabled = false;
        rb.isKinematic = true;

        if (currentHole != null)
        {
            Destroy(currentHole);
            currentHole = null;
        }

        Destroy(gameObject, 3f);
    }
}
