using UnityEngine;
using UnityEngine.UI;

public class DebateTimerEffect : MonoBehaviour
{
    [Header("Jumble Settings")]
    public float jumbleSpeed = 0.1f; // How fast numbers change

    [Header("Glitch Settings")]
    public float shakeFactor = 3f; // Max shake distance
    public float shakeSpeed = 10f; // Shake frequency
    public float flickerChance = 0.2f; // Probability of flickering (0 to 1)

    [Header("Number Sprites (0-9)")]
    public Sprite[] numberSprites;
    public Image[] timerImages;

    [Header("Time Indicator")]
    public Image timeIndicatorImage;
    public Sprite[] indicatorSprites;
    public float indicatorShakeFactor = 4f; // Shake effect for indicator
    public float indicatorShakeSpeed = 8f; // Shake speed for indicator

    private float nextJumbleTime;
    private Vector3[] originalPositions;
    private Vector3 indicatorOriginalPosition;

    void Start()
    {
        originalPositions = new Vector3[timerImages.Length];
        for (int i = 0; i < timerImages.Length; i++)
        {
            originalPositions[i] = timerImages[i].transform.localPosition;
        }

        if (timeIndicatorImage != null)
        {
            indicatorOriginalPosition = timeIndicatorImage.transform.localPosition;
        }
    }

    void Update()
    {
        if (Time.time >= nextJumbleTime)
        {
            JumbleNumbers();
            RandomizeIndicator();
            nextJumbleTime = Time.time + jumbleSpeed;
        }

        ApplyGlitchEffect();
        ShakeIndicator(); // New function to make the time indicator shake
    }

    void JumbleNumbers()
    {
        foreach (var image in timerImages)
        {
            int randomDigit = Random.Range(0, 10);
            image.sprite = numberSprites[randomDigit];
        }
    }

    void RandomizeIndicator()
    {
        if (timeIndicatorImage != null && indicatorSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, indicatorSprites.Length);
            timeIndicatorImage.sprite = indicatorSprites[randomIndex];
        }
    }

    void ApplyGlitchEffect()
    {
        for (int i = 0; i < timerImages.Length; i++)
        {
            float xOffset = Random.Range(-shakeFactor, shakeFactor);
            float yOffset = Random.Range(-shakeFactor, shakeFactor);
            timerImages[i].transform.localPosition = originalPositions[i] + new Vector3(xOffset, yOffset, 0);

            if (Random.value < flickerChance)
            {
                timerImages[i].enabled = false;
            }
            else
            {
                timerImages[i].enabled = true;
            }
        }
    }

    void ShakeIndicator()
    {
        if (timeIndicatorImage != null)
        {
            float xOffset = Random.Range(-indicatorShakeFactor, indicatorShakeFactor);
            float yOffset = Random.Range(-indicatorShakeFactor, indicatorShakeFactor);

            timeIndicatorImage.transform.localPosition = indicatorOriginalPosition + new Vector3(xOffset, yOffset, 0);
        }
    }
}
