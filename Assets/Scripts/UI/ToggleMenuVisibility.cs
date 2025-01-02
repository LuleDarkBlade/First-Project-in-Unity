using UnityEngine;

public class ToggleMenuVisibility : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject menuActive; // The active menu
    [SerializeField] private GameObject menuInactive; // The inactive menu

    public void ToggleMenu()
    {
        if (menuActive != null && menuInactive != null)
        {
            // Swap visibility states
            bool isMenuActiveVisible = menuActive.activeSelf;

            menuActive.SetActive(!isMenuActiveVisible); // Toggle Menu Active visibility
            menuInactive.SetActive(isMenuActiveVisible); // Toggle Menu Inactive visibility
        }
        else
        {
            Debug.LogWarning("Menu objects are not assigned in the inspector!");
        }
    }
}
