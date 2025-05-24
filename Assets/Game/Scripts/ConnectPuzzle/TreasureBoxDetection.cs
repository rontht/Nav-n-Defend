using UnityEngine;
using UnityEngine.EventSystems;

public class TreasureBoxDetection : MonoBehaviour
{
    private static TreasureBoxDetection instance;

    void Awake()
    {
        // Ensure only one instance is managed
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // ignore tap on box if UI is in the way
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

                // if ray cast hit the box
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    GameStart();
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // ignore tap on box if UI is in the way
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // if ray cast hit the box
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                GameStart();
            }
        }
    }

    void GameStart()
    {
        Debug.Log($"{instance.gameObject.name} tapped!");

        // Create an anchor at the box location to make it independent of the original box location
        // GameObject anchor = new GameObject("PuzzleAnchor");
        // anchor.transform.position = transform.position;

        // Create puzzle board with anchor's position in place of the box
        // PuzzleManager.Instance.StartPuzzle(anchor.transform.position, anchor.transform);
        PuzzleManager.Instance.StartPuzzle(transform.position);
        UISoundPlayer.Instance.PlayGameStartSound();
        instance.gameObject.SetActive(false);
    }

    public static void GameEnd()
    {
        UISoundPlayer.Instance.PlayForwardClickSound();
        if (instance != null)
            instance.gameObject.SetActive(true);
    }
}
