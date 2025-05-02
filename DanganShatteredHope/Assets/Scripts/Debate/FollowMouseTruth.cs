using UnityEngine;

public class FollowMouseTruth : MonoBehaviour
{
    public Canvas canvas;
    public float planeDistance = 10f;
    public float anchorAngle = 0f; // Adjusts initial rotation
    public Vector3 positionOffset = Vector3.zero; // Allows tweaking the position offset

    private void Update()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition() + positionOffset;
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

            // Ensure correct depth in UI space
            worldPosition.z = canvas.worldCamera ? canvas.worldCamera.transform.position.z + planeDistance : planeDistance;
            return worldPosition;
        }
        else
        {
            // Convert screen to world coordinates correctly
            mousePosition.z = planeDistance;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }
}
