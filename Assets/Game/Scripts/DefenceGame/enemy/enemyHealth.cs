using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    // Variable stat, higher equals more points?
    public int maxHealth = 30;

    public static int totalKills = 0;
    private int currentHealth;

    public static string lastKillCause = "Unknown";

    // Create an event to notify when a kill happens.
    public static event System.Action<int> OnKill; 

    void Start()
    {
        currentHealth = maxHealth;
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
        
        // OnKill?.Invoke(totalKills);
        // Supposedly the short hand version for above,
        // Stumbled upon it when researching subscribing.

        Destroy(gameObject);
        UnityEngine.Debug.Log($"Enemy killed by: {cause}. Total Kills = {totalKills}");
    }
}
