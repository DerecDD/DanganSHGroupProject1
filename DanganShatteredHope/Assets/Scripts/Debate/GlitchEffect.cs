using UnityEngine;
using UnityEngine.UI;

public class GlitchEffect : MonoBehaviour
{
    [Header("VHS Distortion Images")]
    public Image topVhsImage;  // Top VHS distortion layer
    public Image bottomVhsImage; // Bottom VHS distortion layer

    [Header("Bottom VHS Alternating Sprites")]
    public Sprite[] bottomSprites; // 4 alternating sprites
    public float bottomSpriteSpeed = 0.5f; // Speed of alternation

    [Header("Floating Image")]
    public Image floatingImage; // Moves up/down slightly
    public float floatAmount = 5f; // Max movement distance
    public float floatSpeed = 2f; // Speed of movement

    [Header("Scanline Images")]
    public Image[] scanlineImages; // Four scanline images
    public float scanlineSpeed = 5f; // Speed of vertical jitter
    public float scanlineJitter = 3f; // Amount of movement

    private float nextSpriteChangeTime;
    private int currentSpriteIndex = 0;
    private Vector3 floatingOriginalPos;
    private Vector3[] scanlineOriginalPos;

    void Start()
    {
        if (floatingImage != null)
            floatingOriginalPos = floatingImage.transform.localPosition;

        scanlineOriginalPos = new Vector3[scanlineImages.Length];
        for (int i = 0; i < scanlineImages.Length; i++)
            scanlineOriginalPos[i] = scanlineImages[i].transform.localPosition;
    }

    void Update()
    {
        ApplyFloatingEffect();
        ApplyScanlineEffect();
        CycleBottomSprites();
    }

    void ApplyFloatingEffect()
    {
        if (floatingImage != null)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            floatingImage.transform.localPosition = floatingOriginalPos + new Vector3(0, yOffset, 0);
        }
    }

    void ApplyScanlineEffect()
    {
        for (int i = 0; i < scanlineImages.Length; i++)
        {
            float jitter = Random.Range(-scanlineJitter, scanlineJitter);
            scanlineImages[i].transform.localPosition = scanlineOriginalPos[i] + new Vector3(0, jitter, 0);
        }
    }

    void CycleBottomSprites()
    {
        if (bottomVhsImage != null && bottomSprites.Length > 0)
        {
            if (Time.time >= nextSpriteChangeTime)
            {
                currentSpriteIndex = (currentSpriteIndex + 1) % bottomSprites.Length;
                bottomVhsImage.sprite = bottomSprites[currentSpriteIndex];
                nextSpriteChangeTime = Time.time + bottomSpriteSpeed;
            }
        }
    }
}
