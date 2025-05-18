using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int maxHealth = 125;
    private int currentHealth;

    void Start()
    {
        ResetHealth();  // Ensure health is reset at the start of the object lifecycle
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;  // Reset health to maxHealth
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UnityEngine.Debug.Log($"Remaining HP - {currentHealth}");
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}