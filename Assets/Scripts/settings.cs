using UnityEngine;

public class settingsClick : MonoBehaviour
{

    public GameObject settings;// Assign this in the inspector to the help options GameObject
    public AudioSource AudioOpen; // Assign this in the inspector to the audio source for the open sound
    public AudioSource AudioClose; // Assign this in the inspector to the audio source for the close sound
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (settings.activeSelf)
        {
            settings.SetActive(false);
            AudioClose.Play(); // Play the audio when the settings are closed

        }
        else
        {
            settings.SetActive(true); // Show the settings options when this GameObject is clicked
            AudioOpen.Play(); // Play the audio when the settings are opened
        }
    }

    public void OnCloseClick()
    {
        settings.SetActive(false); // Hide the help options when the close button is clicked
        AudioClose.Play(); // Play the audio when the settings are closed
    }
}
