using UnityEngine;
using System.Collections.Generic;

public class PlayLegacyAnimation : MonoBehaviour
{
    // Reference to the Animation component
    public Animation legacyAnimation;

    // List of animation clips
    public List<string> animationNames = new List<string>();

    // Method to play a specific animation by its name
    public void PlayAnimation(string animationName)
    {
        if (legacyAnimation == null)
        {
            Debug.LogError("No Animation component found! Assign one in the inspector.");
            return;
        }

        if (string.IsNullOrEmpty(animationName) || !legacyAnimation[animationName])
        {
            Debug.LogError($"Animation clip '{animationName}' not found or name is empty!");
            return;
        }

        legacyAnimation.Play(animationName);
        Debug.Log($"Playing animation: {animationName}");
    }

    // Method to play all animations in sequence
    public void PlayAllAnimations()
    {
        if (legacyAnimation == null)
        {
            Debug.LogError("No Animation component found! Assign one in the inspector.");
            return;
        }

        if (animationNames.Count == 0)
        {
            Debug.LogError("No animation clips found! Add some animation names to the list.");
            return;
        }

        foreach (string animationName in animationNames)
        {
            if (!legacyAnimation[animationName])
            {
                Debug.LogError($"Animation clip '{animationName}' not found!");
                continue;
            }

            legacyAnimation.Play(animationName);
            Debug.Log($"Playing animation: {animationName}");
            // Optionally, add delays or logic for sequential playback
        }
    }
}
