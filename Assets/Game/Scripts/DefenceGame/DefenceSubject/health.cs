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
    private int maxHealth = PlayerStats.Instance.maxHP;
    private int currentHealth;

    void Start()
    {
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