using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingSwitch : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume postProcessingVolume;

    [Header("Color Adjustment Settings")]
    public Color firstColor = Color.white; // Starting color
    public Color secondColor = Color.red;  // Target color
    public float colorTransitionDuration = 1f; // Transition time

    [Header("Depth of Field Settings")]
    public float depthOfFieldTransitionDuration = 1f; // Transition time for Depth of Field

    private bool isToggled = false; // Switch state
    private ColorAdjustments colorAdjustments; // Color Adjustments component
    private DepthOfField depthOfField; // Depth of Field component

    void Start()
    {
        // Ensure Volume is assigned
        if (postProcessingVolume == null)
        {
            Debug.LogError("PostProcessing Volume is not assigned.");
            return;
        }

        // Attempt to get Color Adjustments component
        if (!postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("Color Adjustments component not found in the Volume.");
        }

        // Attempt to get Depth of Field component
        if (!postProcessingVolume.profile.TryGet(out depthOfField))
        {
            Debug.LogError("Depth of Field component not found in the Volume.");
        }
    }

    // Trigger the switch (can be called from other scripts)
    public void Trigger()
    {
        if (isToggled)
        {
            StartCoroutine(TransitionToFirstColor());
            StartCoroutine(DisableDepthOfField());
        }
        else
        {
            StartCoroutine(TransitionToSecondColor());
            StartCoroutine(EnableDepthOfField());
        }

        isToggled = !isToggled; // Toggle the state
    }

    private IEnumerator TransitionToFirstColor()
    {
        if (colorAdjustments == null) yield break;

        // Gradually transition to firstColor
        float elapsedTime = 0f;
        Color initialColor = colorAdjustments.colorFilter.value;

        while (elapsedTime < colorTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, firstColor, elapsedTime / colorTransitionDuration);
            yield return null;
        }

        colorAdjustments.colorFilter.value = firstColor;
    }

    private IEnumerator TransitionToSecondColor()
    {
        if (colorAdjustments == null) yield break;

        // Gradually transition to secondColor
        float elapsedTime = 0f;
        Color initialColor = colorAdjustments.colorFilter.value;

        while (elapsedTime < colorTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(initialColor, secondColor, elapsedTime / colorTransitionDuration);
            yield return null;
        }

        colorAdjustments.colorFilter.value = secondColor;
    }

    private IEnumerator EnableDepthOfField()
    {
        if (depthOfField == null) yield break;

        depthOfField.active = true;

        // Gradually enable Depth of Field
        float elapsedTime = 0f;
        float initialFocusDistance = depthOfField.focusDistance.value;
        float targetFocusDistance = 1f; // Corrected focus distance value

        while (elapsedTime < depthOfFieldTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            depthOfField.focusDistance.value = Mathf.Lerp(initialFocusDistance, targetFocusDistance, elapsedTime / depthOfFieldTransitionDuration);
            yield return null;
        }

        depthOfField.focusDistance.value = targetFocusDistance;
    }

    private IEnumerator DisableDepthOfField()
    {
        if (depthOfField == null) yield break;

        // Gradually disable Depth of Field
        float elapsedTime = 0f;
        float initialFocusDistance = depthOfField.focusDistance.value;
        float targetFocusDistance = 100f; // Example disabled focus distance (far away)

        while (elapsedTime < depthOfFieldTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            depthOfField.focusDistance.value = Mathf.Lerp(initialFocusDistance, targetFocusDistance, elapsedTime / depthOfFieldTransitionDuration);
            yield return null;
        }

        depthOfField.active = false;
        depthOfField.focusDistance.value = targetFocusDistance;
    }

}
