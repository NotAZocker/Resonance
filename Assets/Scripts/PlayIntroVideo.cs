using UnityEngine;
using UnityEngine.Video;

public class PlayIntroVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject uiPanel;

    void Start()
    {
        Time.timeScale = 0; // Pause the game
        uiPanel.SetActive(true);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        videoPlayer.prepareCompleted -= OnVideoPrepared; // Unsubscribe to prevent memory leaks
        videoPlayer.loopPointReached += OnVideoFinished; // Event when video ends
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Time.timeScale = 1; // Resume game
        vp.loopPointReached -= OnVideoFinished; // Unsubscribe event
        uiPanel.SetActive(false);
        Destroy(gameObject); // Destroy this script
    }
}
