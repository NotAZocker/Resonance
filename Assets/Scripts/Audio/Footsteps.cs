using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] float stepInterval = 0.5f;

    Rigidbody rb;

    private float stepTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer when not moving
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            footstepAudioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.Play();
        }
    }
}
