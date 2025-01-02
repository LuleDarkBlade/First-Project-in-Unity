using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToggle : MonoBehaviour, IPointerEnterHandler
{
    [Header("Active and Inactive Objects")]
    [SerializeField] private GameObject activeObject;
    [SerializeField] private GameObject inactiveObject;

    [Header("Selection Group")]
    [SerializeField] private string group; // e.g., "Hand" or "Backhand"

    // Hand group references (static so they persist across all HoverToggle instances)
    private static GameObject currentActiveHand;
    private static GameObject currentInactiveHand;

    // Backhand group references
    private static GameObject currentActiveBackhand;
    private static GameObject currentInactiveBackhand;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Decide which group this button belongs to
        if (group == "Hand")
        {
            UpdateGroup(ref currentActiveHand, ref currentInactiveHand);
        }
        else if (group == "Backhand")
        {
            UpdateGroup(ref currentActiveBackhand, ref currentInactiveBackhand);
        }
    }

    private void UpdateGroup(ref GameObject currActive, ref GameObject currInactive)
    {
        // 1) Hide the previously active object in this group
        if (currActive != null)
            currActive.SetActive(false);

        // 2) Show the previously inactive object in this group
        if (currInactive != null)
            currInactive.SetActive(true);

        // 3) Update references to the newly hovered button’s objects
        currActive = activeObject;
        currInactive = inactiveObject;

        // 4) Show the newly active object
        if (activeObject != null)
            activeObject.SetActive(true);

        // 5) Hide the newly inactive object
        if (inactiveObject != null)
            inactiveObject.SetActive(false);
    }
}
