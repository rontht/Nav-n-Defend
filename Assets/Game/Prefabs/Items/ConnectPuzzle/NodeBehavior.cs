using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    public Color nodeColor; // Color of the node
    private static GameObject selectedNode = null; // The currently selected node

    private void OnMouseDown()
    {
        // Handle touch or click event on the node
        if (selectedNode == null)
        {
            // First node selected
            selectedNode = gameObject;
            Debug.Log($"Node {gameObject.name} selected");
        }
        else
        {
            // Second node selected
            if (selectedNode != gameObject && selectedNode.GetComponent<NodeBehavior>().nodeColor == nodeColor)
            {
                Debug.Log("Nodes matched!");
                // Here, you can add logic to visually show the line connection
                selectedNode = null; // Reset selection
            }
            else
            {
                Debug.Log("Not a match!");
                selectedNode = null; // Reset selection
            }
        }
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        nodeColor = color;
    }
}
