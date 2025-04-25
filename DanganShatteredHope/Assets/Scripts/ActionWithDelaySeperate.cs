using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomActionElementSeperate
{
    public string actionName; // Name of the individual action
    public UnityEngine.Events.UnityEvent action; // The action itself
}

public class ActionWithDelaySeperate : MonoBehaviour
{
    public float delayBeforeActions = 0f; // Delay before executing actions
    public List<CustomActionElementSeperate> actionElements = new List<CustomActionElementSeperate>(); // List of actions

    public void ActivateActionWithDelay(string actionName)
    {
        StartCoroutine(ExecuteActionWithDelay(actionName));
    }

    private IEnumerator ExecuteActionWithDelay(string actionName)
    {
        yield return new WaitForSeconds(delayBeforeActions);

        foreach (var element in actionElements)
        {
            if (element.actionName == actionName)
            {
                element.action.Invoke(); // Execute the specific action
                break;
            }
        }
    }

    // New method to toggle an action by name
    public void ToggleActionByName(string actionName)
    {
        foreach (var element in actionElements)
        {
            if (element.actionName == actionName)
            {
                element.action.Invoke(); // Invoke the action directly
                break;
            }
        }
    }
}
