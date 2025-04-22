using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuFunction : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetObject; // The GameObject to toggle
    public float disableDelay = 0f; // Delay before disabling the GameObject
    public UnityEvent preEnableAction; // Action to execute when enabling the GameObject
    public UnityEvent preDisableAction; // Action to execute before the delay starts

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
            Debug.LogWarning("Target GameObject is not assigned.");
            return;
        }

        if (!isActive)
        {
            // Execute pre-enable action
            preEnableAction?.Invoke();

            // Enable the GameObject
            targetObject.SetActive(true);
            isActive = true;
        }
        else
        {
            // Disable the GameObject with a delay
            StartCoroutine(DisableAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        // Execute pre-disable action
        preDisableAction?.Invoke();

        // Wait for the specified delay before disabling
        yield return new WaitForSeconds(disableDelay);

        if (targetObject != null)
        {
            targetObject.SetActive(false);
            isActive = false;
        }
    }
}
