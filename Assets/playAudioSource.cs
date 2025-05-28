using UnityEngine;
using UnityEngine.InputSystem; // Import new input system namespace

public class ARTouchInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip soundClip;

    [Header("Touch Settings")]
    public LayerMask touchLayer = -1;

    void Start()
    {
        // Get or add AudioSource component
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        HandleTouchOrClick();
    }

    void HandleTouchOrClick()
    {
        // Handle touchscreen input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            CheckTouchHit(touchPos);
        }

        // Handle mouse input for editor or desktop
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            CheckTouchHit(mousePos);
        }
    }

    void CheckTouchHit(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                PlaySound();
                OnObjectTouched();
            }
        }
    }

    void PlaySound()
    {
        if (audioSource != null && soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
    }

    // Override this method for additional touch behavior
    protected virtual void OnObjectTouched()
    {
        Debug.Log("AR Object was touched!");
    }
}
