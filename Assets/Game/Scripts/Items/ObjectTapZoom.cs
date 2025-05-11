using UnityEngine;

public class ObjectTapZoom : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas infoCanvas;
    public Transform zoomTarget;
    public float zoomSpeed = 5f;
    public float zoomDistance = 0.5f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomed = false;

    void Start()
    {
        originalPosition = mainCamera.transform.position;
        originalRotation = mainCamera.transform.rotation;
        infoCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit Object: " + hit.transform.name);
                if (hit.transform == transform)
                {
                    if (!isZoomed)
                        ZoomIn();
                    else
                        ZoomOut();
                }
            }
        }
    }

    void ZoomIn()
    {
        Vector3 direction = (zoomTarget.position - mainCamera.transform.position).normalized;
        mainCamera.transform.position = zoomTarget.position - direction * zoomDistance;
        mainCamera.transform.LookAt(zoomTarget);
        infoCanvas.gameObject.SetActive(true);
        isZoomed = true;
    }

    void ZoomOut()
    {
        mainCamera.transform.position = originalPosition;
        mainCamera.transform.rotation = originalRotation;
        infoCanvas.gameObject.SetActive(false);
        isZoomed = false;
    }
}
