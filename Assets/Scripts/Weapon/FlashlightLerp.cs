using UnityEngine;

public class FlashlightLerp : MonoBehaviour
{
    [SerializeField] private Transform target; // Assign the player or camera transform here
    [SerializeField] private float lerpSpeed = 5f;

    void Update()
    {
        if (target == null) return;

        // Smoothly interpolate the flashlight's forward direction towards the target's forward direction
        transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * lerpSpeed);
    }
}