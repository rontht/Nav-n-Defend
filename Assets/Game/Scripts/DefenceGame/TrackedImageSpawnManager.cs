using UnityEngine;
using System.Collections;
using System;

public class TrackedImageSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxSpawns = 9;

    private Coroutine spawnRoutine;
    private int spawnCount = 0;

    void Start()
    {
        if (spawnPoints.Length > 0 && enemyPrefab != null)
        {
            spawnRoutine = StartCoroutine(SpawnRoutine());
            UnityEngine.Debug.Log("Started enemy spawning.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Spawn points or enemy prefab not assigned.");
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(10f);

        while (spawnCount < maxSpawns)
        {
            SpawnEnemy();
            spawnCount++;
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

        UnityEngine.Debug.Log($"Spawned enemy {spawnCount + 1} at {spawnPoint.name}");
    }
}