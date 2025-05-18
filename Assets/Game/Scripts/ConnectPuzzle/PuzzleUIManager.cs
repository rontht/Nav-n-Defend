using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public Button backButton;
    public Button finishButton;
    public GameObject winPanel;
    public TMP_Text reward;
    public GameObject scanUI;

    [Header("Game Configs")]
    public int expReward = 3;
    public int coinReward = 10;

    private int currentScore = 0;
    private int maxScore;
    public bool isPuzzleActive = false;

    private void Start()
    {
        // Initialize the UI
        UpdateScoreText(0);
        backButton.onClick.AddListener(GoBack);
        finishButton.onClick.AddListener(GoBack);
        scoreText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        winPanel.SetActive(false);
    }

    public void StartPuzzle(int nodePairs)
    {
        maxScore = nodePairs;
        currentScore = 0;
        UpdateScoreText(0);
        scoreText.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        winPanel.SetActive(false);
        isPuzzleActive = true;
        scanUI.SetActive(false);
    }

    public void UpdateScore()
    {
        currentScore++;
        UpdateScoreText(currentScore);

        // Check if the player has completed the puzzle
        if (currentScore >= maxScore)
        {
            Debug.Log("Puzzle Completed!");
            scoreText.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            winPanel.SetActive(true);
            reward.text = $"{expReward} experience, {coinReward} coins";
            isPuzzleActive = false;

            PlayerStats.Instance.GainExperience(expReward);
            PlayerStats.Instance.AddCoins(coinReward);
            Debug.Log($"You've gained {expReward} EXP for completing the puzzle.");
        }
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score} / {maxScore}";
    }

    private void GoBack()
    {
        // Hide UI when going back
        scoreText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        winPanel.SetActive(false);
        isPuzzleActive = false;
        scanUI.SetActive(true);

        // Reset the puzzle board
        PuzzleManager.Instance.ResetTile();
        PuzzleManager.Instance.CleanUpBoard();

        TreasureBoxDetection.GameEnd();
    }
}
