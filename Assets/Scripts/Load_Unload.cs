using UnityEngine;
using Vuforia;

public class Load_Unload : MonoBehaviour
{
    public GameObject modelPrefab; // Assign your car or 3D object prefab here
    private GameObject spawnedModel;

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            LoadModel();
        }
        else
        {
            UnloadModel();
        }
    }

    private void LoadModel()
    {
        if (spawnedModel == null)
        {
            spawnedModel = Instantiate(modelPrefab, transform);
            spawnedModel.transform.localPosition = Vector3.zero;
        }
    }

    private void UnloadModel()
    {
        if (spawnedModel != null)
        {
            Destroy(spawnedModel);
        }
    }
}
