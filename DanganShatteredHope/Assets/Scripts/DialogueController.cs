using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueElement
    {
        public string text;
        public TextMeshProUGUI textMeshPro;
        public AudioClip audioClip;
        public UnityEngine.Events.UnityEvent action;
    }

    public List<DialogueElement> dialogueElements;
    public AudioSource audioSource;
    public AudioClip advanceSound;
    public GameObject autoModeIndicator;
    public GameObject manualModeIndicator; // Used as the "next" indicator
    public float autoModeDelay = 0.5f;
    public float manualModeDelay = 0.5f;
    public float typewriterSpeed = 0.05f;

    private int currentIndex = 0;
    private bool isAutoMode = false;
    private bool isSkipping = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (!isAutoMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    CompleteTextImmediately();
                }
                else
                {
                    AdvanceDialogue();
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                StartSkipping();
            }
            else
            {
                StopSkipping();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleAutoMode();
        }
    }

    private void AdvanceDialogue()
    {
        if (currentIndex >= dialogueElements.Count) return;

        var element = dialogueElements[currentIndex];

        // Ensure clicking first completes the text
        if (isTyping)
        {
            CompleteTextImmediately();
            return;
        }

        // Now it's safe to advance the dialogue
        currentIndex++;

        if (currentIndex < dialogueElements.Count)
        {
            element = dialogueElements[currentIndex];

            if (audioSource && advanceSound)
            {
                audioSource.PlayOneShot(advanceSound);
            }

            if (element.textMeshPro)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeText(element.textMeshPro, element.text));
            }

            if (element.audioClip && audioSource)
            {
                audioSource.PlayOneShot(element.audioClip);
            }

            element.action?.Invoke();
        }
    }


    private IEnumerator TypeText(TextMeshProUGUI textMeshPro, string text)
    {
        textMeshPro.text = "";
        manualModeIndicator?.SetActive(false); // Hide indicator while typing
        isTyping = true;

        foreach (char c in text.ToCharArray())
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;

        if (!isAutoMode && manualModeIndicator != null)
        {
            yield return new WaitForSeconds(manualModeDelay);
            manualModeIndicator.SetActive(true); // Show when ready to advance
        }
    }

    private void CompleteTextImmediately()
    {
        if (currentIndex >= dialogueElements.Count) return;

        var element = dialogueElements[currentIndex];

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (element.textMeshPro)
        {
            element.textMeshPro.text = element.text; // Instantly show full text
        }

        isTyping = false;

        if (manualModeIndicator)
        {
            manualModeIndicator.SetActive(true); // Show when ready to advance
        }
    }

    private void ToggleAutoMode()
    {
        isAutoMode = !isAutoMode;

        if (autoModeIndicator)
        {
            autoModeIndicator.SetActive(isAutoMode);
        }

        if (manualModeIndicator)
        {
            manualModeIndicator.SetActive(!isAutoMode);
        }

        if (isAutoMode)
        {
            StartCoroutine(AutoModeCoroutine());
        }
    }

    private IEnumerator AutoModeCoroutine()
    {
        while (isAutoMode && currentIndex < dialogueElements.Count)
        {
            AdvanceDialogue();
            yield return new WaitForSeconds(autoModeDelay);
        }
    }

    private void StartSkipping()
    {
        if (!isSkipping)
        {
            isSkipping = true;
            StartCoroutine(SkipCoroutine());
        }
    }

    private void StopSkipping()
    {
        isSkipping = false;
    }

    private IEnumerator SkipCoroutine()
    {
        while (isSkipping && currentIndex < dialogueElements.Count)
        {
            AdvanceDialogue();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateUI()
    {
        if (autoModeIndicator)
        {
            autoModeIndicator.SetActive(isAutoMode);
        }

        if (manualModeIndicator)
        {
            manualModeIndicator.SetActive(false); // Hide on start
        }
    }
}
