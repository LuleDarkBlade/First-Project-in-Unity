using UnityEngine;
using UnityEngine.SceneManagement;       // For scene loading
using UnityEngine.InputSystem;           // For the new Input System

public class SceneSwitcherOnKeyPress : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Rise of the Maestro Scene 2";
    

    private void Update()
    {
        // Ensure we're using the new Input System & detect any key pressed this frame
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // Load the specified scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
