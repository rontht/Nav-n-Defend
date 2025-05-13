using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/*public class SpawnManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;

    Camera arCam;
    GameObject spawnedObject;

    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
            return;

        if(m_RaycastManager.Raycast(Input.GetTouch(0).position, m_hits))
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    if (m_hits.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = m_hits.collider.gameObject;
                    }
                    else
                    {
                        spawnablePrefab(m_hits[0].pose.position);
                    }
                }
            }
        }
    }
}*/
