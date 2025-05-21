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
    public static event Action OnAllEnemiesDefeated;

    private Coroutine spawnRoutine;
    private int spawnCount = 0;

    public int liveEnemies = 0;
    public bool doneSpawning = false;

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
        yield return new WaitForSeconds(7f);

        while (spawnCount < maxSpawns)
        {
            SpawnEnemy();
            spawnCount++;
            remainingSpawns--;
            yield return new WaitForSeconds(spawnInterval);
        }

        UnityEngine.Debug.Log("Max enemy spawns reached.");
        doneSpawning = true;

        CheckVictoryCondition();  // Check the victory condition after all spawns are done.
    }

    private void SpawnEnemy()
    {
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        // Instantiate the enemy at the spawn point.
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the enemyHealth component.
        enemyHealth enemyHealth = enemy.GetComponent<enemyHealth>();
        if (enemyHealth != null)
        {
            // Assuming health is a separate component on the same enemy object.
            health enemyHealthComponent = enemy.GetComponent<health>();
            if (enemyHealthComponent != null)
            {
                enemyHealthComponent.ResetHealth();  // Call ResetHealth from the health class.
            }

            // Subscribe to the OnDeath event to track enemy deaths.
            enemyHealth.OnDeath += () =>
            {
                liveEnemies--;
                CheckVictoryCondition();  // Check the victory condition after the enemy dies.
            };
        }

        // Enemy Movement.
        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetTarget(transform);  // Ensure movement behavior is set.
        }

        liveEnemies++; // Count this enemy as alive.
        UnityEngine.Debug.Log($"Spawns left - {remainingSpawns - 1}");

        // Update remaining spawns.
        UpdateRemainingSpawns();
    }

    public void UpdateRemainingSpawns()
    {
        if (remainingSpawns <= 0)
        {
            UnityEngine.Debug.Log("No more spawns left.");
        }
    }

    // Check for victory condition after every death/spawn completion.
    private void CheckVictoryCondition()
    {
        if (doneSpawning && liveEnemies <= 0)
        {
            UnityEngine.Debug.Log("All enemies defeated!");
            OnAllEnemiesDefeated?.Invoke(); 
        }
    }
}