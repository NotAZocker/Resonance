using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class PlayIntroVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject uiPanel;
    [SerializeField] bool securityMode = true;
    [SerializeField] bool skipVideo = false;

    bool itHasWorked;
    double securityTimer;

    Coroutine securityCoroutine;

    void Start()
    {
        if (skipVideo)
        {
            StartGame();
            return;
        }

        Time.timeScale = 0; // Pause the game
        uiPanel.SetActive(true);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
        securityTimer = videoPlayer.clip.length + 1; // Add 1 second to the video length

        if(securityMode) securityCoroutine = StartCoroutine(IfBrokenStartGame());
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        videoPlayer.prepareCompleted -= OnVideoPrepared; // Unsubscribe to prevent memory leaks
        videoPlayer.loopPointReached += OnVideoFinished; // Event when video ends
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        itHasWorked = true;

        vp.loopPointReached -= OnVideoFinished; // Unsubscribe event

        StartGame();
    }

    void StartGame()
    {
        if(securityCoroutine != null)
        {
            StopCoroutine(securityCoroutine);
        }

        Time.timeScale = 1; // Resume game
        uiPanel.SetActive(false);
        Destroy(gameObject); // Destroy this script
    }

    IEnumerator IfBrokenStartGame()
    {
        for (float i = 0; i < securityTimer; i+= Time.unscaledDeltaTime)
        {
            yield return null;
        }

        if(!itHasWorked)
        {
            StartGame();
        }
    }
}
