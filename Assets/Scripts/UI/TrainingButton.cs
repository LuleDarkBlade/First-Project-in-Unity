using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class TrainingButton : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Rise of the Maestro Scene 5";

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnContinueClicked);
    }

    private void OnContinueClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
