using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class TrackedImageSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxSpawns = 6;

    private GameObject objectToSpawn;
    private Coroutine spawnRoutine;
    private int spawnCount = 0;

    public void Initialize(GameObject spawnPrefab)
    {
        objectToSpawn = spawnPrefab;
        if (spawnRoutine == null && spawnPoints.Length > 0 && objectToSpawn != null)
        {
            spawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawnCount < maxSpawns)
        {
            SpawnOneAtRandomPoint();
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        UnityEngine.Debug.Log("Max spawns reached. Stopping.");
    }

    private void SpawnOneAtRandomPoint()
    {
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform randomPoint = spawnPoints[index];

        GameObject enemy = Instantiate(objectToSpawn, randomPoint.position, randomPoint.rotation);

        // Set the movement target
        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetTarget(this.transform); // Move toward the cube that owns this spawn manager
        }

        spawnCount++;
        UnityEngine.Debug.Log($"Spawned enemy {spawnCount} at {randomPoint.name}");
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            UnityEngine.Debug.Log("Spawning stopped due to image loss.");
        }
    }
}