// ============================================
// MiniGameManager.cs
// Main controller for the coin catching game
// ============================================
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject miniGameContainer;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject gameModeSelectPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button mode30sButton;
    [SerializeField] private Button mode60sButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitButton;
    
    [Header("Game Components")]
    [SerializeField] private CoinSpawner coinSpawner;
    [SerializeField] private MoneyBagDrag moneyBag;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coinCatchSound;
    [SerializeField] private AudioClip[] endGameVoices; // Random voice lines
    [SerializeField] private AudioClip[] collectVoices; // Random collect voice lines
    
    private int currentScore = 0;
    private int highScore30s = 0;
    private int highScore60s = 0;
    private float gameTime = 0f;
    private float gameDuration = 30f;
    private bool isGameActive = false;
    
    private const string HIGH_SCORE_30S_KEY = "YazukaGame_HighScore30s";
    private const string HIGH_SCORE_60S_KEY = "YazukaGame_HighScore60s";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            miniGameContainer.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        SetupButtons();
        LoadHighScores();
    }
    
    private void SetupButtons()
    {
        closeButton.onClick.AddListener(CloseMiniGame);
        mode30sButton.onClick.AddListener(() => StartGameWithMode(30f));
        mode60sButton.onClick.AddListener(() => StartGameWithMode(60f));
        playAgainButton.onClick.AddListener(ShowModeSelect);
        exitButton.onClick.AddListener(CloseMiniGame);
    }
    
    public static void OpenMiniGame()
    {
        if (Instance != null)
        {
            Instance.ShowModeSelect();
        }
    }
    
    private void ShowModeSelect()
    {
        miniGameContainer.SetActive(true);
        gameModeSelectPanel.SetActive(true);
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 0f;
    }
    
    private void StartGameWithMode(float duration)
    {
        gameDuration = duration;
        gameTime = 0f;
        currentScore = 0;
        isGameActive = true;
        
        gameModeSelectPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        
        UpdateScoreDisplay();
        coinSpawner.StartSpawning(duration);
    }
    
    private void Update()
    {
        if (!isGameActive) return;
        
        // Use unscaled time since main game is paused
        gameTime += Time.unscaledDeltaTime;
        float timeRemaining = gameDuration - gameTime;
        
        if (timeRemaining <= 0)
        {
            EndGame();
        }
        else
        {
            UpdateTimerDisplay(timeRemaining);
        }
    }
    
    private void EndGame()
    {
        isGameActive = false;
        coinSpawner.StopSpawning();
        ClearAllCoins();
        
        // Play random end game voice
        if (endGameVoices.Length > 0)
        {
            AudioClip randomVoice = endGameVoices[Random.Range(0, endGameVoices.Length)];
            audioSource.PlayOneShot(randomVoice);
        }
        
        // Check and save high score
        bool isNewHighScore = false;
        if (gameDuration == 30f && currentScore > highScore30s)
        {
            highScore30s = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_30S_KEY, highScore30s);
            isNewHighScore = true;
        }
        else if (gameDuration == 60f && currentScore > highScore60s)
        {
            highScore60s = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_60S_KEY, highScore60s);
            isNewHighScore = true;
        }
        PlayerPrefs.Save();
        
        ShowGameOver(isNewHighScore);
    }
    
    private void ShowGameOver(bool isNewHighScore)
    {
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        
        finalScoreText.text = $"Score: {currentScore}R";
        int currentHighScore = (gameDuration == 30f) ? highScore30s : highScore60s;
        highScoreText.text = isNewHighScore ? 
            "NEW HIGH SCORE!" : 
            $"High Score: {currentHighScore}R";
    }
    
    public void CloseMiniGame()
    {
        miniGameContainer.SetActive(false);
        Time.timeScale = 1f;
        isGameActive = false;
        coinSpawner.StopSpawning();
        ClearAllCoins();
    }
    
    public void AddScore(int rupees)
    {
        currentScore += rupees;
        UpdateScoreDisplay();
        
        // Play catch sound
        if (coinCatchSound != null)
        {
            audioSource.PlayOneShot(coinCatchSound);
        }
        
        // Occasionally play collect voice
        if (collectVoices.Length > 0 && Random.value < 0.2f) // 20% chance
        {
            AudioClip randomVoice = collectVoices[Random.Range(0, collectVoices.Length)];
            audioSource.PlayOneShot(randomVoice);
        }
    }
    
    private void UpdateScoreDisplay()
    {
        scoreText.text = $"{currentScore}R";
    }
    
    private void UpdateTimerDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    
    private void LoadHighScores()
    {
        highScore30s = PlayerPrefs.GetInt(HIGH_SCORE_30S_KEY, 0);
        highScore60s = PlayerPrefs.GetInt(HIGH_SCORE_60S_KEY, 0);
    }
    
    private void ClearAllCoins()
    {
        foreach (YazukaCoin coin in FindObjectsOfType<YazukaCoin>())
        {
            Destroy(coin.gameObject);
        }
    }
    
    public float GetCurrentGameProgress()
    {
        return gameTime / gameDuration;
    }
}

