using UnityEngine;
using UnityEngine.UI;

public class DebateTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public int startMinutes = 0;
    public int startSeconds = 0;
    public int startMilliseconds = 0;

    private float timeRemaining;
    private float timeScale = 1f; // Default speed

    [Header("Speed Settings")]
    public float speedUpMultiplier = 2f;   // Speed-up rate (set in Inspector)
    public float slowDownMultiplier = 0.5f; // Slow-down rate (set in Inspector)

    [Header("Number Sprites (0-9)")]
    public Sprite[] numberSprites; // Assign number sprites in Inspector
    public Image[] timerImages; // The images representing the timer digits (MM:SS:FFF)

    [Header("Time Indicator")]
    public Image timeIndicatorImage;
    public Sprite normalSpeedSprite;
    public Sprite speedUpSprite;
    public Sprite slowDownSprite;

    void Start()
    {
        timeRemaining = (startMinutes * 60) + startSeconds + (startMilliseconds / 1000f);
        if (timerImages.Length != 8)
        {
            Debug.LogError("Timer requires exactly 8 Image components (MM:SS:FFF).");
        }
        UpdateTimeIndicator(normalSpeedSprite);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime * timeScale;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        int milliseconds = Mathf.FloorToInt((timeRemaining * 1000) % 1000);

        string timeString = string.Format("{0:00}{1:00}{2:000}", minutes, seconds, milliseconds);

        for (int i = 0; i < timerImages.Length; i++)
        {
            int digit = int.Parse(timeString[i].ToString());
            timerImages[i].sprite = numberSprites[digit];
        }
    }

    // Speed Control Functions
    public void SpeedUp()
    {
        timeScale = speedUpMultiplier;
        UpdateTimeIndicator(speedUpSprite);
    }

    public void SlowDown()
    {
        timeScale = slowDownMultiplier;
        UpdateTimeIndicator(slowDownSprite);
    }

    public void NormalSpeed()
    {
        timeScale = 1f;
        UpdateTimeIndicator(normalSpeedSprite);
    }

    // Update Time Indicator Sprite
    void UpdateTimeIndicator(Sprite newSprite)
    {
        if (timeIndicatorImage != null)
        {
            timeIndicatorImage.sprite = newSprite;
        }
    }
}
