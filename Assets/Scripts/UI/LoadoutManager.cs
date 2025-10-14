using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Weapon
{
    public string weaponName;
    public GameObject weaponModel; // 3D model prefab
    public Sprite weaponIcon; // 2D sprite for UI (optional)
    public float damage;
    public float attackSpeed;
    [TextArea(3, 5)]
    public string description;
}

public class LoadoutManager : MonoBehaviour
{
    [Header("Weapon Database")]
    public Weapon[] weapons;
    private int currentWeaponIndex = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;
    public TextMeshProUGUI weaponDescriptionText;
    public Image weaponIconImage; // Optional
    public Button nextButton;
    public Button previousButton;
    public Button startGameButton;
    public Button backButton;

    [Header("3D Preview")]
    public Transform weaponPreviewParent; // Where to spawn the 3D model
    public Vector3 previewRotationSpeed = new Vector3(0, 50, 0);
    public bool autoRotatePreview = true;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip scrollSound;
    public AudioClip selectSound;

    private GameObject currentWeaponPreview;

    void Start()
    {
        // Hook up buttons
        if (nextButton != null) nextButton.onClick.AddListener(NextWeapon);
        if (previousButton != null) previousButton.onClick.AddListener(PreviousWeapon);
        if (startGameButton != null) startGameButton.onClick.AddListener(StartGame);
        if (backButton != null) backButton.onClick.AddListener(BackToMenu);

        // Display first weapon
        if (weapons.Length > 0)
        {
            UpdateWeaponDisplay();
        }
        else
        {
            Debug.LogWarning("No weapons in database!");
        }
    }

    void Update()
    {
        // Auto-rotate weapon preview
        if (autoRotatePreview && currentWeaponPreview != null)
        {
            currentWeaponPreview.transform.Rotate(previewRotationSpeed * Time.deltaTime);
        }
    }

    public void NextWeapon()
    {
        PlaySound(scrollSound);
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0; // Loop back
        }
        UpdateWeaponDisplay();
    }

    public void PreviousWeapon()
    {
        PlaySound(scrollSound);
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Length - 1; // Loop back
        }
        UpdateWeaponDisplay();
    }

    void UpdateWeaponDisplay()
    {
        if (weapons.Length == 0) return;

        Weapon currentWeapon = weapons[currentWeaponIndex];

        // Update UI text
        if (weaponNameText != null)
            weaponNameText.text = currentWeapon.weaponName;

        if (weaponStatsText != null)
            weaponStatsText.text = $"Damage: {currentWeapon.damage}\nAttack Speed: {currentWeapon.attackSpeed}";

        if (weaponDescriptionText != null)
            weaponDescriptionText.text = currentWeapon.description;

        if (weaponIconImage != null && currentWeapon.weaponIcon != null)
            weaponIconImage.sprite = currentWeapon.weaponIcon;

        // Update 3D preview
        Update3DPreview(currentWeapon);
    }

    void Update3DPreview(Weapon weapon)
    {
        // Destroy old preview
        if (currentWeaponPreview != null)
        {
            Destroy(currentWeaponPreview);
        }

        // Spawn new preview
        if (weapon.weaponModel != null && weaponPreviewParent != null)
        {
            currentWeaponPreview = Instantiate(weapon.weaponModel, weaponPreviewParent);
            currentWeaponPreview.transform.localPosition = Vector3.zero;
            currentWeaponPreview.transform.localRotation = Quaternion.identity;
        }
    }

    public void StartGame()
    {
        PlaySound(selectSound);
        
        // Save selected weapon index for Game scene to use
        PlayerPrefs.SetInt("SelectedWeaponIndex", currentWeaponIndex);
        PlayerPrefs.SetString("SelectedWeaponName", weapons[currentWeaponIndex].weaponName);
        PlayerPrefs.Save();

        Debug.Log($"Starting game with weapon: {weapons[currentWeaponIndex].weaponName}");
        
        // Load cabin transition
        SceneManager.LoadScene("CabinTransition");
    }

    public void BackToMenu()
    {
        PlaySound(selectSound);
        SceneManager.LoadScene("MainMenu");
    }

    void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    void OnDestroy()
    {
        // Clean up listeners
        if (nextButton != null) nextButton.onClick.RemoveListener(NextWeapon);
        if (previousButton != null) previousButton.onClick.RemoveListener(PreviousWeapon);
        if (startGameButton != null) startGameButton.onClick.RemoveListener(StartGame);
        if (backButton != null) backButton.onClick.RemoveListener(BackToMenu);
    }
}


/* handcrafted documentation: 
 * 
 *    1. Loadout:
 *      0. Start:
 *      5. Back to MainMenu. -^
 *    
 *    0 -> into CabinTransition Animation scene....
 *    1 -> into Loadout scene -> into CabinTransition scene;  return back to MainMenu is possible.
 *    
 *    
 *    subject to changes as the project goes....
 
 *    a database with a Cursor Iterator smth, smth <>  
 *    I totally forgot how to do it. It's been 5 years since I did a real project of this size and importance.
 *    I'm rusty and my skills need polishing.
 *    I know it's doable because of the Gestures Project on my GitHub profile. Which has Exactly This Functionality!!!
 *    Java and C# are interchangeable. I code well in Java. I teach well in Java. I know OOP. I'm basically a real coder.
 *    in the age of Vibe coding...

 *    The Cursor will have the weapons:
 *    -Katana (1st 3d model weapon created)
 *    -Fork (original weapon meant to be used in Concept Art and Design of the Game stage...)
 *    -Knife (why not..
 *    -any other additional weapons included for variety of gameplay...

 *    It will Iterate through that List, HashMap, Array, whatever you wish to call it.... 
 *    a dynamic loading feature for the Loadout.
 *    Before going into Gameplay.
 *    
 *    Start: goes into CabinTransition Animation scene....
 *    the game begins...
 *    



 *    vers. 1 artifact of vibecoded script C#
 *    
 *    many, many, many more edits to be included.
 *    