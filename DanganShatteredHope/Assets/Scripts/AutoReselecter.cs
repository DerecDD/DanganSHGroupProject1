using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoReselecter : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private List<GameObject> buttons; // Add your buttons here in the inspector or via code
    private GameObject lastSelectedObject;
    private int currentIndex = 0;

    void Awake()
    {
        if (eventSystem == null)
            eventSystem = gameObject.GetComponent<EventSystem>();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(lastSelectedObject); // No current selection, go back to last selected
        }
        else
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject; // Keep setting current selected object
        }

        HandleScrollInput();
        HandleLeftClick(); // Check for left click input
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // Scrolling up
        {
            currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
            eventSystem.SetSelectedGameObject(buttons[currentIndex]);
        }
        else if (scroll < 0f) // Scrolling down
        {
            currentIndex = (currentIndex + 1) % buttons.Count;
            eventSystem.SetSelectedGameObject(buttons[currentIndex]);
        }
    }

    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button is pressed
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                // Simulate a button press on the currently selected button
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject,
                                      new PointerEventData(eventSystem),
                                      ExecuteEvents.submitHandler);
            }
        }
    }
}
