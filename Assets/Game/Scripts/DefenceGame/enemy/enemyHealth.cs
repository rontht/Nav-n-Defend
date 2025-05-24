using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public class enemyHealth : MonoBehaviour
{
    // Variable stat, higher equals more points?
    public int maxHealth = 30;
    public static int totalKills = 0;
    public static string lastKillCause = "Unknown";

    private int currentHealth;

    // Events.
    public static event Action<int> OnKill;
    public static event Action OnEnemyHit;
    public event Action OnDeath;

    void Start()
    {
        ResetHealth();  // Call ResetHealth to initialize health at the start.
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;  // Reset health to maxHealth value.
    }

    public void TakeDamage(int amount, string cause = "Unknown")
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die(cause);
        }
    }

    private void Die(string cause)
    {
        totalKills++;

        lastKillCause = cause;

        // Trigger the event when a kill happens.
        if (OnKill != null)
        {
            OnKill(totalKills);
        }

        if (OnDeath != null)
        {
            OnDeath();
        }
        UISoundPlayer.Instance.PlayDeathSound();
        Destroy(gameObject);  // Destroy the enemy object.
        UnityEngine.Debug.Log($"Enemy killed by: {cause}. Total Kills = {totalKills}");
    }

    public static void TriggerEnemyHit()
    {
        if (OnEnemyHit != null)
        {
            OnEnemyHit();
        }
    }
}
