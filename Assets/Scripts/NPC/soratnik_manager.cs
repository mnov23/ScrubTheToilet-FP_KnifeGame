using UnityEngine;
using UnityEngine.UI;
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

public class Soratnik : MonoBehaviour
{
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
    private bool isActive = true;

    void Start()
    {
        SetupSoratnik();
        ScheduleNextDialogue();
    }

    void SetupSoratnik()
    {
        // Set initial emotion (default/happy)
        SetEmotion("default");
        
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
        // Check if it's time for Soratnik to speak
        if (isActive && !dialogueActive && Time.time >= nextDialogueTime)
        {
            ShowRandomDialogue();
        }
    }

    public void SetEmotion(string emotionName)
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

    public void ShowRandomDialogue()
    {
        if (randomDialogues.Length == 0) return;
        
        // Pick random dialogue
        int randomIndex = Random.Range(0, randomDialogues.Length);
        SoratnikDialogue dialogue = randomDialogues[randomIndex];
        
        StartCoroutine(DisplayDialogue(dialogue));
    }

    public void ShowSpecificDialogue(int index)
    {
        if (index < 0 || index >= randomDialogues.Length) return;
        
        StartCoroutine(DisplayDialogue(randomDialogues[index]));
    }

    public void ShowCustomDialogue(string text, string emotion, float duration = 3f)
    {
        SoratnikDialogue customDialogue = new SoratnikDialogue();
        customDialogue.dialogueText = text;
        customDialogue.emotionState = emotion;
        customDialogue.displayDuration = duration;
        
        StartCoroutine(DisplayDialogue(customDialogue));
    }

    IEnumerator DisplayDialogue(SoratnikDialogue dialogue)
    {
        dialogueActive = true;
        
        // Set emotion
        SetEmotion(dialogue.emotionState);
        
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
        SetEmotion("default");
        
        dialogueActive = false;
        
        // Schedule next dialogue (only if using random system)
        if (isActive)
        {
            ScheduleNextDialogue();
        }
    }

    void ScheduleNextDialogue()
    {
        float interval = Random.Range(minDialogueInterval, maxDialogueInterval);
        nextDialogueTime = Time.time + interval;
    }

    public void StopDialogues()
    {
        isActive = false;
        StopAllCoroutines();
        
        // Hide any active dialogue
        if (dialogueBox != null)
            dialogueBox.enabled = false;
        if (dialogueText != null)
            dialogueText.enabled = false;
        
        SetEmotion("default");
    }

    public void ResumeDialogues()
    {
        isActive = true;
        ScheduleNextDialogue();
    }
}