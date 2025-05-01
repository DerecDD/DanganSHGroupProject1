using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowTimeAnim : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume targetVolume;
    private ColorAdjustments colorAdjustments;

    [Header("Transition Settings")]
    public float transitionSpeed = 1f;

    [System.Serializable]
    public class ColorPreset
    {
        public string name;
        public Color targetColor;
    }

    public List<ColorPreset> colorPresets = new List<ColorPreset>();

    private Color currentColor;
    private Color targetColor;
    private Coroutine transitionCoroutine;

    void Start()
    {
        if (targetVolume != null && targetVolume.profile.TryGet(out colorAdjustments))
        {
            currentColor = colorAdjustments.colorFilter.value;
        }
        else
        {
            Debug.LogError("Color Adjustments component not found in the provided Volume!");
        }
    }

    public void TransitionToColor(string colorName)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        ColorPreset preset = colorPresets.Find(p => p.name == colorName);
        if (preset != null)
        {
            targetColor = preset.targetColor;
            transitionCoroutine = StartCoroutine(SmoothTransition());
        }
        else
        {
            Debug.LogWarning($"Color '{colorName}' not found in presets!");
        }
    }

    private IEnumerator SmoothTransition()
    {
        float timer = 0f;
        Color startColor = colorAdjustments.colorFilter.value;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, timer);
            yield return null;
        }

        colorAdjustments.colorFilter.value = targetColor;
        currentColor = targetColor;
        transitionCoroutine = null;
    }
}
