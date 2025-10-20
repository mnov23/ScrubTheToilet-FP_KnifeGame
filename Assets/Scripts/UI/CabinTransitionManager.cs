using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/*
 // commented to prevent compiler errors.

public class CabinTransitionController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Transform cameraStartPosition;
    public Transform cameraEndPosition;
    public float cameraPanDuration = 3f;
    public AnimationCurve cameraPanCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Door Settings")]
    public Transform doorTransform;
    public Vector3 doorOpenRotation = new Vector3(0, -90, 0);
    public float doorOpenDuration = 2f;
    public float doorOpenDelay = 0.5f; // Delay after camera reaches cabin
    public AnimationCurve doorOpenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Scene Transition")]
    public float delayBeforeGameScene = 1f; // Wait after door opens

    private Vector3 doorStartRotation;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (doorTransform != null)
        {
            doorStartRotation = doorTransform.localEulerAngles;
        }

        // Position camera at start
        if (cameraStartPosition != null)
        {
            cameraTransform.position = cameraStartPosition.position;
            cameraTransform.rotation = cameraStartPosition.rotation;
        }

        StartCoroutine(PlayTransitionSequence());
    }

    IEnumerator PlayTransitionSequence()
    {
        // Pan camera to cabin
        yield return StartCoroutine(PanCamera());

        // Wait a bit before opening door
        yield return new WaitForSeconds(doorOpenDelay);

        // Open door
        yield return StartCoroutine(OpenDoor());

        // Wait before loading game scene
        yield return new WaitForSeconds(delayBeforeGameScene);

        // Load the Game scene
        SceneManager.LoadScene("Game");
    }

    IEnumerator PanCamera()
    {
        if (cameraStartPosition == null || cameraEndPosition == null)
        {
            Debug.LogWarning("Camera positions not set!");
            yield break;
        }

        float elapsed = 0f;
        Vector3 startPos = cameraStartPosition.position;
        Quaternion startRot = cameraStartPosition.rotation;
        Vector3 endPos = cameraEndPosition.position;
        Quaternion endRot = cameraEndPosition.rotation;

        while (elapsed < cameraPanDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cameraPanDuration;
            float curveValue = cameraPanCurve.Evaluate(t);

            cameraTransform.position = Vector3.Lerp(startPos, endPos, curveValue);
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, curveValue);

            yield return null;
        }

        // Ensure final position is exact
        cameraTransform.position = endPos;
        cameraTransform.rotation = endRot;
    }

    IEnumerator OpenDoor()
    {
        if (doorTransform == null)
        {
            Debug.LogWarning("Door transform not set!");
            yield break;
        }

        float elapsed = 0f;
        Vector3 startRot = doorStartRotation;
        Vector3 endRot = doorStartRotation + doorOpenRotation;

        while (elapsed < doorOpenDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / doorOpenDuration;
            float curveValue = doorOpenCurve.Evaluate(t);

            doorTransform.localEulerAngles = Vector3.Lerp(startRot, endRot, curveValue);

            yield return null;
        }

        // Ensure final rotation is exact
        doorTransform.localEulerAngles = endRot;
    }
}  */  // commented to prevent compiler errors.


/* handcrafted documentation: 
 * 
 *    0 or 1 -> into CabinTransition Animation scene....
 *    
 *    
 *    subject to changes as the project goes....

*       this is the Genshin Impact :trademark: feature login screen
*       which is a True work of Art
*       the moment my friend showed my what the Genshin Impact login screen looked like,
*       ...I instantly fell in love with the game and realized I'm there to stay for a good eternity.
*       I overstayed my welcome in Genshin Impact for ~2 years
*       many waifus were whaled on my burning Savings account
*       combined with a horrible personal life past...
*       and no hope for future...
*       
*       it was a personal Hell of my own making.
*       
*       I caught 'em all waifus.
*       then I gave away my account to the most toxic person in that community I was part of. For free.
*       and alleviated myself of that burden.
*       
*       needless to say, I am not returning back into Gayming.
*       But I will make Games for other people to enjoy.
*       I'm in it for the money. Sorry to disappoint you, but I am coming into Gamedev with the Triple "A" (AAA) philosophy.
*       you won't see any Indie Game Dev creativity here. Nor any revolutionary, meta changing games, nor any timeless classics immortalized into eternity.
*       
*       
*       Genshin Impact login style... A Camera is panned out to the Object.
*       The Camera pans in closer to the Object, a doorway opens into the magical La-la-land that will suck all of your spare time
*       and consume your Soul. If the game is genuinely good, it will be time that you will wish to WASTE by your free will.
*       If the game is bad, it won't take away too much of your time.
*       Regardless, Mobile Games are a time-sink to waste your time as you sit in a queue and do your adult chores like paying your bills or going to a doctor for a checkup.
*       
*       The ads won't feed my empty stomach. They are there to irritate you. And to let me have a passive income and a working Banking System to payroll my future (already presently recruited) 3D artist.
*       
 *    I am still open to employement opportunities in South Africa or overseas (GMT+2 timezone), even willing to work with a different timezone.
 *    I'm at best a Junior Software Developer, but my life matters too. (Flexible on position and terms; I have a wide range of talents.)
 *    Just like everybody else out there, I want to pay my bills.

 
 *    vers. 2 of vibe coded script.
 *    
 *    many, many, many more edits to be included.
 *    
 *    */