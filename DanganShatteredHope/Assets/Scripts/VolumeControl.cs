using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class VolumeControl : MonoBehaviour
{
    public AudioSource audioSource; // Assign the AudioSource component for background audio
    public Image[] volumeImages; // Assign 10 image objects in order
    public Color activeColor = Color.white; // Color for active volume level
    public Color inactiveColor = Color.gray; // Color for inactive volume levels
    public AudioClip intervalSoundClip; // Assign an AudioClip to play for interval navigation
    public AudioClip maxVolumeAudioClip; // Assign an AudioClip to play when max volume is exceeded
    public AudioClip minVolumeAudioClip; // Assign an AudioClip to play when min volume is exceeded
    public AudioSource keyPressAudioSource; // Separate AudioSource for audio effects

    private float volumeStep = 0.1f; // Step size for volume adjustment
    private int maxVolumeSteps = 10; // Number of volume steps (0.0 to 1.0)
    private string saveFilePath; // Path to save the volume file

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/VolumeData.txt";

        LoadVolume();
        UpdateVolumeIndicators();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            AttemptChangeVolume(volumeStep);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            AttemptChangeVolume(-volumeStep);
        }

        UpdateVolumeIndicators();
    }

    void AttemptChangeVolume(float change)
    {
        float newVolume = Mathf.Clamp(audioSource.volume + change, 0f, 1f);

        if (change > 0 && audioSource.volume == 1f)
        {
            PlayMaxVolumeAudio();
        }
        else if (change < 0 && audioSource.volume == 0f)
        {
            PlayMinVolumeAudio();
        }
        else
        {
            ChangeVolume(newVolume - audioSource.volume);
            PlayIntervalSound();
        }
    }

    void ChangeVolume(float change)
    {
        audioSource.volume += change;
        SaveVolume();
    }

    void UpdateVolumeIndicators()
    {
        int currentStep = Mathf.RoundToInt(audioSource.volume * maxVolumeSteps);

        for (int i = 0; i < volumeImages.Length; i++)
        {
            volumeImages[i].color = (i < currentStep) ? activeColor : inactiveColor;
        }
    }

    void SaveVolume()
    {
        File.WriteAllText(saveFilePath, audioSource.volume.ToString());
    }

    void LoadVolume()
    {
        if (File.Exists(saveFilePath))
        {
            string volumeData = File.ReadAllText(saveFilePath);
            if (float.TryParse(volumeData, out float savedVolume))
            {
                audioSource.volume = Mathf.Clamp(savedVolume, 0f, 1f);
            }
        }
    }

    void PlayMaxVolumeAudio()
    {
        if (keyPressAudioSource != null && maxVolumeAudioClip != null)
        {
            keyPressAudioSource.PlayOneShot(maxVolumeAudioClip);
        }
    }

    void PlayMinVolumeAudio()
    {
        if (keyPressAudioSource != null && minVolumeAudioClip != null)
        {
            keyPressAudioSource.PlayOneShot(minVolumeAudioClip);
        }
    }

    void PlayIntervalSound()
    {
        if (keyPressAudioSource != null && intervalSoundClip != null)
        {
            keyPressAudioSource.PlayOneShot(intervalSoundClip);
        }
    }
}
