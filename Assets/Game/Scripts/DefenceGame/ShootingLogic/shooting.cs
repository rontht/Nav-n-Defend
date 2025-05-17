using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public GameObject spherePrefab;
    public float shootForce = 500f;

    public Camera arCamera;

    public void ShootSphere()
    {
        if (spherePrefab == null || arCamera == null) return;

        GameObject sphere = Instantiate(spherePrefab, arCamera.transform.position + arCamera.transform.forward * 0.5f, Quaternion.identity);

        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(arCamera.transform.forward * shootForce);
        }
    }
}

