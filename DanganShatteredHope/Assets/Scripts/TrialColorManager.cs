using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

[System.Serializable]
public class ColorEntry
{
    public string label;
    public Color color;
}

public class TrialColorManager : MonoBehaviour
{
    public float transitionSpeed = 1.0f; // Speed of color change
    public List<ColorEntry> colors = new List<ColorEntry>(); // Editable in Inspector
    public List<Image> targetImages = new List<Image>(); // Drag and drop UI images here

    private Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
    private Color targetColor;

    private void Start()
    {
        targetColor = targetImages.Count > 0 ? GetInitialColor() : Color.white;

        // Populate dictionary from list
        foreach (ColorEntry entry in colors)
        {
            if (!colorDictionary.ContainsKey(entry.label))
            {
                colorDictionary.Add(entry.label, entry.color);
            }
        }
    }

    private void Update()
    {
        foreach (Image img in targetImages)
        {
            if (img != null)
            {
                img.color = Color.Lerp(img.color, targetColor, transitionSpeed * Time.deltaTime);
            }
        }
    }

    public void ChangeColor(string colorName)
    {
        if (colorDictionary.ContainsKey(colorName))
        {
            targetColor = colorDictionary[colorName];
        }
        else
        {
            Debug.LogWarning("Color name not found!");
        }
    }

    private Color GetInitialColor()
    {
        foreach (Image img in targetImages)
        {
            if (img != null)
            {
                return img.color;
            }
        }
        return Color.white;
    }
}
