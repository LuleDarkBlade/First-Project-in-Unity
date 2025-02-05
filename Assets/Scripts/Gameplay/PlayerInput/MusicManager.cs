using UnityEngine;

public class MusicManager : MonoBehaviour
{
    
    // Singleton instance
    public static MusicManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Implement singleton pattern: if an instance already exists, destroy this one
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make sure this GameObject persists between scene loads
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Start playing the music if it's not already playing
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
