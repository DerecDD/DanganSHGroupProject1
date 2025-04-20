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
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        UpdateUI();
        PlayDialogue(currentIndex);
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

    private void PlayDialogue(int index)
    {
        if (index >= dialogueElements.Count) return;

        var element = dialogueElements[index];

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

    private void AdvanceDialogue()
    {
        if (currentIndex + 1 >= dialogueElements.Count) return;

        currentIndex++;

        if (audioSource && advanceSound)
        {
            audioSource.PlayOneShot(advanceSound);
        }

        PlayDialogue(currentIndex);
    }

    private IEnumerator TypeText(TextMeshProUGUI textMeshPro, string text)
    {
        textMeshPro.text = "";
        manualModeIndicator?.SetActive(false);
        isTyping = true;

        foreach (char c in text)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;

        if (!isAutoMode && manualModeIndicator != null)
        {
            yield return new WaitForSeconds(manualModeDelay);
            manualModeIndicator.SetActive(true);
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
            element.textMeshPro.text = element.text;
        }

        isTyping = false;

        if (manualModeIndicator)
        {
            manualModeIndicator.SetActive(true);
        }
    }

    private void ToggleAutoMode()
    {
        isAutoMode = !isAutoMode;

        autoModeIndicator?.SetActive(isAutoMode);
        manualModeIndicator?.SetActive(!isAutoMode);

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
        autoModeIndicator?.SetActive(isAutoMode);
        manualModeIndicator?.SetActive(false);
    }
}
