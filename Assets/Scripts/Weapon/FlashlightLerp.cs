using UnityEngine;
using UnityEngine.EventSystems;

public class FlashlightLerp : MonoBehaviour
{
    [SerializeField] private Transform target; // Assign the player or camera transform here
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private Vector3 transformOffset;

    private Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
        flashlight.enabled = false;
    }

    void Update()
    {
        if (target == null) return;

        // Smoothly interpolate the flashlight's forward direction towards the target's forward direction
        transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * lerpSpeed);

        transform.position = target.transform.position + transformOffset;

        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled; // yuh
        }
    }
}