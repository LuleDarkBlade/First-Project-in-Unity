using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSceneOrQuit : MonoBehaviour
{
    [Header("Target Scene Name")]
    [SerializeField] private string targetSceneName = "Rise of the Maestro Scene 3";

    private void Update()
    {
        // Check for Backspace key press to go back to the target scene
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadTargetScene();
        }

        // Check for Alt + F4 to quit the application (handled automatically by OS in most cases)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void OnQuitButtonClicked()
    {
        // Called when the Quit button is clicked
        QuitGame();
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("Target scene name is not set!");
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // Note: Application.Quit() doesn't work in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the Unity Editor
#endif
    }
}
