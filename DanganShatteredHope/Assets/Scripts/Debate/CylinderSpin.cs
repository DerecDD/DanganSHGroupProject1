using UnityEngine;
using System.Collections;

public class CylinderSpeed : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Default speed
    public float boostedSpeed = 20f;   // Faster speed (editable in Inspector)
    public float transitionDuration = 1f; // Time to speed up/down (editable)
    public float timeAtBoostedSpeed = 2f; // Time spent at boosted speed (editable)

    private float currentSpeed;
    private Coroutine speedCoroutine; // Stores the active speed change coroutine

    private void Start()
    {
        currentSpeed = rotationSpeed;
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
    }

    public void SpeedUpTemporarily()
    {
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);  // Cancel any ongoing speed transitions
        }

        speedCoroutine = StartCoroutine(SpeedUpRoutine(boostedSpeed, timeAtBoostedSpeed, transitionDuration));
    }

    private IEnumerator SpeedUpRoutine(float targetSpeed, float timeAtBoostedSpeed, float transitionDuration)
    {
        float startSpeed = currentSpeed;
        float elapsedTime = 0f;

        // Smoothly transition to boosted speed
        while (elapsedTime < transitionDuration)
        {
            currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = targetSpeed;

        // Wait at boosted speed
        yield return new WaitForSeconds(timeAtBoostedSpeed);

        elapsedTime = 0f;

        // Smoothly transition back to normal speed
        while (elapsedTime < transitionDuration)
        {
            currentSpeed = Mathf.Lerp(targetSpeed, rotationSpeed, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = rotationSpeed;
    }
}
