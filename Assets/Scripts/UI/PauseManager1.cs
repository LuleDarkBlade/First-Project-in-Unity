using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager1 : MonoBehaviour
{
    [Tooltip("The UI panel that shows the pause menu.")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Update()
    {
        // Toggle pause if the Escape key is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;          // Resume game time
        isPaused = false;
        Debug.Log("Game Resumed");
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show the pause menu
        Time.timeScale = 0f;          // Freeze game time
        isPaused = true;
        Debug.Log("Game Paused");
    }

    // Optional: Methods for other buttons
    public void LoadMainMenu()
    {
        // Resume time before changing scenes
        Time.timeScale = 1f;
        // Load the Rise of the Maestro Scene 4 scene (make sure the scene is added to Build Settings)
        SceneManager.LoadScene("Rise of the Maestro Scene 4");
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
    }
}
