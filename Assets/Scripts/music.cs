using UnityEngine;

public class music : MonoBehaviour
{
    public GameObject musicSymbol; // Reference to the music symbol GameObject
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        if (musicSymbol.activeSelf)
        {
            musicSymbol.SetActive(false);
        }
        else
        {
            musicSymbol.SetActive(true);
        }
    }
}
