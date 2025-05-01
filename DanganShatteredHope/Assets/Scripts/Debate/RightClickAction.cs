using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class RightClickAction : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Click Delay Settings")]
    public float clickDelay = 1f;
    private bool canClick = true;

    [Header("Actions")]
    public UnityEvent onClickActions;
    public UnityEvent afterDelayActions;

    [Header("UI Animation")]
    public TextMeshProUGUI textElement;
    public float animationDuration = 0.5f;
    public Vector3 startScale = new Vector3(3f, 3f, 3f); // Large at start
    public Vector3 endScale = Vector3.one; // Normal at center
    public Vector3 startPosition = new Vector3(800, 0, 0); // Right side of screen
    public Vector3 endPosition = new Vector3(0, 0, 0); // Center of screen
    public float startRotation = 20f; // Tilts when appearing
    public float endRotation = 0f; // Straight when centered

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canClick)
        {
            StartCoroutine(HandleClick());
        }
    }

    private IEnumerator HandleClick()
    {
        canClick = false;

        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

        onClickActions?.Invoke();

        if (textElement != null)
        {
            StartCoroutine(AnimateText());
        }

        yield return new WaitForSeconds(clickDelay);

        afterDelayActions?.Invoke();

        canClick = true;
    }

    private IEnumerator AnimateText()
    {
        float timer = 0;
        textElement.rectTransform.anchoredPosition = startPosition;
        textElement.rectTransform.localScale = startScale;
        textElement.rectTransform.eulerAngles = new Vector3(0, 0, startRotation);

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;
            textElement.rectTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            textElement.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, progress);
            textElement.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(startRotation, endRotation, progress));
            timer += Time.deltaTime;
            yield return null;
        }

        textElement.rectTransform.localScale = endScale;
        textElement.rectTransform.anchoredPosition = endPosition;
        textElement.rectTransform.eulerAngles = new Vector3(0, 0, endRotation);
    }
}
