using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformChanger : MonoBehaviour
{
    public enum InterpolationType
    {
        Linear,     // Moves steadily from start to end
        EaseIn,     // Starts slowly and accelerates toward the end
        EaseOut,    // Starts quickly and decelerates toward the end
        EaseInOut   // Combines ease-in and ease-out for smooth transitions
    }

    [System.Serializable]
    public class TransformElement
    {
        public string name;         // Unique identifier for this transform element
        public GameObject targetObject; // Target object to transform
        public Vector3 newPosition; // Desired position
        public Vector3 newRotation; // Desired rotation (Euler angles)
        public float transitionSpeed = 1f; // Speed of the transition
        public InterpolationType interpolationType = InterpolationType.Linear; // Type of interpolation
    }

    [Header("Transform Elements")]
    public List<TransformElement> transformElements; // List of transform settings

    /// <summary>
    /// Trigger transformation based on element name.
    /// </summary>
    /// <param name="elementName">The name of the element to execute transformation.</param>
    public void TriggerTransformation(string elementName)
    {
        foreach (TransformElement element in transformElements)
        {
            if (element.name == elementName)
            {
                if (element.targetObject != null)
                {
                    StartCoroutine(SmoothTransform(element));
                }
                else
                {
                    Debug.LogWarning($"Target object is not assigned for element: {elementName}");
                }
                return; // Exit once the matching element is found and triggered
            }
        }

        Debug.LogError($"No transform element found with the name: {elementName}");
    }

    private IEnumerator SmoothTransform(TransformElement element)
    {
        Transform targetTransform = element.targetObject.transform;
        Vector3 startPosition = targetTransform.position;
        Vector3 startRotation = targetTransform.eulerAngles;

        float elapsedTime = 0f;

        while (elapsedTime < element.transitionSpeed)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / element.transitionSpeed;
            t = ApplyInterpolation(t, element.interpolationType);

            // Smoothly interpolate position and rotation
            targetTransform.position = Vector3.Lerp(startPosition, element.newPosition, t);
            targetTransform.eulerAngles = Vector3.Lerp(startRotation, element.newRotation, t);

            yield return null; // Wait until the next frame
        }

        // Ensure final position and rotation are applied
        targetTransform.position = element.newPosition;
        targetTransform.eulerAngles = element.newRotation;
    }

    private float ApplyInterpolation(float t, InterpolationType type)
    {
        switch (type)
        {
            case InterpolationType.EaseIn:
                return Mathf.Pow(t, 2); // Accelerates over time
            case InterpolationType.EaseOut:
                return Mathf.Sqrt(t); // Decelerates over time
            case InterpolationType.EaseInOut:
                return Mathf.SmoothStep(0, 1, t); // Combines ease-in and ease-out
            case InterpolationType.Linear:
            default:
                return t; // Constant speed
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TransformChanger))]
    public class TransformChangerEditor : Editor
    {
        private int selectedElementIndex = 0; // Index to track the selected element

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TransformChanger script = (TransformChanger)target;

            GUILayout.Space(10);

            if (script.transformElements.Count > 0)
            {
                GUILayout.Label("Select Element to Copy Transform", EditorStyles.boldLabel);

                // Dropdown menu to select the element from the list
                selectedElementIndex = EditorGUILayout.Popup(
                    selectedElementIndex,
                    GetElementNames(script.transformElements)
                );

                GUILayout.Space(5);

                // Button to copy the transform
                if (GUILayout.Button("COPY CURRENT LOCATION AND ROTATION"))
                {
                    CopyCurrentTransform(script, selectedElementIndex);
                }
            }
            else
            {
                GUILayout.Label("No elements available in the transform list.", EditorStyles.helpBox);
            }
        }

        private string[] GetElementNames(List<TransformChanger.TransformElement> elements)
        {
            List<string> names = new List<string>();
            foreach (var element in elements)
            {
                names.Add(element.name);
            }
            return names.ToArray();
        }

        private void CopyCurrentTransform(TransformChanger script, int index)
        {
            if (index < 0 || index >= script.transformElements.Count)
            {
                Debug.LogError("Selected index is out of bounds.");
                return;
            }

            TransformElement selectedElement = script.transformElements[index];

            if (selectedElement != null && selectedElement.targetObject != null)
            {
                selectedElement.newPosition = selectedElement.targetObject.transform.position;
                selectedElement.newRotation = selectedElement.targetObject.transform.eulerAngles;

                Debug.Log($"Copied transform to element: {selectedElement.name}");
            }
            else
            {
                Debug.LogError($"Target object is not assigned for element: {selectedElement.name}");
            }
        }
    }
#endif
}
