using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemCleaner : MonoBehaviour
{
    void Awake()
    {
        // Use the recommended FindObjectsByType with no sorting for efficiency
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (systems.Length > 1)
        {
            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] != this.GetComponent<EventSystem>())
                {
                    Destroy(systems[i].gameObject);
                }
            }
        }
    }
}
