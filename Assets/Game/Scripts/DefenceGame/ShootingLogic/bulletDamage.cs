using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletDamage : MonoBehaviour
{
    public int damageAmount = 20;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject root = collision.transform.root.gameObject;

        if (root.CompareTag("Enemy"))
        {
            enemyHealth health = root.GetComponent<enemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }

        Destroy(gameObject);
    }
}
