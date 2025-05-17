using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

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
        //UnityEngine.Debug.Log($"{gameObject.name} has died.");

        //Good for now. Look into death animation.
        Destroy(gameObject); 
    }
}
