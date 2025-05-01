#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
#endif
