using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueControllerTest : MonoBehaviour
{
    [System.Serializable]
    public class DialogueElement
    {
        public string text;
        public AudioClip audioClip;
        public UnityEngine.Events.UnityEvent action;
    }


    [Header("Dialogue Settings")]
    public List<DialogueElement> dialogueElements;
    public AudioSource audioSource;
    public AudioClip advanceSound;
    public GameObject autoModeIndicator;
    public GameObject manualModeIndicator;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueTextMeshPro;

    [Header("Object Settings")]
    public string uniqueObjectID;

    [Header("Interaction Settings")]
    public bool onReceiveInteract = false;
    public bool repeatDialogue = false;
    public float interactionDelay = 0f;

    [Header("Typewriter & Auto Mode Settings")]
    public float typewriterSpeed = 0.05f;
    public float autoModeSpeed = 0.5f; // New variable for controlling auto mode speed

    [Header("Dialogue Start Settings")]
    public List<UnityEngine.Events.UnityEvent> startActions;
    public List<UnityEngine.Events.UnityEvent> startActionsAfterDelay;

    [Header("Dialogue End Settings")]
    public GameObject disableAfterDialogue;
    public float disableDelayAfterDialogue = 0f;
    public List<UnityEngine.Events.UnityEvent> endActions;
    public List<UnityEngine.Events.UnityEvent> endActionsAfterDelay;

    [Header("Cursor & Sound Effects")]
    public GameObject idleCursor;
    public GameObject hoverCursor;
    public GameObject clickCursor;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip interactionDelaySound;

    [Header("Raycast & Collider Settings")]
    public Camera mainCamera;
    private bool isInspecting = false;
    private MeshCollider meshCollider;

    private bool hasHovered = false;
    private bool hasPlayedInteractionSound = false;
    private bool hasTriggeredStartActionsAfterDelay = false;
    private int currentIndex = 0;
    private bool isAutoMode = false;
    private bool isSkipping = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool hasBeenInteracted = false;


    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();

        // Log the object's unique ID for debugging
        if (!string.IsNullOrEmpty(uniqueObjectID))
        {
            Debug.Log($"Object ID: {uniqueObjectID} initialized.");
        }
        else
        {
            Debug.LogWarning($"Object on {gameObject.name} does not have a Unique Object ID assigned.");
        }

    }

        void Update()
    {
        HandleRaycastHover();

        if (dialogueBox.activeSelf && hasBeenInteracted && !isAutoMode)
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


    private void HandleRaycastHover()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the raycast hits THIS object
            if (hit.collider.gameObject == gameObject)
            {
                if (!hasHovered) // Prevent redundant state toggling
                {
                    OnHoverEnter();
                }
            }
            else
            {
                if (hasHovered) // Prevent redundant state toggling
                {
                    OnHoverExit();
                }
            }
        }
        else
        {
            if (hasHovered) // Ensure state resets only when necessary
            {
                OnHoverExit();
            }
        }
    }


    public void SendInteractCommand()
    {
        if (!hasBeenInteracted)
        {
            hasBeenInteracted = true;

            // Reset relevant flags for repeated interaction
            isSkipping = false;
            hasTriggeredStartActionsAfterDelay = false;

            // Trigger the start actions
            StartCoroutine(TriggerStartActions());

            // Start the dialogue with delay and trigger delayed start actions
            StartCoroutine(StartDialogueWithDelay());
        }
    }




    private IEnumerator StartDialogueWithDelay()
    {
        yield return new WaitForSeconds(interactionDelay);

        // Trigger Start Actions After Delay (only once)
        if (!hasTriggeredStartActionsAfterDelay)
        {
            hasTriggeredStartActionsAfterDelay = true;
            StartCoroutine(TriggerStartActionsAfterDelay());
        }

        if (!hasPlayedInteractionSound && interactionDelaySound && audioSource)
        {
            audioSource.PlayOneShot(interactionDelaySound);
            hasPlayedInteractionSound = true;
        }

        if (!dialogueBox.activeSelf)
        {
            dialogueBox.SetActive(true);
            yield return new WaitUntil(() => dialogueBox.activeSelf);
        }

        currentIndex = 0;
        UpdateUI();
        PlayDialogue(currentIndex);
    }

    private void PlayDialogue(int index)
    {
        if (index >= dialogueElements.Count) return;

        var element = dialogueElements[index];

        if (dialogueTextMeshPro)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            dialogueTextMeshPro.text = "";
            typingCoroutine = StartCoroutine(TypeText(dialogueTextMeshPro, element.text));
        }

        if (element.audioClip && audioSource)
        {
            audioSource.PlayOneShot(element.audioClip);
        }

        element.action?.Invoke();
    }

    private void AdvanceDialogue()
    {
        if (currentIndex + 1 >= dialogueElements.Count)
        {
            dialogueBox.SetActive(false);
            hasBeenInteracted = false;
            isSkipping = false; // Reset skipping state

            if (disableAfterDialogue)
            {
                StartCoroutine(DisableAfterDialogue());
            }

            // Trigger End Actions After Dialogue
            StartCoroutine(TriggerEndActionsAfterDelay());

            StartCoroutine(TriggerEndActions());

            if (repeatDialogue)
            {
                currentIndex = 0;
                PlayDialogue(currentIndex);
            }
            return;
        }

        currentIndex++;

        if (audioSource && advanceSound)
        {
            audioSource.PlayOneShot(advanceSound);
        }

        PlayDialogue(currentIndex);
    }

    private IEnumerator DisableAfterDialogue()
    {
        yield return new WaitForSeconds(disableDelayAfterDialogue);
        disableAfterDialogue.SetActive(false);
    }

    private IEnumerator TriggerEndActions()
    {
        foreach (var action in endActions)
        {
            action?.Invoke();
            yield return null; // Ensures actions are triggered sequentially
        }
    }

    private void UpdateUI()
    {
        autoModeIndicator?.SetActive(isAutoMode);
        manualModeIndicator?.SetActive(!isAutoMode);
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
            yield return new WaitForSeconds(0.5f);
            manualModeIndicator.SetActive(true);
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

    public void OnHoverEnter()
    {
        hasHovered = true;

        // Enable hover cursor specific to THIS object
        if (hoverCursor)
        {
            hoverCursor.SetActive(true);
        }
        if (idleCursor)
        {
            idleCursor.SetActive(false);
        }
        if (hoverSound && audioSource)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnHoverExit()
    {
        hasHovered = false;

        // Disable hover cursor specific to THIS object
        if (hoverCursor)
        {
            hoverCursor.SetActive(false);
        }
        if (idleCursor)
        {
            idleCursor.SetActive(true);
        }
    }

    public void OnClick()
    {
        if (clickCursor) clickCursor.SetActive(true);
        if (clickSound && audioSource) audioSource.PlayOneShot(clickSound);

        StartCoroutine(ResetClickCursor());
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
            yield return new WaitForSeconds(autoModeSpeed); // Adjusts delay based on autoModeSpeed
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

        if (dialogueTextMeshPro)
        {
            dialogueTextMeshPro.text = element.text;
        }

        isTyping = false;

        if (manualModeIndicator)
        {
            manualModeIndicator.SetActive(true);
        }
    }

    private IEnumerator ResetClickCursor()
    {
        yield return new WaitForSeconds(0.2f);
        if (clickCursor) clickCursor.SetActive(false);
    }


    private IEnumerator TriggerStartActions()
    {
        foreach (var action in startActions)
        {
            action?.Invoke();
            yield return null; // Ensures actions are triggered sequentially
        }
    }

    private IEnumerator TriggerStartActionsAfterDelay()
    {
        foreach (var action in startActionsAfterDelay)
        {
            action?.Invoke();
            yield return null; // Ensures actions are triggered sequentially
        }
    }

    private IEnumerator TriggerEndActionsAfterDelay()
    {
        yield return new WaitForSeconds(disableDelayAfterDialogue);

        foreach (var action in endActionsAfterDelay)
        {
            action?.Invoke();
            yield return null; // Ensures actions are triggered sequentially
        }
    }

}
