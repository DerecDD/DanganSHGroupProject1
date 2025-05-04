using UnityEngine;

public class FollowMouseTruth : MonoBehaviour
{
    public Canvas canvas;
    public float planeDistance = 10f;
    public float anchorAngle = 0f; // Adjusts initial rotation
    public Vector3 positionOffset = Vector3.zero; // Allows tweaking the position offset
    public float minX = 825.1f; // Minimum RectTransform anchored X position
    public float maxX = 1016.5f;  // Maximum RectTransform anchored X position
    public float movementFactor = 0.1f; // Adjusts sensitivity of movement
    public bool invertMovement = false; // Toggle movement direction

    private Vector3 previousMouseWorldPosition;
    private RectTransform rectTransform;
    private float screenMiddleX;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // Get RectTransform reference
        screenMiddleX = Screen.width / 2f; // Find middle point of screen
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition() + positionOffset;
        float mouseX = Input.mousePosition.x;

        // Only allow movement if the mouse is on the left half of the screen
        if (mouseX <= screenMiddleX)
        {
            if (Vector3.Distance(previousMouseWorldPosition, mouseWorldPosition) > 0.01f)
            {
                float deltaX = (mouseWorldPosition.x - previousMouseWorldPosition.x) * movementFactor;

                if (invertMovement)
                {
                    deltaX *= -1;
                }

                // Constrain the RectTransform's anchored X position within limits
                Vector2 anchoredPosition = rectTransform.anchoredPosition;
                anchoredPosition.x = Mathf.Clamp(anchoredPosition.x + deltaX, minX, maxX);
                rectTransform.anchoredPosition = anchoredPosition;

                previousMouseWorldPosition = mouseWorldPosition;
            }
        }
        else
        {
            // If mouse is in the right half, keep object at minX
            rectTransform.anchoredPosition = new Vector2(minX, rectTransform.anchoredPosition.y);
        }

        // Rotation logic remains unchanged
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = targetRotation * Quaternion.Euler(0, anchorAngle, 0);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, canvas.worldCamera, out localPoint);
            Vector3 worldPosition = canvasRect.TransformPoint(localPoint);

            worldPosition.z = canvas.worldCamera ? canvas.worldCamera.transform.position.z + planeDistance : planeDistance;
            return worldPosition;
        }
        else
        {
            mousePosition.z = planeDistance;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }
}
