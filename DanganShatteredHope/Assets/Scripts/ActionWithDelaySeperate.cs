using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomActionElementSeperate
{
    public string actionName;
    public UnityEngine.Events.UnityEvent action;
}

public class ActionWithDelaySeperate : MonoBehaviour
{
    public float delayBeforeActions = 1f;
    public List<CustomActionElementSeperate> actionElements = new List<CustomActionElementSeperate>();

    public void ActivateActionWithDelay(string actionName)
    {
        Debug.Log($"Scheduling '{actionName}' for execution in {delayBeforeActions} seconds.");
        Invoke(nameof(InvokeAction), delayBeforeActions);
    }

    private void InvokeAction()
    {
        Debug.Log("Delay completed. Executing action.");

        foreach (var element in actionElements)
        {
            element.action.Invoke();
            Debug.Log($"Action '{element.actionName}' invoked.");
        }
    }

}
