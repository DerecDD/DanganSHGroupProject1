using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuFunction : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetObject; // The GameObject to toggle
    public float disableDelay = 0f; // Delay before disabling the GameObject

    [Header("Pre-Enable Actions")]
    public List<ActionElement> preEnableActions = new List<ActionElement>(); // List of actions before enabling

    [Header("Pre-Disable Actions")]
    public List<ActionElement> preDisableActions = new List<ActionElement>(); // List of actions before disabling

    private bool isActive = false; // Tracks the current state (enabled/disabled)

    void Update()
    {
        // Check for the F1 key press
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleObject();
        }
    }

    private void ToggleObject()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target GameObject is missing or not assigned. Ignoring toggle request.");
            return;
        }

        if (!HasRequiredComponents(targetObject))
        {
            Debug.LogWarning($"Target GameObject '{targetObject.name}' is missing required components. Ignoring toggle request.");
            return;
        }

        if (!isActive)
        {
            ExecuteActions(preEnableActions); // Execute enabled pre-enable actions
            targetObject.SetActive(true);
            isActive = true;
        }
        else
        {
            StartCoroutine(DisableAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        ExecuteActions(preDisableActions); // Execute enabled pre-disable actions
        yield return new WaitForSeconds(disableDelay);

        if (targetObject != null && HasRequiredComponents(targetObject))
        {
            targetObject.SetActive(false);
            isActive = false;
        }
        else
        {
            Debug.LogWarning($"Target GameObject '{targetObject.name}' was destroyed or missing components before disabling.");
        }
    }

    private void ExecuteActions(List<ActionElement> actions)
    {
        foreach (var actionElement in actions)
        {
            if (actionElement.isEnabled) // Only execute enabled actions
            {
                actionElement.action?.Invoke();
            }
        }
    }

    private bool HasRequiredComponents(GameObject obj)
    {
        return obj.GetComponent<MonoBehaviour>() != null; // Ensures at least one script exists
    }
}

// **Helper Class to Manage Actions**
[System.Serializable]
public class ActionElement
{
    public string actionName; // Name for better organization
    public UnityEvent action; // The UnityEvent action
    public bool isEnabled = true; // Toggle for enabling/disabling this action
}
