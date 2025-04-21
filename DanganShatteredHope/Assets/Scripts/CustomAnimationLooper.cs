using UnityEngine;

public class CustomAnimationLooper : MonoBehaviour
{
    public Animation animationComponent; // Assign the legacy Animation component here
    public string animationName; // Specify the name of the animation clip to play
    public float startTime = 0f; // Set the time (in seconds) at which the animation starts looping
    public float animationSpeed = 1f; // Control the speed of the animation
    public bool playOnceThenLoop = false; // Checkbox to decide if the animation should play once and then loop from the specified time

    private float clipLength; // Store the length of the animation clip
    private bool hasPlayedOnce = false; // Track if the animation has played once

    void OnEnable()
    {
        // Check if the animation component and specified animation clip exist
        if (animationComponent != null && animationComponent[animationName] != null)
        {
            // Store the animation clip length
            clipLength = animationComponent[animationName].clip.length;

            // Set animation settings
            animationComponent[animationName].wrapMode = WrapMode.Loop; // Ensure looping is enabled
            animationComponent[animationName].speed = animationSpeed;

            // Reset state on enable
            hasPlayedOnce = false;

            if (playOnceThenLoop)
            {
                // Play the animation from the start for the first time
                animationComponent[animationName].time = 0f;
            }
            else
            {
                // Start at the specified loop time
                animationComponent[animationName].time = startTime % clipLength; // Ensure valid start time within clip length
            }

            animationComponent.Play(animationName);
            Debug.Log($"Animation '{animationName}' started. PlayOnceThenLoop: {playOnceThenLoop}");
        }
        else
        {
            Debug.LogError("Animation component or specified animation clip not found!");
        }
    }

    void Update()
    {
        if (animationComponent != null && animationComponent[animationName] != null)
        {
            float currentTime = animationComponent[animationName].time;

            if (playOnceThenLoop)
            {
                // If the animation played once and reached the end, loop indefinitely from the specified start time
                if (!hasPlayedOnce && currentTime >= clipLength)
                {
                    hasPlayedOnce = true; // Mark as played once
                    LoopFromSpecifiedTime();
                }
                else if (hasPlayedOnce && (currentTime >= clipLength || currentTime < startTime))
                {
                    LoopFromSpecifiedTime();
                }
            }
            else
            {
                // Regular looping logic starting from the specified time
                if (currentTime >= clipLength || currentTime < startTime)
                {
                    LoopFromSpecifiedTime();
                }
            }
        }
    }

    private void LoopFromSpecifiedTime()
    {
        if (animationComponent != null && animationComponent[animationName] != null)
        {
            animationComponent[animationName].time = startTime % clipLength; // Loop back to the specified start time
            animationComponent.Sample(); // Sample the animation at the new time
            animationComponent.Play(animationName); // Continue playing the animation
            Debug.Log($"Animation '{animationName}' looping from {startTime} seconds.");
        }
    }
}
