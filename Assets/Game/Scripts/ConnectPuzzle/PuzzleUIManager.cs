using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public Button backButton;
    public GameObject scorePanel;
    public Button finishButton;
    public GameObject winPanel;
    public TMP_Text rewardCoins;
    public TMP_Text rewardExp;
    public GameObject scanUI;

    [Header("Game Configs")]
    public int expReward = 3;
    public int coinReward = 10;

    private int currentScore = 0;
    private int maxScore;
    public bool isPuzzleActive = false;

    private int totalCount = 0;

    private void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in the PuzzleUIManager.");
        }
        // Initialize the UI
        UpdateScoreText(0);
        backButton.onClick.AddListener(GoBack);
        finishButton.onClick.AddListener(GoBack);
        scorePanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void StartPuzzle(int nodePairs)
    {
        maxScore = nodePairs;
        currentScore = 0;
        totalCount = 0;
        UpdateScoreText(0);
        scorePanel.SetActive(true);
        winPanel.SetActive(false);
        isPuzzleActive = true;
        scanUI.SetActive(false);
    }

    public void UpdateScore(int succcessCount)
    {
        currentScore++;
        totalCount += succcessCount;
        UpdateScoreText(currentScore);

        // Check if the player has completed the puzzle
        if (currentScore >= maxScore)
        {
            int stars = UpdateStarRating();
            Debug.Log($"You've gained {stars} Stars for completing the puzzle.");

            scorePanel.SetActive(false);
            winPanel.SetActive(true);

            rewardExp.text = $"EXP Earned: {expReward}";
            rewardCoins.text = $"Coins Earned: {coinReward}";
            isPuzzleActive = false;

            PlayerStats.Instance.GainExperience(expReward);
            PlayerStats.Instance.AddCoins(coinReward);
            UISoundPlayer.Instance.PlayVictorySound();
        }
    }

    private int UpdateStarRating()
    {
        if (totalCount <= 15)
            return 3;
        else if (totalCount >= 16 && totalCount <= 20)
            return 2;
        else if (totalCount >= 21)
            return 1;
        return 0;
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score} / {maxScore}. Tile Count: {totalCount}";
    }

    private void GoBack()
    {
        // Hide UI when going back
        scorePanel.SetActive(false);
        winPanel.SetActive(false);
        isPuzzleActive = false;
        scanUI.SetActive(true);

        // Reset the puzzle board
        PuzzleManager.Instance.ResetTile();
        PuzzleManager.Instance.CleanUpBoard();

        TreasureBoxDetection.GameEnd();
    }
}
