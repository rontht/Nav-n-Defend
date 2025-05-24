using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public GameObject spherePrefab; // What the logic should be attached to.
    public float shootForce = 500f;

    public Camera arCamera; // Where the bullet comes out from.

    public static event System.Action OnShotFired; // Event for shooting.

    public void ShootSphere()
    {
        if (spherePrefab == null || arCamera == null) return; // Layered safety to ensure bullet never fires unless ready. -gun safety!-

        UISoundPlayer.Instance.PlayAttackClickSound();
        GameObject sphere = Instantiate(spherePrefab, arCamera.transform.position + arCamera.transform.forward * 0.5f, Quaternion.identity);

        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(arCamera.transform.forward * shootForce);
        }

        Destroy(sphere, 2f); // Cleanup shots missed.

        // Notify UI of a shot made.
        if (OnShotFired != null)
        {
            OnShotFired(); // Allows for tracking of things like accuracy, kill count.
        }
    }
}
