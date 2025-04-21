using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public Camera targetCamera; // Assign the camera in the Inspector

    // Rotation speed
    public float rotationSpeed = 50f;

    // Rotation limits
    public Vector2 rotationLimitsX = new Vector2(-30f, 30f); // Tilt (up/down)
    public Vector2 rotationLimitsY = new Vector2(-60f, 60f); // Rotate (left/right)

    // Limit GameObjects
    public GameObject limitUp;
    public GameObject limitLeft;
    public GameObject limitDown;
    public GameObject limitRight;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        if (targetCamera == null) return;

        // Handle left/right rotation (A/D keys)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            rotationY += horizontalInput * rotationSpeed * Time.deltaTime;
        }

        // Handle tilt up/down (W/S keys)
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (verticalInput != 0)
        {
            rotationX -= verticalInput * rotationSpeed * Time.deltaTime; // Negative to fix inversion
        }

        // Apply rotation constraints
        rotationX = Mathf.Clamp(rotationX, rotationLimitsX.x, rotationLimitsX.y);
        rotationY = Mathf.Clamp(rotationY, rotationLimitsY.x, rotationLimitsY.y);

        // Apply rotation to the camera
        targetCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Activate limit GameObjects
        ActivateLimit(rotationX <= rotationLimitsX.x, limitUp);    // Upper tilt limit
        ActivateLimit(rotationX >= rotationLimitsX.y, limitDown);  // Lower tilt limit
        ActivateLimit(rotationY <= rotationLimitsY.x, limitLeft);  // Left rotation limit
        ActivateLimit(rotationY >= rotationLimitsY.y, limitRight); // Right rotation limit
    }

    private void ActivateLimit(bool atLimit, GameObject limitObject)
    {
        if (limitObject != null)
        {
            limitObject.SetActive(atLimit);
        }
    }
}
