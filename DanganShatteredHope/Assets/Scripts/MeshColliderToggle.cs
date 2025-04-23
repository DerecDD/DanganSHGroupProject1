using UnityEngine;

public class MeshColliderToggle : MonoBehaviour
{
    private bool isActive = true; // Tracks the current state of the Mesh Colliders
    private MeshCollider[] meshColliders;

    private void Awake()
    {
        // Find all Mesh Colliders in the child GameObjects
        meshColliders = GetComponentsInChildren<MeshCollider>();
    }

    public void SwitchState()
    {
        // Toggle the active state
        isActive = !isActive;

        // Loop through each Mesh Collider and set its enabled state
        foreach (MeshCollider meshCollider in meshColliders)
        {
            meshCollider.enabled = isActive;
        }
    }
}
