using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource musicAudioSource, sfxAudioSource;
    [SerializeField] AudioClip[] musicClips;

    List<AudioClip> shuffledClips;
    int currentClipIndex = 0;
    AudioClip lastPlayedClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShuffleMusicList();
    }

    void ShuffleMusicList()
    {
        List<AudioClip> tempClips = new List<AudioClip>(musicClips);
        if (lastPlayedClip != null)
        {
            tempClips.Remove(lastPlayedClip);
            tempClips = Shuffle(tempClips);
            tempClips.Insert(Random.Range(1, tempClips.Count + 1), lastPlayedClip); // Ensures last played isn't first
        }
        else
        {
            tempClips = Shuffle(tempClips);
        }
        shuffledClips = tempClips;
        currentClipIndex = 0;
    }

    List<AudioClip> Shuffle(List<AudioClip> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    void PlayNextTrack()
    {
        if (musicClips.Length == 0) return;

        if (currentClipIndex >= shuffledClips.Count)
        {
            ShuffleMusicList();
        }

        lastPlayedClip = shuffledClips[currentClipIndex];
        musicAudioSource.clip = lastPlayedClip;
        musicAudioSource.Play();
        currentClipIndex++;

        Invoke(nameof(PlayNextTrack), lastPlayedClip.length);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void StartMusic()
    {
        PlayNextTrack();
    }
}
