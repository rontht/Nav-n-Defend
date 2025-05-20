using UnityEngine;
using System.Collections;
using System;

public class trackedImageSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxSpawns = 9;
    public int remainingSpawns;
    public static event Action<trackedImageSpawnManager> OnSpawnManagerReady;

    private Coroutine spawnRoutine;
    private int spawnCount = 0;

    void Start()
    {
        remainingSpawns = maxSpawns;
        if (spawnPoints.Length > 0 && enemyPrefab != null)
        {
            spawnRoutine = StartCoroutine(SpawnRoutine());
            UnityEngine.Debug.Log("Started enemy spawning.");
        }
        OnSpawnManagerReady?.Invoke(this);
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (spawnCount < maxSpawns)
        {
            SpawnEnemy();
            spawnCount++;
            remainingSpawns--;
            yield return new WaitForSeconds(spawnInterval);
        }

        UnityEngine.Debug.Log("Max enemy spawns reached.");
    }

    private void SpawnEnemy()
    {
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        // Instantiate the enemy at the spawn point.
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Reset the health of the enemy.
        health enemyHealth = enemy.GetComponent<health>();
        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();  // Ensure health is reset to maxHealth value.
        }

        // Setup the enemy movement if required.
        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetTarget(transform);  // Ensure movement behavior is set.
        }

        UnityEngine.Debug.Log($"Spawns left - {remainingSpawns - 1}");

        // Optionally update remaining spawns count.
        UpdateRemainingSpawns();
    }

    public void UpdateRemainingSpawns()
    {
        if (remainingSpawns <= 0)
        {
            UnityEngine.Debug.Log("No more spawns left.");
        }
    }
}