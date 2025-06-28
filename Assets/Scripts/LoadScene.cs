using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Header("Scene Names")]
    public string uiSceneName;
    public string arSceneName;

    public void LoadARScene()
    {
        // Load AR Scene Additively
        SceneManager.LoadScene(arSceneName, LoadSceneMode.Additive);

        // Unload UI Scene after AR Scene is loaded
        StartCoroutine(UnloadSceneAfterDelay(uiSceneName, 0.1f));
    }

    public void LoadUIScene()
    {
        // Load UI Scene Additively
        SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);

        // Unload AR Scene after UI Scene is loaded
        StartCoroutine(UnloadSceneAfterDelay(arSceneName, 0.1f));
    }

    private System.Collections.IEnumerator UnloadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
