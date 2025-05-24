using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerTriggerDamage : MonoBehaviour
{
    private int structureDamageAmount = 25; // Attaching this value to a modifier would be good.
    private health structureHealth;

    public static event Action onStructureDestroyed; // Void paramater, don't confuse with below.

    public static event Action<int> onStructureDamaged; // Int paramater, specific event for tracking the HP. Don't confuse with above.

    void Start()
    {
        gameObject.SetActive(true);
        structureHealth = GetComponent<health>();

        if (structureHealth != null)
        {
            // Ensure health is reset and initialized before notifying
            structureHealth.resetHealth();

            onStructureDamaged?.Invoke(structureHealth.getCurrentHealth());
        }
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

                if (onStructureDamaged != null)
                {
                    onStructureDamaged(structureHealth.getCurrentHealth()); // Track current HP.
                }

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


