using UnityEngine;
using UnityEngine.SceneManagement; // for scene management


public class ThreeDPlacer : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("ThreeDPlacerScene", LoadSceneMode.Single);
    }
}
