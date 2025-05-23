using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerTriggerDamage : MonoBehaviour
{
    private int structureDamageAmount = 25; // Attaching this value to a modifier would be good.
    private health structureHealth;

    public static event Action onStructureDestroyed;

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
                structureHealth.takeDamage(structureDamageAmount);
                //UnityEngine.Debug.Log($"Remaining HP - {Health}");
            }

            if (structureHealth.getCurrentHealth() <= 0)
            {

                if (onStructureDestroyed != null)
                {
                    onStructureDestroyed();
                }

                gameObject.SetActive(false);
            }
        }
    }
}


