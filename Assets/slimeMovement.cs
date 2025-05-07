using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class slimeMovement : MonoBehaviour
{
    public GameObject Cube;
    public GameObject SlimeMesh;
    public float speed;
    public float totalTime = 4f;
    private float currentTime;
   
    // Start is called before the first frame update
    void Start()
    {
        currentTime = totalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }

        if (currentTime <= 0)
        {
           SlimeMesh.transform.position = Vector3.MoveTowards(SlimeMesh.transform.position, Cube.transform.position, speed);
        }
    }
}
