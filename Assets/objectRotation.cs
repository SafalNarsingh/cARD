using UnityEngine;
using UnityEngine.InputSystem;

public class ARObjectRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 0.13f;
    public bool enableXRotation = false;
    public bool enableYRotation = true;
    public bool enableZRotation = false;

    [Header("Debug Settings")]
    public bool enableDebugLogs = false;

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector2 lastInputPosition;
    private bool isObjectHit = false;

    void Start()
    {
        // Setup camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        if (enableDebugLogs)
        {
            Debug.Log("=== AR OBJECT ROTATION STARTED ===");
            Debug.Log("Target Object: " + gameObject.name);
        }
    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput(); // For editor testing
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        Vector2 touchPos = touch.position.ReadValue();

        // Touch started
        if (touch.press.wasPressedThisFrame)
        {
            if (IsTouchingObject(touchPos))
            {
                isDragging = true;
                lastInputPosition = touchPos;
                isObjectHit = true;

                if (enableDebugLogs)
                    Debug.Log("✓ Starting drag rotation");
            }
        }
        // Touch held and dragging
        else if (touch.press.isPressed && isDragging && isObjectHit)
        {
            Vector2 deltaPosition = touchPos - lastInputPosition;
            RotateObject(deltaPosition);
            lastInputPosition = touchPos;
        }
        // Touch ended
        else if (touch.press.wasReleasedThisFrame)
        {
            if (isDragging && enableDebugLogs)
                Debug.Log("Stopping drag rotation");

            isDragging = false;
            isObjectHit = false;
        }
    }

    void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Mouse button pressed
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsTouchingObject(mousePos))
            {
                isDragging = true;
                lastInputPosition = mousePos;
                isObjectHit = true;

                if (enableDebugLogs)
                    Debug.Log("✓ Starting mouse drag rotation");
            }
        }
        // Mouse held and dragging
        else if (Mouse.current.leftButton.isPressed && isDragging && isObjectHit)
        {
            Vector2 deltaPosition = mousePos - lastInputPosition;
            RotateObject(deltaPosition);
            lastInputPosition = mousePos;
        }
        // Mouse button released
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDragging && enableDebugLogs)
                Debug.Log("Stopping mouse drag rotation");

            isDragging = false;
            isObjectHit = false;
        }
    }

    bool IsTouchingObject(Vector2 screenPos)
    {
        if (mainCamera == null)
        {
            if (enableDebugLogs)
                Debug.LogError("Camera is NULL! Cannot perform raycast.");
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if hit object is this gameObject or any of its children
            bool hitTarget = hit.collider.gameObject == gameObject ||
                           hit.collider.transform.IsChildOf(transform) ||
                           hit.collider.gameObject.name == gameObject.name;

            if (enableDebugLogs && hitTarget)
                Debug.Log($"🎯 Hit AR object for rotation!");

            return hitTarget;
        }

        return false;
    }

    void RotateObject(Vector2 deltaPosition)
    {
        if (deltaPosition.magnitude < 0.1f) return; // Avoid micro-movements

        // Convert screen movement to rotation
        float rotationX = 0f;
        float rotationY = 0f;
        float rotationZ = 0f;

        if (enableYRotation)
        {
            // Horizontal drag rotates around Y-axis (left/right rotation)
            rotationY = deltaPosition.x * rotationSpeed;
        }

        if (enableXRotation)
        {
            // Vertical drag rotates around X-axis (up/down rotation)
            rotationX = -deltaPosition.y * rotationSpeed;
        }

        if (enableZRotation)
        {
            // Optional: Diagonal movement for Z-axis rotation
            rotationZ = (deltaPosition.x + deltaPosition.y) * 0.5f * rotationSpeed;
        }

        // Apply rotation relative to current rotation
        transform.Rotate(rotationX, rotationY, rotationZ, Space.World);

        if (enableDebugLogs)
            Debug.Log($"Rotating - X: {rotationX:F2}, Y: {rotationY:F2}, Z: {rotationZ:F2}");
    }

    // Public methods for external control
    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        if (enableDebugLogs)
            Debug.Log("🔄 Object rotation reset");
    }

    public void SetRotation(Vector3 eulerAngles)
    {
        transform.rotation = Quaternion.Euler(eulerAngles);
        if (enableDebugLogs)
            Debug.Log($"🔄 Object rotation set to: {eulerAngles}");
    }
}