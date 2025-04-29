using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CameraTransition
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public float fieldOfView;
    public float transitionSpeed = 1.0f; // Speed of transition
}

public class CameraTrial : MonoBehaviour
{
    public Camera targetCamera; // Drag and drop the camera here
    public List<CameraTransition> transitions = new List<CameraTransition>();
    public AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Ease-in-out curve

    private Coroutine transitionCoroutine;

    // Method to transition to a specified position by name
    public void GoToPositionByName(string transitionName)
    {
        CameraTransition targetTransition = transitions.Find(t => t.name == transitionName);
        if (targetTransition != null)
        {
            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine); // Stop the current transition if any

            transitionCoroutine = StartCoroutine(SmoothTransition(targetTransition));
        }
        else
        {
            Debug.LogWarning($"Transition with name '{transitionName}' not found.");
        }
    }

    private IEnumerator SmoothTransition(CameraTransition targetTransition)
    {
        Vector3 startPosition = targetCamera.transform.position;
        Quaternion startRotation = targetCamera.transform.rotation;
        float startFOV = targetCamera.fieldOfView;

        Vector3 endPosition = targetTransition.position;
        Quaternion endRotation = Quaternion.Euler(targetTransition.rotation);
        float endFOV = targetTransition.fieldOfView;

        float time = 0.0f;
        float duration = targetTransition.transitionSpeed;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Apply ease in and out curve
            float easedT = easeInOutCurve.Evaluate(t);

            targetCamera.transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
            targetCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, easedT);
            targetCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, easedT);

            yield return null;
        }

        targetCamera.transform.position = endPosition;
        targetCamera.transform.rotation = endRotation;
        targetCamera.fieldOfView = endFOV;
    }
}

// Custom Editor to add the "Copy Position, Rotation, and FOV" button
[CustomEditor(typeof(CameraTrial))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraTrial controller = (CameraTrial)target;

        if (GUILayout.Button("Copy Position, Rotation, and FOV"))
        {
            if (controller.targetCamera != null && controller.transitions.Count > 0)
            {
                // Copy data to the last selected transition
                CameraTransition lastTransition = controller.transitions[controller.transitions.Count - 1];
                lastTransition.position = controller.targetCamera.transform.position;
                lastTransition.rotation = controller.targetCamera.transform.rotation.eulerAngles;
                lastTransition.fieldOfView = controller.targetCamera.fieldOfView;

                Debug.Log("Copied camera settings to the last transition.");
            }
            else
            {
                Debug.LogWarning("No camera or transition selected to copy settings.");
            }
        }
    }
}
