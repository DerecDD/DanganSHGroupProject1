using UnityEngine;

public class FollowMouseTruth : MonoBehaviour
{
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z; // Adjust for camera depth
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Compute direction from object to mouse
        Vector3 direction = worldMousePos - transform.position;

        // Compute rotation towards mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation to object
        transform.rotation = Quaternion.Euler(new Vector3(0, 83, angle));
    }
}
