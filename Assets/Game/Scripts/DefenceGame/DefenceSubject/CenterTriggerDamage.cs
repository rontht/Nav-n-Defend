using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterTriggerDamage : MonoBehaviour
{
    public int structureDamageAmount = 10;  
    private Health structureHealth;         

    void Start()
    {
        structureHealth = GetComponent<Health>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        // Check if the object that triggered the event is an enemy.
        if (root.CompareTag("Enemy"))
        {
            enemyHealth health = root.GetComponent<enemyHealth>();

            //Damage equal to slimes maximum health.
            if (health != null)
            {
                health.TakeDamage(health.maxHealth);  
            }

            //Structure takes 10 damage.
            if (structureHealth != null)
            {
                structureHealth.TakeDamage(structureDamageAmount);
            }
        }
    }
}


