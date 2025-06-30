using UnityEngine;
using UnityEngine.SceneManagement; // for scene management

public class cardscanner : MonoBehaviour
{
    public string sceneToLoad; // assign the scene name in the inspector
    public AudioSource audioSource;
    public void LoadScene()
    {
        Debug.Log("Loading scene: " + sceneToLoad);
        audioSource.Play(); // Play the audio before loading the scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
