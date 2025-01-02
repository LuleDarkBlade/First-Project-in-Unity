using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToggleButtons : MonoBehaviour, IPointerEnterHandler
{
    [Header("References to All Four Objects")]
    [SerializeField] private GameObject singleplayerNoColor;
    [SerializeField] private GameObject singleplayerWithColor;
    [SerializeField] private GameObject multiplayerNoColor;
    [SerializeField] private GameObject multiplayerWithColor;

    // Called automatically when pointer hovers over the GameObject with this script
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check which "NoColor" object we are hovering:
        if (gameObject == singleplayerNoColor)
        {
            // 1) Show the Singleplayer "WithColor"
            singleplayerWithColor.SetActive(true);
            singleplayerNoColor.SetActive(false);

            // 2) Hide Multiplayer "WithColor"
            multiplayerWithColor.SetActive(false);
            multiplayerNoColor.SetActive(true);
        }
        else if (gameObject == multiplayerNoColor)
        {
            // 1) Show the Multiplayer "WithColor"
            multiplayerWithColor.SetActive(true);
            multiplayerNoColor.SetActive(false);

            // 2) Hide Singleplayer "WithColor"
            singleplayerWithColor.SetActive(false);
            singleplayerNoColor.SetActive(true);
        }
    }
}
