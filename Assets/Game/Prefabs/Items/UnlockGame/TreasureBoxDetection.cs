using UnityEngine;
using UnityEngine.EventSystems;

public class TreasureBoxDetection : MonoBehaviour
{
    void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Ignore if user is tapping the box through UI
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        Debug.Log($"{gameObject.name} tapped! Tap Version");
                        PuzzleManager.Instance.StartPuzzle(transform.position);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // For testing in simulation
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.Log($"{gameObject.name} tapped! Mouse Version");
                PuzzleManager.Instance.StartPuzzle(transform.position);
                gameObject.SetActive(false);
            }
        }
    }
}
