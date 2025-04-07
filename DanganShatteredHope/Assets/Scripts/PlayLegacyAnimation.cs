using UnityEngine;

public class PlayLegacyAnimation : MonoBehaviour
{
    // Reference to the Animation component
    public Animation legacyAnimation;

    // Name of the animation clip to play
    public string animationName;

    // Method to play the animation
    public void PlaySelectedAnimation()
    {
        if (legacyAnimation == null)
        {
            Debug.LogError("No Animation component found! Assign one in the inspector.");
            return;
        }

        if (string.IsNullOrEmpty(animationName) || !legacyAnimation[animationName])
        {
            Debug.LogError("Animation clip not found or name is empty! Check the animation clip name.");
            return;
        }

        // Play the specified animation
        legacyAnimation.Play(animationName);
        Debug.Log($"Playing animation: {animationName}");
    }

    // Method to reset the animation to the first frame
    public void ResetAnimationToStart()
    {
        if (legacyAnimation == null)
        {
            Debug.LogError("No Animation component found! Assign one in the inspector.");
            return;
        }

        if (string.IsNullOrEmpty(animationName) || !legacyAnimation[animationName])
        {
            Debug.LogError("Animation clip not found or name is empty! Check the animation clip name.");
            return;
        }

        // Ensure the animation exists and set its normalizedTime to the first frame
        var animationState = legacyAnimation[animationName];
        animationState.time = 0f; // Set time to the first frame
        legacyAnimation.Sample(); // Force the animation to update to the current time
        legacyAnimation.Stop(); // Stop the animation playback
        Debug.Log($"Animation {animationName} reset to the first frame.");
    }
}
