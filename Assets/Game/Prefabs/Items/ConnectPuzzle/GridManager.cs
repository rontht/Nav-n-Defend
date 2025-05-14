using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int gridSize = 5;
    public float tileSize = 1.0f;
    public Color[] nodeColors;

    private List<GameObject> nodes = new List<GameObject>();

    // Method to spawn the grid and place nodes
    public void SetupGrid(Vector3 origin)
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        // Generate positions for each tile
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                availablePositions.Add(new Vector2Int(i, j));
            }
        }

        // Create node pairs
        for (int pair = 0; pair < nodeColors.Length; pair++)
        {
            for (int n = 0; n < 2; n++)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector2Int position = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                // Calculate the node position
                Vector3 nodePosition = origin + new Vector3(position.x * tileSize, 0.5f, position.y * tileSize);

                // Create and setup the node
                GameObject node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
                node.GetComponent<NodeBehavior>().SetColor(nodeColors[pair]);

                nodes.Add(node);
            }
        }
    }
}