// ============================================
// MoneyBagDrag.cs
// Handles dragging the money bag left/right
// ============================================
using UnityEngine;
using UnityEngine.EventSystems;

public class MoneyBagDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform bagTransform;
    [SerializeField] private RectTransform gameBounds;
    [SerializeField] private float smoothSpeed = 15f;
    
    private Canvas canvas;
    private bool isDragging = false;
    private float minX, maxX;
    private float bagHalfWidth;
    private float targetX;
    
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        
        // Calculate boundaries
        bagHalfWidth = bagTransform.rect.width / 2f;
        minX = gameBounds.rect.xMin + bagHalfWidth;
        maxX = gameBounds.rect.xMax - bagHalfWidth;
        
        targetX = bagTransform.localPosition.x;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameBounds,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );
        
        targetX = Mathf.Clamp(localPoint.x, minX, maxX);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
    
    private void Update()
    {
        // Smooth movement using unscaled time
        float currentX = bagTransform.localPosition.x;
        float smoothX = Mathf.Lerp(currentX, targetX, smoothSpeed * Time.unscaledDeltaTime);
        bagTransform.localPosition = new Vector3(smoothX, bagTransform.localPosition.y, 0);
    }
}

// ============================================
// CoinSpawner.cs
// Spawns coins with increasing speed over time
// ============================================
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] coinPrefabs; // 5 different coin types
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float baseSpawnInterval = 0.8f;
    [SerializeField] private float spawnIntervalRandomness = 0.2f;
    
    [Header("Speed Progression")]
    [SerializeField] private float baseFallSpeed = 200f;
    [SerializeField] private float maxSpeedMultiplier = 1.5f; // 50% faster at end
    
    private float nextSpawnTime;
    private bool isSpawning = false;
    private float gameDuration;
    
    public void StartSpawning(float duration)
    {
        isSpawning = true;
        gameDuration = duration;
        nextSpawnTime = Time.unscaledTime + baseSpawnInterval;
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
    }
    
    private void Update()
    {
        if (!isSpawning) return;
        
        if (Time.unscaledTime >= nextSpawnTime)
        {
            SpawnCoin();
            float randomOffset = Random.Range(-spawnIntervalRandomness, spawnIntervalRandomness);
            nextSpawnTime = Time.unscaledTime + baseSpawnInterval + randomOffset;
        }
    }
    
    private void SpawnCoin()
    {
        if (coinPrefabs.Length == 0) return;
        
        // Random coin type (weighted towards lower values)
        int coinIndex = GetWeightedCoinIndex();
        GameObject coinPrefab = coinPrefabs[coinIndex];
        
        // Random X position
        float randomX = Random.Range(spawnArea.rect.xMin + 50f, spawnArea.rect.xMax - 50f);
        Vector3 spawnPos = new Vector3(randomX, spawnArea.rect.yMax, 0);
        
        GameObject coinObj = Instantiate(coinPrefab, spawnArea);
        coinObj.GetComponent<RectTransform>().localPosition = spawnPos;
        
        // Set speed based on game progress
        YazukaCoin coin = coinObj.GetComponent<YazukaCoin>();
        if (coin != null)
        {
            float progress = MiniGameManager.Instance.GetCurrentGameProgress();
            float speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, progress);
            coin.SetFallSpeed(baseFallSpeed * speedMultiplier);
        }
    }
    
    private int GetWeightedCoinIndex()
    {
        // Weighted spawn: more common coins spawn more often
        // R1: 40%, 50c: 30%, R2: 15%, 10c: 10%, R5: 5%
        float rand = Random.value;
        
        if (rand < 0.40f) return 2; // R1
        if (rand < 0.70f) return 3; // 50c
        if (rand < 0.85f) return 1; // R2
        if (rand < 0.95f) return 4; // 10c
        return 0; // R5
    }
}

