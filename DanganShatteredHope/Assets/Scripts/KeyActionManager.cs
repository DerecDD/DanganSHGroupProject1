using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class KeyActionManager : MonoBehaviour
{
    [System.Serializable]
    public class KeyActionElement
    {
        public KeyCode key; // The keyboard key to monitor

        // Immediate actions
        public UnityEvent onPressActions; // Actions executed on key press
        public UnityEvent onLiftActions;  // Actions executed on key release

        // Delayed actions
        public float onPressActionsDelay = 0f; // Delay before executing onPressActionsAfterDelay
        public UnityEvent onPressActionsAfterDelay; // Actions executed after delay

        public float onLiftActionsDelay = 0f; // Delay before executing onLiftActionsAfterDelay
        public UnityEvent onLiftActionsAfterDelay; // Actions executed after delay
    }

    public List<KeyActionElement> keyActions = new List<KeyActionElement>();

    void Update()
    {
        foreach (KeyActionElement element in keyActions)
        {
            if (Input.GetKeyDown(element.key)) // Detect key press
            {
                element.onPressActions?.Invoke(); // Execute immediate press actions
                StartCoroutine(ExecuteAfterDelay(element.onPressActionsAfterDelay, element.onPressActionsDelay));
            }
            if (Input.GetKeyUp(element.key)) // Detect key release
            {
                element.onLiftActions?.Invoke(); // Execute immediate release actions
                StartCoroutine(ExecuteAfterDelay(element.onLiftActionsAfterDelay, element.onLiftActionsDelay));
            }
        }
    }

    private IEnumerator ExecuteAfterDelay(UnityEvent actionEvent, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        actionEvent?.Invoke();
    }
}
