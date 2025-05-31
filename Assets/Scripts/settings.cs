using UnityEngine;

public class settingsClick : MonoBehaviour
{

    public GameObject settings;// Assign this in the inspector to the help options GameObject
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
        }
        else
        {
            settings.SetActive(true); // Show the help options when this GameObject is clicked
        }
    }

    public void OnCloseClick()
    {
        settings.SetActive(false); // Hide the help options when the close button is clicked
    }
}
