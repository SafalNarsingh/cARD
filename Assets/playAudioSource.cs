using UnityEngine;

public class ARTouchInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip soundClip;

    [Header("Touch Settings")]
    public LayerMask touchLayer = -1;

    void Start()
    {
        // Get AudioSource component if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Add AudioSource if it doesn't exist
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        // Handle touch input (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                CheckTouchHit(touch.position);
            }
        }

        // Handle mouse input (for testing in editor)
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouchHit(Input.mousePosition);
        }
    }

    void CheckTouchHit(Vector2 screenPosition)
    {
        // Create a ray from camera through the touch point
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // Check if ray hits this object
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
        // Add any additional touch behavior here
    }
}