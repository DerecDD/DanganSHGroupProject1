using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float runSpeed = 12.0f; // Speed when running
    [SerializeField] float gravity = -13.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [SerializeField] bool lockCursor = true;

    [SerializeField] float bobSpeedWalking = 5.0f; // Bobbing speed while walking
    [SerializeField] float bobSpeedRunning = 8.0f; // Bobbing speed while running
    [SerializeField] float bobAmount = 0.05f; // Amount of bobbing movement

    [SerializeField] AudioSource audioSource = null; // AudioSource for playing sounds
    [SerializeField] List<AudioClip> footstepSounds = new List<AudioClip>(); // Shared list of footstep sounds

    [SerializeField] float walkingSoundDelay = 0.5f; // Delay between footstep sounds while walking
    [SerializeField] float runningSoundDelay = 0.3f; // Delay between footstep sounds while running

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    float defaultCameraYPos;
    float bobTimer = 0.0f;

    bool isPlayingFootstepSound = false; // To manage sound playback timing

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Store the default Y position of the camera
        defaultCameraYPos = playerCamera.localPosition.y;
    }

    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        UpdateCameraBob();
        UpdateAudio();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
            velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime;

        // Determine movement speed based on whether the Shift key is pressed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateCameraBob()
    {
        // Check if the player is moving
        if (currentDir != Vector2.zero)
        {
            // Apply sine wave-based bobbing
            bobTimer += Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? bobSpeedRunning : bobSpeedWalking);
            float newY = defaultCameraYPos + Mathf.Sin(bobTimer) * bobAmount;

            // Update camera Y position
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, newY, playerCamera.localPosition.z);
        }
        else
        {
            // Reset camera position when not moving
            bobTimer = 0.0f;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, defaultCameraYPos, playerCamera.localPosition.z);
        }
    }

    void UpdateAudio()
    {
        // Check if the player is moving
        if (currentDir != Vector2.zero) 
        {
            if (!isPlayingFootstepSound)
            {
                StartCoroutine(PlayFootstepSound());
            }
        }
        else
        {
            audioSource.Stop();
            isPlayingFootstepSound = false;
        }
    }


    IEnumerator PlayFootstepSound()
    {
        isPlayingFootstepSound = true;

        float delay = Input.GetKey(KeyCode.LeftShift) ? runningSoundDelay : walkingSoundDelay;

        if (footstepSounds.Count > 0)
        {
            // Pick a random sound from the shared list
            audioSource.clip = footstepSounds[Random.Range(0, footstepSounds.Count)];
            audioSource.Play();
        }

        // Wait for the delay before allowing the next sound
        yield return new WaitForSeconds(delay);

        isPlayingFootstepSound = false;
    }
}
