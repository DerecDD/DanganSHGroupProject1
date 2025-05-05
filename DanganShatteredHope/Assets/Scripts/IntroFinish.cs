using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SkipIntro : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public int sceneIndex; // Set this in the Unity Inspector

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
