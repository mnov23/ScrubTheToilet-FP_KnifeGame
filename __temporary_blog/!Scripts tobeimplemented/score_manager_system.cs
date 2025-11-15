using UnityEngine;
using System;

/// <summary>
/// Singleton Score Manager - persists across scenes
/// Handles current score, high score, and data passing between scenes
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Data")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int highScore = 0;
    [SerializeField] private int previousScore = 0;

    [Header("PlayerPrefs Keys")]
    [SerializeField] private string highScoreKey = "HighScore";
    [SerializeField] private string totalGamesKey = "TotalGames";
    [SerializeField] private string winsKey = "TotalWins";
    [SerializeField] private string lossesKey = "TotalLosses";

    // Statistics
    private int totalGames = 0;
    private int totalWins = 0;
    private int totalLosses = 0;

    // Events for UI updates
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    private void Awake()
    {
        // Singleton pattern - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Score Management

    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        
        CheckAndUpdateHighScore();
    }

    public void SetScore(int score)
    {
        currentScore = score;
        OnScoreChanged?.Invoke(currentScore);
        
        CheckAndUpdateHighScore();
    }

    public void ResetCurrentScore()
    {
        previousScore = currentScore;
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    private void CheckAndUpdateHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            OnHighScoreChanged?.Invoke(highScore);
            Debug.Log($"New High Score: {highScore}");
        }
    }

    #endregion

    #region Game End Methods

    public void RegisterWin()
    {
        totalGames++;
        totalWins++;
        SaveStatistics();
        
        Debug.Log($"Win registered! Total: {totalWins}/{totalGames}");
    }

    public void RegisterLoss()
    {
        totalGames++;
        totalLosses++;
        SaveStatistics();
        
        Debug.Log($"Loss registered! Total: {totalLosses}/{totalGames}");
    }

    #endregion

    #region Data Persistence (PlayerPrefs)

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        totalGames = PlayerPrefs.GetInt(totalGamesKey, 0);
        totalWins = PlayerPrefs.GetInt(winsKey, 0);
        totalLosses = PlayerPrefs.GetInt(lossesKey, 0);
        
        Debug.Log($"High Score loaded: {highScore}");
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, highScore);
        PlayerPrefs.Save();
    }

    private void SaveStatistics()
    {
        PlayerPrefs.SetInt(totalGamesKey, totalGames);
        PlayerPrefs.SetInt(winsKey, totalWins);
        PlayerPrefs.SetInt(lossesKey, totalLosses);
        PlayerPrefs.Save();
    }

    public void ResetAllData()
    {
        highScore = 0;
        currentScore = 0;
        previousScore = 0;
        totalGames = 0;
        totalWins = 0;
        totalLosses = 0;
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("All score data reset");
    }

    #endregion

    #region Getters

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;
    public int GetPreviousScore() => previousScore;
    public int GetTotalGames() => totalGames;
    public int GetTotalWins() => totalWins;
    public int GetTotalLosses() => totalLosses;
    public float GetWinRate() => totalGames > 0 ? (float)totalWins / totalGames * 100f : 0f;

    #endregion

    #region Static Helper Methods (Alternative data passing method)

    // Static data holder for quick scene-to-scene data passing
    private static int staticCurrentScore;
    private static int staticHighScore;
    private static bool staticIsNewHighScore;

    public static void PassScoreData(int current, int high, bool isNewHigh)
    {
        staticCurrentScore = current;
        staticHighScore = high;
        staticIsNewHighScore = isNewHigh;
    }

    public static void GetPassedScoreData(out int current, out int high, out bool isNewHigh)
    {
        current = staticCurrentScore;
        high = staticHighScore;
        isNewHigh = staticIsNewHighScore;
    }

    #endregion
}