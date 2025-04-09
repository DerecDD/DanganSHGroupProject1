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
    public GameObject manualModeIndicator;
    public float autoModeDelay = 0.5f;
    public float manualModeDelay = 0.5f;
    public float typewriterSpeed = 0.05f;

    private int currentIndex = 0;
    private bool isAutoMode = false;
    private bool isSkipping = false;
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
                AdvanceDialogue();
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

        currentIndex++;
    }

    private IEnumerator TypeText(TextMeshProUGUI textMeshPro, string text)
    {
        textMeshPro.text = "";
        manualModeIndicator?.SetActive(false); // Hide the manual mode indicator during typing

        foreach (char c in text.ToCharArray())
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // Enable manual mode indicator after typewriter effect and its own delay
        if (!isAutoMode && manualModeIndicator != null)
        {
            yield return new WaitForSeconds(manualModeDelay);
            manualModeIndicator.SetActive(true);
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
            yield return new WaitForSeconds(autoModeDelay); // Separate delay for auto mode
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
            yield return new WaitForSeconds(0.1f); // Adjust skipping speed
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
            manualModeIndicator.SetActive(!isAutoMode);
        }
    }
}
