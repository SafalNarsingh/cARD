using UnityEngine;

public class music : MonoBehaviour
{
    public GameObject musicSymbolOn;    // "Music On" symbol, always active
    public GameObject musicSymbolOff;   // "Music Off" symbol, toggles visibility
    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }

        // Start with music playing and "Off" symbol disabled
        audioSource.loop = true;
        audioSource.Play();
        musicSymbolOn.SetActive(true); // Always on
        musicSymbolOff.SetActive(false); // Off symbol hidden at start
    }

    public void onClick()
    {
        // Check if music is currently playing
        if (audioSource.isPlaying)
        {
            // Mute music
            audioSource.Pause();
            musicSymbolOff.SetActive(true);   // Show "Off" symbol
        }
        else
        {
            // Unmute music
            audioSource.Play();
            musicSymbolOff.SetActive(false);  // Hide "Off" symbol
        }
    }
}