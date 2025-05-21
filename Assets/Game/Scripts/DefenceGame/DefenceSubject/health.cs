using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class health : MonoBehaviour
{
    private int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        ResetHealth();  
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;  
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