using UnityEngine;

public class ARObjectInteraction : MonoBehaviour
{
    private float minScale;
    private float maxScale;
    private float rotationSpeed;

    private Vector3 initialScale;
    private Quaternion initialRotation;

    public void Initialize(float min, float max, float rotSpeed)
    {
        minScale = min;
        maxScale = max;
        rotationSpeed = rotSpeed;

        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    public void ResetTransform()
    {
        transform.localScale = initialScale;
        transform.rotation = initialRotation;
    }

    // Optional: Add visual feedback when selected
    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
    }

    public void Highlight(bool enable)
    {
        if (renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                if (enable)
                {
                    renderers[i].material.color = originalColors[i] * 1.3f;
                }
                else
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
        }
    }
}