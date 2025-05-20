using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerTriggerDamage : MonoBehaviour
{
    private int structureDamageAmount = 25;  
    private health structureHealth;

    public static event Action OnStructureDestroyed;

    void Start()
    {
        gameObject.SetActive(true);
        structureHealth = GetComponent<health>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        // Check if the object that triggered the event is an enemy.
        if (root.CompareTag("Enemy"))
        {
            enemyHealth health = root.GetComponent<enemyHealth>();

            // Damage equal to slimes maximum health.
            if (health != null)
            {
                health.TakeDamage(health.maxHealth, "CenterTrigger");  
            }

            //Structure structureDamageAmount value.
            if (structureHealth != null)
            {
                structureHealth.TakeDamage(structureDamageAmount);
                //UnityEngine.Debug.Log($"Remaining HP - {Health}");
            }

            if (structureHealth.GetCurrentHealth() <= 0)
            {

                if (OnStructureDestroyed != null)
                {
                    OnStructureDestroyed();
                }

                gameObject.SetActive(false);
            }
        }
    }
}


