using UnityEngine;

public class Music : MonoBehaviour
{
    private static Music Instance;

    public GameObject musicObject;       // GameObject with AudioSource
    public GameObject musicSymbolOn;     // "Music On" symbol (UI)
    public GameObject musicSymbolOff;    // "Music Off" symbol (UI)

    private AudioSource audioSource;

    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject); // Prevent duplicates
        //    return;
        //}

        //Instance = this;
        //DontDestroyOnLoad(gameObject); // Persist entire GameObject (with symbols)

        // Setup AudioSource
        if (musicObject == null)
            musicObject = this.gameObject;

        audioSource = musicObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = musicObject.AddComponent<AudioSource>();

        // Start music if clip assigned
        if (audioSource.clip != null)
        {
            audioSource.loop = true;
            //audioSource.Play();
        }
    }

    private void Start()
    {
        UpdateSymbolState();
    }

    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
        UpdateSymbolState();

    }

    void UpdateSymbolState()
    {
        bool isPlaying = audioSource.isPlaying;

        if (musicSymbolOn != null)
            musicSymbolOn.SetActive(isPlaying);

        if (musicSymbolOff != null)
            musicSymbolOff.SetActive(!isPlaying);
    }
}