// ============================================
// YazukaCoin.cs
// Individual coin behavior
// ============================================
using UnityEngine;

public class YazukaCoin : MonoBehaviour
{
    [SerializeField] private int rupeeValue = 1;
    
    private RectTransform rectTransform;
    private RectTransform gameBounds;
    private float fallSpeed = 200f;
    private bool isCaught = false;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameBounds = GetComponentInParent<CoinSpawner>().GetComponent<RectTransform>();
    }
    
    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
    }
    
    private void Update()
    {
        if (isCaught) return;
        
        // Move down using unscaled time
        rectTransform.localPosition += Vector3.down * fallSpeed * Time.unscaledDeltaTime;
        
        // Destroy if fell into void
        if (rectTransform.localPosition.y < gameBounds.rect.yMin - 100f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MoneyBag") && !isCaught)
        {
            isCaught = true;
            MiniGameManager.Instance.AddScore(rupeeValue);
            Destroy(gameObject);
        }
    }
}

// ============================================
// SETUP INSTRUCTIONS
// ============================================
/*
HIERARCHY SETUP:
================
MiniGameSystem (DontDestroyOnLoad)
├── MiniGameManager (script)
└── Canvas (Screen Space - Overlay, Sort Order: 1000)
    ├── InputBlocker (Full screen Image, raycast target)
    └── MiniGameContainer
        ├── GameModeSelectPanel
        │   ├── Title Text: "Yazuka Rupee Collector"
        │   ├── Mode30sButton: "30 Seconds"
        │   └── Mode60sButton: "60 Seconds"
        ├── GamePlayPanel
        │   ├── GameBounds (RectTransform - defines play area)
        │   │   ├── SpawnArea (top section for coin spawning)
        │   │   │   └── CoinSpawner (script)
        │   │   └── MoneyBag (Image with BoxCollider2D trigger)
        │   │       └── MoneyBagDrag (script)
        │   ├── ScoreText (Top Left): "0R"
        │   ├── TimerText (Top Center): "00:30"
        │   └── CloseButton (Top Right): "X"
        └── GameOverPanel
            ├── FinalScoreText: "Score: 0R"
            ├── HighScoreText: "High Score: 0R"
            ├── PlayAgainButton: "Play Again"
            └── ExitButton: "Exit"

COIN PREFABS (Create 5 prefabs):
=================================
YazukaCoin_R5 (value: 5)
YazukaCoin_R2 (value: 2)
YazukaCoin_R1 (value: 1)
YazukaCoin_50c (value: 0.5 or use int 50 for cents)
YazukaCoin_10c (value: 0.1 or use int 10 for cents)

Each coin prefab needs:
- Image (your custom coin sprite)
- RectTransform
- BoxCollider2D (Is Trigger: true)
- Rigidbody2D (Kinematic: true)
- YazukaCoin script

TAGS:
=====
Create tag: "MoneyBag"
Assign to MoneyBag GameObject

PHYSICS 2D SETUP:
=================
Edit > Project Settings > Physics 2D
- Ensure collision matrix allows UI layer collisions
- Or use specific layers for coin collection

AUDIO CLIPS:
============
Assign in MiniGameManager:
- coinCatchSound: single coin catch SFX
- endGameVoices[]: array of voice lines for game end
- collectVoices[]: array of voice lines during collection

CANVAS SCALER:
==============
Canvas Scaler component:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920x1080
- Match: 0.5
*/


/* proudly Vibe coded by Claude in a 5 minutes session to fake productivity while I was busy doing other chores in my life.
 *   I am so going to get a subscription with Claude and commit fully into Claude with CLI.....
 *   best Vibe coder AI out there... Hands down.
 *   */