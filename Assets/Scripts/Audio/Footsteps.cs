using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private Transform headTransform;
    [SerializeField] private float bobbingAmount = 0.1f;
    [SerializeField] private float bobbingSpeed = 6f;
    [SerializeField] private float lerpSpeed = 5f;

    private Rigidbody rb;
    private float stepTimer;
    private float bobbingTimer;
    private Vector3 originalHeadPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (headTransform != null)
        {
            originalHeadPosition = headTransform.localPosition;
        }
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            bobbingTimer += Time.deltaTime * bobbingSpeed;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }

            if (headTransform != null)
            {
                headTransform.localPosition = originalHeadPosition + new Vector3(0, Mathf.Sin(bobbingTimer) * bobbingAmount, 0);
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer when not moving
            bobbingTimer = 0f;

            if (headTransform != null)
            {
                headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, originalHeadPosition, Time.deltaTime * lerpSpeed);
            }
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
