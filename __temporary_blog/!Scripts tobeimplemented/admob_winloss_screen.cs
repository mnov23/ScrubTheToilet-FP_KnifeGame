using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// Manages Win/Loss screen with Google AdMob integration and High Score display
/// </summary>
public class WinLossScreenManager : MonoBehaviour
{
    [Header("UI References - Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject lossPanel;
    
    [Header("UI References - Text Elements")]
    [SerializeField] private Text resultText;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text newHighScoreText; // "NEW HIGH SCORE!" indicator
    [SerializeField] private Text statsText; // Optional: Win rate, total games, etc.
    [SerializeField] private Text rewardText;
    
    [Header("UI References - Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button watchAdButton;

    [Header("AdMob Settings")]
    [SerializeField] private bool useTestAds = true;
    [SerializeField] private string androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] private string androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    
    [Header("Ad Behavior")]
    [SerializeField] private bool showInterstitialOnLoss = true;
    [SerializeField] private bool showRewardedOnWin = true;
    [SerializeField] private int rewardAmount = 100;

    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Score Display Format")]
    [SerializeField] private string scoreFormat = "Score: {0}";
    [SerializeField] private string highScoreFormat = "High Score: {0}";
    [SerializeField] private Color newHighScoreColor = Color.yellow;

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private bool isWin = false;
    private bool isNewHighScore = false;
    private int displayedCurrentScore;
    private int displayedHighScore;

    private void Start()
    {
        InitializeAdMob();
        SetupButtons();
        LoadAndDisplayScores();
    }

    #region Score Management

    private void LoadAndDisplayScores()
    {
        if (ScoreManager.Instance != null)
        {
            displayedCurrentScore = ScoreManager.Instance.GetCurrentScore();
            displayedHighScore = ScoreManager.Instance.GetHighScore();
            isNewHighScore = (displayedCurrentScore >= displayedHighScore && displayedCurrentScore > 0);
        }
        else
        {
            // Fallback if ScoreManager not found - use static method
            ScoreManager.GetPassedScoreData(out displayedCurrentScore, out displayedHighScore, out isNewHighScore);
            
            Debug.LogWarning("ScoreManager instance not found. Using static data.");
        }

        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        // Update current score
        if (currentScoreText != null)
        {
            currentScoreText.text = string.Format(scoreFormat, displayedCurrentScore);
        }

        // Update high score
        if (highScoreText != null)
        {
            highScoreText.text = string.Format(highScoreFormat, displayedHighScore);
        }

        // Show new high score indicator
        if (newHighScoreText != null)
        {
            newHighScoreText.gameObject.SetActive(isNewHighScore);
            if (isNewHighScore)
            {
                newHighScoreText.text = "NEW HIGH SCORE!";
                newHighScoreText.color = newHighScoreColor;
            }
        }

        // Optional: Display statistics
        if (statsText != null && ScoreManager.Instance != null)
        {
            int totalWins = ScoreManager.Instance.GetTotalWins();
            int totalGames = ScoreManager.Instance.GetTotalGames();
            float winRate = ScoreManager.Instance.GetWinRate();
            
            statsText.text = $"Wins: {totalWins}/{totalGames} ({winRate:F1}%)";
        }

        Debug.Log($"Scores displayed - Current: {displayedCurrentScore}, High: {displayedHighScore}, New High: {isNewHighScore}");
    }

    #endregion

    #region AdMob Initialization

    private void InitializeAdMob()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized successfully");
            LoadAds();
        });
    }

