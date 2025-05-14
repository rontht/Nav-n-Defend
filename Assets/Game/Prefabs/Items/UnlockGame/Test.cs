using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public static Test Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject gridPrefab;
    public GameObject linePrefab; // This will be used to draw lines

    [Header("Game Configs")]
    public int tileCount = 5;
    public float tileSize = 1.0f;
    public int nodeCount = 3;
    public Color[] nodeColors;

    private GameObject[,] gridPieces;
    private List<GameObject> nodes;
    private GameObject selectedNode;
    private LineRenderer currentLine; // To draw lines between nodes

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // set up game objects
        nodes = new List<GameObject>();
        gridPieces = new GameObject[tileCount, tileCount];
    }

    public void StartPuzzle(Vector3 origin, Transform parent)
    {
        // cleanup the board and start spawning game objects
        CleanUpBoard();
        SpawnTiles(origin, parent);
        SpawnNodes(nodeCount, origin, parent);

        // scaling for the board
        transform.localScale = Vector3.one;
    }

    private void CleanUpBoard()
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

    private void SpawnTiles(Vector3 origin, Transform parent)
    {
        // spawn a grid of tiles based on tileCount
        for (int i = 0; i < tileCount; i++)
            for (int j = 0; j < tileCount; j++)
            {
                Vector3 position = origin + new Vector3(i * tileSize - (tileCount - 1) * tileSize / 2, 0, j * tileSize - (tileCount - 1) * tileSize / 2);
                GameObject gridPiece = Instantiate(gridPrefab, position, Quaternion.identity);
                gridPiece.name = $"Grid_{i}_{j}";
                gridPiece.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                gridPiece.transform.parent = parent.transform;
                gridPieces[i, j] = gridPiece;
            }
    }

    private void SpawnNodes(int pairCount, Vector3 origin, Transform parent)
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
                Vector3 nodePosition = origin + new Vector3(gridPos.x * tileSize - (tileCount - 1) * tileSize / 2, tileSize / 2, gridPos.y * tileSize - (tileCount - 1) * tileSize / 2);
                GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
                node.name = $"Node_{pair}_{n}";

                // scale node to fit within the tile
                node.transform.localScale = Vector3.one * (tileSize * 0.6f);
                node.transform.parent = parent.transform;

                // set the node color
                node.GetComponent<Renderer>().material.color = pairColor;
                nodes.Add(node);
            }
        }
    }

    private void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTouch(Input.GetTouch(0).position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
    }

    private void HandleTouch(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (nodes.Contains(hit.transform.gameObject))
            {
                GameObject tappedNode = hit.transform.gameObject;
                OnNodeTapped(tappedNode);
            }
        }
    }

    private void OnNodeTapped(GameObject tappedNode)
    {
        if (selectedNode == null)
        {
            // First node tapped, select it
            selectedNode = tappedNode;
            Debug.Log($"Node Selected: {tappedNode.name}");

            // Start drawing the line
            currentLine = Instantiate(linePrefab).GetComponent<LineRenderer>();
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, selectedNode.transform.position);
            currentLine.SetPosition(1, selectedNode.transform.position); // Start position
        }
        else
        {
            // Second node tapped, check if they match
            if (MatchNodes(selectedNode, tappedNode))
            {
                Debug.Log("Matched!");
                selectedNode.SetActive(false);
                tappedNode.SetActive(false);

                // Hide the line after matching
                Destroy(currentLine.gameObject);
            }
            else
            {
                Debug.Log("Not a match!");
            }

            // End the line and reset
            currentLine.SetPosition(1, tappedNode.transform.position);
            selectedNode = null;
            currentLine = null;
        }
    }

    private bool MatchNodes(GameObject node1, GameObject node2)
    {
        // this tries to match 2 nodes and return true or false
        return node1.GetComponent<Renderer>().material.color == node2.GetComponent<Renderer>().material.color;
    }
}
