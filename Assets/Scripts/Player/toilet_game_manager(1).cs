using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class SoratnikEmotion
{
    public string emotionName;
    public Sprite faceSprite;
}

[System.Serializable]
public class SoratnikDialogue
{
    [TextArea(2, 4)]
    public string dialogueText;
    public AudioClip voiceClip;
    public string emotionState; // default, amazed, neutral, devilish, angry, smirk, superior
    public float displayDuration = 3f;
}

public class ToiletGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int maxStain = 100;
    public int currentStain = 100;
    public int stainRemovalRate = 1; // How much stain removed per touch move
    public int playerMaxHP = 3;
    public int playerCurrentHP = 3;

    [Header("UI Elements")]
    public Text stainText;
    public Text scoreText;
    public Image[] heartImages; // Array of heart UI images

    [Header("3D Objects")]
    public GameObject handWithFork;
    public GameObject toiletModel;

    [Header("Touch Settings")]
    public float touchDamageInterval = 1f; // Damage player every X seconds while touching
    private float lastDamageTime = 0f;
    private bool isTouching = false;
    private int touchesThisSession = 0;

    [Header("Game Over")]
    public string victorySceneName = "Victory";
    public string defeatSceneName = "Defeat";
    public float gameOverDelay = 2f;

    [Header("Soratnik Character")]
    public Image soratnikBodyImage; // Full body sprite (without face and hand)
    public Image soratnikFaceImage; // Swappable face
    public Image soratnikHandImage; // Swappable hand with tool
    public SoratnikEmotion[] emotions; // Array of 7 emotions
    public Sprite forkHandSprite;
    public Sprite katanaHandSprite;

    [Header("Soratnik Dialogue")]
    public Text dialogueText;
    public Image dialogueBox; // Background for dialogue
    public SoratnikDialogue[] randomDialogues; // Random lines during gameplay
    public AudioSource soratnikVoiceSource;
    public float minDialogueInterval = 10f;
    public float maxDialogueInterval = 20f;
    private float nextDialogueTime = 0f;
    private bool dialogueActive = false;

    private int score = 0;
    private bool gameEnded = false;

    void Start()
    {
        currentStain = maxStain;
        playerCurrentHP = playerMaxHP;
        UpdateUI();
        
        // Setup Soratnik initial state
        SetupSoratnik();
        
        // Schedule first dialogue
        ScheduleNextDialogue();
    }

    void SetupSoratnik()
    {
        // Set initial emotion (default/happy)
        SetSoratnikEmotion("default");
        
        // Set weapon hand based on selected weapon
        string selectedWeapon = PlayerPrefs.GetString("SelectedWeaponName", "Fork");
        if (selectedWeapon == "Katana")
        {
            if (soratnikHandImage != null && katanaHandSprite != null)
                soratnikHandImage.sprite = katanaHandSprite;
        }
        else
        {
            if (soratnikHandImage != null && forkHandSprite != null)
                soratnikHandImage.sprite = forkHandSprite;
        }
        
        // Hide dialogue initially
        if (dialogueBox != null)
            dialogueBox.enabled = false;
        if (dialogueText != null)
            dialogueText.enabled = false;
    }

    void Update()
    {
        HandleTouchInput();
        
        // Damage player over time while touching (optional mechanic)
        if (isTouching && Time.time - lastDamageTime >= touchDamageInterval)
        {
            DamagePlayer(1);
            lastDamageTime = Time.time;
        }
        
        // Check if it's time for Soratnik to speak
        if (!dialogueActive && !gameEnded && Time.time >= nextDialogueTime)
        {
            ShowRandomDialogue();
        }
    }

    void SetSoratnikEmotion(string emotionName)
    {
        if (soratnikFaceImage == null) return;
        
        foreach (SoratnikEmotion emotion in emotions)
        {
            if (emotion.emotionName == emotionName)
            {
                soratnikFaceImage.sprite = emotion.faceSprite;
                Debug.Log("Soratnik emotion changed to: " + emotionName);
                return;
            }
        }
        
        Debug.LogWarning("Emotion not found: " + emotionName);
    }

    void ShowRandomDialogue()
    {
        if (randomDialogues.Length == 0) return;
        
        // Pick random dialogue
        int randomIndex = Random.Range(0, randomDialogues.Length);
        SoratnikDialogue dialogue = randomDialogues[randomIndex];
        
        StartCoroutine(DisplayDialogue(dialogue));
    }

    IEnumerator DisplayDialogue(SoratnikDialogue dialogue)
    {
        dialogueActive = true;
        
        // Set emotion
        SetSoratnikEmotion(dialogue.emotionState);
        
        // Show dialogue box and text
        if (dialogueBox != null)
            dialogueBox.enabled = true;
        if (dialogueText != null)
        {
            dialogueText.enabled = true;
            dialogueText.text = dialogue.dialogueText;
        }
        
        // Play voice clip
        if (soratnikVoiceSource != null && dialogue.voiceClip != null)
        {
            soratnikVoiceSource.PlayOneShot(dialogue.voiceClip);
        }
        
        // Wait for duration
        yield return new WaitForSeconds(dialogue.displayDuration);
        
        // Hide dialogue
        if (dialogueBox != null)
            dialogueBox.enabled = false;
        if (dialogueText != null)
            dialogueText.enabled = false;
        
        // Return to default emotion
        SetSoratnikEmotion("default");
        
        dialogueActive = false;
        
        // Schedule next dialogue
        ScheduleNextDialogue();
    }

    void ScheduleNextDialogue()
    {
        float interval = Random.Range(minDialogueInterval, maxDialogueInterval);
        nextDialogueTime = Time.time + interval;
    }

    void HandleTouchInput()
    {
        // Handle mobile touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchStart();
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnTouchMove();
                    break;

                case TouchPhase.Ended:
                    OnTouchEnd();
                    break;

                case TouchPhase.Canceled:
                    // Exception handling: treat cancel as end
                    OnTouchEnd();
                    break;
            }
        }
        // Handle mouse for testing in editor
        else if (Input.GetMouseButton(0))
        {
            if (!isTouching)
            {
                OnTouchStart();
            }
            OnTouchMove();
        }
        else if (Input.GetMouseButtonUp(0) && isTouching)
        {
            OnTouchEnd();
        }
    }

    void OnTouchStart()
    {
        if (gameEnded) return;

        isTouching = true;
        touchesThisSession = 0;
        lastDamageTime = Time.time;
        Debug.Log("Touch started - cleaning begins!");
    }

    void OnTouchMove()
    {
        if (gameEnded || !isTouching) return;

        // Remove stain
        if (currentStain > 0)
        {
            currentStain -= stainRemovalRate;
            currentStain = Mathf.Max(0, currentStain); // Don't go below 0
            touchesThisSession++;
            
            // Add score for cleaning
            score += stainRemovalRate;
            
            UpdateUI();
            
            Debug.Log("Cleaning... Stain: " + currentStain);
        }
    }

    void OnTouchEnd()
    {
        if (gameEnded) return;

        isTouching = false;
        
        Debug.Log("Touch ended - cleaned " + touchesThisSession + " times this session");
        
        // Check win condition
        if (currentStain <= 0)
        {
            Victory();
        }
    }

    void DamagePlayer(int damage)
    {
        if (gameEnded) return;

        playerCurrentHP -= damage;
        playerCurrentHP = Mathf.Max(0, playerCurrentHP);
        
        UpdateUI();
        
        Debug.Log("Player took damage! HP: " + playerCurrentHP);
        
        // Check lose condition
        if (playerCurrentHP <= 0)
        {
            Defeat();
        }
    }

    void UpdateUI()
    {
        // Update stain text
        if (stainText != null)
        {
            stainText.text = "Stain: " + currentStain + " / " + maxStain;
        }

        // Update score
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        // Update hearts
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < playerCurrentHP)
            {
                heartImages[i].enabled = true; // Show heart
            }
            else
            {
                heartImages[i].enabled = false; // Hide heart
            }
        }
    }

    void Victory()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("VICTORY! Final Score: " + score);
        
        // Save score
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
        
        Invoke("LoadVictoryScene", gameOverDelay);
    }

    void Defeat()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("DEFEAT! Final Score: " + score);
        
        // Save score
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
        
        Invoke("LoadDefeatScene", gameOverDelay);
    }

    void LoadVictoryScene()
    {
        SceneManager.LoadScene(victorySceneName);
    }

    void LoadDefeatScene()
    {
        SceneManager.LoadScene(defeatSceneName);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Handle interruptions (phone calls, minimize)
        if (pauseStatus && isTouching)
        {
            Debug.Log("Application paused during touch - treating as touch end");
            OnTouchEnd();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Handle focus loss (phone calls, notifications)
        if (!hasFocus && isTouching)
        {
            Debug.Log("Application lost focus during touch - treating as touch end");
            OnTouchEnd();
        }
    }
}