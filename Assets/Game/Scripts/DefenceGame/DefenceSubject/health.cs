using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


/// <summary>
/// Health of defence subject. 
/// Should be based off of player max HP.
/// </summary>
public class health : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;

    void Awake()
    {
        if (PlayerStats.Instance != null)
        {
            maxHealth = PlayerStats.Instance.maxHP;
        }
        else
        {
            UnityEngine.Debug.LogWarning("PlayerStats.Instance is null! Using fallback value.");
            maxHealth = 150;
        }
        resetHealth();
    }

    public void resetHealth()
    {
        currentHealth = maxHealth;
    }

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        UnityEngine.Debug.Log($"Remaining HP - {currentHealth}");
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }
}