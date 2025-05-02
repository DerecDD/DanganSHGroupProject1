using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextExplosion : MonoBehaviour
{
    public float explosionForce = 5f; // Adjust the force of the explosion
    public float explosionRadius = 2f; // Adjust the radius within which characters move
    public float fadeDuration = 1.5f; // Time to fade characters out

    private TextMeshProUGUI textMeshPro;
    private List<GameObject> letters = new List<GameObject>();

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        CreateLetterObjects();
    }

    void CreateLetterObjects()
    {
        string originalText = textMeshPro.text;
        textMeshPro.text = ""; // Clear the original text

        for (int i = 0; i < originalText.Length; i++)
        {
            GameObject letterObj = new GameObject("Letter_" + i);
            letterObj.transform.SetParent(transform);
            TextMeshPro letterTMP = letterObj.AddComponent<TextMeshPro>();
            letterTMP.text = originalText[i].ToString();
            letterTMP.font = textMeshPro.font;
            letterTMP.color = textMeshPro.color;
            letterTMP.fontSize = textMeshPro.fontSize;
            letters.Add(letterObj);
        }
    }

    public void BreakText()
    {
        foreach (GameObject letter in letters)
        {
            Rigidbody2D rb = letter.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Optional: Prevent gravity effects
            rb.AddForce(new Vector2(Random.Range(-explosionForce, explosionForce),
                                    Random.Range(-explosionForce, explosionForce)), ForceMode2D.Impulse);
            StartCoroutine(FadeLetter(letter));
        }
    }

    IEnumerator FadeLetter(GameObject letter)
    {
        TextMeshPro tmp = letter.GetComponent<TextMeshPro>();
        Color originalColor = tmp.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }

        Destroy(letter);
    }
}
