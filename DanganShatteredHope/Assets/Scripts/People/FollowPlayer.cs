using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Camera targetCamera; // Assign the main camera or another camera in the Inspector

    void Update()
    {
        if (targetCamera != null)
        {
            // Get direction to the camera
            Vector3 directionToCamera = targetCamera.transform.position - transform.position;

            // Ignore the X and Z rotation and only rotate on the Y-axis
            directionToCamera.y = 0;

            // Apply rotation
            transform.rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
        }
    }
}
