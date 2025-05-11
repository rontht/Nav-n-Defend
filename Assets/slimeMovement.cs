using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class slimeMovement : MonoBehaviour
{

    //[SerializeField] private float _moveSpeed = 5f;
    
    public GameObject Cube;
    public GameObject SlimeMesh;
    
    //private Vector2 _velocity;
    
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
            float step = speed * Time.deltaTime;
            SlimeMesh.transform.position = Vector3.MoveTowards(SlimeMesh.transform.position, Cube.transform.position, step);
            //storedSpeed = speed;
            //SlimeMesh.transform.position = (Vector3)_velocity * _moveSpeed * Time.deltaTime;
        }
    }
}
