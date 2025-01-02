using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button nameButton;
    [SerializeField] private Button surnameButton;
    [SerializeField] private Button nationalityButton;
    [SerializeField] private Button mainHandButton;
    [SerializeField] private Button backhandButton;
    [SerializeField] private Button genderButton;

    [Header("Description Objects")]
    [SerializeField] private GameObject nameDescription;
    [SerializeField] private GameObject surnameDescription;
    [SerializeField] private GameObject nationalityDescription;
    [SerializeField] private GameObject mainHandDescription;
    [SerializeField] private GameObject backhandDescription;
    [SerializeField] private GameObject genderDescription;

    [Header("Buttons to Hide")]
    [SerializeField] private GameObject[] buttonsToHide; // Array for all buttons to hide

    private bool nameLocked = false;
    private bool surnameLocked = false;
    private bool nationalityLocked = false;
    private bool mainHandLocked = false;
    private bool backhandLocked = false;
    private bool genderLocked = false;

    private void Start()
    {
        // Hook up button clicks
        nameButton.onClick.AddListener(() => OnButtonClicked("name"));
        surnameButton.onClick.AddListener(() => OnButtonClicked("surname"));
        nationalityButton.onClick.AddListener(() => OnButtonClicked("nationality"));
        mainHandButton.onClick.AddListener(() => OnButtonClicked("mainHand"));
        backhandButton.onClick.AddListener(() => OnButtonClicked("backhand"));
        genderButton.onClick.AddListener(() => OnButtonClicked("gender"));

        // Hide all descriptions initially
        ClearDescriptions();
    }

    private void OnButtonClicked(string type)
    {
        Debug.Log($"Button clicked: {type}"); // Debugging which button was clicked

        // Handle specific button clicks and lock the corresponding value
        switch (type)
        {
            case "name":
                if (!nameLocked)
                {
                    nameDescription.SetActive(true); // Show Name Description
                    nameLocked = true;
                }
                break;

            case "surname":
                if (!surnameLocked)
                {
                    surnameDescription.SetActive(true); // Show Surname Description
                    surnameLocked = true;
                }
                break;

            case "nationality":
                if (!nationalityLocked)
                {
                    nationalityDescription.SetActive(true); // Show Nationality Description
                    nationalityLocked = true;
                }
                break;

            case "mainHand":
                if (!mainHandLocked)
                {
                    mainHandDescription.SetActive(true); // Show Main Hand Description
                    mainHandLocked = true;
                }
                break;

            case "backhand":
                if (!backhandLocked)
                {
                    backhandDescription.SetActive(true); // Show Backhand Description
                    backhandLocked = true;
                }
                break;

            case "gender":
                if (!genderLocked)
                {
                    genderDescription.SetActive(true); // Show Gender Description
                    genderLocked = true;
                }
                break;
        }

        // Reveal all descriptions once any button is clicked
        RevealAllDescriptions();

        // Hide all buttons
        HideAllButtons();
    }

    private void ClearDescriptions()
    {
        // Initially hide all description objects
        nameDescription.SetActive(false);
        surnameDescription.SetActive(false);
        nationalityDescription.SetActive(false);
        mainHandDescription.SetActive(false);
        backhandDescription.SetActive(false);
        genderDescription.SetActive(false);
    }

    private void RevealAllDescriptions()
    {
        // Make all descriptions visible (if not already shown)
        nameDescription.SetActive(true);
        surnameDescription.SetActive(true);
        nationalityDescription.SetActive(true);
        mainHandDescription.SetActive(true);
        backhandDescription.SetActive(true);
        genderDescription.SetActive(true);
    }

    private void HideAllButtons()
    {
        Debug.Log("Hiding all buttons"); // Debugging when buttons should be hidden

        // Loop through the array and deactivate each button GameObject
        foreach (var button in buttonsToHide)
        {
            if (button != null)
            {
                button.SetActive(false); // Hide the button
                Debug.Log($"Button hidden: {button.name}"); // Debugging which button was hidden
            }
            else
            {
                Debug.LogWarning("Button in buttonsToHide array is null!"); // Warn if any button is missing
            }
        }
    }
}
