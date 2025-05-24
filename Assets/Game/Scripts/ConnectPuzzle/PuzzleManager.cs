using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject gridPrefab;

    [Header("Game Configs")]
    public int tileCount = 5;
    public float tileSize = 1.0f;
    public int nodeCount = 3;
    public Color[] nodeColors;

    private GameObject[,] gridPieces;
    private List<GameObject> nodes;
    private GameObject startingNode;
    private List<Vector2Int> connectingPath = new List<Vector2Int>();
    private int countForCurrentPath = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destroy duplicate instances
    }

    private void Start()
    {
        // set up game objects
        nodes = new List<GameObject>();
        gridPieces = new GameObject[tileCount, tileCount];
    }

    public void StartPuzzle(Vector3 origin)
    {
        // cleanup the board and start spawning game objects
        CleanUpBoard();
        SpawnTiles(origin);
        SpawnNodes(nodeCount, origin);

        // scaling for the board
        transform.localScale = Vector3.one;

        PuzzleUIManager uiManager = FindObjectOfType<PuzzleUIManager>();
        if (uiManager != null)
            uiManager.StartPuzzle(nodeCount);
    }

    public void CleanUpBoard()
    {
        // wipe everything clean
        foreach (var node in nodes)
            Destroy(node);
        nodes.Clear();
        for (int i = 0; i < tileCount; i++)
            for (int j = 0; j < tileCount; j++)
                if (gridPieces[i, j] != null)
                    Destroy(gridPieces[i, j]);
    }

    private void SpawnTiles(Vector3 origin)
    {
        // spawn a grid of tiles based on tileCount
        for (int i = 0; i < tileCount; i++)
            for (int j = 0; j < tileCount; j++)
            {
                Vector3 position = origin + new Vector3(i * tileSize - (tileCount - 1) * tileSize / 2, 0, j * tileSize - (tileCount - 1) * tileSize / 2);
                GameObject gridPiece = Instantiate(gridPrefab, position, Quaternion.identity);
                gridPiece.name = $"Grid_{i}_{j}";
                gridPiece.transform.localScale = new Vector3(tileSize, 0.01f, tileSize);
                gridPieces[i, j] = gridPiece;
            }
    }

    private void SpawnNodes(int pairCount, Vector3 origin)
    {
        // count available positions on the puzzle board
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int i = 0; i < tileCount; i++)
            for (int j = 0; j < tileCount; j++)
                availablePositions.Add(new Vector2Int(i, j));

        // randomly spawn each node pairs
        for (int pair = 0; pair < pairCount; pair++)
        {
            Color pairColor = nodeColors[pair % nodeColors.Length];
            for (int n = 0; n < 2; n++)
            {
                // pick a random available position
                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector2Int gridPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                // calculate the node position (centered within the tile)
                Vector3 nodePosition = origin + new Vector3(gridPos.x * tileSize - (tileCount - 1) * tileSize / 2, 0, gridPos.y * tileSize - (tileCount - 1) * tileSize / 2);
                GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
                node.name = $"Node_{pair}_{n}";

                // scale node to fit within the tile
                node.transform.localScale = Vector3.one * (tileSize * 0.9f);

                // set the node color
                node.GetComponent<Renderer>().material.color = pairColor;
                nodes.Add(node);
            }
        }
    }

    private void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            // for mobile AR touch
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch.position);
                UpdateTileHighlight(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                UpdateTileHighlight(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                ResetTile();
            }
        }
        else
        {
            // for testing in simulator
            if (Input.GetMouseButtonDown(0))
                HandleTouch(Input.mousePosition);
            else if (Input.GetMouseButton(0))
                UpdateTileHighlight(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0))
                ResetTile();
        }
    }

    private void HandleTouch(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            if (nodes.Contains(hit.transform.gameObject))
                startingNode = hit.transform.gameObject;
    }

    private bool checkIfAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dz = Mathf.Abs(a.y - b.y);
        return (dx == 1 && dz == 0) || (dx == 0 && dz == 1);
    }

    private void UpdateTileHighlight(Vector3 screenPosition)
    {
        // reset if no node is selected
        if (startingNode == null || !startingNode)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject currentTile = hit.transform.gameObject;

            if (currentTile.name.StartsWith("Grid_"))
            {
                // get coordinates of currently touched tile
                string[] parts = currentTile.name.Split('_');
                int x = int.Parse(parts[1]);
                int z = int.Parse(parts[2]);
                Vector2Int currentPos = new Vector2Int(x, z);

                // check if current tile is second most recent tile
                if (connectingPath.Count > 1)
                {
                    Vector2Int previousPos = connectingPath[connectingPath.Count - 2];
                    if (currentPos == previousPos)
                    {
                        // de-highlight the tile most recent tile
                        Vector2Int lastPos = connectingPath[connectingPath.Count - 1];
                        GameObject lastTile = gridPieces[lastPos.x, lastPos.y];
                        if (lastTile != null)
                            lastTile.GetComponent<Renderer>().material.color = Color.white;

                        connectingPath.RemoveAt(connectingPath.Count - 1);
                        countForCurrentPath--;
                        UISoundPlayer.Instance.PlayBackwardClickSound();
                        return;
                    }
                }

                // for next tile check if adjacent
                if (connectingPath.Count > 0)
                {
                    Vector2Int lastPos = connectingPath[connectingPath.Count - 1];
                    if (!checkIfAdjacent(lastPos, currentPos))
                        return;
                }

                // highlight the tile
                if (!connectingPath.Contains(currentPos))
                {
                    Renderer tileRenderer = currentTile.GetComponent<Renderer>();
                    Color highlightColor = startingNode.GetComponent<Renderer>().material.color;
                    tileRenderer.material.color = highlightColor;
                    connectingPath.Add(currentPos);
                    countForCurrentPath++;
                    UISoundPlayer.Instance.PlayHightlightSound();
                }
            }
            // check if current tile contains a node of same color as starting node
            else if (nodes.Contains(currentTile) && currentTile != startingNode)
            {
                if (MatchNodes(startingNode, currentTile))
                {
                    // remove the matching pair
                    nodes.Remove(startingNode);
                    nodes.Remove(currentTile);
                    Destroy(startingNode);
                    Destroy(currentTile);

                    // notify UI
                    PuzzleUIManager uiManager = FindObjectOfType<PuzzleUIManager>();
                    if (uiManager != null)
                        uiManager.UpdateScore(countForCurrentPath);
                    countForCurrentPath = 0;
                    UISoundPlayer.Instance.PlayConnectSound();

                    // reset the tile effects
                    ResetTile();

                    Debug.Log($"Nodes matched and removed.");
                }
                else
                {
                    // reset the path if blocked by different colored nodes
                    ResetTile();
                    Debug.Log("Blocked by a different node.");
                    UISoundPlayer.Instance.PlayBackwardClickSound();
                }
            }
        }
    }

    public void ResetTile()
    {
        foreach (Vector2Int pos in connectingPath)
        {
            GameObject tile = gridPieces[pos.x, pos.y];
            if (tile != null)
                tile.GetComponent<Renderer>().material.color = Color.white;
        }
        connectingPath.Clear();
        startingNode = null;
    }

    private bool MatchNodes(GameObject node1, GameObject node2)
    {
        // this tries to match 2 nodes and return true or false
        return node1.GetComponent<Renderer>().material.color == node2.GetComponent<Renderer>().material.color;
    }
}
