using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Health Stats")]
    public int maxHP;
    public int currentHP;

    [Header("Combat Stats")]
    public int damage;
    public int armor;


    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
