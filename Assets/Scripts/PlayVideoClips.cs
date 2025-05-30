﻿using ECM2.Walkthrough.Ex52;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideoClips : MonoBehaviour
{
    [SerializeField] VideoClip[] visionVideoClips;

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] CanvasGroup uiPanel;
    [SerializeField] RawImage videoImage;

    [SerializeField] float fadeTime = 0.5f;

    [SerializeField] Transform playerCam;
    FirstPersonController playerController;
    [SerializeField] float camPanUpTime = 0.5f;
    [SerializeField] float camStartAngle = 45, camEndAngle = 0;

    [Header("For Testing: Security Mode")]
    [SerializeField] bool securityMode = true;
    [SerializeField] bool skipVideo = false;

    int visioinClipIndex = 0;

    bool coroutineStarted;

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

        FindAnyObjectByType<PlayerInteraction>().OnInteract += CheckPlayClip;


        PrepareForVideo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartGame();
        }
    }

    void CheckPlayClip(Interactable interactable)
    {
        if(visioinClipIndex == 0 && interactable is Weapon)
        {
            PlayVideoClip(visionVideoClips[visioinClipIndex]);
            visioinClipIndex++;
        }
        else if(interactable is Core)
        {
            PlayVideoClip(visionVideoClips[visioinClipIndex]);
            visioinClipIndex++;
        }
    }

    void PlayVideoClip(VideoClip clip)
    {
        videoPlayer.clip = clip;

        PrepareForVideo();
    }

    void PrepareForVideo()
    {

        Debug.Log("Start Video");

        Time.timeScale = 0; // Pause the game
        uiPanel.gameObject.SetActive(true);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
        securityTimer = videoPlayer.clip.length + 1; // Add 1 second to the video length

        playerController = playerCam.GetComponentInParent<FirstPersonController>();
        playerController.enabled = false;

        AudioManager.Instance.StopMusic();

        if (securityMode) securityCoroutine = StartCoroutine(IfBrokenStartGame());
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

        if(!coroutineStarted) StartCoroutine(StartGameCoroutine());
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

    IEnumerator StartGameCoroutine()
    {
        coroutineStarted = true;

        videoImage.gameObject.SetActive(false);

        playerCam.transform.rotation = Quaternion.Euler(camStartAngle, playerCam.rotation.eulerAngles.y, playerCam.rotation.eulerAngles.z);

        for (float i = 0; i < fadeTime; i+= Time.unscaledDeltaTime)
        {
            uiPanel.alpha = 1 - (i / fadeTime);

            yield return null;
        }

        uiPanel.alpha = 0;
        uiPanel.blocksRaycasts = false;

        for (float i = 0; i < camPanUpTime; i += Time.unscaledDeltaTime)
        {
            float angle = Mathf.Lerp(camStartAngle, camEndAngle, i / camPanUpTime);

            playerCam.transform.rotation = Quaternion.Euler(angle, playerCam.rotation.eulerAngles.y, playerCam.rotation.eulerAngles.z);

            yield return null;
        }
        Debug.Log("Start Game");

        playerCam.transform.rotation = Quaternion.Euler(camEndAngle, playerCam.rotation.eulerAngles.y, playerCam.rotation.eulerAngles.z);

        if(playerController != null) playerController.enabled = true;

        AudioManager.Instance.StartMusic();

        Time.timeScale = 1; // Resume game

        uiPanel.gameObject.SetActive(false);
    }
}