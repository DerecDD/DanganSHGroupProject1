using System.Collections;
using TMPro;
using UnityEngine;

public class TextScrambler : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Drag and drop your TextMeshPro object here
    public float duration = 2f; // Duration for the scrambling effect

    private string originalText; // To save the original text
    private bool isScrambling = false; // To ensure we don't trigger multiple scrambling effects simultaneously

    public void Scramble()
    {
        if (!isScrambling && textMeshPro != null)
        {
            originalText = textMeshPro.text; // Save the original text
            StartCoroutine(ScrambleText());
        }
    }

    private IEnumerator ScrambleText()
    {
        isScrambling = true;

        float timer = 0f;

        while (timer < duration)
        {
            textMeshPro.text = GenerateScrambledText(originalText);
            timer += Time.deltaTime;
            yield return null;
        }

        textMeshPro.text = originalText; // Restore the original text
        isScrambling = false;
    }

    private string GenerateScrambledText(string sourceText)
    {
        char[] scrambled = sourceText.ToCharArray();

        for (int i = 0; i < scrambled.Length; i++)
        {
            int randomIndex = Random.Range(0, scrambled.Length);
            char temp = scrambled[i];
            scrambled[i] = scrambled[randomIndex];
            scrambled[randomIndex] = temp;
        }

        return new string(scrambled);
    }
}
