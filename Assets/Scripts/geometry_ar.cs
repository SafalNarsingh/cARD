using UnityEngine;
using UnityEngine.InputSystem; // for the new input system

public class Geometry_ar : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetObject; // The AR object to interact with

    [Header("Debug Settings")]
    public bool enableDebugLogs = true; // Enable debug messages in console

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        // If no target is specified, use this gameObject
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        if (enableDebugLogs)
        {
            Debug.Log("=== INTERACTION SYSTEM STARTED ===");
            Debug.Log("Target Object: " + targetObject.name);
            Debug.Log("Camera: " + (mainCamera != null ? mainCamera.name : "NULL"));
        }
    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            // Touch started
            if (touch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = touch.position.ReadValue();

                if (enableDebugLogs)
                    Debug.Log($"TOUCH DETECTED at position: {touchPos}");

                if (IsTouchingObject(touchPos, "TOUCH"))
                {
                    if (enableDebugLogs)
                        Debug.Log("✓ TOUCH HIT TARGET OBJECT!");
                }
            }
        }
    }

    void HandleMouseInput()
    {
        if (Mouse.current != null)
        {
            // Mouse button pressed
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                if (enableDebugLogs)
                    Debug.Log($"MOUSE CLICK at position: {mousePos}");

                if (IsTouchingObject(mousePos, "MOUSE"))
                {
                    if (enableDebugLogs)
                        Debug.Log("✓ MOUSE HIT TARGET OBJECT!");
                }
            }
        }
    }

    bool IsTouchingObject(Vector2 screenPos, string inputType)
    {
        if (mainCamera == null)
        {
            if (enableDebugLogs)
                Debug.LogError("Camera is NULL! Cannot perform raycast.");
            return false;
        }

        if (targetObject == null)
        {
            if (enableDebugLogs)
                Debug.LogError("Target Object is NULL!");
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (enableDebugLogs)
            Debug.Log($"{inputType} - Casting ray from: {ray.origin} in direction: {ray.direction}");

        if (Physics.Raycast(ray, out hit))
        {
            if (enableDebugLogs)
                Debug.Log($"{inputType} - Hit object: '{hit.collider.gameObject.name}' at distance: {hit.distance:F2}");

            bool hitTarget = hit.collider.gameObject == targetObject;

            if (hitTarget)
            {
                if (enableDebugLogs)
                    Debug.Log($"🎯 {inputType} - SUCCESS! Hit the target object!");
                return true;
            }
            else
            {
                if (enableDebugLogs)
                    Debug.Log($"❌ {inputType} - Hit different object. Expected: '{targetObject.name}', Got: '{hit.collider.gameObject.name}'");
                return false;
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log($"{inputType} - No objects hit by raycast");
            return false;
        }
    }
}