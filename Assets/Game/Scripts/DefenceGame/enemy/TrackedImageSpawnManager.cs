using UnityEngine;
using System.Collections;
using System;

public class TrackedImageSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxSpawns = 9;
    public int remainingSpawns;
    public static event Action<TrackedImageSpawnManager> OnSpawnManagerReady;

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

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetTarget(transform);
        }

        UnityEngine.Debug.Log($"Spawns left - {remainingSpawns - 1}");

        // This method could be called if you want to notify when a spawn happens, but
        // it's already reflected in the remainingSpawns variable.
        UpdateRemainingSpawns();
    }

    // Optionally add a method to notify others of spawn count changes
    public void UpdateRemainingSpawns()
    {
        if (remainingSpawns <= 0)
        {
            UnityEngine.Debug.Log("No more spawns left.");
        }
    }
}