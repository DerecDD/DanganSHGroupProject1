using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target; // Assign the target GameObject in the Inspector
    public bool flipRotation = false; // Toggle to invert the rotation
    public float followSpeed = 5f; // Speed at which the object follows the target

    private Vector3 offset; // Stores the initial positional offset

    void Start()
    {
        if (target != null)
        {
            // Calculate the initial offset between this object and the target
            offset = transform.position - target.position;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Rotate to face the target
            Vector3 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, flipRotation ? angle + 180f : angle);

            // Move along with the target while maintaining the offset
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);
        }
    }
}
