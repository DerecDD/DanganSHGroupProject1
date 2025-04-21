using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public List<string> elements = new List<string>();
    public float typingSpeed = 0.1f;
    public float eraseSpeed = 0.05f;
    public float delayBeforeNewText = 1.5f;
    public float delayBetweenTexts = 2f;
    public float cursorBlinkSpeed = 0.5f;

    private string currentText = "";
    private bool cursorVisible = true;
    private int currentIndex = -1;
    private Coroutine typingCoroutine;
    private Coroutine cursorCoroutine;

    void OnEnable()
    {
        if (cursorCoroutine == null)
            cursorCoroutine = StartCoroutine(CursorBlink());

        if (typingCoroutine == null)
            typingCoroutine = StartCoroutine(TextLoop());
    }

    void OnDisable()
    {
        if (cursorCoroutine != null)
        {
            StopCoroutine(cursorCoroutine);
            cursorCoroutine = null;
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    IEnumerator TextLoop()
    {
        while (true)
        {
            if (elements.Count > 0)
            {
                if (currentIndex == -1) // If no previous index, select a new text
                    currentIndex = Random.Range(0, elements.Count);

                yield return TypeText(elements[currentIndex]);
                yield return new WaitForSeconds(delayBetweenTexts);
                yield return EraseText();
                yield return new WaitForSeconds(delayBeforeNewText);

                currentIndex = Random.Range(0, elements.Count); // Pick next text
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        currentText = "";

        foreach (char letter in text)
        {
            currentText += letter;
            UpdateTextDisplay();
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator EraseText()
    {
        while (currentText.Length > 0)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            UpdateTextDisplay();
            yield return new WaitForSeconds(eraseSpeed);
        }
    }

    IEnumerator CursorBlink()
    {
        while (true)
        {
            cursorVisible = !cursorVisible;
            UpdateTextDisplay();
            yield return new WaitForSeconds(cursorBlinkSpeed);
        }
    }

    void UpdateTextDisplay()
    {
        textMeshPro.text = currentText + $" <size=100%>{(cursorVisible ? "|" : " ")}</size>";
    }

}
