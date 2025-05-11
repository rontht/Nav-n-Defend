using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    public GameObject unlockPopUp;
    public GameObject closeButton;
    public GameObject puzzleBoard;

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                ObjectOnTap(ray);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                ObjectOnTap(ray);
            }
        }
    }

    void ObjectOnTap(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                // Debug.Log($"{gameObject.name} tapped!");
                ShowUnlockPopUp();
            }
        }
    }

    void ShowUnlockPopUp()
    {
        if (unlockPopUp != null && puzzleBoard != null)
        {
            unlockPopUp.SetActive(true);
            puzzleBoard.SetActive(true);
        }
    }

    public void HideUnlockPopUp()
    {
        if (unlockPopUp != null & puzzleBoard != null)
        {
            unlockPopUp.SetActive(false);
            puzzleBoard.SetActive(false);
        }
    }
}
