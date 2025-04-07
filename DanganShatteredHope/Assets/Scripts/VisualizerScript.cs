using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerScript : MonoBehaviour
{
    public float minHeight = 15.0f;
    public float maxHeight = 425.0f;
    public float updateSentivity = 10.0f;
    public Color visualizerColor = Color.gray;
    [Space(15)]
    public AudioSource audioSource; // Reference to an existing AudioSource
    public AudioClip audioClip; // Clip to be played in the AudioSource
    [Space(15), Range(64, 8192)]
    public int visualizerSimples = 64;

    VisualizerObjectScript[] visualizerObjects;

    // Use this for initialization
    void Start()
    {
        visualizerObjects = GetComponentsInChildren<VisualizerObjectScript>();

        if (!audioSource)
        {
            Debug.LogWarning("No AudioSource assigned!");
            return;
        }

        if (audioClip)
        {
            audioSource.clip = audioClip; // Assign the audio clip to the provided AudioSource
            audioSource.loop = true; // Optional: Set loop state
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!audioSource || !audioSource.isPlaying)
            return;

        float[] spectrumData = audioSource.GetSpectrumData(visualizerSimples, 0, FFTWindow.Rectangular);

        for (int i = 0; i < visualizerObjects.Length; i++)
        {
            Vector2 newSize = visualizerObjects[i].GetComponent<RectTransform>().rect.size;

            newSize.y = Mathf.Clamp(
                Mathf.Lerp(newSize.y, minHeight + (spectrumData[i] * (maxHeight - minHeight) * 5.0f), updateSentivity * 0.5f),
                minHeight,
                maxHeight
            );
            visualizerObjects[i].GetComponent<RectTransform>().sizeDelta = newSize;

            visualizerObjects[i].GetComponent<Image>().color = visualizerColor;
        }
    }
}
