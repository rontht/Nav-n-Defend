using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletDamage : MonoBehaviour
{
    // This value should be based off of player damage stat.
    public int damageAmount = 20;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject root = collision.transform.root.gameObject;

        if (root.CompareTag("Enemy"))
        {
            enemyHealth health = root.GetComponent<enemyHealth>();
            if (health != null)
            {
                enemyHealth.TriggerEnemyHit();

                health.TakeDamage(damageAmount, "Bullet");
            }
        }

        // Remove Bullet.
        Destroy(gameObject);
    }
}
