using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class HoldClickAction : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Actions")]
    public UnityEvent onHoldActions;
    public UnityEvent onLetGoActions;
    public UnityEvent onDisableActions; // Runs before the script is disabled

    [Header("UI Animation")]
    public TextMeshProUGUI textElement;
    public float animationDuration = 0.5f;
    public Vector3 startScale = new Vector3(3f, 3f, 3f);
    public Vector3 endScale = Vector3.one;
    public Vector3 startPosition = new Vector3(800, 0, 0);
    public Vector3 endPosition = new Vector3(0, 0, 0);
    public float startRotation = 20f;
    public float endRotation = 0f;

    [Header("Legacy Animation")]
    public Animation legacyAnimation;
    public string animationName = "Idle";

    [Header("Animation Settings")]
    public bool playOnce = false; // Toggle between playing once or looping
    private bool animationPlayed = false; // Tracks if animation has already played

    [Header("Click Delay Settings")]
    public float clickCooldown = 1f; // Delay before allowing another click
    private bool canClick = true; // Ensures clicks are delayed

    private bool isHolding = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            isHolding = true;
            StartCoroutine(HandleHold());
            StartCoroutine(StartClickCooldown()); // Start cooldown timer
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            onLetGoActions?.Invoke();
            animationPlayed = false; // Reset for next hold
        }
    }

    private IEnumerator HandleHold()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

        onHoldActions?.Invoke();

        if (textElement != null)
        {
            StartCoroutine(AnimateText());
        }

        if (!playOnce || (playOnce && !animationPlayed))
        {
            PlayAnimation();
            animationPlayed = true;
        }

        while (isHolding && !playOnce)
        {
            yield return null;
        }
    }

    private void PlayAnimation()
    {
        if (legacyAnimation != null && legacyAnimation[animationName] != null)
        {
            legacyAnimation[animationName].wrapMode = playOnce ? WrapMode.Once : WrapMode.Loop;
            legacyAnimation.Play(animationName);
        }
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

    private IEnumerator StartClickCooldown()
    {
        canClick = false; // Disable clicking
        yield return new WaitForSeconds(clickCooldown); // Wait for the cooldown time
        canClick = true; // Enable clicking again
    }

    private void OnEnable()
    {
        animationPlayed = false; // Reset animation state when script is enabled
        canClick = true; // Ensure clicking is allowed
    }


    private void OnDisable()
    {
        StartCoroutine(DelayedDisableActions());
    }

    private IEnumerator DelayedDisableActions()
    {
        yield return new WaitForSeconds(0.1f);
        onDisableActions?.Invoke();
    }
}
