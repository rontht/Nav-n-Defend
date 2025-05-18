using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    public static int totalKills = 0;
    public int maxHealth = 30;
    private int currentHealth;

    // Create an event to notify when a kill happens.
    public static event System.Action<int> OnKill; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        totalKills++;

        // Trigger the event when a kill happens.
        if (OnKill != null)
        {
            OnKill(totalKills);
        }
        
        // OnKill?.Invoke(totalKills);
        // Supposedly the short hand version for above,
        // Stumbled upon it when researching subscribing.

        Destroy(gameObject);
        UnityEngine.Debug.Log($"Total Kills = {totalKills}");
    }
}
