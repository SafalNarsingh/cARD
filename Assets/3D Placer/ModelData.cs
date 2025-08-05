using UnityEngine;

[System.Serializable]
public class ModelData
{
    public GameObject modelPrefab;
    public string englishName;
    public string nepaliName;

    public string GetLocalizedName(bool isNepali)
    {
        return isNepali ? nepaliName : englishName;
    }
}

[System.Serializable]
public class ModelSection
{
    public string sectionName;
    public ModelData[] models = new ModelData[5];
}