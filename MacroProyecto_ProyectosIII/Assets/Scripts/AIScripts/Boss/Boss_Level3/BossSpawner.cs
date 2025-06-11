using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
    public List<SnakeBossAI> bosses = new List<SnakeBossAI>();
    public List<Transform> spawnPoints = new List<Transform>();
    public Transform player;

    public float minDistanceBetweenBosses = 3f;
    public float respawnDelay = 5f;

    public float baseShootInterval = 1.5f;
    public float shootIntervalDecreasePerDeath = 0.2f;
    public float shootIntervalReductionPercent = 0.5f;
    private int deadSnakesCount = 0;

    private bool isSpawning = false;

    public int totalBossHealth;
    [SerializeField] private Slider totalHealthSlider;
    private int maxTotalBossHealth;
    private bool totalHealthInitialized = false;
    private int accumulatedDamage = 0;

    private LevelController levelController;

    private void Start()
    {

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        deadSnakesCount = 0;
        UpdateShootIntervals();

        StartCoroutine(SpawnBossesRoutine());

        levelController = LevelController.Instance;
    }

    private IEnumerator SpawnBossesRoutine()
    {
        while (true)
        {
            if (!isSpawning)
            {
                isSpawning = true;
                SpawnBossesAtClosestPoints();
            }
            yield return null;
        }
    }

    void SpawnBossesAtClosestPoints()
    {
        bosses = bosses.Where(b => b != null && !b.IsDead()).ToList();
        if (bosses.Count == 0 || spawnPoints.Count == 0) return;

        List<Transform> orderedPoints = spawnPoints.OrderBy(sp => Vector3.Distance(sp.position, player.position)).ToList();

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < bosses.Count; i++)
        {
            SnakeBossAI boss = bosses[i];
            if (boss == null) continue;

            Vector3 chosenPos = Vector3.zero;
            bool found = false;

            foreach (var point in orderedPoints)
            {
                if (!usedPositions.Any(pos => Vector3.Distance(pos, point.position) < minDistanceBetweenBosses))
                {
                    chosenPos = point.position;
                    usedPositions.Add(chosenPos);
                    found = true;
                    break;
                }
            }

            if (!found)
                chosenPos = orderedPoints[0].position;

            boss.AppearAtPosition(chosenPos);

            if (!totalHealthInitialized)
            {
                boss.currentHealth = boss.maxHealth;  // resetea salud individual solo la primera vez
            }
        }

        // Solo calcula y asigna la salud total la primera vez
        if (!totalHealthInitialized)
        {
            totalBossHealth = bosses.Sum(b => b.maxHealth);
            maxTotalBossHealth = totalBossHealth;
            totalHealthInitialized = true;
            UpdateHealthUI();
        }
    }

    public void UpdateTotalBossHealth()
    {
        int totalHealth = 0;
        int totalMaxHealth = 0;

        foreach (var boss in bosses)
        {
            if (boss != null)
            {
                totalHealth += boss.currentHealth;  // Vida actual
                totalMaxHealth += boss.maxHealth;   // Máxima vida (para el máximo del slider)
            }
        }
        totalBossHealth = totalHealth;
        maxTotalBossHealth = totalMaxHealth;

        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (totalHealthSlider != null && maxTotalBossHealth > 0)
        {
            totalHealthSlider.maxValue = maxTotalBossHealth;
            totalHealthSlider.value = totalBossHealth;
        }
    }

    public void ApplyDamageToTotalHealth(int damage)
    {
        totalBossHealth -= damage;
        if (totalBossHealth < 0) totalBossHealth = 0;

        accumulatedDamage += damage;
        UpdateHealthUI();

        int snakeMaxHealth = bosses.Count > 0 ? bosses[0].maxHealth : 4;

        while (accumulatedDamage >= snakeMaxHealth)
        {
            accumulatedDamage -= snakeMaxHealth;

            // Buscar una serpiente activa o apareciendo
            SnakeBossAI target = bosses.FirstOrDefault(b => b != null && (b.IsVisible() || b.IsAppearing()));
            if (target != null)
            {
                target.Die();
                bosses.Remove(target);
                deadSnakesCount++;  // Aumentamos el contador de serpientes muertas
                UpdateShootIntervals();  // Actualizamos intervalos de disparo
            }
            else
            {
                // Si no hay ninguna visible, eliminar una cualquiera muerta
                SnakeBossAI any = bosses.FirstOrDefault();
                if (any != null)
                {
                    any.ForceDisappear();
                    bosses.Remove(any);
                }
            }
        }

        DespawnAllBosses();

        if (totalBossHealth <= 0)
        {
            DespawnAllBosses();

            if (levelController != null && levelController.esLevel3)
            {
                levelController.FinishLevelEarly("¡Todos los jefes derrotados!");
            }
        }
    }

    private void UpdateShootIntervals()
    {
        float newInterval = baseShootInterval * Mathf.Pow(1f - shootIntervalReductionPercent, deadSnakesCount);

        newInterval = Mathf.Max(0.2f, newInterval);

        foreach (var boss in bosses)
        {
            if (boss != null)
            {
                boss.UpdateShootInterval(newInterval);
            }
        }
    }

    public void DespawnAllBosses()
    {
        foreach (var boss in bosses)
        {
            if (boss != null)
                boss.Disappear();
        }

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        isSpawning = false;
    }
}
