using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;       // For IPointerClickHandler
using UnityEngine.InputSystem;        // For the new Input System (Keyboard)

public class ReturnToScene1 : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string previousSceneName = "Rise of the Maestro Scene 1";

    private void Update()
    {
        // 1) Check if we have a keyboard and if Backspace was pressed this frame
        if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            GoToPreviousScene();
        }
    }

    // 2) Detect clicks on this GameObject (UI or 3D object)
    public void OnPointerClick(PointerEventData eventData)
    {
        GoToPreviousScene();
    }

    // Loads the specified scene
    private void GoToPreviousScene()
    {
        SceneManager.LoadScene(previousSceneName);
    }
}