    private void LoadAds()
    {
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    #endregion

    #region Interstitial Ad Management

    private void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        string adUnitId;
        #if UNITY_ANDROID
            adUnitId = useTestAds ? "ca-app-pub-3940256099942544/1033173712" : androidInterstitialAdUnitId;
        #elif UNITY_IOS
            adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            adUnitId = "unused";
        #endif

        AdRequest request = new AdRequest.Builder().Build();
        
        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded successfully");
            interstitialAd = ad;
            RegisterInterstitialEventHandlers(interstitialAd);
        });
    }

    private void RegisterInterstitialEventHandlers(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad closed");
            LoadInterstitialAd();
            OnInterstitialAdClosed();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to show: " + error);
            LoadInterstitialAd();
            OnInterstitialAdClosed();
        };
    }

    private void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready");
            OnInterstitialAdClosed();
        }
    }

    private void OnInterstitialAdClosed()
    {
        if (continueButton != null)
        {
            continueButton.interactable = true;
        }
    }

    #endregion

    #region Rewarded Ad Management

    private void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        string adUnitId;
        #if UNITY_ANDROID
            adUnitId = useTestAds ? "ca-app-pub-3940256099942544/5224354917" : androidRewardedAdUnitId;
        #elif UNITY_IOS
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
            adUnitId = "unused";
        #endif

        AdRequest request = new AdRequest.Builder().Build();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                if (watchAdButton != null) watchAdButton.interactable = false;
                return;
            }

            Debug.Log("Rewarded ad loaded successfully");
            rewardedAd = ad;
            if (watchAdButton != null) watchAdButton.interactable = true;
            RegisterRewardedEventHandlers(rewardedAd);
        });
    }

    private void RegisterRewardedEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            LoadRewardedAd();
        };
    }

    private void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad completed. Reward: {reward.Amount}");
                GiveReward();
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready");
            if (rewardText != null)
            {
                rewardText.text = "Ad not available. Try again later.";
            }
        }
    }

    private void GiveReward()
    {
        // Add reward to score or currency
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(rewardAmount);
            displayedCurrentScore = ScoreManager.Instance.GetCurrentScore();
            displayedHighScore = ScoreManager.Instance.GetHighScore();
            UpdateScoreDisplay();
        }
        
        if (rewardText != null)
        {
            rewardText.text = $"+{rewardAmount} Bonus!";
        }

        Debug.Log($"Player rewarded with {rewardAmount}");
        
        if (watchAdButton != null)
        {
            watchAdButton.interactable = false;
            watchAdButton.GetComponentInChildren<Text>().text = "Reward Claimed!";
        }
    }

    #endregion

    #region Win/Loss Screen Control

    public void ShowWinScreen()
    {
        isWin = true;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterWin();
        }
        
        if (winPanel != null) winPanel.SetActive(true);
        if (lossPanel != null) lossPanel.SetActive(false);
        
        if (resultText != null)
        {
            resultText.text = isNewHighScore ? "VICTORY!\nNEW RECORD!" : "VICTORY!";
        }

        if (showRewardedOnWin && watchAdButton != null)
        {
            watchAdButton.gameObject.SetActive(true);
            watchAdButton.interactable = (rewardedAd != null && rewardedAd.CanShowAd());
        }

        LoadAndDisplayScores();
        Debug.Log("Win screen displayed");
    }

    public void ShowLossScreen()
    {
        isWin = false;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterLoss();
        }
        
        if (winPanel != null) winPanel.SetActive(false);
        if (lossPanel != null) lossPanel.SetActive(true);
        
        if (resultText != null)
        {
            resultText.text = isNewHighScore ? "DEFEAT\nBut New High Score!" : "DEFEAT";
        }

        if (watchAdButton != null)
        {
            watchAdButton.gameObject.SetActive(false);
        }

        if (showInterstitialOnLoss)
        {
            if (continueButton != null) continueButton.interactable = false;
            Invoke(nameof(ShowInterstitialAd), 0.5f);
        }

        LoadAndDisplayScores();
        Debug.Log("Loss screen displayed");
    }

    #endregion

    #region Button Handlers

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
        }

        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
            watchAdButton.gameObject.SetActive(false);
        }
    }

    private void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked");
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }
        
        LoadScene(mainMenuSceneName);
    }

    private void OnRetryButtonClicked()
    {
        Debug.Log("Retry button clicked");
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }
        
        LoadScene(gameSceneName);
    }

    private void OnWatchAdButtonClicked()
    {
        Debug.Log("Watch Ad button clicked");
        ShowRewardedAd();
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Call this when player wins the game
    /// Optionally pass the final score
    /// </summary>
    public void OnGameWin(int finalScore = -1)
    {
        if (finalScore >= 0 && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScore(finalScore);
        }
        
        ShowWinScreen();
    }

    /// <summary>
    /// Call this when player loses the game
    /// Optionally pass the final score
    /// </summary>
    public void OnGameLoss(int finalScore = -1)
    {
        if (finalScore >= 0 && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScore(finalScore);
        }
        
        ShowLossScreen();
    }

    #endregion
}