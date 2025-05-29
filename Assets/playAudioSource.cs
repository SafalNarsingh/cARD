using UnityEngine;
using UnityEngine.InputSystem; // for the new input system

public class ObjectClickSound : MonoBehaviour
{
    public AudioClip audioClip; // Assign your honk sound
    private AudioSource audioSource;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Get or add AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
    }

    void Update()
    {
        // Touchscreen input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleClick(touchPos);
        }

        // Mouse input (editor/testing)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            HandleClick(mousePos);
        }
    }

    void HandleClick(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                PlayHonkSound();
            }
        }
    }

    void PlayHonkSound()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
