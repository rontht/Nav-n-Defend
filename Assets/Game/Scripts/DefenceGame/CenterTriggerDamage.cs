using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterTriggerDamage : MonoBehaviour
{
    public int damageAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Enemy"))
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }

            Destroy(root); // Destroy entire enemy
        }
    }
}

