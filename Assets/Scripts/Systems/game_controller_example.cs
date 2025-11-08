using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Example Game Controller showing how to integrate ScoreManager
/// Place this in your main gameplay scene
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text scoreDisplayText;
    [SerializeField] private Text highScoreDisplayText;

    [Header("Game Settings")]
    [SerializeField] private string winLossSceneName = "WinLossScene";
    [SerializeField] private int pointsPerAction = 10; // Example: points per enemy killed, coin collected, etc.

    [Header("Win/Loss Conditions")]
    [SerializeField] private int scoreToWin = 1000;
    [SerializeField] private int livesToLose = 3;

    private int currentLives;
    private bool gameEnded = false;

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        UpdateScoreUI();
        
        // Example: Press Space to add score (replace with your actual game logic)
        if (Input.GetKeyDown(KeyCode.Space) && !gameEnded)
        {
            AddPoints(pointsPerAction);
        }

        // Example: Press L to lose a life (replace with your actual game logic)
        if (Input.GetKeyDown(KeyCode.L) && !gameEnded)
        {
            LoseLife();
        }
    }

    #region Initialization

    private void InitializeGame()
    {
        // Reset score for new game
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
            
            // Subscribe to score change events (optional, for real-time UI updates)
            ScoreManager.Instance.OnScoreChanged += OnScoreUpdated;
        }
        else
        {
            Debug.LogError("ScoreManager not found! Make sure it's in your scene or set to DontDestroyOnLoad");
        }

        currentLives = livesToLose;
        gameEnded = false;

        Debug.Log("Game initialized");
    }

    #endregion

    #region Score Management

    public void AddPoints(int points)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
            
            // Check win condition
            if (ScoreManager.Instance.GetCurrentScore() >= scoreToWin)
            {
                TriggerWin();
            }
        }
    }

    private void OnScoreUpdated(int newScore)
    {
        Debug.Log($"Score updated to: {newScore}");
        
        // You can add score popup effects, sounds, etc. here
    }

    private void UpdateScoreUI()
    {
        if (ScoreManager.Instance != null)
        {
            if (scoreDisplayText != null)
            {
                scoreDisplayText.text = $"Score: {ScoreManager.Instance.GetCurrentScore()}";
            }

            if (highScoreDisplayText != null)
            {
                highScoreDisplayText.text = $"Best: {ScoreManager.Instance.GetHighScore()}";
            }
        }
    }

    #endregion

    #region Game End Conditions

    public void LoseLife()
    {
        currentLives--;
        Debug.Log($"Lives remaining: {currentLives}");

        if (currentLives <= 0)
        {
            TriggerLoss();
        }
    }

    public void TriggerWin()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("Player Won!");

        // Optional: Pass score data using static method (if not using singleton)
        if (ScoreManager.Instance != null)
        {
            int currentScore = ScoreManager.Instance.GetCurrentScore();
            int highScore = ScoreManager.Instance.GetHighScore();
            bool isNewHigh = currentScore >= highScore;
            
            ScoreManager.PassScoreData(currentScore, highScore, isNewHigh);
        }

        // Load Win/Loss scene
        LoadWinLossScene();
    }

    public void TriggerLoss()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("Player Lost!");

        // Optional: Pass score data using static method
        if (ScoreManager.Instance != null)
        {
            int currentScore = ScoreManager.Instance.GetCurrentScore();
            int highScore = ScoreManager.Instance.GetHighScore();
            bool isNewHigh = currentScore >= highScore;
            
            ScoreManager.PassScoreData(currentScore, highScore, isNewHigh);
        }

        // Load Win/Loss scene
        LoadWinLossScene();
    }

    private void LoadWinLossScene()
    {
        Time.timeScale = 1f; // Ensure time is running
        SceneManager.LoadScene(winLossSceneName);
    }

    #endregion

    #region Example Game Events (Replace with your actual game logic)

    // Example: Call this when player collects a coin
    public void OnCoinCollected()
    {
        AddPoints(10);
        Debug.Log("Coin collected!");
    }

    // Example: Call this when player defeats an enemy
    public void OnEnemyDefeated()
    {
        AddPoints(50);
        Debug.Log("Enemy defeated!");
    }

    // Example: Call this when player completes a level
    public void OnLevelCompleted()
    {
        AddPoints(500);
        Debug.Log("Level completed!");
    }

    // Example: Call this when player takes damage
    public void OnPlayerDamaged()
    {
        LoseLife();
        Debug.Log("Player damaged!");
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= OnScoreUpdated;
        }
    }

    #endregion
}

// ==============================================
// ALTERNATIVE: Simple Data Passing Script
// Use this if you don't want a singleton pattern
// ==============================================

/// <summary>
/// Simple static class for passing score data between scenes
/// No persistence, just scene-to-scene data transfer
/// </summary>
public static class GameDataTransfer
{
    public static int CurrentScore { get; set; }
    public static int HighScore { get; set; }
    public static bool IsWin { get; set; }
    public static bool IsNewHighScore { get; set; }

    public static void SetGameEndData(int currentScore, int highScore, bool isWin)
    {
        CurrentScore = currentScore;
        HighScore = highScore;
        IsWin = isWin;
        IsNewHighScore = currentScore >= highScore && currentScore > 0;
        
        // Save high score to PlayerPrefs
        if (IsNewHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}

// ==============================================
// USAGE EXAMPLES IN YOUR GAME CODE:
// ==============================================

/*
// METHOD 1: Using ScoreManager (Recommended)
void Start()
{
    ScoreManager.Instance.AddScore(100);
}

void OnGameWin()
{
    SceneManager.LoadScene("WinLossScene");
    // ScoreManager persists automatically via DontDestroyOnLoad
}

// METHOD 2: Using GameDataTransfer (Simple)
void OnGameEnd()
{
    int currentScore = 1500;
    int highScore = GameDataTransfer.LoadHighScore();
    bool isWin = true;
    
    GameDataTransfer.SetGameEndData(currentScore, highScore, isWin);
    SceneManager.LoadScene("WinLossScene");
}

// Then in WinLossScene:
void Start()
{
    int score = GameDataTransfer.CurrentScore;
    int high = GameDataTransfer.HighScore;
    bool newHigh = GameDataTransfer.IsNewHighScore;
}
*/