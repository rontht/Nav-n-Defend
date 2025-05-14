using UnityEngine;
using UnityEngine.EventSystems;

public class TreasureBoxDetection : MonoBehaviour
{
    private GameObject puzzleAnchor;

    void OnEnable()
    {
        transform.parent = null;
    }

    void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // ignore the tap if UI is in the way
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform)
                    {
                        GameStart("Tap");
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            GameStart("Mouse");
        }
    }

    void GameStart(string text)
    {
        Debug.Log($"{gameObject.name} tapped! {text} Version");

        // create an anchor at box location to make it indepedent of original box location
        puzzleAnchor = new GameObject("PuzzleAnchor");
        puzzleAnchor.transform.position = transform.position;

        // create puzzle board with anchor's position in place of the box
        PuzzleManager.Instance.StartPuzzle(puzzleAnchor.transform.position, puzzleAnchor.transform);
        gameObject.SetActive(false);
    }
}
