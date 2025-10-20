using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button ImageStartButton;
    public Button ImageLoadoutButton;
    public Button ImageAboutButton;
    public Button ImageExitButton;

    [Header("Audio")]
    public AudioSource menuMusic;
    public AudioSource sfxSource;
    public AudioClip buttonClickSound;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    void Start()
    {
        // Setup music
        if (menuMusic != null)
        {
            menuMusic.loop = true;
            menuMusic.volume = musicVolume;
            if (!menuMusic.isPlaying)
            {
                menuMusic.Play();
            }
        }

        // Setup SFX source
        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
        }

        // Hook up button clicks
        ImageStartButton.onClick.AddListener(OnPlayClicked);
        ImageLoadoutButton.onClick.AddListener(OnSettingsClicked);
        ImageAboutButton.onClick.AddListener(OnCreditsClicked);
        ImageExitButton.onClick.AddListener(OnQuitClicked);
    }

    void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    void OnPlayClicked()
    {
        PlayButtonSound();
        Debug.Log("Play button clicked");
        // Load CabinTransition scene which will then load Game scene
        SceneManager.LoadScene("CabinTransition");

        // The animation into Cabin Transition.... AH.... I wanna go back to Genshin Impact!!!
        // it's a proper game where you flush all of your $$$ down the toilet!!!
        // waifus are optional. 

        /* Start the game! */
    }

    void OnSettingsClicked()
    {
        PlayButtonSound();
        Debug.Log("Loadout button clicked");
        // Load settings scene or open settings panel
        // SceneManager.LoadScene("SettingsScene");

        // instead of Settings, there will be a Loadout with the weapons.
        // not much Settings to have, given that it's a 1st project.
        // what is there to tweak? Graphics fidelity? It either runs on your crappy device or it does not run!
        // I will assure you that this crappy game will run on your crummy device. Because my device is even worse than yours. 
        // If it runs on MY DEVICE, it will run on your modern shiny new expensive Smartphone.
        // 3 weapons, maybe even more for variety of gameplay, ... so the player doesn't get bored.
        // Katana, Fork (main device), Knife, etc... to be determined.


        /* Settings (vibe coded) -> Loadout (my original structure) */

        SceneManager.LoadScene("Loadout");
        // go into Loadout.
    }

    void OnCreditsClicked()
    {
        PlayButtonSound();
        Debug.Log("About button clicked");
        // Load about scene or open credits panel
        // SceneManager.LoadScene("CreditsScene");

        // about won't have much because I am a solo dev startup supported by a wannabe 3D artist who's not officially on payroll yet...
        // advertise Redbeard Pirate Studio and go about with business....
        // some kind of socials need to be included.

        /* Credits (vibe coded) -> About (my original structure) */
    }

    void OnQuitClicked()
    {
        PlayButtonSound();
        Debug.Log("Quit button clicked");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif


        /* This one goes without explanation.
        Terminate the Game. Exit. */
    }

    void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks
        if (ImageStartButton != null) ImageStartButton.onClick.RemoveListener(OnPlayClicked);
        if (ImageLoadoutButton != null) ImageLoadoutButton.onClick.RemoveListener(OnSettingsClicked);
        if (ImageAboutButton != null) ImageAboutButton.onClick.RemoveListener(OnCreditsClicked);
        if (ImageExitButton != null) ImageExitButton.onClick.RemoveListener(OnQuitClicked);
    }
}


/* handcrafted documentation: 
 * 
 *    0. Start
 *    1. Loadout
 *    2. About
 *    3. Exit
 *    
 *    0 -> into CabinTransition Animation scene....
 *    1 -> into Loadout scene -> into CabinTransition scene;  return back to MainMenu is possible.
 *    2 -> into About scene;  return back to MainMenu is possible.
 *    3 -> terminate the game. Back to OS.
 *    
 *    
 *    subject to changes as the project goes....
 *    first structure - (backbone), then working software - (something realistic and achieveable to ship), lastly, art (this project is a work of art... despite what is said otherwise.) !

 *    It's a work of art in my heart.
 *    

 *    vers. 7 artifact of vibecoded script C#
 *    
 *    many, many, many more edits to be included.
 *    
 *   
 *    
 *    */