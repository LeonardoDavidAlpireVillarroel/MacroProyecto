using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public List<SnakeBossAI> bosses = new List<SnakeBossAI>();
    public List<Transform> spawnPoints = new List<Transform>();
    public Transform player;

    public float minDistanceBetweenBosses = 3f;
    public float respawnDelay = 5f;

    private bool isSpawning = false;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SpawnBossesRoutine());
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
