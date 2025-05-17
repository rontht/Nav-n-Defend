using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshCollider), typeof(ARPlaneMeshVisualizer))]
public class PlaneColliderUpdater : MonoBehaviour
{
    private MeshCollider meshCollider;
    private ARPlaneMeshVisualizer meshVisualizer;

    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
    }

    void Update()
    {
        meshCollider.sharedMesh = meshVisualizer.mesh;
    }
}
