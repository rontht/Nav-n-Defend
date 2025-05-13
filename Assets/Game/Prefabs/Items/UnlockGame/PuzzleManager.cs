using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    public GameObject nodePrefab;         // Capsule node prefab
    public GameObject gridPrefab;         // Single grid piece prefab (like a chessboard tile)
    public int gridSize = 5;              // Grid size (5x5)
    public float tileSize = 1.0f;         // Size of each grid tile
    public Color[] nodeColors;            // Colors for the node pairs (at least 3 colors)

    private GameObject[,] gridPieces;     // Store grid pieces for layout
    private List<GameObject> nodes;       // Store spawned nodes
    private GameObject selectedNode;      // Currently selected node

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        nodes = new List<GameObject>();
        gridPieces = new GameObject[gridSize, gridSize];
    }

    public void StartPuzzle()
    {
        Debug.Log("Puzzle Started!");

        // Clear any existing grid and nodes
        ClearExistingGridAndNodes();

        // Spawn the grid
        SpawnGrid();

        // Spawn 3 pairs of nodes
        SpawnNodePairs(3);
    }

    private void ClearExistingGridAndNodes()
    {
        foreach (var node in nodes)
        {
            Destroy(node);
        }
        nodes.Clear();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (gridPieces[i, j] != null)
                {
                    Destroy(gridPieces[i, j]);
                }
            }
        }
    }

    private void SpawnGrid()
    {
        float halfGrid = (gridSize - 1) * tileSize / 2;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(i * tileSize - halfGrid, 0, j * tileSize - halfGrid);
                GameObject gridPiece = Instantiate(gridPrefab, position, Quaternion.identity);
                gridPiece.name = $"Grid_{i}_{j}";
                gridPiece.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                gridPieces[i, j] = gridPiece;
            }
        }
    }

    private void SpawnNodePairs(int pairCount)
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        // Populate all possible grid positions
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                availablePositions.Add(new Vector2Int(i, j));
            }
        }

        // Spawn each pair
        for (int pair = 0; pair < pairCount; pair++)
        {
            Color pairColor = nodeColors[pair % nodeColors.Length];

            for (int n = 0; n < 2; n++)
            {
                // Pick a random available position
                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector2Int gridPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                // Calculate the node position (centered within the tile)
                Vector3 nodePosition = new Vector3(gridPos.x * tileSize - (gridSize - 1) * tileSize / 2,
                                                   tileSize / 2,
                                                   gridPos.y * tileSize - (gridSize - 1) * tileSize / 2);
                
                GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
                node.name = $"Node_{pair}_{n}";
                
                // Scale node to fit within the tile
                node.transform.localScale = Vector3.one * (tileSize * 0.6f);

                // Set the node color
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
        else if (Input.GetMouseButtonDown(0)) // For editor testing
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
        }
        else
        {
            // Second node tapped, check if they match
            if (AreNodesMatching(selectedNode, tappedNode))
            {
                Debug.Log("Matched!");
                selectedNode.SetActive(false);
                tappedNode.SetActive(false);
            }
            else
            {
                Debug.Log("Not a match!");
            }

            selectedNode = null;  // Reset selection
        }
    }

    private bool AreNodesMatching(GameObject node1, GameObject node2)
    {
        return node1.GetComponent<Renderer>().material.color == node2.GetComponent<Renderer>().material.color;
    }
}
