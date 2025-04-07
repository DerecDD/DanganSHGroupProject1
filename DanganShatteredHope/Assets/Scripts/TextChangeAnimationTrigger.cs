using UnityEngine;
using TMPro; // Include TextMeshPro namespace

public class TextChangeAnimationTrigger : MonoBehaviour
{
    public TextMeshProUGUI textMeshProComponent; // Assign the TextMeshPro component
    public Animation animationComponent; // Assign the legacy Animation component
    private string previousText = ""; // Tracks the previous text to detect changes

    void Start()
    {
        // Store the initial text if the TextMeshPro component is assigned
        if (textMeshProComponent != null)
        {
            previousText = textMeshProComponent.text;
        }
    }

    void Update()
    {
        // Check if the text has changed
        if (textMeshProComponent != null && textMeshProComponent.text != previousText)
        {
            previousText = textMeshProComponent.text; // Update the previous text
            PlayAnimation(); // Trigger or override the animation
        }
    }

    void PlayAnimation()
    {
        // Play the legacy animation if the Animation component exists
        if (animationComponent != null)
        {
            animationComponent.Stop(); // Stop the current animation if it's playing
            animationComponent.Play(); // Play the animation from the start
        }
    }
}
