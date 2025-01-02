using UnityEngine;

public class ShowMenuInactive : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject menuActive; // The active menu
    [SerializeField] private GameObject menuInactive; // The inactive menu

    public void ShowInactiveMenu()
    {
        if (menuActive != null && menuInactive != null)
        {
            menuActive.SetActive(false); // Hide Menu Active
            menuInactive.SetActive(true); // Show Menu Inactive
        }
        else
        {
            Debug.LogWarning("Menu objects are not assigned in the inspector!");
        }
    }
}
