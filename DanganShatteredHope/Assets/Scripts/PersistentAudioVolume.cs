using UnityEngine;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class PersistentAudioVolume : MonoBehaviour
{
    public string uniqueAudioSourceName; // Unique name to match the saved volume data
    private AudioSource audioSource; // The attached AudioSource component
    private string saveFilePath; // Path to the saved volume file

    void Awake()
    {
        if (string.IsNullOrEmpty(uniqueAudioSourceName))
        {
            Debug.LogError("Unique Audio Source Name is not set!");
            return;
        }

        // Initialize the save file path
        saveFilePath = Application.persistentDataPath + "/" + uniqueAudioSourceName + "_VolumeData.txt";

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Load and apply the saved volume
        LoadAndApplyVolume();
    }

    void LoadAndApplyVolume()
    {
        // Check if the volume data file exists
        if (File.Exists(saveFilePath))
        {
            string volumeData = File.ReadAllText(saveFilePath);

            // Attempt to parse the saved volume level
            if (float.TryParse(volumeData, out float savedVolume))
            {
                // Clamp the volume level and apply it
                audioSource.volume = Mathf.Clamp(savedVolume, 0f, 1f);
            }
        }
        else
        {
            // If no saved volume exists, set a default volume
            audioSource.volume = 0.5f; // Default volume value
        }
    }
}
