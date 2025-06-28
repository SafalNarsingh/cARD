using UnityEngine;
using Vuforia;

public class DisableCameraUI : MonoBehaviour
{
    void Start()
    {
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = false;
        }
    }
}