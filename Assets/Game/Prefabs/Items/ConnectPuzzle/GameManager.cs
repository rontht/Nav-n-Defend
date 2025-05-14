using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int connectedPairs = 0;
    public int totalPairs = 3; // Set this according to the number of pairs in your game

    public void OnPairConnected()
    {
        connectedPairs++;
        if (connectedPairs == totalPairs)
        {
            Debug.Log("You Win!");
            // Add any logic for displaying a win message, restarting the game, etc.
        }
    }
}
