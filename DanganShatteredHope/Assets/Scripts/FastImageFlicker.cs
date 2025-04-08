using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FastUIImageFlicker : MonoBehaviour
{
    public Image uiImage; // Assign your UI Image component here
    public Sprite[] images; // Drag and drop your images in the Inspector
    public float minFlickerSpeed = 0.02f; // Minimum time between flickers
    public float maxFlickerSpeed = 0.05f; // Maximum time between flickers

    private Coroutine flickerCoroutine; // Reference to the coroutine

    void OnEnable()
    {
        if (images.Length > 0 && uiImage != null)
        {
            flickerCoroutine = StartCoroutine(FlickerImages());
        }
        else
        {
            Debug.LogWarning("Images or UI Image component is not assigned!");
        }
    }

    void OnDisable()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine); // Stop the coroutine if the object is disabled
        }
    }

    IEnumerator FlickerImages()
    {
        while (true)
        {
            // Change to a random image
            int currentImageIndex = Random.Range(0, images.Length);
            uiImage.sprite = images[currentImageIndex];

            // Wait for a random time within the flicker speed range
            float flickerDelay = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            yield return new WaitForSeconds(flickerDelay);
        }
    }
}
